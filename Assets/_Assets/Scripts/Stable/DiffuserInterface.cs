using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Windows;

public class DiffuserInterface : MonoBehaviour
{

    public static DiffuserInterface Instance;

    [Header("Generation Parameters")]
    [SerializeField]
    private StableCompleteRequest stableRequest;

    private Process pythonProcess;
    private bool isProcessRunning = false;
    public static event Action OnPythonProcessEnded;

    private Func<string, int> memoryCallback;
    private string target;


    public UnityEvent FinishedGenerating;

    private void Awake()
    {
        Instance = this;
    }

    public static StableCompleteRequest GetRequestTemplate()
    {
        StableCompleteRequest request = new StableCompleteRequest();
        request.request = new SdRequest("", "image", "", "", 512, 512, 20, 7, -1, false, false, false, "");
        request.device = "";
        request.nbImages = 1;
        request.imagesModels = "";

        return request;

    }


    public void RequestGeneration(StableCompleteRequest requestDetail, Func<string, int> callback = null)
    {

        if (isProcessRunning)
        {
            UnityEngine.Debug.Log("A Diffuser process is already running - quitting and replacing process.");

            if (pythonProcess is { HasExited: false })
            {
                pythonProcess.Kill();
                pythonProcess.Dispose();
            }

            pythonProcess = null;
            isProcessRunning = false;
        }

        UnityEngine.Debug.Log("GENERATION STARTED");
        memoryCallback = callback;
        target = Path.Combine(GlobalVariables.Instance.GetImagePath(), requestDetail.request.filename + ".png");

        string args = $"{(requestDetail.request.negPrompt == "" ? "" : "--nprompt \"" + requestDetail.request.negPrompt + "\"")} " +
                      $"--width {requestDetail.request.width} " +
                      $"--height {requestDetail.request.height} " +
                      $"{(requestDetail.imagesModels == "" ? "" : "--image-model \"" + requestDetail.imagesModels + "\"")} " +
                      $"--steps {requestDetail.request.steps} " +
                      $"--cfg {requestDetail.request.cfgScale} " +
                      $"--seed {requestDetail.request.seed} " +
                      $"{(requestDetail.request.tilling ? "--tilling" : "")} " +
                      $"{(requestDetail.request.tileX ? "--tileX" : "")} " +
                      $"{(requestDetail.request.tileY ? "--tileY" : "")} " +
                      $"--nbImages {requestDetail.nbImages} " +
                      $"{(requestDetail.device == "" ? "" : "--device \"" + requestDetail.device + "\"")} " +
                      $"--output_path \"{Path.Combine(Application.dataPath, requestDetail.request.directory)}\" " +
                      $"--file_name \"{requestDetail.request.filename}\" " +
                      $" \"{requestDetail.request.prompt + "," + requestDetail.request.hiddenPrompt}\" ";

        // {Path.Combine(Application.dataPath, outputPath)}
        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = GlobalVariables.Instance.GetPythonPath(),
            Arguments = $"{Path.Combine(GlobalVariables.Instance.GetPyScriptDirectory(), "StableTest.py")} {args}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        UnityEngine.Debug.Log("PROCESS STARTED : " + $"{Path.Combine(GlobalVariables.Instance.GetPyScriptDirectory(), "/StableTest.py")} {args}");

        pythonProcess = new Process { StartInfo = start };
        pythonProcess.StartInfo = start;
        pythonProcess.EnableRaisingEvents = true;
        pythonProcess.Exited += OnPythonProcessExited;

        pythonProcess.OutputDataReceived += (sender, e) =>
        {
            UnityEngine.Debug.Log("Data :" + e.Data);
            if (e.Data.Contains("Generated image"))
            {
                //callback("");
            }
        };

        pythonProcess.ErrorDataReceived += (sender, e) =>
        {
            UnityEngine.Debug.Log("Error :" + e.Data);
            if (e.Data.Contains("%"))
            {
                int i = int.Parse(e.Data.Substring(e.Data.IndexOf("%") - 3, 3).Replace("%", ""));
                if (i >= 20)
                    GlobalProgressBar.Instance.NotifyPhaseChange(ApplicationStatePhase._20_100);
                if (i >= 40)
                    GlobalProgressBar.Instance.NotifyPhaseChange(ApplicationStatePhase._40_100);
                if (i >= 60)
                    GlobalProgressBar.Instance.NotifyPhaseChange(ApplicationStatePhase._60_100);
                if (i >= 80)
                    GlobalProgressBar.Instance.NotifyPhaseChange(ApplicationStatePhase._80_100);
                if (i >= 100)
                    GlobalProgressBar.Instance.NotifyPhaseChange(ApplicationStatePhase._100_100);
            }

            if (e.Data.Contains("Generated image"))
            {
                //callback("");
            }
        };

        // External set because it cause an unkown bug either way

        pythonProcess.Start();
        pythonProcess.BeginOutputReadLine();
        pythonProcess.BeginErrorReadLine();
        isProcessRunning = true;
    }


    private void OnPythonProcessExited(object sender, EventArgs e)
    {
        UnityEngine.Debug.Log("Generation ended");
        isProcessRunning = false;
        pythonProcess = null;

        if (memoryCallback != null)
        {
            UnityEngine.Debug.Log("Calling back...");
#if UNITY_EDITOR
            FinishedGenerating.Invoke();
            memoryCallback(target);
#else
            StartCoroutine(CoolBack());
#endif
        }
    }


    private IEnumerator CoolBack()
    {
        yield return new WaitForEndOfFrame();
        FinishedGenerating.Invoke();
        memoryCallback(target);
        yield return null;
    }


}
