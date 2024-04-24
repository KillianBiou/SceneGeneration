using GLTFast;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using GLTFast.Export;
using System.Collections.Generic;

enum OutputFormat
{
    None = -1,
    GLB = 0,
}

[Serializable]
public struct GenerationInformations
{
    public string prompt;
    public string localSaveFilename;
}

public class ShapeClient : MonoBehaviour
{
    [Header("Server Parameters")]
    [SerializeField]
    private string generationPath;
    [SerializeField]
    private OutputFormat fileFormat;

    [Header("Generation Parameters")]
    [SerializeField]
    private string prompt;

    private Queue<GenerationInformations> generationQueue = new Queue<GenerationInformations>();
    private bool canGenerate = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            RequestDebugGeneration();

        if(canGenerate && generationQueue.Count > 0)
            StartCoroutine(DL(generationQueue.Dequeue()));
    }

    public void RequestDebugGeneration()
    {
        GenerationInformations newGen = new GenerationInformations();
        newGen.prompt = prompt;
        newGen.localSaveFilename = prompt.Replace(" ", "");

        generationQueue.Enqueue(newGen);
    }

    public void RequestGeneration(string prompt, string localSaveFilename)
    {
        GenerationInformations newGen = new GenerationInformations();
        newGen.prompt = prompt;
        newGen.localSaveFilename = localSaveFilename;

        generationQueue.Enqueue(newGen);
    }

    public IEnumerator DL(GenerationInformations genInfo)
    {
        canGenerate = false;

        string localDownloadPath = Application.dataPath + "/ServerDownload/" + genInfo.localSaveFilename + "." + fileFormat.ToString().ToLower();
        //Download
        UnityWebRequest dlreq = new UnityWebRequest(generationPath + "/" + genInfo.prompt + "." + fileFormat.ToString().ToLower());
        dlreq.downloadHandler = new DownloadHandlerFile(localDownloadPath);


        UnityWebRequestAsyncOperation op = dlreq.SendWebRequest();

        Debug.Log("Generation started for : " + genInfo.prompt);

        while (!op.isDone)
        {
            //here you can see download progress
            //Debug.Log(dlreq.downloadedBytes / 1000 + "KB");

            yield return null;
        }

        if (dlreq.isNetworkError || dlreq.isHttpError)
        {
            Debug.Log("Download Failed : " + dlreq.error);
        }
        else
        {
            Debug.Log("Download Success ! File saved at : " + localDownloadPath);
            yield return new WaitForSeconds(1f);

            LoadGltfBinaryFromMemory(localDownloadPath);
        }


        dlreq.Dispose();

        canGenerate = true;

        yield return null;

    }

    async void LoadGltfBinaryFromMemory(string filePath)
    {
        /*Debug.Log("Try instantiation");
        GameObject temp = Instantiate(new GameObject());
        temp.AddComponent<GltfAsset>();
        temp.GetComponent<GltfAsset>().Url = "file://" + filePath;*/

        Debug.Log("Instantiation Try");
        byte[] data = File.ReadAllBytes(filePath);
        var gltf = new GltfImport();
        bool success = await gltf.LoadGltfBinary(
            data,
            // The URI of the original data is important for resolving relative URIs within the glTF
            new Uri(filePath)
            );
        if (success)
        {
            Debug.Log("Instantiation Success");
            success = await gltf.InstantiateMainSceneAsync(transform);
        }
    }
}
