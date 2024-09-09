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


    private int amount;
    private bool pending;

    public StableCompleteRequest req;

    private Process pythonProcess;
    private bool isProcessRunning = false;

    public static event Action OnPythonProcessEnded;

    // Start is called before the first frame update
    void Awake()
    {
        pending = false;

        req = DiffuserInterface.GetRequestTemplate();

        if (!Directory.Exists(Path.Combine(Application.dataPath, "/GeneratedData", folderName)))
            Directory.CreateDirectory(Path.Combine(Application.dataPath, "GeneratedData", folderName));
    }


    public void ClearPicker()
    {
        for (int i = uiContainer.transform.childCount; i > 0; i--)
        {
            Destroy(uiContainer.transform.GetChild(i - 1).gameObject);
        }

        DirectoryInfo dir = new DirectoryInfo(Path.Combine(Application.dataPath + "/GeneratedData/" + folderName));
        FileInfo[] info = dir.GetFiles("*.*");

        foreach (FileInfo f in info)
        {
            f.Delete();
        }

        title.text = "Choices";
    }



    public void GenerateChoices(SdRequest rq, int n)
    {
        if (pending)
            return;

        ClearPicker();

        GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.ZERO_IMAGE);

        uiContainer.transform.parent.gameObject.SetActive(true);

        req.request = rq;
        req.request.directory = Path.Combine("GeneratedData", folderName);
        req.request.filename = req.request.prompt;
        req.nbImages = n;

        DiffuserInterface.Instance.RequestGeneration(req, RemoveBackground);

        amount = n;
        pending = true;
    }

    public int RemoveBackground(string s)
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
        UnityEngine.Debug.Log("Starting removing all BG from " + Path.Combine(Application.dataPath, req.request.directory));

        DirectoryInfo dir = new DirectoryInfo(Path.Combine(Application.dataPath, req.request.directory));
        FileInfo[] infos = dir.GetFiles("*.png");

        // Progress Bar Notification
        switch (infos.Length)
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

        UnityEngine.Debug.Log("Nb element = " + infos.Length);

        if (infos.Length >= amount)
        {
            string[] names = new string[infos.Length];

            for (int i = 0; i < infos.Length; i++)
            {
                names[i] =  infos[i].FullName;
            }

            string args = $"\"{string.Join("\" \"", names)}\" ";

            ProcessStartInfo start = new ProcessStartInfo
            {
                FileName = GlobalVariables.Instance.GetPythonPath(),
                Arguments = $"{Path.Combine(GlobalVariables.Instance.GetPyScriptDirectory(), "BackgroundRemover.py")} {args}",
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
        return 0;
    }

    private void OnPythonProcessExited(object sender, EventArgs e)
    {
        isProcessRunning = false;
        pythonProcess = null;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += GenerateButtons;

        UnityEditor.EditorApplication.delayCall += () => OnPythonProcessEnded?.Invoke();
#else
        StartCoroutine(CoolBack());
        //CountingResults();
#endif
    }

    IEnumerator CoolBack()
    {
        yield return new WaitForEndOfFrame();
        GenerateButtons();
        yield return null;
    }




    public void GenerateButtons()
    {

        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/GeneratedData/" +  folderName); // CALL THE DIR
        FileInfo[] infos = dir.GetFiles("*.png");

        foreach (FileInfo f in infos)
        {
            GameObject last = Instantiate(buttonChoice);

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(File.ReadAllBytes(f.FullName));

            last.GetComponent<Button>().onClick.AddListener(() => PickedImageCallback(f.FullName));

            last.transform.GetChild(0).GetComponent<RawImage>().texture = tex;
            last.transform.SetParent(uiContainer.transform, false);
        }

        title.text = "Choices (" + amount + ")";
        pending = false;
        GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.USER_SELECTION);
    }


    public void PickedImageCallback(string path)
    {
        gameObject.SetActive(false);
        //TripoSRForUnity.Instance.RunTripoSR_GLB();
        Player.Instance.ImgToMesh(path, generationPos);
    }







    public void SetPrompt(string s)
    {
        req.request.prompt = s;
    }

    public void SetNegPrompt(string s)
    {
        req.request.negPrompt = s;
    }


    public void SetWidth(float i)
    {
        req.request.width = (int)i;
    }

    public void SetHeight(float i)
    {
        req.request.height = (int)i;
    }



    public void SetWidth(string i)
    {
        req.request.width = int.Parse(i);
    }

    public void SetHeight(string i)
    {
        req.request.height = int.Parse(i);
    }

    public void SetWidth(int i)
    {
        req.request.width = i;
    }

    public void SetHeight(int i)
    {
        req.request.height = i;
    }



    public void SetSteps(string i)
    {
        req.request.steps = int.Parse(i);
    }
    public void SetCfg(string i)
    {
        req.request.cfgScale = int.Parse(i);
    }
    public void SetSeed(string i)
    {
        req.request.seed = int.Parse(i);
    }



    public void SetFileName(string s)
    {
        req.request.filename = s;
    }
    public void SetDirectory(string s)
    {
        req.request.directory = s;
    }

}
