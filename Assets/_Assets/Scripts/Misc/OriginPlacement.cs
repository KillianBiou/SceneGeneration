using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class OriginPlacement : MonoBehaviour
{   
    public void ReplaceOrigin()
    {

        // TRY WITH COLLIDER + RAYCAST FOR 
        Vector3 bottomDistance = Vector3.zero;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.down, Vector3.up, out hit, 10f))
        {
            bottomDistance = hit.point - transform.position;
            transform.rotation = Quaternion.FromToRotation(hit.normal, Vector3.down) * transform.rotation;
        }
        transform.position = transform.parent.transform.position + Vector3.up * bottomDistance.magnitude;
        /*if (Physics.Raycast(transform.position, Vector3.down, out hit, 10f))
        {
            //transform.Translate(Vector3.down * hit.distance, Space.World);
            transform.Translate(Vector3.down * hit.distance, Space.World);
        }*/
        transform.parent.position = new Vector3(transform.position.x, 0, transform.position.z);

        Destroy(this, 1f);




        // TRY WITH RAYCAST, NOT BASE ROTATION
        /*RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.forward * 10, out hit, 100f))
        {
            Debug.Log(hit.normal);
        }*/

        // TRY WITH VERTEX NORMALS, MESH HAS NONE
        /*_filter = GetComponent<MeshFilter>();
        Mesh mesh = _filter.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = new Vector2[vertices.Length];
        Bounds bounds = mesh.bounds;

        int topVertex = -1;
        float maxDist = float.PositiveInfinity;

        for (int i = 0; i < vertices.Length; i++)
        {
            float dist = vertices[i].z;
            if (dist < maxDist)
            {
                maxDist = dist;
                topVertex = i;
            }
        }

        Debug.Log(topVertex);
        Debug.Log(mesh.normals.Length);*/
    }
}
