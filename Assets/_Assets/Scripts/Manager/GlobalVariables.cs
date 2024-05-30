using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ApplicationState
{
    UNKNOWN = -1,
    IDLE = 0,
    GENERATION2D = 1,
    GENERATION3D = 2
}

[System.Serializable]

public enum ApplicationStatePhase
{
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
}

public class GlobalVariables : MonoBehaviour
{
    [Header("Application State")]
    [SerializeField]
    private ApplicationState currentApplicationState;
    [SerializeField]
    private ApplicationStatePhase currentPhase;

    [Header("References")]
    [SerializeField]
    private GlobalProgressBar progressBar;

    public static GlobalVariables Instance;

    private void Awake()
    {
        Instance = this;
        EndOfGen();
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
}
