using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CirclePicker : MonoBehaviour
{
    [SerializeField]
    private ColorPicker _colorPicker;

    [SerializeField]
    private Image _gradient;

    private bool _selecting;


    private void Update()
    {
        if (_selecting)
        {
            PickOnCicle();
        }
    }

    public void PickOnCicle()
    {

        Vector3 dir = Input.mousePosition - transform.position;
        float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Inverse(Quaternion.AngleAxis(angle-90, Vector3.forward));

        if (_colorPicker)
            _colorPicker.SetH(gameObject.transform.rotation.eulerAngles.z/360);

        _gradient.material.SetColor("_Color", Color.HSVToRGB(gameObject.transform.rotation.eulerAngles.z / 360, 1, 1));
    }


    public void SetSelecting(bool b)
    {
        _selecting = b;
    }
}
