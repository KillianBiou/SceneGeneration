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
        foreach(string remove  in toRemove)
        {
            assetDatabase.Remove(remove);
        }
        SaveDatabase();
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
                AssetDatabase.ImportAsset(assetDatabase[key], ImportAssetOptions.ForceUpdate);
                return AssetDatabase.LoadAssetAtPath<GameObject>(assetDatabase[key]);
            }
            catch
            {
                return null;
            }
        }
        return null;
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
