using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquarePicker : MonoBehaviour
{
    [SerializeField]
    private ColorPicker _colorPicker;
    private bool _selecting;


    private void Awake()
    {
        _colorPicker.ColorChanged.AddListener(ResetCursor);
    }

    private void Update()
    {
        if (_selecting)
        {
            PickOnSquare();
        }
    }




    public void PickOnSquare()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();

        transform.position = Input.mousePosition;
        rectTransform.anchoredPosition = new Vector3(Mathf.Clamp(rectTransform.anchoredPosition.x, 0, 135), Mathf.Clamp(rectTransform.anchoredPosition.y, 0, 135), 0);

        if (_colorPicker)
        {
            //_colorPicker.SetS(gameObject.transform.localPosition.y / transform.parent.GetComponent<RectTransform>().rect.height);
            //_colorPicker.SetV(gameObject.transform.localPosition.x / transform.parent.GetComponent<RectTransform>().rect.width);
            _colorPicker.SetS((gameObject.transform.localPosition.x +65) / 130);
            _colorPicker.SetV((gameObject.transform.localPosition.y +65) / 130);
        }
    }


    private void ResetCursor(Color c)
    {
        float h, s, v;
        Color.RGBToHSV(c, out h, out s, out v);

        gameObject.transform.localPosition = new Vector3(s*130-65, v*130-65, 0);
    }


    public void SetSelecting(bool b)
    {
        _selecting = b;
    }
}
