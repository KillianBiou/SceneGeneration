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

    private void Awake()
    {
        Instance = this;

        if (!File.Exists(DatabaseLocation))
        {
            File.Create(DatabaseLocation);
        }
        else
        {
            LoadDatabase();
        }

        /*List<string> toRemove = new List<string>();

        foreach(KeyValuePair<string, string> entry in assetDatabase)
        {
            if (!File.Exists(entry.Value))
                toRemove.Add(entry.Key);
        }
        foreach(string remove in toRemove)
        {
            assetDatabase.Remove(remove);
        }
        SaveDatabase();*/
    }

    private void Start()
    {
        Debug.Log("STATIC INSTANCE :" + Instance.name);
        CheckGenerationEntry();
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
                if(!(basePos.magnitude == 0))
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

    public void CheckGenerationEntry()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(Application.dataPath, "Models"));

        // Remove phantom entry
        List<string> toRemove = new List<string>();
        foreach (string assetName in assetDatabase.Keys)
        {
            if (!Directory.Exists(Path.Combine(directoryInfo.FullName, assetName)))
            {
                Debug.Log(assetName + " missing, removing !");
                //assetDatabase.Remove(assetName);
                toRemove.Add(assetName);
            }
        }
        foreach (string assetName in toRemove)
            assetDatabase.Remove(assetName);


        // Add Missing
        List<(string, string)> toAdd = new List<(string, string)>();
        foreach (DirectoryInfo info in directoryInfo.GetDirectories())
        {
            string targetFullPath = Path.Combine(info.FullName, info.Name + ".json");
            if (!assetDatabase.ContainsKey(info.Name) && File.Exists(targetFullPath))
            {
                Debug.Log("Found " + info.Name + " unregistered, adding to db");
                toAdd.Add((info.Name, targetFullPath));
            }
        }
        foreach ((string, string) assetName in toAdd)
            assetDatabase.Add(assetName.Item1, assetName.Item2);

        SaveDatabase();
    }

    public void SaveGeneratedAsset(GameObject gameobject, string path)
    {
        Debug.Log(gameobject.name);

        GameObjectSerializable parentSerializable = new GameObjectSerializable();
        parentSerializable.assetName = gameobject.name;
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

        string savingPath = path.Substring(0, path.Length - (gameobject.name + ".obj").Length);

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

    public void LoadDatabase()
    {
        Debug.Log(DatabaseLocation);
        if (!File.Exists(DatabaseLocation))
        {
            File.Create(DatabaseLocation);
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
