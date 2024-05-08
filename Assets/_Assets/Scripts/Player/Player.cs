using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.RuntimeSceneSerialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GameObjectSerializable
{
    public string assetName;
    public Vector3 position;
    public Quaternion rotation;
    public int childNumber;
    public GameObjectSerializable[] child;
}

public class Player : MonoBehaviour
{
    [Header("Scene Parameters")]
    [SerializeField]
    private string sceneSavePath;

    [Header("Generation parameters")]
    [SerializeField, Tooltip("Path to input image(s).")]
    private List<string> images;
    [SerializeField]
    private Material generatedMat;

    [Header("Debug Parameters")]
    [SerializeField]
    private bool debugMode;
    [SerializeField]
    private GameObject debugObject;
    [SerializeField]
    private string debugLoadObjectName;
    [SerializeField]
    private string debugScenePath;
    [ReadOnly, SerializeField]
    private bool generationLock;

    [Header("References")]
    [SerializeField]
    private Transform camera;
    [SerializeField]
    private Transform playgroundHolder;
    [SerializeField]
    private Material baseMat;

    private Vector3 instanciationPoint;

    public static Player Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        sceneSavePath = Path.Combine(Application.dataPath, sceneSavePath);
    }

    private void Update()
    {
        if (!debugMode)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            SaveScene();
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            LoadScene(debugScenePath);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            //GenerationDatabase.Instance.GetObject(debugLoadObjectName);
            Instantiate(Resources.Load<GameObject>(debugLoadObjectName), GameObject.FindGameObjectWithTag("Playground").transform);
        }
    }

    public void GetLookPos()
    {
        if(images.Count > 0 && !generationLock)
        {
            RaycastHit hit;
            if (Physics.Raycast(camera.position, camera.forward, out hit, 100f))
            {
                string imagePath = images[0]; //AssetDatabase.GetAssetPath(images[0]);
                instanciationPoint = hit.point;

                generationLock = true;
                TripoSRForUnity.Instance.RunTripoSR(InstantiationCallback, imagePath);
            }
        }
    }

    public int InstantiationCallback(GameObject obj)
    {
        if (obj != null)
        {
            GameObject instantiatedObj = Instantiate(obj, instanciationPoint + Vector3.up, Quaternion.identity, playgroundHolder);
            instantiatedObj.name = obj.name;
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

            GenerationDatabase.Instance.SaveGeneratedAsset(instantiatedObj, objPath);
        }


        if(images.Count > 0)
            images.RemoveAt(0);
        generationLock = false;
        return 0;
    }


    public void SaveScene()
    {
        GameObject parent = GameObject.FindGameObjectWithTag("Playground");
        
        GameObjectSerializable parentSerializable = new GameObjectSerializable();
        parentSerializable.assetName = parent.name;
        parentSerializable.position = parent.transform.position;
        parentSerializable.rotation = parent.transform.rotation;
        parentSerializable.childNumber = parent.transform.childCount;
        parentSerializable.child = new GameObjectSerializable[parent.transform.childCount];

        int i = 0;
        foreach (Transform child in parent.transform)
        {
            GameObjectSerializable childTemp = new GameObjectSerializable();
            childTemp.assetName = child.name;
            childTemp.position = child.transform.position;
            childTemp.rotation = child.transform.rotation;
            childTemp.childNumber = 1;

            //
            GameObjectSerializable mesh = new GameObjectSerializable();
            mesh.assetName = child.GetChild(0).name;
            mesh.position = child.GetChild(0).transform.position;
            mesh.rotation = child.GetChild(0).transform.rotation;
            mesh.childNumber = 0;
            mesh.child = null;
            //

            childTemp.child = new GameObjectSerializable[1];
            childTemp.child[0] = mesh;

            parentSerializable.child[i] = childTemp;
            i++;
        }

        parentSerializable.childNumber = parent.transform.childCount;


        if (!Directory.Exists(sceneSavePath)) Directory.CreateDirectory(sceneSavePath);

        string savePath = Path.Combine(sceneSavePath, Directory.GetDirectories(sceneSavePath).Length.ToString());

        Directory.CreateDirectory(savePath);

        File.WriteAllText(Path.Combine(savePath, "SceneSave.json"), JsonUtility.ToJson(parentSerializable));

        Debug.Log("Scene saved at " +  savePath);
    }

    public void LoadScene(string scenePath)
    {
        if (!File.Exists(scenePath))
            return;

        GameObjectSerializable parentSerializable = JsonUtility.FromJson<GameObjectSerializable>(File.ReadAllText(scenePath));

        foreach (GameObjectSerializable child in parentSerializable.child)
        {
            GameObject temp = Instantiate(GenerationDatabase.Instance.GetObject(child.assetName), child.position, child.rotation, GameObject.FindGameObjectWithTag("Playground").transform);

            temp.transform.GetChild(0).position = child.child[0].position;
            temp.transform.GetChild(0).rotation = child.child[0].rotation;
            temp.transform.GetChild(0).GetComponent<Renderer>().material = baseMat;
        }

        Debug.Log("Scene loaded from " + scenePath);
    }

    public void AddImage(Texture2D imagePath)
    {
        //images.Add(imagePath);
    }

    public void AddImage(string imagePath)
    {
        //Texture2D tex = new Texture2D(2, 2);
        //tex.LoadImage(File.ReadAllBytes(imagePath));
        images.Add(imagePath);
    }
    public void AddImage(string imagePath, Vector3 generationPos)
    {
        instanciationPoint = generationPos;

        generationLock = true;
        TripoSRForUnity.Instance.RunTripoSR(InstantiationCallback, imagePath);
    }

    private void OnDrawGizmos()
    {
        if (camera)
        {
            Debug.DrawRay(camera.position, camera.forward * 100, Color.red);
        }
    }
}
