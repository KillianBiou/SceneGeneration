using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightInspector : MonoBehaviour
{
    public TileLight lightObj;

    [SerializeField]
    private Slider intensity, range, height;

    [SerializeField]
    private EditableColor picker;

    [SerializeField]
    private Image colorIcon;

    public static LightInspector Instance;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }


    public void Inspect(TileLight l)
    {
        lightObj = l;
        intensity.value = lightObj.GetLightIntensity();
        range.value = lightObj.GetLightRange();
        height.value = lightObj.GetLightHeight();
        picker.GetComponent<Image>().color = lightObj.GetLightColor();
        picker.ColorHook.AddListener(SetColor);
    }
    

    public void SetIntensity(float f)
    {
        lightObj.SetLightIntensity(f);
    }

    public void SetRange(float f)
    {
        lightObj.SetLightRange(f);
    }

    public void SetHeight(float f)
    {
        lightObj.SetLightHeight(f);
    }

    public void SetColor(Color c)
    {
        lightObj.SetLightColor(c);
        colorIcon.color = c;
    }
}
