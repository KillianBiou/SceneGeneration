using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SliderTexter : MonoBehaviour
{

    [SerializeField]
    public TMP_Text text;

    public string prefix, sufix;
    public int decimals = 2;

    private void Start()
    {
        Slider s = null;

        if ((s = gameObject.GetComponent<Slider>()) != null)
            SetText(s.value);
    }


    public void SetText(float i)
    {
        if(text)
            text.text = prefix + (float)Math.Round((double)i, decimals) + sufix;
    }
}
