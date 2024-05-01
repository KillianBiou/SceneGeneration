using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GenerativeChoice : MonoBehaviour
{

    public string folderName;
    public GameObject uiContainer, buttonChoice;
    public Vector3 generationPos;
    [SerializeField]
    private TMP_Text title;




    private StableHandler sdh;
    private int amount;
    private bool pending;

    public SdRequest req;

    private Process pythonProcess;
    private bool isProcessRunning = false;

    public static event Action OnPythonProcessEnded;

    // Start is called before the first frame update
    void Start()
    {
        pending = false;

        sdh = FindFirstObjectByType<StableHandler>();
    }


    public void ClearPicker()
    {
        for (int i = uiContainer.transform.childCount; i > 0; i--)
        {
            Destroy(uiContainer.transform.GetChild(i - 1).gameObject);
        }

        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/GeneratedData/" + folderName);
        FileInfo[] info = dir.GetFiles("*.*");

        foreach (FileInfo f in info)
        {
            f.Delete();
        }

        title.text = "Choices";
    }



    public void GenerateChoices(SdRequest rq, int n)
    {
        if (!sdh || pending)
            return;

        ClearPicker();

        rq.directory = folderName +"/";
        for (int i = 0; i < n; i++)
        {
            req.filename.Replace(".png", "_" + i + ".png");
            sdh.RequestGeneration(rq);
        }

        sdh.FinishedGenerating.AddListener(CountingResults);
        amount = n;
        pending = true;
    }



    public void Test(int n)
    {
        if (!sdh)
            return;

        ClearPicker();

        uiContainer.transform.parent.gameObject.SetActive(true);

        if (!folderName.EndsWith('/'))
            folderName = folderName + '/';


        req.directory = "/GeneratedData/" + folderName + "/";
        for (int i = 0; i < n; i++)
        {
            req.filename = req.filename.Replace(".png", "_" + i + ".png");
            sdh.RequestGeneration(req);
        }

        sdh.FinishedGenerating.AddListener(RemoveBackground);
        amount = n;
        pending = true;
    }

    public void RemoveBackground()
    {
        if (isProcessRunning)
        {
            UnityEngine.Debug.Log("A background remover process is already running - quitting and replacing process.");

            if (pythonProcess is { HasExited: false })
            {
                pythonProcess.Kill();
                pythonProcess.Dispose();
            }

            pythonProcess = null;
            isProcessRunning = false;
        }

        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/GeneratedData/" + folderName);
        FileInfo[] info = dir.GetFiles("*.png");

        if (info.Length >= amount)
        {
            string[] names = new string[info.Length];

            for (int i = 0; i < info.Length; i++)
            {
                names[i] = Application.dataPath + "/GeneratedData/" + folderName + "/" + info[i].Name;
            }

            string args = $"\"{string.Join("\" \"", names)}\" ";

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = TripoSRForUnity.Instance.pythonPath,
                Arguments = $"{Path.Combine(Application.dataPath, "TripoSR/BackgroundRemover.py")} {args}",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };



            pythonProcess = new Process { StartInfo = start };
            pythonProcess.StartInfo = start;
            pythonProcess.EnableRaisingEvents = true;
            pythonProcess.Exited += OnPythonProcessExited;

            pythonProcess.OutputDataReceived += (sender, e) =>
            {
                UnityEngine.Debug.Log(e.Data);
            };

            pythonProcess.ErrorDataReceived += (sender, e) =>
            {
                UnityEngine.Debug.Log(e.Data);
            };

            pythonProcess.Start();
            pythonProcess.BeginOutputReadLine();
            pythonProcess.BeginErrorReadLine();
            isProcessRunning = true;
        }
    }

    private void OnPythonProcessExited(object sender, EventArgs e)
    {
        isProcessRunning = false;
        pythonProcess = null;

        UnityEditor.EditorApplication.delayCall += CountingResults;

        UnityEditor.EditorApplication.delayCall += () => OnPythonProcessEnded?.Invoke();
    }


    public void CountingResults()
    {

        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/GeneratedData/" +  folderName);
        FileInfo[] info = dir.GetFiles("*.png");

        if (info.Length >= amount)
        {
            //generate buttons
            foreach (FileInfo f in info)
            {


                GameObject last = Instantiate(buttonChoice);

                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(File.ReadAllBytes(Application.dataPath + "/GeneratedData/" + folderName + "/" + f.Name));

                last.GetComponent<Button>().onClick.AddListener(() => ChooseImage(Application.dataPath + "/GeneratedData/" + folderName + "/" + f.Name, 
                                                                                Application.dataPath + "/GeneratedData/" + folderName + req.prompt.Replace(" ", "") + (System.DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds + ".png"));

                last.transform.GetChild(0).GetComponent<RawImage>().texture = tex;
                last.transform.SetParent(uiContainer.transform);

            }

            //LayoutRebuilder.MarkLayoutForRebuild(GetComponent<RectTransform>());
            //LayoutRebuilder.MarkLayoutForRebuild(gameObject.transform as RectTransform);
            //LayoutRebuilder.MarkLayoutForRebuild(uiContainer.transform as RectTransform);

            //EditorUtility.SetDirty(transform.parent.GetComponent<Canvas>());

            title.text = "Choices (" + amount + ")";
            uiContainer.SetActive(true);

            pending = false;
            sdh.FinishedGenerating.RemoveListener(CountingResults);
            gameObject.SetActive(false);
        }
    }

    public void ChooseImage(string oldPath, string newPath)
    {
        uiContainer.transform.parent.gameObject.SetActive(false);

        File.Move(oldPath, newPath);

        //Player.Instance.AddImage(newPath);
        Player.Instance.AddImage(newPath, generationPos);
    }

    public void SetPrompt(string s)
    {
        req.prompt = s;
    }

    public void SetNegPrompt(string s)
    {
        req.negPrompt = s;
    }


    public void SetWidth(float i)
    {
        req.width = (int)i;
    }

    public void SetHeight(float i)
    {
        req.height = (int)i;
    }



    public void SetWidth(string i)
    {
        req.width = int.Parse(i);
    }

    public void SetHeight(string i)
    {
        req.height = int.Parse(i);
    }

    public void SetWidth(int i)
    {
        req.width = i;
    }

    public void SetHeight(int i)
    {
        req.height = i;
    }



    public void SetSteps(string i)
    {
        req.steps = int.Parse(i);
    }
    public void SetCfg(string i)
    {
        req.cfgScale = int.Parse(i);
    }
    public void SetSeed(string i)
    {
        req.seed = int.Parse(i);
    }



    public void SetFileName(string s)
    {
        req.filename = s;
    }
    public void SetDirectory(string s)
    {
        req.directory = s;
    }

}
