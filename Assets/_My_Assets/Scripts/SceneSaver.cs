using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


[System.Serializable]
public struct SceneDescription
{
    public MapData tileData;
    public LightsData lightData;
    public GameObjectSerializable goData;
    
}



    public class SceneSaver : MonoBehaviour
{
    public string fileName;

    public string debugPath;

    public static SceneSaver Instance;

    private void Awake()
    {
        Instance = this;
    }


    public void SaveScene(string path)
    {
        SceneDescription saveData = new SceneDescription();

        saveData.tileData = RoomMap.Instance.GetSaveMapData();
        saveData.lightData = RoomMap.Instance.GetLightsSaveData();
        saveData.goData = Player.Instance.GetGameObjectSaveData();

        string fullPath = Path.Combine(GlobalVariables.Instance.GetScenePath(), fileName + ".json");


        if (File.Exists(fullPath))
        {
            Debug.Log("File already exist, overwritting...");
            File.Delete(fullPath);
        }

        File.WriteAllText(fullPath, JsonUtility.ToJson(saveData));
        Debug.Log("File successfully saved at : " + fullPath);
    }

    public void LoadScene(string path)
    {
        if (File.Exists(path))
        {
            SceneDescription data = JsonUtility.FromJson<SceneDescription>(File.ReadAllText(path));

            RoomMap.Instance.LoadMap(data.tileData);
            RoomMap.Instance.LoadLightsData(data.lightData);
            Player.Instance.LoadScene(data.goData);

            Debug.Log("Successfully loaded from '" + path + "'.");
        }
        else
            Debug.Log("Loading '" + path + "' - File does not exist.");
    }

    private void OnEnable()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }


    public void SetMapName(string s)
    {
        fileName = s;
    }

    public void QuitApp()
    { Application.Quit(); }

}
