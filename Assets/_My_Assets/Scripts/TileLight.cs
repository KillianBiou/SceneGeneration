using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TileLight : MonoBehaviour
{

    //public float lightHeight, lightIntensity;
    public Light lightRef;


    public GameObject remover, editGizmo, lightObj;
    public GameObject vrGizmo, vrRemover;
    public EditmapMode editmap;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if(editmap)
            editmap.RemoveLight(gameObject);
    }

    public void ActivateRemover()
    {
        editGizmo.SetActive(false);
        remover.SetActive(true);
        vrRemover.SetActive(true);
        GetComponent<BoxCollider>().enabled = true;
    }
    public void DeactivateRemover()
    {
        remover.SetActive(false);
        vrRemover.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
    }


    public void ActivateEdit()
    {
        editGizmo.SetActive(true);
        vrGizmo.SetActive(true);
        GetComponent<BoxCollider>().enabled = true;
    }
    public void DeactivateEdit()
    {
        editGizmo.SetActive(false);
        vrGizmo.SetActive(false);
        GetComponent<BoxCollider>().enabled = false;
    }
}
