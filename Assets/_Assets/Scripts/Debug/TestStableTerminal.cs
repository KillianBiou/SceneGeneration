using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class TestStableTerminal : MonoBehaviour
{

    [Header("Generation Parameters")]
    [SerializeField]
    private string prompt;
    [SerializeField]
    private string outputPath;
    [SerializeField]
    private string imagesModels;
    [SerializeField]
    [Range(10, 50)]
    private int steps;
    [SerializeField]
    [Range(1, 16)]
    private int nbImages;
    [SerializeField]
    private string devices;

    private Process pythonProcess;
    private bool isProcessRunning = false;
    public static event Action OnPythonProcessEnded;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            RequestGeneration();
        }
    }

    private void RequestGeneration()
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

        string args = $"--nbImages {nbImages} " +
                      $"{(imagesModels == "" ? "" : "--image-model " + imagesModels)} " +
                      $"--steps {steps}" +
                      $" \"{prompt}\" {Path.Combine(Application.dataPath, outputPath)} ";

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
