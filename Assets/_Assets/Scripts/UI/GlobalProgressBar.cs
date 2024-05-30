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
    private TMP_Text stateText;
    [SerializeField]
    private TMP_Text percentText;
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
        percentText.text = "0 %";
    }

    public void NotifyPhaseChange(ApplicationStatePhase newPhase)
    {
        // Advancement value
        int currentAdvancement = currentProcedureList.IndexOf(newPhase);
        // Set slider at current percentage (advancement for correct mapping)
        progressBar.value = currentAdvancement;

        percentText.text = Mathf.Ceil(((float)currentAdvancement / currentProcedureList.Count) * 100) +  " %";

        SetProgressText(newPhase);
    }

    private void SetProgressText(ApplicationStatePhase phase)
    {
        switch (phase)
        {
            case ApplicationStatePhase.NONE:
                stateText.text = "Idle";
                stateText.color = longPhaseColor;
                break;
            case ApplicationStatePhase.PROMPT_INPUT:
                stateText.text = "Enter your prompt";
                stateText.color = userInputPhaseColor;
                break;
            case ApplicationStatePhase.ZERO_IMAGE:
                stateText.text = "Generation first image";
                stateText.color = longPhaseColor;
                break;
            case ApplicationStatePhase.ONE_IMAGE:
                stateText.text = "Generation second image";
                stateText.color = longPhaseColor;
                break;
            case ApplicationStatePhase.TWO_IMAGE:
                stateText.text = "Generation third image";
                stateText.color = longPhaseColor;
                break;
            case ApplicationStatePhase.THREE_IMAGE:
                stateText.text = "Generation forth image";
                stateText.color = longPhaseColor;
                break;
            case ApplicationStatePhase.FOUR_IMAGE:
                stateText.text = "All Image generated";
                stateText.color = shortPhaseColor;
                break;
            case ApplicationStatePhase.BACKGROUND_REMOVING:
                stateText.text = "Removing background";
                stateText.color = longPhaseColor;
                break;
            case ApplicationStatePhase.USER_SELECTION:
                stateText.text = "Select an image";
                stateText.color = userInputPhaseColor;
                break;
            case ApplicationStatePhase.TRIPOSR_INIT:
                stateText.text = "Initializing TripoSR";
                stateText.color = shortPhaseColor;
                break;
            case ApplicationStatePhase.TRIPOSR_PROCESSING:
                stateText.text = "Processing Image";
                stateText.color = shortPhaseColor;
                break;
            case ApplicationStatePhase.TRIPOSR_RUNNING:
                stateText.text = "Generating model";
                stateText.color = shortPhaseColor;
                break;
            case ApplicationStatePhase.TRIPOSR_EXPORT:
                stateText.text = "Generating 3D Mesh";
                stateText.color = longPhaseColor;
                break;
            case ApplicationStatePhase.MODEL_IMPORT:
                stateText.text = "Import mesh into the scene";
                stateText.color = shortPhaseColor;
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
