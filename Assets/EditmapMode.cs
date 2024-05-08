using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;



public enum EDIT_STATE
{
    NULL = 0,
    GROUND = 1,
    LIGHT = 2,
    EDITLIGHT = 3,
    SKY = 4
}



public class EditmapMode : MonoBehaviour
{
    [HideInInspector]
    public EDIT_STATE state;


    public RoomMap roomMap;

    public GameObject lightPrefab, lightInspector;


    private GameObject lightGizmo;
    private List<GameObject> llights; //SAVE THIS

    // Start is called before the first frame update
    void Start()
    {
        llights = new List<GameObject>();
        lightGizmo = Instantiate(lightPrefab);
        lightGizmo.SetActive(false);

        lightInspector.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {


        if (state == EDIT_STATE.NULL)
            return;

        if (state == EDIT_STATE.SKY)
            return;





        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (state == EDIT_STATE.GROUND)
        {
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
        }

        if (state == EDIT_STATE.LIGHT)
        {
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
                        GameObject last = Instantiate(lightPrefab);
                        llights.Add(last);
                        last.transform.position = hit.point;
                        last.GetComponent<TileLight>().editmap = this;
                        last.GetComponent<TileLight>().ActivateRemover();
                    }
                }
                else
                    lightGizmo.SetActive(false);
            }
        }


        if (state == EDIT_STATE.EDITLIGHT)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.collider.gameObject.GetComponent<TileLight>() != null)
                    {
                        lightInspector.GetComponent<LightInspector>().Inspect(hit.collider.gameObject.GetComponent<TileLight>().lightObj);
                    }
                }
            }
        }
    }


    public void RemoveLight(GameObject me)
    {
        llights.Remove(me);
    }

    private void OnDisable()
    {
        ExitPrevious();
    }


    public void ExitPrevious()
    {
        switch(state)
        {
            case EDIT_STATE.GROUND:
                roomMap.ExitEdit();
                break;
            case EDIT_STATE.LIGHT:
                lightGizmo.SetActive(false);
                foreach (GameObject l in llights)
                {
                    l.GetComponent<TileLight>().DeactivateRemover();
                }
                break;
            case EDIT_STATE.SKY:
                break;
            case EDIT_STATE.EDITLIGHT:
                foreach (GameObject l in llights)
                {
                    l.GetComponent<TileLight>().DeactivateEdit();
                }
                lightInspector.SetActive(false);
                break;
        }

        state = EDIT_STATE.NULL;
    }


    public void EnterGroundEdit()
    {
        ExitPrevious();

        if (!roomMap.isEditing)
            roomMap.EnterEdit();

        state = EDIT_STATE.GROUND;
    }

    public void EnterLightEdit()
    {
        ExitPrevious();


        foreach(GameObject l in llights)
        {
            l.GetComponent<TileLight>().ActivateRemover();
        }

        state = EDIT_STATE.LIGHT;
    }

    public void EnterLightInspector()
    {
        ExitPrevious();


        foreach (GameObject l in llights)
        {
            l.GetComponent<TileLight>().ActivateEdit();
        }
        lightInspector.SetActive(true);

        state = EDIT_STATE.EDITLIGHT;
    }


}
