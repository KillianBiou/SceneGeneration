using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightInspector : MonoBehaviour
{
    public GameObject lightObj;

    [SerializeField]
    private Slider intensity, range, height;

    [SerializeField]
    private EditableColor picker;


    public void Inspect(GameObject l)
    {
        lightObj = l;
        intensity.value = lightObj.GetComponent<Light>().intensity;
        range.value = lightObj.GetComponent<Light>().range;
        height.value = lightObj.transform.position.y;
        picker.ColorHook.AddListener(SetColor);
    }


    public void SetIntensity(float f)
    {
        lightObj.GetComponent<Light>().intensity = f;
    }

    public void SetRange(float f)
    {
        lightObj.GetComponent <Light>().range = f;
    }

    public void SetHeight(float f)
    {
        lightObj.transform.position = new Vector3(lightObj.transform.position.x, f, lightObj.transform.position.z);
    }

    public void SetColor(Color c)
    {
        lightObj.GetComponent<Light>().color = c;
    }
}
