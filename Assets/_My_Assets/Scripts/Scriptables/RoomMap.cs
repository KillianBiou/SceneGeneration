using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine.XR;





[System.Serializable]
public struct ShaderValues
{
    [SerializeField]
    public string t;
    [SerializeField]
    public Vector3 p;
    [SerializeField]
    public Vector3 s;
    [SerializeField]
    public Vector3 r;
    [SerializeField]
    public bool bN;
    [SerializeField]
    public string n;

    public ShaderValues(Material m)
    {
        t = m.GetTexture("_BaseMap").name;
        p = m.GetVector("_position_offset");
        s = m.GetVector("_Scaling");
        r = m.GetVector("_Rotation");
        bN = m.GetFloat("_use_normal") == 1.0f;
        n = m.GetTexture("_Normal_Map").name;
    }
}


[System.Serializable]
public struct TileMat
{
    [SerializeField]
    public Vector3 pos;
    [SerializeField]
    public ShaderValues gnd, n1, n2, w1, w2, s1, s2, e1, e2;
}


[System.Serializable]
public struct MapData
{
    [SerializeField]
    public string name;
    [SerializeField]
    public List<wrapList> map;

    [SerializeField]
    public List<TileMat> tileMats;
}


[System.Serializable]
public struct wrapList
{
    [SerializeField]
    public List<bool> list;
}





public class RoomMap : MonoBehaviour
{
    public static RoomMap Instance;
    

    public int size;
    public List<List<TileObject>> mapObj; // SAVE THIS
    public List<TileLight> llights; //SAVE THIS
    public string nameOfMap;
    public GameObject tilePrefab, lightPrefab;
    public bool isEditing;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        isEditing = false;

        mapObj = new List<List<TileObject>>();
        llights = new List<TileLight>();

        for (int i = 0; i < size; i++)
        {
            mapObj.Add(new List<TileObject>());
            for (int j = 0; j < size; j++)
            {
                mapObj[i].Add(CreateTile(new Vector3(i, 0, j)));
            }
        }
    }


    public void TileToggleEdit(bool b)
    {
        isEditing = true;

        for (int i = 1; i < size-1; i++)
        {
            for (int j = 1; j < size-1; j++)
            {
                if(b)
                    mapObj[i][j].ShowGizmoEditGround();
                else
                    mapObj[i][j].HideButton();
            }
        }
    }


    public void LightTogglePlaceGizmo(bool b)
    {
        foreach(TileLight l in llights)
        {
            if (b)
                l.ActivateRemover();
            else
                l.DeactivateRemover();
        }
    }

    public void LightToggleEditGizmo(bool b)
    {
        foreach (TileLight l in llights)
        {
            if (b)
                l.ActivateEdit();
            else
                l.DeactivateEdit();
        }
    }



    public void Retile(int x, int y, bool b)
    {
        if (!b)
        {
            if (!(y + 1 > size - 1))
                if (!mapObj[x][y + 1].isWall) { mapObj[x][y + 1].openCardinal(Cardinal.SOUTH); }
            if (!(x + 1 > size - 1))
                if (!mapObj[x + 1][y].isWall) { mapObj[x + 1][y].openCardinal(Cardinal.WEST); }
            if (!(y - 1 < 0))
                if (!mapObj[x][y - 1].isWall) { mapObj[x][y - 1].openCardinal(Cardinal.NORTH); }
            if (!(x - 1 < 0))
                if (!mapObj[x - 1][y].isWall) { mapObj[x - 1][y].openCardinal(Cardinal.EAST); }
        }
        else
        {
            if (!(y + 1 > size - 1))
                if (!mapObj[x][y + 1].isWall) { mapObj[x][y + 1].closeCardinal(Cardinal.SOUTH); }
            if (!(x + 1 > size - 1))
                if (!mapObj[x + 1][y].isWall) { mapObj[x + 1][y].closeCardinal(Cardinal.WEST); }
            if (!(y - 1 < 0))
                if (!mapObj[x][y - 1].isWall) { mapObj[x][y - 1].closeCardinal(Cardinal.NORTH); }
            if (!(x - 1 < 0))
                if (!mapObj[x - 1][y].isWall) { mapObj[x - 1][y].closeCardinal(Cardinal.EAST); }
        }

        mapObj[x][y].Init(b, y+1 > size-1 ? true : mapObj[x][y + 1].isWall, x+1 > size-1 ? true : mapObj[x + 1][y].isWall, y-1 < 0 ? true : mapObj[x][y - 1].isWall, x-1 < 0 ? true : mapObj[x - 1][y].isWall);
    }


    public List<TileObject> GetTiles(List<TileObject> l, Vector3 v1, Vector3 v2)
    {
        l.Clear();
        
        for (int i = (int)Mathf.Min(v1.x, v2.x); i <= (int)Mathf.Max(v1.x, v2.x); i++)
        {
            for (int j = (int)Mathf.Min(v1.z, v2.z); j <= (int)Mathf.Max(v1.z, v2.z); j++)
            {
                l.Add(mapObj[i][j]);
            }
        }
        return l;
    }



    public void SetRoomName(string s)
    {
        nameOfMap = s;
    }

    public void AddLightTile(Vector3 tilePos, float lightHeight)
    {
        GameObject last = Instantiate(lightPrefab);
        last.transform.position = tilePos;
        TileLight lightComp = last.GetComponent<TileLight>();
        lightComp.lightObj.transform.localPosition = new Vector3(0, lightHeight, 0);
        lightComp.ActivateRemover();
        llights.Add(lightComp);
    }

    public void RemoveLightTile(GameObject me)
    {
        llights.Remove(me.GetComponent<TileLight>());
    }



    private TileObject CreateTile(Vector3 position)
    {
        GameObject last = Instantiate(tilePrefab);
        last.transform.position = position;
        TileObject t = last.GetComponent<TileObject>();
        t.Init(true, true, true, true, true);
        last.transform.SetParent(gameObject.transform, false);

        return t;
    }





    // MAP SAVE LOAD

    public void SaveMapToFile(string path)
    {
        string json = JsonUtility.ToJson(GetSaveMapData());

        File.WriteAllText(Path.Combine(Application.dataPath, path, nameOfMap + ".json"), json);
    }

    public GameObject ReturnOnlyActivatedTile()
    {
        GameObject parent = new GameObject();

        foreach(List<TileObject> lObject in mapObj)
        {
            foreach (TileObject obj in lObject)
            {
                if (!obj.isWall)
                {
                    GameObject copy = obj.GetCleanCopy(); //Instantiate(obj.gameObject);
                    copy.transform.parent = parent.transform;
                }
            }
        }

        return parent;
    }

    public MapData GetSaveMapData()
    {
        MapData data = new MapData();
        data.name = nameOfMap;
        data.map = new List<wrapList>();
        data.tileMats = new List<TileMat>();

        for (int i = 0; i < size; i++)
        {
            wrapList wl = new wrapList();
            wl.list = new List<bool>();
            for (int j = 0; j < size; j++)
            {
                wl.list.Add(mapObj[i][j].isWall);

                if (!mapObj[i][j].isWall)
                {
                    TileMat tm = new TileMat();
                    tm.gnd = new ShaderValues();

                    tm.pos = new Vector3(i, 0, j);
                    tm.gnd = new ShaderValues(mapObj[i][j].ground.GetComponent<Renderer>().material);
                    tm.n1 = new ShaderValues(mapObj[i][j].northWall.GetComponent<Renderer>().material);
                    tm.n2 = new ShaderValues(mapObj[i][j].northWall.transform.GetChild(0).GetComponent<Renderer>().material);
                    tm.e1 = new ShaderValues(mapObj[i][j].eastWall.GetComponent<Renderer>().material);
                    tm.e2 = new ShaderValues(mapObj[i][j].eastWall.transform.GetChild(0).GetComponent<Renderer>().material);
                    tm.s1 = new ShaderValues(mapObj[i][j].southWall.GetComponent<Renderer>().material);
                    tm.s2 = new ShaderValues(mapObj[i][j].southWall.transform.GetChild(0).GetComponent<Renderer>().material);
                    tm.w1 = new ShaderValues(mapObj[i][j].westWall.GetComponent<Renderer>().material);
                    tm.w2 = new ShaderValues(mapObj[i][j].westWall.transform.GetChild(0).GetComponent<Renderer>().material);

                    data.tileMats.Add(tm);
                }
            }
            data.map.Add(wl);
        }
        return data;
    }

    public LightsData GetLightsSaveData()
    {
        LightsData ld = new LightsData();
        ld.lights = new List<LightPlace>();

        foreach (TileLight l in llights)
        {
            Light linfo = l.lightRef;
            ld.lights.Add(new LightPlace(linfo.intensity, linfo.range, linfo.transform.position.y, l.transform.position));
        }
        return ld;
    }

    public void LoadLightsData(LightsData data)
    {
        DeleteAllLights();

        foreach (LightPlace l in data.lights)
        {
            GameObject last = Instantiate(lightPrefab);
            last.transform.position = l.position;
            Light li = last.GetComponent<TileLight>().lightObj.GetComponent<Light>();
            li.transform.localPosition = new Vector3(0, l.height, 0);
            li.intensity = l.intensity;
            li.range = l.range;
            last.GetComponent<TileLight>().DeactivateEdit();
            llights.Add(last.GetComponent<TileLight>());
        }
    }

    public void DropCurrentMap()
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Destroy(mapObj[i][j].gameObject);
            }
        }
        mapObj.Clear();
    }

    public void DeleteAllLights()
    {
        foreach (TileLight l in llights)
            Destroy(l.gameObject);
        llights.Clear();
    }

    public void LoadMapFromFile(string path)
    {
        if (!File.Exists(Path.Combine(Application.dataPath, path)))
            return;

        MapData data = new MapData();
        data = JsonUtility.FromJson<MapData>(File.ReadAllText(Path.Combine(Application.dataPath, path)));

        LoadMap(data);
    }

    public void LoadMap(MapData data)
    {
        DropCurrentMap();

        size = data.map.Count;

        for (int i = 0; i < size; i++)
        {
            mapObj.Add(new List<TileObject>());
            for (int j = 0; j < size; j++)
            {
                mapObj[i].Add(CreateTile(new Vector3(i, 0, j)));
                mapObj[i][j].isWall = data.map[i].list[j];
            }
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                Retile(i, j, mapObj[i][j].isWall);
            }
        }



        foreach (TileMat tm in data.tileMats)
        {
            ApplyShaderValues(mapObj[(int)tm.pos.x][(int)tm.pos.z].ground, tm.gnd);

            ApplyShaderValues(mapObj[(int)tm.pos.x][(int)tm.pos.z].northWall, tm.n1);
            ApplyShaderValues(mapObj[(int)tm.pos.x][(int)tm.pos.z].northWall.transform.GetChild(0).gameObject, tm.n2);

            ApplyShaderValues(mapObj[(int)tm.pos.x][(int)tm.pos.z].eastWall, tm.e1);
            ApplyShaderValues(mapObj[(int)tm.pos.x][(int)tm.pos.z].eastWall.transform.GetChild(0).gameObject, tm.e2);

            ApplyShaderValues(mapObj[(int)tm.pos.x][(int)tm.pos.z].southWall, tm.s1);
            ApplyShaderValues(mapObj[(int)tm.pos.x][(int)tm.pos.z].southWall.transform.GetChild(0).gameObject, tm.s2);

            ApplyShaderValues(mapObj[(int)tm.pos.x][(int)tm.pos.z].westWall, tm.w1);
            ApplyShaderValues(mapObj[(int)tm.pos.x][(int)tm.pos.z].westWall.transform.GetChild(0).gameObject, tm.w2);
        }

    }

    public void GetShaderValues()
    {

    }




    public void ApplyShaderValues(GameObject go, ShaderValues sv)
    {
        Material m = go.GetComponent<Renderer>().material;

        Debug.Log(sv.t);
        if (sv.t != "default")
        {
            Texture2D tex = null;
            tex = new Texture2D(2, 2);
            Debug.Log(Path.Combine(Application.dataPath, "GeneratedData", sv.t));
            if(File.Exists(Path.Combine(Application.dataPath, "GeneratedData", sv.t)))
                tex.LoadImage(File.ReadAllBytes(Path.Combine(Application.dataPath, "GeneratedData", sv.t)));
            m.SetTexture("_BaseMap", tex);
            tex.name = sv.t;
        }

        /*if (sv.n != "NormalMap")
        {
            Texture2D tex = null;
            tex = new Texture2D(2, 2);
            tex.LoadImage(System.IO.File.ReadAllBytes(Path.Combine(Application.dataPath, "GeneratedData", sv.n)));
            m.SetTexture("_Normal_Map", tex);
        }*/

        m.SetVector("_position_offset", sv.p);
        m.SetVector("_Scaling", sv.s);
        m.SetVector("_Rotation", sv.r);
        m.SetFloat("_use_normal", sv.bN ? 1.0f : 0.0f);
    }

}
