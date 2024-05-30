using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalProgressBar : MonoBehaviour
{
    [Header("Color")]
    [SerializeField]
    private Color shortPhaseColor;
    [SerializeField]
    private Color longPhaseColor;
    [SerializeField]
    private Color userInputPhaseColor;

    [Header("References")]
    [SerializeField]
    private TMP_Text txt;
    [SerializeField]
    private Slider progressBar;

    [Header("State Procedure")]
    [SerializedDictionary("Application state", "Procedure")]
    [SerializeField]
    private SerializedDictionary<ApplicationState, List<ApplicationStatePhase>> statesProcedure;

    private List<ApplicationStatePhase> currentProcedureList;

    public void SetProcedure(ApplicationState state)
    {
        // Set procedure if exist
        if(statesProcedure.ContainsKey(state))
            currentProcedureList = statesProcedure[state];

        // Set Slider var
        progressBar.maxValue = currentProcedureList.Count - 1;
        progressBar.value = 0;

        // Set text
        SetProgressText(currentProcedureList[0]);
    }

    public void NotifyPhaseChange(ApplicationStatePhase newPhase)
    {
        // Advancement value
        int currentAdvancement = currentProcedureList.IndexOf(newPhase);
        // Set slider at current percentage (advancement - 1 for correct mapping)
        progressBar.value = currentAdvancement - 1;

        SetProgressText(newPhase);
    }

    private void SetProgressText(ApplicationStatePhase phase)
    {
        switch (phase)
        {
            case ApplicationStatePhase.NONE:
                txt.text = "Idle";
                txt.color = longPhaseColor;
                break;
            case ApplicationStatePhase.PROMPT_INPUT:
                txt.text = "Enter your prompt";
                txt.color = userInputPhaseColor;
                break;
            case ApplicationStatePhase.ZERO_IMAGE:
                txt.text = "Generation first image";
                txt.color = longPhaseColor;
                break;
            case ApplicationStatePhase.ONE_IMAGE:
                txt.text = "Generation second image";
                txt.color = longPhaseColor;
                break;
            case ApplicationStatePhase.TWO_IMAGE:
                txt.text = "Generation third image";
                txt.color = longPhaseColor;
                break;
            case ApplicationStatePhase.THREE_IMAGE:
                txt.text = "Generation forth image";
                txt.color = longPhaseColor;
                break;
            case ApplicationStatePhase.FOUR_IMAGE:
                txt.text = "All Image generated";
                txt.color = shortPhaseColor;
                break;
            case ApplicationStatePhase.BACKGROUND_REMOVING:
                txt.text = "Removing background";
                txt.color = longPhaseColor;
                break;
            case ApplicationStatePhase.USER_SELECTION:
                txt.text = "Select an image";
                txt.color = userInputPhaseColor;
                break;
            case ApplicationStatePhase.TRIPOSR_INIT:
                txt.text = "Initializing TripoSR";
                txt.color = shortPhaseColor;
                break;
            case ApplicationStatePhase.TRIPOSR_PROCESSING:
                txt.text = "Processing Image";
                txt.color = shortPhaseColor;
                break;
            case ApplicationStatePhase.TRIPOSR_RUNNING:
                txt.text = "Generating model";
                txt.color = shortPhaseColor;
                break;
            case ApplicationStatePhase.TRIPOSR_EXPORT:
                txt.text = "Generating 3D Mesh";
                txt.color = longPhaseColor;
                break;
            case ApplicationStatePhase.MODEL_IMPORT:
                txt.text = "Import mesh into the scene";
                txt.color = shortPhaseColor;
                break;
        }
    }

    public void StopProcedure()
    {
        currentProcedureList = null;
        progressBar.value = 0;
        progressBar.maxValue = 100;
        SetProgressText(ApplicationStatePhase.NONE);
    }

}
