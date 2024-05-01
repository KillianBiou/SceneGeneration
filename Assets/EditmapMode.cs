using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditmapMode : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.collider.gameObject.GetComponent<TileObject>() != null)
                {
                    hit.collider.gameObject.GetComponent<TileObject>().TileClick();
                }
            }
        }
    }
}
