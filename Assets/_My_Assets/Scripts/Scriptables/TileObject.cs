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
    public GameObject ground, northWall, eastWall, southWall, westWall;
    public GameObject groundAdd, groundRemove, selected;

    public bool isWall = true;
    public BoxCollider editorCollider;


    public bool isNorthClosed()
    {
        return northWall.activeInHierarchy;
    }

    public bool isEastClosed()
    {
        return eastWall.activeInHierarchy;
    }

    public bool isSouthClosed()
    {
        return eastWall.activeInHierarchy;
    }

    public bool isWestClosed()
    {
        return westWall.activeInHierarchy;
    }

    public bool isClosed(Cardinal c)
    {
        switch (c)
        {
            case Cardinal.NORTH:
                return northWall.activeInHierarchy;
            
            case Cardinal.EAST:
                return eastWall.activeInHierarchy;

            case Cardinal.SOUTH:
                return southWall.activeInHierarchy;

            case Cardinal.WEST:
                return westWall.activeInHierarchy;
        }
        return false;
    }

    public void openCardinal(Cardinal c)
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

    public void closeCardinal(Cardinal c)
    {
        switch (c)
        {
            case Cardinal.NORTH:
                northWall.SetActive(true);
                break;

            case Cardinal.EAST:
                eastWall.SetActive(true);
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

    public void Init(bool me, bool n, bool e, bool s, bool w)
    {
        isWall = me;

        if (isWall)
        {
            openCardinal(Cardinal.NORTH);
            openCardinal(Cardinal.EAST);
            openCardinal(Cardinal.SOUTH);
            openCardinal(Cardinal.WEST);
            ground.SetActive(false);
            return;
        }

        ground.SetActive(true);
        if (n)
            closeCardinal(Cardinal.NORTH);
        if (e)
            closeCardinal(Cardinal.EAST);
        if (s)
            closeCardinal(Cardinal.SOUTH);
        if (w)
            closeCardinal(Cardinal.WEST);
    }


    public void ShowGizmoEditGround()
    {
        selected.SetActive(false);

        if (isWall)
            groundAdd.SetActive(true);
        else
            groundRemove.SetActive(true);

        editorCollider.enabled = true;

        northWall.GetComponent<Collider>().enabled = false;
        northWall.transform.GetChild(0).GetComponent<Collider>().enabled = false;

        eastWall.GetComponent<Collider>().enabled = false;
        eastWall.transform.GetChild(0).GetComponent<Collider>().enabled = false;

        southWall.GetComponent<Collider>().enabled = false;
        southWall.transform.GetChild(0).GetComponent<Collider>().enabled = false;

        westWall.GetComponent<Collider>().enabled = false;
        westWall.transform.GetChild(0).GetComponent<Collider>().enabled = false;
    }

    public void HideButton()
    {
        if (isWall)
            groundAdd.SetActive(false);
        else
            groundRemove.SetActive(false);

        editorCollider.enabled = false;

        northWall.GetComponent<Collider>().enabled = true;
        northWall.transform.GetChild(0).GetComponent<Collider>().enabled = true;

        eastWall.GetComponent<Collider>().enabled = true;
        eastWall.transform.GetChild(0).GetComponent<Collider>().enabled = true;

        southWall.GetComponent<Collider>().enabled = true;
        southWall.transform.GetChild(0).GetComponent<Collider>().enabled = true;

        westWall.GetComponent<Collider>().enabled = true;
        westWall.transform.GetChild(0).GetComponent<Collider>().enabled = true;
    }


    public void TileClick()
    {
        if (isWall)
        {
            isWall = false;
            groundAdd.SetActive(false);
            groundRemove.SetActive(true);
        }
        else
        {
            isWall = true;
            groundRemove.SetActive(false);
            groundAdd.SetActive(true);
        }
        RoomMap.Instance.Retile((int)gameObject.transform.position.x, (int)gameObject.transform.position.z, isWall);
    }

    public void ShowSelected()
    {
        groundAdd.SetActive(false);
        selected.SetActive(true);
    }

    public void HideSelected()
    {
        groundAdd.SetActive(true);
        selected.SetActive(false);
    }
}
