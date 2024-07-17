using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ColorPicker : MonoBehaviour
{
    static public ColorPicker Instance;


    public Color color;
    private float h,s,v;

    public Image old, newer;

    [HideInInspector]
    public UnityEvent<Color> ColorChanged;
    [HideInInspector]
    public UnityEvent QuitPicker;


    private void Awake()
    {
        Instance = this;
        color = new Color(0, 0, 0);
        gameObject.SetActive(false);
    }


    public void InitColor(Color c)
    {
        Color.RGBToHSV(c, out h, out s, out v);
        UpdateColor();
    }


    private void UpdateColor()
    {
        color = Color.HSVToRGB(h, s, v);
        ColorChanged.Invoke(color);
    }


    private void UpdateColor(Color c)
    {
        color = c;
        ColorChanged.Invoke(color);
    }


    public Color GetColor()
    {
        return color;
    }

    private void OnDisable()
    {
        QuitPicker.Invoke();
        QuitPicker.RemoveAllListeners();
        ColorChanged.RemoveAllListeners();
    }




    public void SetH(float f)
    {
        h = f;
        UpdateColor();
    }

    public void SetS(float f)
    {
        s = f;
        UpdateColor();
    }

    public void SetV(float f)
    {
        v = f;
        UpdateColor();
    }
}
