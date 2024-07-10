using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EditableColor : MonoBehaviour
{

    public UnityEvent<Color> ColorHook;
    public UnityEvent Deconnected;

    public void ColorPickStart()
    {
        ColorPicker.instance.gameObject.SetActive(true);
        ColorPicker.instance.ColorChanged.AddListener(ColorChanged);
        ColorPicker.instance.QuitPicker.AddListener(Deconnect);
        ColorPicker.instance.InitColor(gameObject.GetComponent<Image>().color);
    }

    public void ColorChanged(Color c)
    {
        ColorHook.Invoke(ColorPicker.instance.color);
    }

    public void Deconnect()
    {

    }

    private void OnDisable()
    {
        
    }
}
