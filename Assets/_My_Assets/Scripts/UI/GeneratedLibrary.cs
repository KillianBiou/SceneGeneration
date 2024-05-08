using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEditor;

public class GeneratedLibrary : MonoBehaviour
{

    public string libraryName;
    public GameObject buttonPrefab;

    public MatInfo selectedMaterial;

    [HideInInspector]
    public Texture2D texture, normal;

    // Start is called before the first frame update
    void Start()
    {
        LoadLibrary();


        StableHandler sdh;
        if ((sdh = FindFirstObjectByType<StableHandler>()) != null)
            sdh.FinishedGenerating.AddListener(ReloadLibrary);
    }

    public void ApplyTexture()
    {
        if (selectedMaterial == null || selectedMaterial.selectedId ==-1)
            return;

        selectedMaterial.materials[selectedMaterial.selectedId].SetTexture("_Texture2D", texture);
        if(normal != null)
            selectedMaterial.materials[selectedMaterial.selectedId].SetTexture("_Normal_Map", normal);
    }

    public void ReloadLibrary()
    {
        for (int i = transform.childCount; i > 1; i--)
        {
            Destroy(transform.GetChild(i - 1).gameObject);
        }

        LoadLibrary();
    }


    public void LoadLibrary()
    {
        if (!Directory.Exists(Application.dataPath + libraryName))
            return;

        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + libraryName);
        FileInfo[] info = dir.GetFiles("*_T.png");
        foreach (FileInfo f in info)
        {
            GameObject last = Instantiate(buttonPrefab);

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(File.ReadAllBytes(Application.dataPath + libraryName + f.Name));

            last.GetComponent<MaterialSetter>().tex = tex;
            if (File.Exists((Application.dataPath + libraryName + f.Name).Replace("_T.png", "_N.png")))
            {
                Texture2D norm = new Texture2D(2, 2);
                norm.LoadImage(File.ReadAllBytes((Application.dataPath + libraryName + f.Name).Replace("_T.png", "_N.png")));

                last.transform.GetChild(1).gameObject.SetActive(true);
                last.GetComponent<MaterialSetter>().norm = norm;
            }
            last.GetComponent<MaterialSetter>().tex = tex;
            last.GetComponent<MaterialSetter>().matInfo = selectedMaterial;

            last.transform.GetChild(0).GetComponent<RawImage>().texture = tex;

            last.transform.SetParent(transform, false);
        }
    }
}
