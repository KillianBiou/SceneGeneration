using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ToolSelectionState
{
    UNKNOWN = -1,
    MODE_ROOM = 0,
    MODE_2D = 1,
    MODE_3D = 2,
}

public class ToolMenu : MonoBehaviour
{

    public Button A, B, C, D;
    public GameObject[] modeA, modeB, modeC, modeD;

    public ToolSelectionState currentState;

    private GameObject[] currentMode;

    public static ToolMenu Instance;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentState = ToolSelectionState.MODE_ROOM;
        currentMode = modeA;

        HideAll();
        ShowCurrent();
        A.interactable = false;
    }

    public void ShowCurrent()
    {
        foreach (GameObject go in currentMode)
            go.SetActive(true);
    }


    public void HideAll()
    {
        foreach (GameObject go in modeA)
            go.SetActive(false);

        foreach (GameObject go in modeB)
            go.SetActive(false);

        foreach (GameObject go in modeC)
            go.SetActive(false);

        foreach (GameObject go in modeD)
            go.SetActive(false);

        A.interactable = true;
        B.interactable = true;
        C.interactable = true;
        D.interactable = true;
    }


    public void ChangeMode(int i)
    {
        if (i < 0 || i > 3)
            return;

        A.interactable = true;
        B.interactable = true;
        C.interactable = true;
        D.interactable = true;

        foreach (GameObject go in currentMode)
            go.SetActive(false);


        switch (i)
        {
            case 0:
                currentMode = modeA;
                A.interactable = false;
                currentState = ToolSelectionState.MODE_ROOM;
                break;
            case 1:
                currentMode = modeB;
                B.interactable = false;
                currentState = ToolSelectionState.MODE_2D;
                break;
            case 2:
                currentMode = modeC;
                C.interactable = false;
                currentState = ToolSelectionState.MODE_3D;
                break;
            case 3:
                currentMode = modeD;
                D.interactable = false;
                currentState = ToolSelectionState.UNKNOWN;
                break;
        }
        ShowCurrent();
    }


}
