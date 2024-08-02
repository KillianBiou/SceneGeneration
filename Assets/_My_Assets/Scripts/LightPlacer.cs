using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class LightPlacer : MonoBehaviour
{
    [Header ("References")]

    [SerializeField]
    private Camera CameraRef;
    [SerializeField]
    private GameObject lightGizmo;


    void Start()
    {
        lightGizmo = Instantiate(lightGizmo);
        lightGizmo.SetActive(false);
    }

    void Update()
    {
        Ray ray = CameraRef.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            lightGizmo.SetActive(false);
            return;
        }


        if (Physics.Raycast(ray, out hit, 100))
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                lightGizmo.SetActive(false);
                return;
            }

            if (hit.collider.gameObject.GetComponent<TileLight>() != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    Destroy(hit.transform.gameObject);
                }
                else
                {
                    lightGizmo.SetActive(false);
                    return;
                }
            }

            if (hit.collider.gameObject.GetComponent<Renderer>() != null)
            {
                lightGizmo.SetActive(true);
                lightGizmo.transform.position = hit.point;
                if (Input.GetMouseButtonDown(0))
                {
                    RoomMap.Instance.AddLightTile(hit.point, 1.6f);
                }
            }
            else
                lightGizmo.SetActive(false);
        }
    }


    private void OnEnable()
    {
        RoomMap.Instance.LightTogglePlaceGizmo(true);
    }

    private void OnDisable()
    {
        RoomMap.Instance.LightTogglePlaceGizmo(false);
        if(lightGizmo)
            lightGizmo.SetActive(false);
    }
}
