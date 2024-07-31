using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class LightEdit : MonoBehaviour
{

    [Header("References")]

    [SerializeField]
    private Camera CameraRef;


    void Update()
    {

        Ray ray = CameraRef.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<TileLight>() != null)
                {
                    LightInspector.Instance.gameObject.SetActive(true);
                    LightInspector.Instance.Inspect(hit.collider.gameObject.GetComponent<TileLight>());
                }
            }
        }
    }

    private void OnEnable()
    {
        RoomMap.Instance.LightToggleEditGizmo(true);
    }


    private void OnDisable()
    {
        RoomMap.Instance.LightToggleEditGizmo(false);
        if (LightInspector.Instance)
            LightInspector.Instance.gameObject.SetActive(false);
        if(ColorPicker.Instance)
            ColorPicker.Instance.gameObject.SetActive(false);
    }
}
