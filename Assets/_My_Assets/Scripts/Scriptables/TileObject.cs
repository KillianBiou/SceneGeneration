using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Cardinal
{
    NORTH = 0,
    EAST,
    SOUTH,
    WEST
}



public class TileObject : MonoBehaviour
{

    [SerializeField]
    public GameObject northWall, eastWall, southWall, westWall;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isNorthClosed()
    {
        return northWall.active;
    }


    public bool isEastClosed()
    {
        return eastWall.active;
    }

    public bool isSouthClosed()
    {
        return eastWall.active;
    }

    public bool isWestClosed()
    {
        return westWall.active;
    }

    public bool isClosed(Cardinal c)
    {
        switch (c)
        {
            case Cardinal.NORTH:
                return northWall.active;
            
            case Cardinal.EAST:
                return eastWall.active;

            case Cardinal.SOUTH:
                return southWall.active;

            case Cardinal.WEST:
                return westWall.active;
        }
        return false;
    }

    public void openCardinal(Cardinal c)
    {
        switch (c)
        {
            case Cardinal.NORTH:
                northWall.SetActive(true);
                break;

            case Cardinal.EAST:
                eastWall.SetActive(false);
                break;

            case Cardinal.SOUTH:
                southWall.SetActive(true);
                break;

            case Cardinal.WEST:
                westWall.SetActive(true);
                break;

        }
        return;
    }

    public void closeCardinal(Cardinal c)
    {
        switch (c)
        {
            case Cardinal.NORTH:
                northWall.SetActive(false);
                break;

            case Cardinal.EAST:
                eastWall.SetActive(false);
                break;

            case Cardinal.SOUTH:
                southWall.SetActive(false);
                break;

            case Cardinal.WEST:
                westWall.SetActive(false);
                break;

        }
        return;
    }

    public void Init(bool n, bool e, bool s, bool w)
    {
        if (n)
            closeCardinal(Cardinal.NORTH);
        if (e)
            closeCardinal(Cardinal.EAST);
        if (s)
            closeCardinal(Cardinal.SOUTH);
        if (w)
            closeCardinal(Cardinal.WEST);
    }
}
