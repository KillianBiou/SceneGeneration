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
        ColorPicker.Instance.gameObject.SetActive(true);
        ColorPicker.Instance.ColorChanged.AddListener(ColorChanged);
        ColorPicker.Instance.QuitPicker.AddListener(Deconnect);
        ColorPicker.Instance.InitColor(gameObject.GetComponent<Image>().color);
    }

    public void ColorChanged(Color c)
    {
        ColorHook.Invoke(ColorPicker.Instance.color);
    }

    public void Deconnect()
    {

    }

    private void OnDisable()
    {
        
    }
}
