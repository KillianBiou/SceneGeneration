using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMap : MonoBehaviour
{

    public int size;
    public List<List<TileObject>> mapObj; // SAVE THIS
    public GameObject prefab;
    public bool isEditing;

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
                GameObject last = Instantiate(prefab);
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

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                mapObj[i][j].ShowButton();
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


    public void Retile(TileObject caller, int x, int y, bool b)
    {
        if (!b)
        {
            if (!mapObj[x][y+1].isWall)
                mapObj[x][y+1].openCardinal(Cardinal.SOUTH);
            if (!mapObj[x+1][y].isWall)
                mapObj[x+1][y].openCardinal(Cardinal.WEST);
            if (!mapObj[x][y-1].isWall)
                mapObj[x][y-1].openCardinal(Cardinal.NORTH);
            if (!mapObj[x-1][y].isWall)
                mapObj[x-1][y].openCardinal(Cardinal.EAST);
        }
        else
        {
            if (!mapObj[x][y+1].isWall)
                mapObj[x][y+1].closeCardinal(Cardinal.SOUTH);
            if (!mapObj[x + 1][y].isWall)
                mapObj[x+1][y].closeCardinal(Cardinal.WEST);
            if (!mapObj[x][y-1].isWall)
                mapObj[x][y-1].closeCardinal(Cardinal.NORTH);
            if (!mapObj[x - 1][y].isWall)
                mapObj[x-1][y].closeCardinal(Cardinal.EAST);
        }

        caller.Init(b, mapObj[x][y+1].isWall, mapObj[x+1][y].isWall, mapObj[x][y-1].isWall, mapObj[x-1][y].isWall);
    }
}
