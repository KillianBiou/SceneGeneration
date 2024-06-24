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
    private Vector3 generationPos;
    [SerializeField]
    private TMP_Text title;




    private StableHandler sdh;
    private Cursor3D cursor;
    private int amount;
    private bool pending;

    public SdRequest req;

    private Process pythonProcess;
    private bool isProcessRunning = false;

    public static event Action OnPythonProcessEnded;

    // Start is called before the first frame update
    void Awake()
    {
        pending = false;

        sdh = FindFirstObjectByType<StableHandler>();
        cursor = FindFirstObjectByType<Cursor3D>();
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

        generationPos = cursor.transform.position;

        ClearPicker();

        GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.ZERO_IMAGE);

        rq.directory = "GeneratedData/" + folderName;
        UnityEngine.Debug.Log(req.directory);
        for (int i = 0; i < n; i++)
        {
            rq.filename = rq.prompt + "_" + DateTime.Now.ToString("yyyyMMdd_hhmm") +  i + "_T.png";
            sdh.RequestGeneration(rq);
        }

        sdh.FinishedGenerating.AddListener(RemoveBackground);
        amount = n;
        pending = true;
    }



    public void Test(int n)
    {
        if (!sdh)
            return;

        ClearPicker();

        GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.ZERO_IMAGE);

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

        DirectoryInfo dir = new DirectoryInfo(Path.Combine(Application.dataPath, "GeneratedData", folderName));
        FileInfo[] info = dir.GetFiles("*.png");

        // Progress Bar Notification
        switch (info.Length)
        {
            case 1:
                GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.ONE_IMAGE);
                break;
            case 2:
                GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.TWO_IMAGE);
                break;
            case 3:
                GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.THREE_IMAGE);
                break;
            case 4:
                GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.FOUR_IMAGE);
                break;
        }

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

            GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.BACKGROUND_REMOVING);

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

        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/GeneratedData/" +  folderName); // CALL THE DIR
        FileInfo[] info = dir.GetFiles("*.png");

        if (info.Length >= amount)
        {
            //generate buttons
            foreach (FileInfo f in info)
            {
                GameObject last = Instantiate(buttonChoice);

                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(File.ReadAllBytes(f.FullName));

                last.GetComponent<Button>().onClick.AddListener(() => PickedImageCallback(f.FullName));

                last.transform.GetChild(0).GetComponent<RawImage>().texture = tex;
                last.transform.SetParent(uiContainer.transform);

            }

            title.text = "Choices (" + amount + ")";

            pending = false;
            sdh.FinishedGenerating.RemoveListener(RemoveBackground);
            //gameObject.SetActive(false);

            GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.USER_SELECTION);
        }
    }

    public void ChooseImage(string oldPath, string newPath)
    {
        //uiContainer.transform.parent.gameObject.SetActive(false);

        //File.Move(oldPath, newPath);

        //Player.Instance.AddImage(newPath);
        Player.Instance.AddImage(newPath, generationPos);
    }


    public void PickedImageCallback(string path)
    {
        Player.Instance.AddImage(path, generationPos);
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
