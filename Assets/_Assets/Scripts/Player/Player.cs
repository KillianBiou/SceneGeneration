using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Generation parameters")]
    [SerializeField, Tooltip("Path to input image(s).")]
    private List<Texture2D> images;
    [SerializeField]
    private Material generatedMat;

    [Header("Debug Parameters")]
    [SerializeField]
    private GameObject debugObject;
    [ReadOnly, SerializeField]
    private bool generationLock;

    [Header("References")]
    [SerializeField]
    private Transform camera;
    [SerializeField]
    private Transform playgroundHolder;

    private Vector3 instanciationPoint;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetLookPos();
        }
    }

    private void GetLookPos()
    {
        if(images.Count > 0 && !generationLock)
        {
            RaycastHit hit;
            if (Physics.Raycast(camera.position, camera.forward, out hit, 100f))
            {
                string imagePath = AssetDatabase.GetAssetPath(images[0]);
                instanciationPoint = hit.point;

                generationLock = true;
                TripoSRForUnity.Instance.RunTripoSR(InstantiationCallback, imagePath);
            }
        }
    }

    public int InstantiationCallback(string objPath)
    {
        GameObject importedObj = AssetDatabase.LoadAssetAtPath<GameObject>(objPath);

        if (importedObj != null)
        {
            GameObject instantiatedObj = Instantiate(importedObj, instanciationPoint + Vector3.up, Quaternion.identity, playgroundHolder);
            instantiatedObj.name = importedObj.name;
            instantiatedObj.transform.GetChild(0).rotation = Quaternion.Euler(new Vector3(-90f, -90f, 0f));

            Debug.Log("Instantiated GameObject prefab: " + instantiatedObj.name);


            GameObject meshObj = instantiatedObj.transform.GetChild(0).gameObject;
            meshObj.GetComponent<Renderer>().material = generatedMat;

            MeshCollider mc = meshObj.AddComponent<MeshCollider>();
            mc.convex = true;

            OriginPlacement OP = meshObj.AddComponent<OriginPlacement>();
            OP.ReplaceOrigin();
        }

        images.RemoveAt(0);
        generationLock = false;
        return 0;
    }

    private void OnDrawGizmos()
    {
        if (camera)
        {
            Debug.DrawRay(camera.position, camera.forward * 100, Color.red);
        }
    }
}
