using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WheelUiButton : MonoBehaviour
{
    [SerializeField]
    private GameObject visual;
    [SerializeField]
    private Image segmentIcon;
    [SerializeField]
    private TMP_Text segmentName;
    [SerializeField]
    private Color hoveredColor;

    private Color normalColor;

    private void Start()
    {
        normalColor = transform.GetChild(0).GetComponent<Image>().color;
    }

    public void Init(WheelVisuals button)
    {
        if (button.img != null)
        {
            segmentIcon.sprite = button.img;
            segmentIcon.gameObject.SetActive(true);
        }

        if (button.name != null)
        {
            if(button.name != "")
            {
                segmentIcon.sprite = button.img;
                segmentIcon.gameObject.SetActive(true);
            }
        }
    }

    public void ResetRot()
    {
        visual.transform.eulerAngles = transform.parent.eulerAngles;
    }

    public void Hover()
    {
        GetComponent<Image>().material.SetColor("_Color", hoveredColor);
    }
    public void EndHover()
    {
        GetComponent<Image>().material.SetColor("_Color", normalColor);
    }
}
