using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VR_LightInfo : MonoBehaviour
{

    public TMP_Text TXTrange, TXTintensity;
    public Slider range, intensity;

    public static VR_LightInfo Instance;
    private TileLight tileLightRef;

    public WheelController colorWheel;
    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }


    public void Init(TileLight light)
    {
        tileLightRef = light;
        range.value = light.GetLightRange();
        intensity.value = light.GetLightIntensity();
    }

    public void EditRange(float f)
    {
        tileLightRef.SetLightRange(f);
        TXTrange.text = "Light Range " + f.ToString("F2");
    }

    public void EditIntensity(float i)
    {
        tileLightRef.SetLightIntensity(i);
        TXTintensity.text = "Light Intensity " + i.ToString("F2");
    }

    public void SetColor(float i)
    {
        tileLightRef.SetLightColor(Color.HSVToRGB(i, .5f, .5f));
    }
}
