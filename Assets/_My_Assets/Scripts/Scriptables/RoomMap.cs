using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMap : MonoBehaviour
{


    public bool[,] map;
    public GameObject prefab;

    // Start is called before the first frame update
    void Start()
    {
        map = new bool[10,10]{
        { false, false, false, false, false, false, false, false, false, false },
        { false, true, true, true, false, false, false, false, false, false },
        { false, true, true, true, false, false, false, false, false, false },
        { false, true, false, true, false, false, false, false, false, false },
        { false, true, true, true, false, false, false, false, false, false },
        { false, true, true, true, true, false, false, false, false, false },
        { false, true, true, true, true, false, false, false, false, false },
        { false, true, true, true, true, false, false, false, false, false },
        { false, true, true, true, true, false, false, false, false, false },
        { false, false, false, false, false, false, false, false, false, false }
        };

        for (int i = 1; i < 9; i++)
        {
            for (int j = 1; j < 9; j++)
            {
                if (map[i,j])
                {
                    GameObject last = Instantiate(prefab);
                    last.transform.position = new Vector3(i, 0, j);
                    last.GetComponent<TileObject>().Init(map[i, j + 1], map[i+1, j], map[i, j - 1], map[i-1, j]);
                    last.transform.SetParent(gameObject.transform, false);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
