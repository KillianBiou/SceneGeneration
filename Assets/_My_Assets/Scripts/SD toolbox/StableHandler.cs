using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Events;






[Serializable]
public struct SdRequest
{
    public string directory, filename;

    public string prompt, negPrompt, hiddenPrompt;
    public int width, height, steps, cfgScale, seed;
    public bool tilling, tileX, tileY;

    public SdRequest(string dir, string fn, string p, string np, int w, int h, int s, int cfg, int sid, bool t, bool tx, bool ty, string hdp)
    {
        directory = dir;
        filename = fn;
        prompt = p;
        negPrompt = np;
        width = w;
        height = h;
        steps = s;
        cfgScale = cfg;
        seed = sid;
        tilling = t;
        tileX = tx;
        tileY = ty;

        hiddenPrompt = hdp;
    }
}





public class StableHandler : StableDiffusionGenerator
{

    //public string model;
    //public string ip;
    //public int port;

    public string directory, filename;

    public string prompt, negPrompt;
    public int width, height, steps, cfgScale, seed, lastSeed;
    public bool tilling, tileX, tileY;
    public long generatedSeed;

    public bool generateStart;
    private bool generating;
    
    
    [HideInInspector]
    public UnityEvent FinishedGenerating;


    [HideInInspector]
    public int selectedSampler = 4;
    [HideInInspector]
    public int selectedModel = 0;

    public StableDiffusionConfiguration sdc;


    private Queue<SdRequest> requests;


    public string[] modelsList
    {
        get
        {
            if (sdc == null)
                sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            return sdc.modelNames;
        }
    }

    public string[] samplersList
    {
        get
        {
            if (sdc == null)
                sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            return sdc.samplers;
        }
    }


    void Start()
    {
        generateStart = false;
        generating = false;
        requests = new Queue<SdRequest>();
    }
    void Update()
    {
        if (generateStart && !generating)
            StartCoroutine(GenerateAsync());
        else if (generateStart && generating)
        {
            requests.Enqueue(new SdRequest(directory, filename, prompt, negPrompt, width, height, steps, cfgScale, seed, tilling, tileX, tileY, ""));
            generateStart = false;
        }

        if((!generateStart && !generating) && requests.Count != 0)
            StartCoroutine(GenerateAsync(requests.Dequeue()));
    }


    public void RequestGeneration(SdRequest rq)
    {
        requests.Enqueue(rq);
    }


    IEnumerator GenerateAsync()
    {
        generating = true;
        generateStart = false;

        // Set the model parameters
        yield return sdc.SetModelAsync(modelsList[selectedModel]);

        // Generate the image
        HttpWebRequest httpWebRequest = null;
        try
        {
            // Make a HTTP POST request to the Stable Diffusion server
            httpWebRequest = (HttpWebRequest)WebRequest.Create(sdc.settings.StableDiffusionServerURL + sdc.settings.TextToImageAPI);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            // add auth-header to request
            if (sdc.settings.useAuth && !sdc.settings.user.Equals("") && !sdc.settings.pass.Equals(""))
            {
                httpWebRequest.PreAuthenticate = true;
                byte[] bytesToEncode = Encoding.UTF8.GetBytes(sdc.settings.user + ":" + sdc.settings.pass);
                string encodedCredentials = Convert.ToBase64String(bytesToEncode);
                httpWebRequest.Headers.Add("Authorization", "Basic " + encodedCredentials);
            }

            // Send the generation parameters along with the POST request
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                SDParamsInTxt2Img sd = new SDParamsInTxt2Img();
                sd.prompt = prompt;
                sd.negative_prompt = negPrompt;
                sd.steps = steps;
                sd.cfg_scale = cfgScale;
                sd.width = width;
                sd.height = height;
                sd.seed = seed;
                sd.tiling = tilling;

                if (selectedSampler >= 0 && selectedSampler < samplersList.Length)
                    sd.sampler_name = samplersList[selectedSampler];

                // Serialize the input parameters
                string json = JsonConvert.SerializeObject(sd);

                // Send to the server
                streamWriter.Write(json);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n\n" + e.StackTrace);
        }

        // Read the output of generation
        if (httpWebRequest != null)
        {
            // Wait that the generation is complete before procedding
            Task<WebResponse> webResponse = httpWebRequest.GetResponseAsync();

            while (!webResponse.IsCompleted)
            {
                /*if (sdc.settings.useAuth && !sdc.settings.user.Equals("") && !sdc.settings.pass.Equals(""))
                    UpdateGenerationProgressWithAuth();
                else
                    UpdateGenerationProgress();*/

                yield return new WaitForSeconds(0.5f);
            }

            // Stream the result from the server
            var httpResponse = webResponse.Result;

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                // Decode the response as a JSON string
                string result = streamReader.ReadToEnd();

                // Deserialize the JSON string into a data structure
                SDResponseTxt2Img json = JsonConvert.DeserializeObject<SDResponseTxt2Img>(result);

                // If no image, there was probably an error so abort
                if (json.images == null || json.images.Length == 0)
                {
                    Debug.LogError("No image was return by the server. This should not happen. Verify that the server is correctly setup.");

                    generating = false;
                    yield break;
                }

                // Decode the image from Base64 string into an array of bytes
                byte[] imageData = Convert.FromBase64String(json.images[0]);



                directory = PathFixer(directory);

                // Write it in the specified project output folder
                using (FileStream imageFile = new FileStream(Application.dataPath + directory + filename + ".png", FileMode.Create))
                {
                    yield return imageFile.WriteAsync(imageData, 0, imageData.Length);
                }

                try
                {
                    // Read back the image into a texture
                    if (File.Exists(Application.dataPath + directory + filename + ".png"))
                    {
                        Texture2D texture = new Texture2D(2, 2);
                        texture.LoadImage(imageData);
                        texture.Apply();
                    }

                    // Read the generation info back (only seed should have changed, as the generation picked a particular seed)
                    if (json.info != "")
                    {
                        SDParamsOutTxt2Img info = JsonConvert.DeserializeObject<SDParamsOutTxt2Img>(json.info);

                        // Read the seed that was used by Stable Diffusion to generate this result
                        generatedSeed = info.seed;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message + "\n\n" + e.StackTrace);
                }

                FinishedGenerating.Invoke();
#if UNITY_EDITOR
                EditorUtility.ClearProgressBar();
#endif
            }
        }
        generating = false;
        yield return null;
    }


    public void Generate()
    {
        StartCoroutine(GenerateAsync());
    }

    public void LoadModel(string name)
    {
        int i = 0;
        foreach (string s in modelsList)
        {
            if (s == name)
                selectedModel = i;

            i++;
        }
    }

    public void LoadModel(int i)
    {
        selectedModel = i;
    }




    IEnumerator GenerateAsync(SdRequest rq)
    {
        generating = true;

        // Set the model parameters
        yield return sdc.SetModelAsync(modelsList[selectedModel]);

        // Generate the image
        HttpWebRequest httpWebRequest = null;
        try
        {
            // Make a HTTP POST request to the Stable Diffusion server
            httpWebRequest = (HttpWebRequest)WebRequest.Create(sdc.settings.StableDiffusionServerURL + sdc.settings.TextToImageAPI);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            // add auth-header to request
            if (sdc.settings.useAuth && !sdc.settings.user.Equals("") && !sdc.settings.pass.Equals(""))
            {
                httpWebRequest.PreAuthenticate = true;
                byte[] bytesToEncode = Encoding.UTF8.GetBytes(sdc.settings.user + ":" + sdc.settings.pass);
                string encodedCredentials = Convert.ToBase64String(bytesToEncode);
                httpWebRequest.Headers.Add("Authorization", "Basic " + encodedCredentials);
            }

            // Send the generation parameters along with the POST request
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                SDParamsInTxt2Img sd = new SDParamsInTxt2Img();
                if(rq.hiddenPrompt != "")
                    sd.prompt = rq.prompt + "," + rq.hiddenPrompt;
                else
                    sd.prompt = rq.prompt;
                sd.negative_prompt = rq.negPrompt;
                sd.steps = rq.steps;
                sd.cfg_scale = rq.cfgScale;
                sd.width = rq.width;
                sd.height = rq.height;
                sd.seed = rq.seed;
                sd.tiling = rq.tilling;

                if (selectedSampler >= 0 && selectedSampler < samplersList.Length)
                    sd.sampler_name = samplersList[selectedSampler];

                // Serialize the input parameters
                string json = JsonConvert.SerializeObject(sd);

                // Send to the server
                streamWriter.Write(json);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n\n" + e.StackTrace);
        }

        // Read the output of generation
        if (httpWebRequest != null)
        {
            // Wait that the generation is complete before procedding
            Task<WebResponse> webResponse = httpWebRequest.GetResponseAsync();
            /*
            while (!webResponse.IsCompleted)
            {
                /*if (sdc.settings.useAuth && !sdc.settings.user.Equals("") && !sdc.settings.pass.Equals(""))
                    UpdateGenerationProgressWithAuth();
                else
                    UpdateGenerationProgress();*/

                yield return new WaitForSeconds(0.5f);

            // Stream the result from the server
            var httpResponse = webResponse.Result;

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                // Decode the response as a JSON string
                string result = streamReader.ReadToEnd();

                // Deserialize the JSON string into a data structure
                SDResponseTxt2Img json = JsonConvert.DeserializeObject<SDResponseTxt2Img>(result);

                // If no image, there was probably an error so abort
                if (json.images == null || json.images.Length == 0)
                {
                    Debug.LogError("No image was return by the server. This should not happen. Verify that the server is correctly setup.");

                    generating = false;
                    yield break;
                }

                // Decode the image from Base64 string into an array of bytes
                byte[] imageData = Convert.FromBase64String(json.images[0]);

                //rq.directory = PathFixer(rq.directory);

                if(!Directory.Exists(Path.Combine(Application.dataPath, rq.directory)))
                    Directory.CreateDirectory(Path.Combine(Application.dataPath, rq.directory));

                // Write it in the specified project output folder
                using (FileStream imageFile = new FileStream(Path.Combine(Application.dataPath, rq.directory, rq.filename), FileMode.Create))
                {
                    yield return imageFile.WriteAsync(imageData, 0, imageData.Length);
                }

                try
                {
                    // Read back the image into a texture
                    if (File.Exists(Application.dataPath + rq.directory + rq.filename))
                    {
                        Texture2D texture = new Texture2D(2, 2);
                        texture.LoadImage(imageData);
                        texture.Apply();
                    }

                    // Read the generation info back (only seed should have changed, as the generation picked a particular seed)
                    if (json.info != "")
                    {
                        SDParamsOutTxt2Img info = JsonConvert.DeserializeObject<SDParamsOutTxt2Img>(json.info);

                        // Read the seed that was used by Stable Diffusion to generate this result
                        generatedSeed = info.seed;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message + "\n\n" + e.StackTrace);
                }

                Debug.Log("genere a lemplacemnt : " + "" + Application.dataPath + rq.directory + rq.filename);

                FinishedGenerating.Invoke();
                EditorUtility.ClearProgressBar();
            }
        }
        generating = false;
        yield return null;
    }


    private string PathFixer(string s)
    {
        string fix = s;

        if (!s.StartsWith('/'))
            fix = "/" + fix;

        if (!s.EndsWith('/'))
            fix = fix + "/";

        return fix;
    }



}
