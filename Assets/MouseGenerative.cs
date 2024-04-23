using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting.Dependencies.Sqlite;

public class MouseGenerative : MonoBehaviour
{

    public GameObject Menu;
    public GameObject MaterialBubble;
    public MatInfo matInfo;


    private GenerativeTexture genTex;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<Renderer>() != null)
                    InspectMat(hit.collider.gameObject.GetComponent<Renderer>().materials);
            }
            else
            {
                Menu.SetActive(false);
                matInfo.materials = null;
                matInfo.selectedId = -1;
            }

        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<Renderer>() != null)
                    ApplyMat(hit.collider.gameObject.GetComponent<Renderer>().materials);
            }
        }
    }


    private void InspectMat(Material[] mats)
    {

        matInfo.materials = mats;
        matInfo.selectedId = 0;

        for (int i= Menu.transform.childCount; i > 0; i--)
        {
            Destroy(Menu.transform.GetChild(i-1).gameObject);
        }

        for (int i = 0; i < mats.Length; i++)
        {
            GameObject last = Instantiate(MaterialBubble);
            last.GetComponent<MatInspector>().Init(mats[i], matInfo, i);
            last.transform.SetParent(Menu.transform, false);
        }
        Menu.SetActive(true);
    }


    private void ApplyMat(Material[] mats)
    {
        if (matInfo.selectedId == -1)
            return;

        foreach (Material mat in mats)
        {
            mat.SetTexture("_Texture2D", matInfo.materials[matInfo.selectedId].GetTexture("_Texture2D"));
            mat.SetVector("_position_offset", matInfo.materials[matInfo.selectedId].GetVector("_position_offset"));
            mat.SetVector("_scale", matInfo.materials[matInfo.selectedId].GetVector("_scale"));

            Texture norm = matInfo.materials[matInfo.selectedId].GetTexture("_Normal_Map");
            if (norm != null)
            {
                mat.SetTexture("_Normal_Map", norm);
                mat.SetFloat("_use_normal", 1);
            }
            else
                mat.SetFloat("_use_normal", 0);


            matInfo.Changed.Invoke();
        }
    }
}
