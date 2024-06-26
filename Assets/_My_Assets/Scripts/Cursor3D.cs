using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cursor3D : MonoBehaviour
{

    static public Cursor3D instance;
    [SerializeField]
    private GameObject loadingFX;

    public bool blocked = false;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }



    private void Update()
    {
        if (blocked) return;


        if (EventSystem.current.IsPointerOverGameObject())
            return;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<Renderer>() != null)
                {
                    SetTransform(hit.point, hit.normal);
                }
            }
        }

    }


    public void SetTransform(Vector3 pos, Quaternion rot)
    {
        if (blocked) return;

        transform.position = pos;
        transform.rotation = rot;
    }
    public void SetTransform(Vector3 pos, Vector3 rot)
    {
        if (blocked) return;

        transform.position = pos;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, rot);
    }

    public void toggleLoadFX(bool b)
    {
        loadingFX.SetActive(b);
    }
}
