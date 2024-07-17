using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using UnityEngine;
using UnityEngine.EventSystems;

public class TilePlacer : MonoBehaviour
{
    [Header("References")]

    [SerializeField]
    private Camera CameraRef;

    private GameObject selStart, selEnd;
    private List<TileObject> selection;


    void Start()
    {
        selection = new List<TileObject>();
    }


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

        if (Input.GetMouseButtonUp(0))
        {
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<TileObject>() != null)
                {
                    GameObject last = hit.collider.gameObject;
                    if (selStart == last)
                    {
                        selStart.GetComponent<TileObject>().TileClick();
                        selStart.GetComponent<TileObject>().ShowGizmoEditGround();
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
                        RoomMap.Instance.Retile((int)t.transform.position.x, (int)t.transform.position.z, false);

                    foreach (TileObject t in selection)
                        t.ShowGizmoEditGround();


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

                selection = RoomMap.Instance.GetTiles(selection, selStart.gameObject.transform.position, hit.collider.transform.position);

                foreach (TileObject t in selection)
                    t.ShowSelected();

                selEnd = hit.collider.gameObject;
            }
        }
    }

    private void OnEnable()
    {
        if(RoomMap.Instance)
            RoomMap.Instance.TileToggleEdit(true);
    }

    private void OnDisable()
    {
        if (RoomMap.Instance)
            RoomMap.Instance.TileToggleEdit(false);
    }
}
