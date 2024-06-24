using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ModelLibraryDragDropHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject modelPlaceholder;
    [SerializeField]
    private LayerMask banLayer;


    private GameObject modelInstance;

    public void BeginDrag(PointerEventData data)
    {
        modelInstance = Instantiate(modelPlaceholder);
    }

    public void Drag(PointerEventData data)
    {
        if (modelInstance)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, ~banLayer))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    modelInstance.SetActive(false);
                    return;
                }
                else
                {
                    modelInstance.SetActive(true);
                }
            }

            modelInstance.transform.position = hit.point;
        }
    }

    public void EndDrag(PointerEventData data)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool onUI = false;
        if (Physics.Raycast(ray, out hit, 100, ~banLayer))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                onUI = true;
            }
        }

        if (!onUI)
        {
            GenerationDatabase.Instance.SpawnObject(data.selectedObject.name, hit.point, Quaternion.FromToRotation(-Vector3.forward, hit.normal), Vector3.one);
            Debug.Log("Loaded : " + data.selectedObject.name);
        }

        if (modelInstance)
        {
            Destroy(modelInstance);
        }
    }
}
