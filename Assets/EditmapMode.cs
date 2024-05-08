using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditmapMode : MonoBehaviour
{
    private bool lightMode;

    public GameObject lightPrefab;
    private GameObject lightGizmo;

    private List<GameObject> llights;

    // Start is called before the first frame update
    void Start()
    {
        llights = new List<GameObject>();
        lightGizmo = Instantiate(lightPrefab);
        lightGizmo.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<TileObject>() != null)
                {
                    hit.collider.gameObject.GetComponent<TileObject>().TileClick();
                }
            }
        }

        if (lightMode)
        {
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<Renderer>() != null)
                {
                    lightGizmo.SetActive(true);
                    lightGizmo.transform.position = hit.point;
                    if (Input.GetMouseButtonDown(0))
                    {
                        llights.Add(Instantiate(lightPrefab));
                        llights[llights.Count - 1].transform.position = hit.point;
                    }
                }
                else
                    lightGizmo.SetActive(false);
            }
        }
    }

    public void ToggleLightEdit()
    {
        if(lightMode)
            lightMode = false;
        else
            lightMode = true;
    }


}
