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

    private GameObject selStart, selEnd;
    private List<TileObject> selection;

    // Start is called before the first frame update
    void Start()
    {
        llights = new List<GameObject>();
        lightGizmo = Instantiate(lightPrefab);
        lightGizmo.SetActive(false);

        lightInspector.SetActive(false);


        selection = new List<TileObject>();
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
                        selStart = hit.collider.gameObject;
                        selEnd = hit.collider.gameObject;
                        selection.Clear();
                        selection.Add(selStart.GetComponent<TileObject>());

                        hit.collider.GetComponent<TileObject>().ShowSelected();
                    }
                }
            }

            if(Input.GetMouseButtonUp(0))
            {
                if (Physics.Raycast(ray, out hit, 100))
                {
                    if (hit.collider.gameObject.GetComponent<TileObject>() != null)
                    {
                        GameObject last = hit.collider.gameObject;
                        if(selStart == last)
                        {
                            selStart.GetComponent<TileObject>().TileClick();
                            selStart.GetComponent<TileObject>().ShowButton();
                            selStart = null;
                            selEnd = null;
                            return;
                        }


                        /*
                        for (int i = (int)Mathf.Min(selectedTile.transform.position.x, last.transform.position.x); i <= (int)Mathf.Max(selectedTile.transform.position.x, last.transform.position.x); i++)
                        {
                            for (int j = (int)Mathf.Min(selectedTile.transform.position.z, last.transform.position.z); j <= (int)Mathf.Max(selectedTile.transform.position.z, last.transform.position.z); j++)
                            {
                                roomMap.Retile(i, j, false);
                            }
                        }*/

                        foreach (TileObject t in selection)
                            roomMap.Retile((int)t.transform.position.x, (int)t.transform.position.z, false);

                        foreach (TileObject t in selection)
                            t.ShowButton();


                        selStart = null;
                        selEnd = null;
                        return;
                    }
                }
            }

            if (selStart == null)
                return;


            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<TileObject>() != null)
                {
                    if (selEnd == hit.collider.gameObject)
                        return;


                    //Debug.Log("fff : " + selection.Count);

                    foreach (TileObject t in selection)
                        t.HideSelected();

                    selection = roomMap.GetTiles(selection, selStart.gameObject.transform.position, hit.collider.transform.position);

                    foreach (TileObject t in selection)
                        t.ShowSelected();

                    selEnd = hit.collider.gameObject;

                }
            }

            return;
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
            return;
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
