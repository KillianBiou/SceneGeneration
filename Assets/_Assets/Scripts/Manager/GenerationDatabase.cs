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
            key += "1";
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

    public GameObject GetObject(string key, Vector3 basePos)
    {
        Debug.Log("Try Load");
        Debug.Log(key);
        Debug.Log(basePos);
        if (assetDatabase.ContainsKey(key))
        {
            try
            {
                string fullPath = Path.Combine(Application.dataPath, assetDatabase[key]);
                string objFullPath = Path.Combine(Application.dataPath, assetDatabase[key].Replace(".json", ".obj"));

                GameObjectSerializable parentSerializable = JsonUtility.FromJson<GameObjectSerializable>(File.ReadAllText(fullPath));
                Debug.Log("Loaded JSON");
                Debug.Log(parentSerializable);

                //GameObject loadedAsset = Resources.Load<GameObject>(Path.Combine(Application.dataPath, assetDatabase[key].Replace("json", "obj")));
                //Debug.Log("Loaded Asset");

                GameObject parent = new GameObject(key);
                parent.transform.parent = GameObject.FindGameObjectWithTag("Playground").transform;
                parent.transform.position = parentSerializable.position;
                parent.transform.rotation = parentSerializable.rotation;
                if (!(basePos.magnitude == 0))
                    parent.transform.position = basePos;

                ImportOptions options = new ImportOptions();
                options.buildColliders = true;
                options.colliderConvex = true;
                options.localPosition = parentSerializable.child[0].position;
                options.localEulerAngles = parentSerializable.child[0].rotation.eulerAngles;


                ObjectImporter.Instance.ImportModelAsync(key, objFullPath, parent.transform, options);

                parent.AddComponent<ParentCheck>();

                Debug.Log("Setup object concluded");


                return parent;
            }
            catch
            {
                return null;
            }
        }
        return null;
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
