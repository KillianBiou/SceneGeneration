using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderTexter : MonoBehaviour
{

    [SerializeField]
    public TMP_Text text;

    private void Start()
    {
        Slider s = null;

        if ((s = gameObject.GetComponent<Slider>()) != null)
            SetText(s.value);
    }


    public void SetText(float i)
    {
        if(text)
            text.text = i.ToString();
    }
}
