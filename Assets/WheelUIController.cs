using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WheelUIController : MonoBehaviour
{

    [SerializeField]
    private GameObject content, cursor;


    [HideInInspector]
    public Vector2 input;
    [HideInInspector]
    public UnityEvent<int> ChoiceDone;

    private int nChoices;

    private bool isChoosing;

    public void Init()
    {
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


        if (input.magnitude > 0.5)
            isChoosing = true;

        if (!isChoosing)
            return;

        if (input.magnitude < 0.5)
            gameObject.SetActive(false);

    }

    private void OnDisable()
    {
        ChoiceDone.Invoke(Choosed());
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
        }
    }


    public int Choosed()
    {
        if(!isChoosing)
            return -1;



        float angle = Vector2.SignedAngle(Vector2.up, input);

        if (angle < 0)
            angle += 360;

        float marge = 360 / nChoices;

        angle += (marge / 2);


        if (angle >= 360)
            return 0;

        Debug.Log("choix : " + Mathf.FloorToInt(angle / marge) + " angle : " + angle + " margew : " + marge);

        return Mathf.FloorToInt(angle/marge);
    }


}
