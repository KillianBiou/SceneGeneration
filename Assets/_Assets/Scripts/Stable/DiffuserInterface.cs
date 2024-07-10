using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;
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

    private Func<string,string> memoryCallback;
    private string target;

    private void Awake()
    {
        Instance = this;
    }

    public void RequestGeneration(StableCompleteRequest requestDetail, Func<string, string> callback = null)
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
            Arguments = $"{Path.Combine(Application.dataPath, "TripoSR", "StableTest.py")} {args}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        UnityEngine.Debug.Log("PROCESS STARTED : " + $"{Path.Combine(Application.dataPath, "TripoSR/StableTest.py")} {args}");

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
            if(e.Data.Contains("Generated image"))
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

    public static StableCompleteRequest GetRequestTemplate()
    {
        StableCompleteRequest request = new StableCompleteRequest();
        request.request = new SdRequest("", "image", "", "", 512, 512, 20, 7, -1, false, false, false, "");
        request.device = "";
        request.nbImages = 1;
        request.imagesModels = "";

        return request;

    }

    private void OnPythonProcessExited(object sender, EventArgs e)
    {
        UnityEngine.Debug.Log("Generation ended");
        isProcessRunning = false;
        pythonProcess = null;

        if (memoryCallback != null)
        {
            UnityEngine.Debug.Log("MEMORY SET");
            memoryCallback(target);
        }
    }

}
