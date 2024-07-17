using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class WheelUIController : MonoBehaviour
{

    [SerializeField]
    private GameObject content, cursor, segmentPrefab;


    [HideInInspector]
    public Vector2 input;
    [HideInInspector]
    public UnityEvent<int> ChoiceDone;
    [HideInInspector]
    public UnityEvent<float> ChoiceFloatDone;

    private int nChoices, currentChoice;

    private bool isChoosing;
    private float floatChoice;

    public void Init(List<WheelVisuals> buttons)
    {

        foreach(WheelVisuals button in buttons)
        {
            GameObject last = Instantiate(segmentPrefab);
            last.transform.SetParent(content.transform, false);
            last.GetComponent<WheelUiButton>().Init(button);
            last.GetComponent<UnityEngine.UI.Image>().material = Instantiate(last.GetComponent<UnityEngine.UI.Image>().material);
        }

        if (content.transform.childCount != 0)
        {
            ArrangeItems();
            nChoices = content.transform.childCount;
        }
        gameObject.SetActive(false);
    }


    private void Update()
    {
        cursor.transform.localPosition = input * (transform as RectTransform).rect.width/2 * 0.8f;


        if (!isChoosing && input.magnitude > 0.5)
        {
            isChoosing = true;
            currentChoice = Choosed();
            content.transform.GetChild(currentChoice).GetComponent<WheelUiButton>().Hover();
            return;
        }

        if (!isChoosing)
            return;

        int newChoice = Choosed();

        if (currentChoice != newChoice)
        {
            content.transform.GetChild(currentChoice).GetComponent<WheelUiButton>().EndHover();
            currentChoice = newChoice;
            content.transform.GetChild(currentChoice).GetComponent<WheelUiButton>().Hover();
        }

        if (input.magnitude < 0.5)
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        content.transform.GetChild(currentChoice).GetComponent<WheelUiButton>().EndHover();
        ChoiceDone.Invoke(Choosed());
        ChoiceFloatDone.Invoke(floatChoice);
    }

    private void OnEnable()
    {
        isChoosing = false;
    }



    public void ArrangeItems()
    {
        for (int i = 0; i < content.transform.childCount; i++)
        {
            content.transform.GetChild(i).localEulerAngles = new Vector3(0, 0, 360 / content.transform.childCount * i);
            content.transform.GetChild(i).GetComponent<WheelUiButton>().ResetRot();

            content.transform.GetChild(i).GetComponent<UnityEngine.UI.Image>().material.SetFloat("_Angle", 180/content.transform.childCount -0.5f);
        }
    }


    public int Choosed()
    {
        if(!isChoosing)
            return -1;


        float angle = Vector2.SignedAngle(Vector2.up, input);

        if (angle < 0)
            angle += 360;
        floatChoice = angle / 360;

        float marge = 360 / nChoices;

        angle += (marge / 2);


        if (angle >= 360)
            return 0;

        //Debug.Log("choix : " + Mathf.FloorToInt(angle / marge) + " angle : " + angle + " margew : " + marge);

        return Mathf.FloorToInt(angle/marge);
    }


}
