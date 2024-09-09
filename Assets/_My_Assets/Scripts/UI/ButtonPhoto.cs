using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ButtonPhoto : MonoBehaviour
{
    public RawImage texture;

    private Texture2D tex;

    private string fullPath;

    public void SetupButton(string path)
    {
        fullPath = path;

        tex = new Texture2D(2, 2);
        tex.LoadImage(File.ReadAllBytes(path));
        tex.name = Path.GetFileName(path);
        texture.texture = tex;
    }

    public void PictureToMesh()
    {
        BGRemover.Instance.RemoveBackground(fullPath, BGRemovedToMeshCallback);
    }

    public int BGRemovedToMeshCallback(List<string> paths)
    {
        if(paths.Count < 1)
            return -1;

        TripoSRForUnity.Instance.RunTripoSR_GLB(MeshInstanciateCallback, paths[0]);
        return 0;
    }

    public int MeshInstanciateCallback(string path)
    {
        //read the json, instanciate bla bla bla
        return 0;
    }
}
