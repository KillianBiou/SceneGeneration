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
    public string mapName;


    private RoomMap rm;
    private EditmapMode le;
    private Player p;


    // Start is called before the first frame update
    void Start()
    {
        rm = RoomMap.instance;
        p = Player.Instance;
    }


    public void SaveScene(string path)
    {
        SceneDescription saveData = new SceneDescription();

        saveData.tileData = rm.GetSaveMapData();
        saveData.lightData = le.GetLightsSaveData();
        saveData.goData = p.GetGameObjectSaveData();


        if (File.Exists(Path.Combine(Application.dataPath, path, rm.nameOfMap + ".json")))
            Debug.Log("File already exist, overwritting...");


        File.WriteAllText(Path.Combine(Application.dataPath, path, rm.nameOfMap + ".json"), JsonUtility.ToJson(saveData));
    }

    public void LoadScene(string path)
    {
        if (File.Exists(path))
        {
            SceneDescription data = JsonUtility.FromJson<SceneDescription>(File.ReadAllText(path));

            rm.LoadMap(data.tileData);
            le.LoadLightsData(data.lightData);
            p.LoadScene(data.goData);

            Debug.Log("Successfully loaded from '" + path + "'.");
        }
        else
            Debug.Log("Loading '" + path + "' - File does not exist.");
    }

    public void SetMapName(string s)
    {
        mapName = s;
    }

    public void QuitApp()
    { Application.Quit(); }

}
