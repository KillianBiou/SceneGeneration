using AsImpL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARSubsystems;
using UnityGLTF;



[Serializable]
public class GeneratedModelSerializable
{
    public string meshName = "", imgName;
    public string objectDescription;
    public Vector3 boundingBoxSize;

    public GameObjectSerializable goData;

    public GeneratedModelSerializable(string directoryPath)
    {
        string[] search = Directory.GetFiles(directoryPath, "*.png");
        if(search.Length>0)
            imgName = Path.GetFileName(search[0]);

        search = Directory.GetFiles(directoryPath, "*.glb");
        if (search.Length > 0)
            meshName = Path.GetFileName(search[0]);

        search = Directory.GetFiles(directoryPath, "*.obj");
        if (search.Length > 0)
            meshName = Path.GetFileName(search[0]);

        objectDescription = Path.GetFileName(directoryPath);
    }

    public void GenerateGoData(GameObject go, bool recursive = false)
    {
        goData = new GameObjectSerializable(go, recursive);
    }
}

public class GeneratedMaterialSerializable
{
    public string Albedo, Normal;

    public GeneratedMaterialSerializable(string directoryPath)
    {
        string[] search = Directory.GetFiles(directoryPath, "*_T.png");
        if (search.Length > 0)
            Albedo = Path.GetFileName(search[0]);

        search = Directory.GetFiles(directoryPath, "*_N.png");
        if (search.Length > 0)
            Normal = Path.GetFileName(search[0]);
    }
}



[Serializable]
public class GameObjectSerializable
{
    public string assetName;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public int childNumber;
    public GameObjectSerializable[] child;

    public GameObjectSerializable()
    {
        assetName = "";
        position = Vector3.zero;
        rotation = Quaternion.identity;
        childNumber = 0;
        child = new GameObjectSerializable[0];
    }

    public GameObjectSerializable(GameObject go, bool recursive = false)
    {
        assetName = go.name;
        position = go.transform.localPosition;
        rotation = go.transform.localRotation;
        scale = go.transform.localScale;

        if (recursive)
        {
            childNumber =go.transform.childCount;
            List<GameObjectSerializable> childs = new List<GameObjectSerializable>();
            for(int i = 0; i<childNumber; i++)
            {
                childs.Add(new GameObjectSerializable(go.transform.GetChild(i).gameObject, recursive));
            }
            child = childs.ToArray();
        }
        else
        {
            childNumber = 0;
            child = null;
        }     
    }

    public void LoadToGameobject(GameObject go)
    {
        go.name = assetName;
        go.transform.localPosition = position;
        go.transform.localRotation = rotation;
        go.transform.localScale = scale;

        foreach (GameObjectSerializable gos in child)
            gos.LoadToGameobject(go.transform.GetChild(0).gameObject);
    }

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
    [SerializeField]
    private bool GLBmode = true;

    [Header("References")]
    [SerializeField]
    private Transform playgroundHolder;
    [SerializeField]
    private Material baseMat;
    [SerializeField]
    public GameObject XrRig;

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
            LoadSceneFromFile(debugScenePath);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            //GenerationDatabase.Instance.GetObject(debugLoadObjectName);
            //Instantiate(Resources.Load<GameObject>(debugLoadObjectName), GameObject.FindGameObjectWithTag("Playground").transform);
        }
    }

    public bool libAdd = false;
    public void SetLibAdd(bool b)
    {
        libAdd = b;
    }


    public int InstantiationCallback_GLB(string objPath)
    {
        GlobalVariables.Instance.SetCurrentPhase(ApplicationStatePhase.MODEL_IMPORT);
        if (images.Count > 0)
            images.RemoveAt(0);
        
        generationLock = false;

        if (GlobalVariables.Instance.isInVr || libAdd)
            return 0;

        Cursor3D.instance.blocked = false;
        Cursor3D.instance.toggleLoadFX(false);
        GenerationDatabase.Instance.SpawnObject(Path.GetFileName(objPath), Cursor3D.instance.transform.position, Quaternion.identity, Vector3.one);
        Debug.Log("Object instancie !!");
        return 0;
    }

    public IEnumerator ReplacementCoroutine(Transform parent, string objPath)
    {
        while(parent.childCount == 0)
            yield return new WaitForSeconds(0.1f);

        OriginPlacement OP = parent.GetChild(0).gameObject.AddComponent<OriginPlacement>();
        OP.ReplaceOrigin();
        Debug.Log("sent �F " + objPath.Substring("Assets/".Length));
        GlobalVariables.Instance.EndOfGen();
        GenerationDatabase.Instance.SaveGeneratedAsset(parent.gameObject, objPath.Substring("Assets/".Length));
    }


    public void SaveScene()
    {
        if (!Directory.Exists(sceneSavePath)) Directory.CreateDirectory(sceneSavePath);

        string savePath = Path.Combine(sceneSavePath, Directory.GetDirectories(sceneSavePath).Length.ToString());

        Directory.CreateDirectory(savePath);

        File.WriteAllText(Path.Combine(savePath, "SceneSave.json"), JsonUtility.ToJson(GetGameObjectSaveData()));

        Debug.Log("Scene saved at " +  savePath);
    }


    public GameObjectSerializable GetGameObjectSaveData()
    {
        GameObject parent = GameObject.FindGameObjectWithTag("Playground");

        GameObjectSerializable parentSerializable = new GameObjectSerializable();
        parentSerializable.assetName = parent.name;
        parentSerializable.position = parent.transform.localPosition;
        parentSerializable.rotation = parent.transform.localRotation;
        parentSerializable.childNumber = parent.transform.childCount;
        parentSerializable.child = new GameObjectSerializable[parent.transform.childCount];

        int i = 0;

        List< GameObjectSerializable > objList = new List< GameObjectSerializable >();
        foreach (Transform child in parent.transform)
            objList.Add(new GameObjectSerializable(child.gameObject, false));
        
        parentSerializable.child = objList.ToArray();
        parentSerializable.childNumber = parentSerializable.child.Length;

        return parentSerializable;
    }


    public void LoadSceneFromFile(string scenePath)
    {
        if (!File.Exists(scenePath))
            return;

        GameObjectSerializable parentSerializable = JsonUtility.FromJson<GameObjectSerializable>(File.ReadAllText(scenePath));

        LoadScene(parentSerializable);

        Debug.Log("Scene loaded from " + scenePath);
    }



    private GameObjectSerializable toImport;
    private int ID;
    public void LoadScene(GameObjectSerializable parentSerializable)
    {
        foreach(Transform child in GameObject.FindGameObjectWithTag("Playground").transform)
        {
            Destroy(child.gameObject);
        }
        toImport = parentSerializable;
        ID = -1;

        ImportAll(null);
    }

    public int ImportAll(GameObject go)
    {
        ID++;
        if(ID < toImport.child.Length)
            GenerationDatabase.Instance.SpawnObject(toImport.child[ID].assetName, toImport.child[ID].position, toImport.child[ID].rotation, toImport.child[ID].scale, null, ImportAll);
        return 0;
    }

    public void AddImage(string imageFullpath, Vector3 generationPos)
    {
        instanciationPoint = generationPos;
        generationLock = true;
        if(GLBmode)
            TripoSRForUnity.Instance.RunTripoSR_GLB(InstantiationCallback_GLB, imageFullpath);
        else
            TripoSRForUnity.Instance.RunTripoSR_GLB(InstantiationCallback_GLB, imageFullpath, "obj");
    }

}
