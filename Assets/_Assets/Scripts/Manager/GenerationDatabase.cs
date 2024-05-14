using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
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

        List<string> toRemove = new List<string>();

        foreach(KeyValuePair<string, string> entry in assetDatabase)
        {
            if (!File.Exists(entry.Value))
                toRemove.Add(entry.Key);
        }
        foreach(string remove in toRemove)
        {
            assetDatabase.Remove(remove);
        }
        SaveDatabase();

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
        if (assetDatabase.ContainsKey(key))
        {
            try
            {
                GameObjectSerializable parentSerializable = JsonUtility.FromJson<GameObjectSerializable>(File.ReadAllText(assetDatabase[key]));
                Debug.Log("Loaded JSON");

                GameObject loadedAsset = Resources.Load<GameObject>(Path.Combine(Application.dataPath, assetDatabase[key].Replace("json", "obj")));
                Debug.Log("Loaded Asset");
                Debug.Log(Path.Combine(Application.dataPath, assetDatabase[key].Replace("json", "obj")));
                GameObject instanciatedParent = Instantiate(loadedAsset, GameObject.FindGameObjectWithTag("Playground").transform);
                Debug.Log("Instanciated Asset");
                instanciatedParent.transform.position = parentSerializable.position;
                instanciatedParent.transform.rotation = parentSerializable.rotation;

                for(int i = 0; i < instanciatedParent.transform.childCount; i++)
                {
                    instanciatedParent.transform.GetChild(i).position = parentSerializable.child[i].position;
                    instanciatedParent.transform.GetChild(i).rotation = parentSerializable.child[i].rotation;
                    instanciatedParent.transform.GetChild(i).tag = "3D Generated";
                }
                Debug.Log("Setup object concluded");

                return instanciatedParent;
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

        foreach(DirectoryInfo info in directoryInfo.GetDirectories())
        {
            Debug.Log(info.Name);
        }

        foreach (KeyValuePair<string, string> entry in assetDatabase)
        {
        }
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
            childTemp.position = child.transform.position;
            childTemp.rotation = child.transform.rotation;

            parentSerializable.child[i] = childTemp;
            i++;
        }

        parentSerializable.childNumber = gameobject.transform.childCount;

        string savingPath = path.Substring(0, path.Length - (gameobject.name + ".obj").Length);

        if (!Directory.Exists(savingPath)) Debug.Log("ERROR FOLDER DOES NOT EXIST");

        string fullSavingPath = Path.Combine(savingPath, gameobject.name + ".json");

        File.WriteAllText(fullSavingPath, JsonUtility.ToJson(parentSerializable));

        Debug.Log("Object pose saved");
        AddEntry(gameobject.name, fullSavingPath);
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
}
