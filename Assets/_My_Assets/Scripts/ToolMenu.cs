using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolMenu : MonoBehaviour
{

    public Button A, B, C, D;
    public GameObject[] modeA, modeB, modeC, modeD;

    private GameObject[] currentMode;


    private void Start()
    {
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
                break;
            case 1:
                currentMode = modeB;
                B.interactable = false;
                break;
            case 2:
                currentMode = modeC;
                C.interactable = false;
                break;
            case 3:
                currentMode = modeD;
                D.interactable = false;
                break;
        }
        ShowCurrent();
    }


}
