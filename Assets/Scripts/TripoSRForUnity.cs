using UnityEngine;
using System.Diagnostics;
using UnityEditor;
using System.IO;
using System.Globalization;
using System;
using AsImpL;
using UnityEngine.Events;
using System.Collections;
using System.Linq;

public enum TripoState
{
    UNKNOWN = -1,
    WAITING = 0,
    INITIALIZATION = 1,
    PROCESSING = 2,
    RUNNING = 3,
    EXPORTING = 4,
}

public class TripoSRForUnity : MonoBehaviour
{
    [Header("General Settings")]
    
    [SerializeField, Tooltip("If true, automatically adds the generated mesh to the scene.")]
    private bool autoAddMesh = true;

    [SerializeField, Tooltip("If true, automatically adds MeshCollider & RigidBody.")]
    private bool autoAddPhysicsComponents = true;
    
    [SerializeField, Tooltip("If true, automatically rotates the mesh's parent GameObject to negate wrong rotations.")]
    private bool autoFixRotation = true;
    
    [ReadOnly, SerializeField, Tooltip("If true, moves and renames the output .obj file (based on the input image's filename)")]
    private bool moveAndRename = true;
    
    [ReadOnly, SerializeField, Tooltip("If moveAndRename = true, specifies the relative path to some folder where the output .obj file will be moved to.")]
    private string destinationPath = "Models";
    
    [SerializeField, Tooltip("If true, TripoSR's run.py debug output is printed to Unity's console.")]
    private bool showDebugLogs = true;

    [SerializeField, Tooltip("Path to input image(s).")]
    private Texture2D[] images;
    
    [Header("TripoSR Parameters")]
    [ReadOnly, SerializeField, Tooltip("Device to use. Default: 'cuda:0'")]
    private string device = "cuda:0";

    [ReadOnly, SerializeField, Tooltip("Path to the pretrained model. Default: 'stabilityai/TripoSR'")]
    private string pretrainedModelNameOrPath = "stabilityai/TripoSR";

    [SerializeField, Tooltip("Evaluation chunk size. Default: 8192")]
    private int chunkSize = 8192;

    [SerializeField, Tooltip("Marching cubes grid resolution. Default: 256")]
    private int marchingCubesResolution = 256;

    [SerializeField, Tooltip("If true, background will not be removed. Default: false")]
    private bool noRemoveBg = false;

    [SerializeField, Tooltip("Foreground to image size ratio. Default: 0.85")]
    private float foregroundRatio = 0.85f;

    [ReadOnly, SerializeField, Tooltip("Output directory. Default: 'output/'")]
    private string outputDir = "output/";

    [ReadOnly, SerializeField, Tooltip("Mesh save format. Default: 'obj'")]
    private string modelSaveFormat = "obj";

    [ReadOnly, SerializeField, Tooltip("If true, saves a rendered video. Default: false")]
    private bool render = false;

    [Header("Misc")]
    [ReadOnly, SerializeField]
    private TripoState currentState = TripoState.WAITING;

    public static TripoSRForUnity Instance;

    private Process pythonProcess;
    private bool isProcessRunning = false;
    private Func<string, int> memoryCallback;
    private string generatedImagePath = null;

    public static event Action OnPythonProcessEnded;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (isProcessRunning)
        {
            SetTripoState();
        }
    }

    private string savePath;
    public UnityEvent<string> memoryCallbackGLB;

    //Generate a 3D model and put it into a folder
    public void RunTripoSR_GLB(Func<string, int> callback = null, string imagefullpath = null, string fileType = "glb")
    {
        savePath = Path.Combine(GlobalVariables.Instance.GetModelPath(), Path.GetFileNameWithoutExtension(imagefullpath).Substring(0, Path.GetFileNameWithoutExtension(imagefullpath).Length-2));
        savePath += DateTime.Now.ToString("yyMMdd-HHmmss");

        UnityEngine.Debug.Log("Creation de la destination :  " + savePath + "...");
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        File.Copy(imagefullpath, Path.Combine(savePath, Path.GetFileName(savePath) + ".png"));

        memoryCallback = callback;

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

        UnityEngine.Debug.Log("GENERATING - " + fileType);
        string s = "a" +
            "b";


        string args = $"\"{string.Join("\" \"", imagefullpath)}\" --device {device} " +
                      $"--pretrained-model-name-or-path {pretrainedModelNameOrPath} " +
                      $"--chunk-size {chunkSize} --mc-resolution {marchingCubesResolution} " +
                      $"{(noRemoveBg ? "--no-remove-bg " : "")} " +
                      $"--foreground-ratio {foregroundRatio.ToString(CultureInfo.InvariantCulture)} " +
                      $"--output-dir \"{savePath}\" " +
                      $"--model-save-format {fileType} " +
                      $"{(render ? "--render" : "")}";

        ProcessStartInfo start = new ProcessStartInfo
        {
            FileName = GlobalVariables.Instance.GetPythonPath(),
            Arguments = $"{Path.Combine(GlobalVariables.Instance.GetPyScriptDirectory(), "run.py")} {args}",
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        pythonProcess = new Process { StartInfo = start };
        pythonProcess.StartInfo = start;
        pythonProcess.EnableRaisingEvents = true;
        pythonProcess.Exited += OnPythonProcessExited_GLB;

        pythonProcess.OutputDataReceived += (sender, e) =>
        {
            if (showDebugLogs && !string.IsNullOrEmpty(e.Data))
            {
                UnityEngine.Debug.Log(e.Data);
            }
        };

        pythonProcess.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data.Contains("Initializing"))
            {
                currentState = TripoState.INITIALIZATION;
            }
            if (e.Data.Contains("Processing"))
            {
                currentState = TripoState.PROCESSING;
            }
            if (e.Data.Contains("Running"))
            {
                currentState = TripoState.RUNNING;
            }
            if (e.Data.Contains("Exporting"))
            {
                currentState = TripoState.EXPORTING;
            }

            UnityEngine.Debug.Log(e.Data);
        };

        pythonProcess.Start();
        pythonProcess.BeginOutputReadLine();
        pythonProcess.BeginErrorReadLine();
        isProcessRunning = true;
    }


    private void OnPythonProcessExited_GLB(object sender, EventArgs e)
    {
        UnityEngine.Debug.Log("Python process exited. ");
        currentState = TripoState.WAITING;
        isProcessRunning = false;
        pythonProcess = null;

        GenerationDatabase.Instance.SetupMeshFolder(savePath);

        UnityEngine.Debug.Log("Calling back for instantiation...");
        if (memoryCallback != null)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () => memoryCallback.Invoke(savePath);
#else
            StartCoroutine(CoolBack());
            //memoryCallback.Invoke(savePath);
#endif
        }
        else
            UnityEngine.Debug.Log("XXX No python exit callback XXX ");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += () => OnPythonProcessEnded?.Invoke();
#endif
    }

    IEnumerator CoolBack()
    {
        yield return new WaitForEndOfFrame();
        memoryCallback.Invoke(savePath);
        yield return null;
    }



    private void SetTripoState()
    {
        switch (currentState)
        {
            case TripoState.INITIALIZATION:
                GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.TRIPOSR_INIT);
                break;
            case TripoState.PROCESSING:
                GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.TRIPOSR_PROCESSING);
                break;
            case TripoState.RUNNING:
                GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.TRIPOSR_RUNNING);
                break;
            case TripoState.EXPORTING:
                GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.TRIPOSR_EXPORT);
                break;
        }
    }

    public TripoState GetCurrentState()
    {
        return currentState;
    }

    void OnDisable() { if (pythonProcess != null && !pythonProcess.HasExited) pythonProcess.Kill(); }
}
