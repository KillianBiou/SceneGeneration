using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveableUI : MonoBehaviour
{

    private Canvas canvas;
    private Vector3 offset;



    private void Awake()
    {
        canvas = transform.parent.GetComponent<Canvas>();
    }

    public void DragHandler(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, pointerData.position, canvas.worldCamera, out position);

        transform.position = canvas.transform.TransformPoint(position) + offset;
    }

    public void BeginHandler(BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;

        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, pointerData.position, canvas.worldCamera, out position);

        offset = transform.position - canvas.transform.TransformPoint(position);
    }
}
