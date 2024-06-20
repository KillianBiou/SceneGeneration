using AsImpL;
using AYellowpaper.SerializedCollections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using UnityGLTF;

public class GenerationDatabase : MonoBehaviour
{
    public static string DatabaseLocation = Path.Combine(Application.dataPath, "AssetsDatabase.save");
    public static GenerationDatabase Instance;

    [SerializedDictionary("Asset name", "Asset Path")]
    [SerializeField]
    private SerializedDictionary<string, string> assetDatabase;

    [SerializedDictionary("Material name", "Material Path")]
    [SerializeField]
    private SerializedDictionary<string, string> MaterialDatabase;

    public static event Action OnDatabaseUpdated;

    public UnityEvent<GameObject> StartLoadingOBJ;

    public List<string> modelsFolders;


    private void Awake()
    {
        Instance = this;
        LoadDatabase();
    }

    private void Start()
    {
        Debug.Log("STATIC INSTANCE :" + Instance.name);
        if(modelsFolders != null)
            CheckEntryAtFolders(modelsFolders);
        //CheckGenerationEntry();
    }

    public void AddEntry(string key, string value)
    {
        if (assetDatabase.ContainsKey(key))
            key += "_01";
        assetDatabase.Add(key, value);
        SaveDatabase();
    }
    
    public GameObject GetObject(string key)
    {
        return GetObject(key, Vector3.zero);
    }

    public async void GetObjectEvent(string key)
    {
        StartLoadingOBJ.Invoke(GetObject(key, Vector3.zero));
    }

    //try instanciating a gameobject version of a 3D model in library, return null if key not in lib
    public GameObject GetObject(string key, Vector3 targetPos)
    {
        Debug.Log("Trying to  Spawn " + key + " at " + targetPos);
        if (!assetDatabase.ContainsKey(key))
        {
            Debug.Log("XXX - KEY NOT FOUND - XXX");
            return null;
        }

        string fullPath = Path.Combine(Application.dataPath, assetDatabase[key]);
        GeneratedModelSerializable data = JsonUtility.FromJson<GeneratedModelSerializable>(File.ReadAllText(fullPath));
        Debug.Log("Loaded - " + Path.GetFileName(fullPath));

        string meshFullPath = Path.Combine(Application.dataPath, Path.GetDirectoryName(assetDatabase[key]), data.meshName);

        Debug.Log("Spawning " + meshFullPath);
        GameObject last = new GameObject("MESH - " + key);
        last.transform.SetParent(GameObject.FindGameObjectWithTag("Playground").transform);

        if (meshFullPath.Contains(".obj"))
        {
            Debug.Log("c'est un obj !");
            AsImpL.ImportOptions options = new AsImpL.ImportOptions();
            options.buildColliders = true;
            options.colliderConvex = true;

            ObjectImporter.Instance.ImportModelAsync("mesh_" + key, meshFullPath, last.transform, options);

            //last.AddComponent<ParentCheck>();
        }
        else if (meshFullPath.Contains(".glb"))
        {
            Debug.Log("c'est un glb !");
            GLTFComponent glb = last.AddComponent<GLTFComponent>();

            glb.Collider = GLTFSceneImporter.ColliderType.MeshConvex;
            glb.ImportNormals = GLTFImporterNormals.Calculate;
            glb.GLTFUri = Path.Combine(meshFullPath);
            /*
            GameObject mem = last.transform.GetChild(0).gameObject;

            last.transform.GetChild(0).GetChild(0).SetParent(last.transform);
            Destroy(mem);
            last.transform.GetChild(0).name = "mesh_" + key;*/
        }

        Debug.Log("Setup object concluded");

        // FIRST TIME ?
        if (data.goData.assetName == "")
        {
            //gestion de la premiere apparition en 0, 0, 0

            //data.goData = new GameObjectSerializable(last.transform.GetChild(0).gameObject, true);
            data.goData = new GameObjectSerializable(last, true);
            File.WriteAllText(fullPath, JsonUtility.ToJson(data));
            Debug.Log("First time spawned Saved to json.");
        }
        else
        {
            Debug.Log("Loading pivot points informations from json.");
            //data.goData.LoadToGameobject(last.transform.GetChild(0).gameObject);
            data.goData.LoadToGameobject(last);
        }

        last.transform.localPosition = targetPos;
        return last;
    }






    public void CheckEntryAtFolders(List<string> folderName)
    {
        RmBadEntry();

        foreach ( string name in folderName )
            CheckNewEntry(name);

        SaveDatabase();
    }

    // Add all found entry in folder and subfolder level 1
    public void CheckNewEntry(string folderName = "Models")
    {
        if (!Directory.Exists(Path.Combine(Application.dataPath, folderName)))
            return;

        Debug.Log("Looking for entry in " + Path.Combine(Application.dataPath, folderName));
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Application.dataPath, folderName));

        foreach (DirectoryInfo info in directoryInfo.GetDirectories())
        {
            string targetFullPath = Path.Combine(info.FullName, info.Name + ".json");
            //Debug.Log("---- Looking for entry " + targetFullPath);
            if (!assetDatabase.ContainsKey(info.Name) && File.Exists(targetFullPath))
            {
                Debug.Log("Found " + info.Name + " unregistered, adding to db");
                assetDatabase.Add(info.Name, Path.Combine(folderName, info.Name, info.Name + ".json"));
            }
        }

    }

    // Remove phantom entry
    private void RmBadEntry()
    {
        List<string> toRemove = new List<string>();
        foreach (string assetName in assetDatabase.Keys)
        {
            if (!File.Exists(Path.Combine(Application.dataPath, assetDatabase[assetName])))
            {
                Debug.Log(Path.Combine(Application.dataPath, assetDatabase[assetName]) + " missing, removing !");
                toRemove.Add(assetName);
            }
        }
        foreach (string assetName in toRemove)
        {
            assetDatabase.Remove(assetName);
        }

    }


    public void SetupMeshFolder(string savePath)
    {
        UnityEngine.Debug.Log("Generating json at " + Path.Combine(savePath, Path.GetFileName(savePath) + ".json"));
        GeneratedModelSerializable data = new GeneratedModelSerializable(savePath);
        File.WriteAllText(Path.Combine(savePath, Path.GetFileName(savePath) + ".json"), JsonUtility.ToJson(data));
        AddEntry(Path.GetFileName(savePath), Path.Combine(savePath.Substring(Application.dataPath.Length+1, savePath.Length - Application.dataPath.Length-1), Path.GetFileName(savePath) + ".json"));
    }


    public void SaveGeneratedAsset(GameObject gameobject, string path)
    {
        Debug.Log(gameobject.name);

        GameObjectSerializable parentSerializable = new GameObjectSerializable();
        parentSerializable.assetName = Path.GetFileName(path);
        parentSerializable.position = gameobject.transform.position;
        parentSerializable.rotation = gameobject.transform.rotation;
        parentSerializable.childNumber = gameobject.transform.childCount;
        parentSerializable.child = new GameObjectSerializable[gameobject.transform.childCount];

        int i = 0;
        foreach (Transform child in gameobject.transform)
        {
            GameObjectSerializable childTemp = new GameObjectSerializable();
            childTemp.assetName = child.name;
            childTemp.position = child.transform.localPosition;
            childTemp.rotation = child.transform.localRotation;

            parentSerializable.child[i] = childTemp;
            i++;
        }

        parentSerializable.childNumber = gameobject.transform.childCount;

        string savingPath = Path.GetDirectoryName(path);

        if (!Directory.Exists(savingPath)) Debug.Log("ERROR FOLDER DOES NOT EXIST");

        string fullSavingPath = Path.Combine(savingPath, gameobject.name + ".json");
        Debug.Log("FULL SAVING PATH : " + fullSavingPath);
        File.WriteAllText(Path.Combine(Application.dataPath, fullSavingPath), JsonUtility.ToJson(parentSerializable));

        Debug.Log("Object pose saved");
        AddEntry(gameobject.name, fullSavingPath);
        OnDatabaseUpdated.Invoke();
    }

    public void SaveDatabase()
    {
        Debug.Log(DatabaseLocation);
        if (!File.Exists(DatabaseLocation))
        {
            File.Create(DatabaseLocation);
        }
        string data = JsonUtility.ToJson(assetDatabase);
        File.WriteAllText(DatabaseLocation, data);
    }

    //Load database from file, ccreate it if not found
    public void LoadDatabase()
    {
        Debug.Log("Looking for " + DatabaseLocation);
        if (!File.Exists(DatabaseLocation))
        {
            File.Create(DatabaseLocation);
            Debug.Log("Generated " + DatabaseLocation);
        }
        assetDatabase = JsonUtility.FromJson<SerializedDictionary<string, string>>(File.ReadAllText(DatabaseLocation));
    }

    public List<(string, string)> GetAssetsDict()
    {
        List<(string, string)> fullPaths = new List<(string, string)>();
        foreach (KeyValuePair<string, string> path in assetDatabase)
        {
            fullPaths.Add((path.Key, Path.Combine(Application.dataPath, path.Value)));
        }
        return fullPaths;
    }

    public List<string> GetAssetsFullPaths()
    {
        List<string> fullPaths = new List<string>();
        foreach(string path in assetDatabase.Values)
        {
            fullPaths.Add(Path.Combine(Application.dataPath, path));
        }
        return fullPaths;
    }
}
