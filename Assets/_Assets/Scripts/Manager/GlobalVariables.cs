using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

[System.Serializable]
public enum ApplicationState
{
    UNKNOWN = -1,
    IDLE = 0,
    GENERATION2D = 1,
    GENERATION3D = 2,
    NONE,
    TERRAIN_PLACE,
    LIGHT_PLACE,
    LIGHT_EDIT,
    OBJ_MOVE,
    OBJ_ROTATE,
    OBJ_SCALE,
    GEN_3D,
    VR_OBJ_EDIT
}

[System.Serializable]

public enum ApplicationStatePhase
{
    UNKNOWN = 0,
    NONE,
    PROMPT_INPUT,
    ZERO_IMAGE,
    ONE_IMAGE,
    TWO_IMAGE,
    THREE_IMAGE,
    FOUR_IMAGE,
    BACKGROUND_REMOVING,
    USER_SELECTION,
    TRIPOSR_INIT,
    TRIPOSR_PROCESSING,
    TRIPOSR_RUNNING,
    TRIPOSR_EXPORT,
    MODEL_IMPORT,
    _0_100,
    _20_100,
    _40_100,
    _60_100,
    _80_100,
    _100_100,
}

[Serializable]
public struct ApplicationMode
{
    [SerializeField]
    public ApplicationState state;
    [SerializeField]
    public GameObject[] gameObjects;
    [SerializeField]
    public Button bouton;
}


public class GlobalVariables : MonoBehaviour
{
    [Header("Application State")]
    [SerializeField]
    private ApplicationState currentApplicationState;
    [SerializeField]
    private ApplicationStatePhase currentPhase;
    [ReadOnly, SerializeField]
    public bool isInVr = false;
    [SerializeField]
    public ApplicationMode[] modes;

    [Header("References")]
    [SerializeField]
    private GlobalProgressBar progressBar;
    [SerializeField]
    private Material meshBaseMaterial;

    //[Header("ReadOnly Global Variable")]
    //[ReadOnly, SerializeField]
    private string pythonPath, scriptDirectory, modelsPath, imagesPath;

    public static GlobalVariables Instance;

    private void Awake()
    {
        Instance = this;
        pythonPath = GetPythonPathFromRegistry();
        scriptDirectory = Path.Combine(Application.dataPath, "TripoSR/");
        modelsPath = Path.Combine(Application.dataPath, "GeneratedData/Models/");
        imagesPath = Path.Combine(Application.dataPath, "GeneratedData/Images/");

        if (!System.IO.Directory.Exists(modelsPath))
            System.IO.Directory.CreateDirectory(modelsPath);
        if (!System.IO.Directory.Exists(imagesPath))
            System.IO.Directory.CreateDirectory(imagesPath);
        EndOfGen();

        currentApplicationState = ApplicationState.IDLE;
        ModeHideAll();
    }

    private void Start()
    {
        ModeActivate(currentApplicationState);
        if (!IsPythonReady())
            return;
    }



    public bool IsPythonReady()
    {

        Process process = new Process();

        // redirect the output stream of the child process.
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = Path.Combine(scriptDirectory, "Check.py");
        process.StartInfo.Arguments = pythonPath;

        if (!string.IsNullOrEmpty(scriptDirectory))
        {
            process.StartInfo.WorkingDirectory = scriptDirectory;
        }
        else
        {
            process.StartInfo.WorkingDirectory = Application.temporaryCachePath;
        }

        int exitCode = -1;
        string output = null;

        try
        {
            process.Start();
            UnityEngine.Debug.Log("Start");
            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Run error" + e.ToString()); // or throw new Exception
        }
        finally
        {
            exitCode = process.ExitCode;

            process.Dispose();
            process = null;
        }

        return exitCode == 0 ? true : false;
    }


    public bool SetCurrentApplicationState(ApplicationState newState)
    {
        if (currentApplicationState == ApplicationState.IDLE)
        {
            currentApplicationState = newState;
            progressBar.SetProcedure(newState);
            return true;
        }
        return false;
    }

    public void SetCurrentPhase(ApplicationStatePhase newState)
    {
        if(newState != currentPhase)
        {
            currentPhase = newState;
            progressBar.NotifyPhaseChange(currentPhase);
        }
    }

    public void EndOfGen()
    {
        currentApplicationState = ApplicationState.IDLE;
        currentPhase = ApplicationStatePhase.NONE;
        progressBar.StopProcedure();
    }

    // GETTERS

    public Material GetBaseMaterial()
    {
        return meshBaseMaterial;
    }

    public string GetPythonPath()
    {
        return pythonPath;
    }

    public string GetPyScriptDirectory()
    {
        return scriptDirectory;
    }

    public string GetModelPath()
    {
        return modelsPath;
    }

    public string GetImagePath()
    {
        return imagesPath;
    }

    private string GetPythonPathFromRegistry(string requiredVersion = "", string maxVersion = "")
    {
        string[] possiblePythonLocations = new string[3] {
        @"HKLM\SOFTWARE\Python\PythonCore\",
        @"HKCU\SOFTWARE\Python\PythonCore\",
        @"HKLM\SOFTWARE\Wow6432Node\Python\PythonCore\"
    };

        //Version number, install path
        Dictionary<string, string> pythonLocations = new Dictionary<string, string>();  

        foreach (string possibleLocation in possiblePythonLocations)
        {
            string regKey = possibleLocation.Substring(0, 4),
                   actualPath = possibleLocation.Substring(5);
            RegistryKey theKey = regKey == "HKLM" ? Registry.LocalMachine : Registry.CurrentUser;
            RegistryKey theValue = theKey.OpenSubKey(actualPath);

            if (theValue != null)
            {
                foreach (var v in theValue.GetSubKeyNames())
                    if (theValue.OpenSubKey(v) is RegistryKey productKey)
                        try
                        {
                            string pythonExePath = productKey.OpenSubKey("InstallPath").GetValue("ExecutablePath").ToString();

                            // Comment this in to get (Default) value instead
                            // string pythonExePath = productKey.OpenSubKey("InstallPath").GetValue("").ToString();

                            if (pythonExePath != null && pythonExePath != "")
                            {
                                //Console.WriteLine("Got python version; " + v + " at path; " + pythonExePath);
                                pythonLocations.Add(v.ToString(), pythonExePath);
                            }
                        }
                        catch
                        {
                            //Install path doesn't exist
                        }
            }
        }

        if (pythonLocations.Count > 0)
        {
            System.Version desiredVersion = new(requiredVersion == "" ? "0.0.1" : requiredVersion);
            System.Version maxPVersion = new(maxVersion == "" ? "999.999.999" : maxVersion);

            string highestVersion = "", highestVersionPath = "";

            foreach (KeyValuePair<string, string> pVersion in pythonLocations)
            {
                //TODO; if on 64-bit machine, prefer the 64 bit version over 32 and vice versa
                int index = pVersion.Key.IndexOf("-"); //For x-32 and x-64 in version numbers
                string formattedVersion = index > 0 ? pVersion.Key.Substring(0, index) : pVersion.Key;

                System.Version thisVersion = new System.Version(formattedVersion);
                int comparison = desiredVersion.CompareTo(thisVersion),
                    maxComparison = maxPVersion.CompareTo(thisVersion);

                if (comparison <= 0)
                {
                    //Version is greater or equal
                    if (maxComparison >= 0)
                    {
                        desiredVersion = thisVersion;

                        highestVersion = pVersion.Key;
                        highestVersionPath = pVersion.Value;
                    }
                    //else
                    //    Console.WriteLine("Version is too high; " + maxComparison.ToString());
                }
                //else
                //    Console.WriteLine("Version (" + pVersion.Key + ") is not within the spectrum.");$
            }

            //Console.WriteLine(highestVersion);
            //Console.WriteLine(highestVersionPath);
            return highestVersionPath;
        }

        return "";
    }



    // MODES SWITCHER

    public void ModeHideAll()
    {
        foreach(ApplicationMode am in modes)
        {
            foreach (GameObject go in am.gameObjects)
                go.SetActive(false);
        }
    }

    public void ModeActivate(ApplicationState newState)
    {
        foreach (ApplicationMode am in modes)
        {
            if (am.state == newState)
            {
                ModeDeactivate(currentApplicationState);
                foreach (GameObject go in am.gameObjects)
                    go.SetActive(true);
                if(am.bouton)
                    am.bouton.interactable = false;
                currentApplicationState = newState;

                progressBar.SetProcedure(newState);
                return;
            }
        }
        UnityEngine.Debug.Log("State : " + newState + " not found.");
    }

    public void ModeDeactivate(ApplicationState oldState)
    {
        foreach (ApplicationMode am in modes)
        {
            if (am.state == oldState)
            {
                foreach (GameObject go in am.gameObjects)
                    go.SetActive(false);
                if (am.bouton)
                    am.bouton.interactable = true;
                return;
            }
        }
        UnityEngine.Debug.Log("State : " + oldState + " not found.");
    }

    public void ModeSwitchTo(AppState applicationState)
    {
        ModeActivate(applicationState.appState);
    }

    [SerializeField]
    private AppState[] Modes;
    public void ModeSwitchTo(int i)
    {
        if(i>-1 &&  i<Modes.Length)
            ModeSwitchTo(Modes[i]);
    }

    public ApplicationState GetCurrentApplicationState()
    {
        return currentApplicationState;
    }
}
