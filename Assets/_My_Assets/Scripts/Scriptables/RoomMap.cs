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
        t = m.GetTexture("_Texture2D").name;
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
    public List<GameObject> llights; //SAVE THIS
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

        for (int i = 0; i < size; i++)
        {
            mapObj.Add(new List<TileObject>());
            for (int j = 0; j < size; j++)
            {
                GameObject last = Instantiate(tilePrefab);
                last.transform.position = new Vector3(i, 0, j);
                TileObject t = last.GetComponent<TileObject>();

                t.Init(true, true, true, true, true);
                t.roomMap = this;
                last.transform.SetParent(gameObject.transform, false);

                mapObj[i].Add(t);
            }
        }
    }


    public void EnterEdit()
    {
        isEditing = true;

        for (int i = 1; i < size-1; i++)
        {
            for (int j = 1; j < size-1; j++)
            {
                mapObj[i][j].ShowGizmoEditGround();
            }
        }
    }
    public void ExitEdit()
    {
        isEditing = false;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                mapObj[i][j].HideButton();
            }
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
        llights.Add(last);
    }

    public void RemoveLightTile(GameObject me)
    {
        llights.Remove(me);
    }

    public void SaveMapToFile(string path)
    {
        string json = JsonUtility.ToJson(GetSaveMapData());

        File.WriteAllText(Path.Combine(Application.dataPath, path, nameOfMap + ".json"), json);
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
                GameObject last = Instantiate(tilePrefab);
                last.transform.position = new Vector3(i, 0, j);
                TileObject t = last.GetComponent<TileObject>();

                t.Init(data.map[i].list[j], true, true, true, true);
                t.roomMap = this;

                last.transform.SetParent(gameObject.transform, false);

                mapObj[i].Add(t);
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
            m.SetTexture("_Texture2D", tex);
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
