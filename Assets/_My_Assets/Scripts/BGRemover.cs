using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

public class BGRemover : MonoBehaviour
{
    public static BGRemover Instance;

    public static event Action OnPythonProcessEnded;


    private int amount;
    private bool pending;
    private Process pythonProcess;
    private bool isProcessRunning = false;


    private void Awake()
    {
        Instance = this;
    }
    

    private Func<List<string>, int> EndJobCallBack;
    private List<string> outputs;
    public int RemoveBackground(string fullPath, Func<List<string>, int> callback = null)
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

        if(callback != null)
            EndJobCallBack = callback;

        outputs = new List<string>();
        outputs.Add(fullPath);

        string args = $"\"{fullPath}\" ";

        /* OUTPUT PATH
        string outPath = Path.Combine(Path.GetDirectoryName(fullPath), "BG_removed");

        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);
        */

        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = GlobalVariables.Instance.GetPythonPath(),
            Arguments = $"{Path.Combine(GlobalVariables.Instance.GetPyScriptDirectory(), "BackgroundRemover.py")} {args}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        //GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.BACKGROUND_REMOVING);

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
        
        return 0;
    }

    private void OnPythonProcessExited(object sender, EventArgs e)
    {
        isProcessRunning = false;
        pythonProcess = null;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () => OnPythonProcessEnded?.Invoke();
        EndJobCallBack(outputs);
#else
        StartCoroutine(CoolBack());
#endif
    }

    IEnumerator CoolBack()
    {
        yield return new WaitForEndOfFrame();
        EndJobCallBack(outputs);
        yield return null;
    }
}
