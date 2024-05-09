using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MouseGenerative2D : MonoBehaviour
{

    private GenerativeInspector materialInspector;
    public MatInfo matInfo;


    private GenerativeTexture genTex;


    void Awake()
    {
        materialInspector = FindObjectOfType<GenerativeInspector>();
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject() || !materialInspector)
            return;


        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<Renderer>() != null)
                {
                    matInfo.materials = hit.collider.gameObject.GetComponent<Renderer>().materials;
                    materialInspector.gameObject.SetActive(true);
                    materialInspector.InspectMat();
                }
            }
            else
            {
                materialInspector.gameObject.SetActive(false);
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


    private void ApplyMat(Material[] mats)
    {
        if (matInfo.selectedId == -1)
            return;

        foreach (Material mat in mats)
        {
            mat.SetTexture("_Texture2D", matInfo.materials[matInfo.selectedId].GetTexture("_Texture2D"));
            mat.SetVector("_position_offset", matInfo.materials[matInfo.selectedId].GetVector("_position_offset"));
            mat.SetVector("_Scaling", matInfo.materials[matInfo.selectedId].GetVector("_Scaling"));
            mat.SetVector("_Rotation", matInfo.materials[matInfo.selectedId].GetVector("_Rotation"));

            Texture norm = matInfo.materials[matInfo.selectedId].GetTexture("_Normal_Map");
            if (matInfo.materials[matInfo.selectedId].GetFloat("_use_normal") == 1.0)
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
