using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelUiButton : MonoBehaviour
{
    [SerializeField]
    private GameObject visual;
    [SerializeField]
    private Color hoveredColor;

    private Color normalColor;

    private void Start()
    {
        normalColor = transform.GetChild(0).GetComponent<Image>().color;
    }

    public void ResetRot()
    {
        visual.transform.eulerAngles = transform.parent.eulerAngles;
    }

    public void Hover()
    {
        transform.GetChild(0).GetComponent<Image>().color = hoveredColor;
    }
    public void EndHover()
    {
        transform.GetChild(0).GetComponent<Image>().color = normalColor;
    }
}
