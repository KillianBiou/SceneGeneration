using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

[Serializable]
public struct StableCompleteRequest
{
    public SdRequest request;
    public string imagesModels;
    public int nbImages;
    public string device;
}

public class TestStableTerminal : MonoBehaviour
{

    [Header("Generation Parameters")]
    [SerializeField]
    private StableCompleteRequest stableRequest;

    private Process pythonProcess;
    private bool isProcessRunning = false;
    public static event Action OnPythonProcessEnded;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RequestGeneration(stableRequest);
        }
    }

    private void RequestGeneration(StableCompleteRequest requestDetail)
    {
        UnityEngine.Debug.Log("GENERATION STARTED");

        if (isProcessRunning)
        {
            UnityEngine.Debug.Log("A TripoSR process is already running - quitting and replacing process.");

            if (pythonProcess is { HasExited: false })
            {
                pythonProcess.Kill();
                pythonProcess.Dispose();
            }

            pythonProcess = null;
            isProcessRunning = false;
        }

        string args = $"{(requestDetail.request.negPrompt == "" ? "" : "--nprompt \"" + requestDetail.request.negPrompt + "\"")} " +
                      $"{(requestDetail.request.hiddenPrompt == "" ? "" : "--hprompt \"" + requestDetail.request.hiddenPrompt + "\"")} " +
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
                      $" \"{requestDetail.request.prompt}\" ";

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
        };

        pythonProcess.ErrorDataReceived += (sender, e) =>
        {
            UnityEngine.Debug.Log("Error :" + e.Data);
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
    }

}
