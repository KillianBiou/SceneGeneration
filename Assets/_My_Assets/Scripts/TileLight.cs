using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

public class TileLight : MonoBehaviour
{
    public Light lightRef;


    public GameObject remover, editGizmo, lightObj;
    public GameObject vrGizmo, vrRemover;
    public EditmapMode editmap;



    private void OnDestroy()
    {
        if(RoomMap.Instance)
            RoomMap.Instance.RemoveLightTile(gameObject);
    }




    // GIZMOS DE/ACTIVATORS

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



    // GETTERS

    public float GetLightIntensity()
    {
        return lightRef.intensity;
    }

    public float GetLightHeight()
    {
        return lightObj.transform.position.y;
    }

    public Color GetLightColor()
    {
        return lightRef.color;
    }

    public float GetLightRange()
    {
        return lightRef.range;
    }



    // SETTERS

    public void SetLightIntensity(float i)
    {
        lightRef.intensity = i;
    }

    public void SetLightHeight(float height)
    {
        lightObj.transform.localPosition = new Vector3(0,height,0);
    }

    public void SetLightColor(Color c)
    {
        lightRef.color = c;
    }

    public void SetLightRange(float r)
    {
        lightRef.range = r;
    }
}
