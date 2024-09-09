using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleProgressBar : MonoBehaviour
{
    public static SimpleProgressBar Instance;

    [SerializeField]
    private TMP_Text stateText;
    [SerializeField]
    private Slider bar;
    [SerializeField]
    private GameObject stepContainer, stepPrefab;

    private int currentStep;

    private void Awake()
    {
        Instance = this;
    }



    public void SetStepCount(int n)
    {
        for(int i = 0; i<n; i++)
        {
            Instantiate(stepPrefab, stepContainer.transform);
        }
    }

    public void SetProgress(float f)
    {
        bar.value = f;
    }

    public void SetStepName(string name)
    {
        stateText.text = name;
    }

    public void SetProcedure(string nameOfFirst, int nbStep)
    {
        SetStepName(nameOfFirst);
        SetProgress(0);
        SetStepCount(nbStep);
        currentStep = 0;

        if (stepContainer.transform.childCount == 0)
            return;

        for (int i = stepContainer.transform.childCount - 1; i >= 0; i--)
            Destroy(stepContainer.transform.GetChild(i));
    }

    public void NextStep(string nameOfStep)
    {
        SetStepName(nameOfStep);
        SetProgress(0);

        //stepContainer.transform.GetChild(currentStep)
        currentStep++;

        if(currentStep >= stepContainer.transform.childCount) // ENDED
        {
            SetStepName("Done.");
            SetProgress(1);
        }
    }
}
