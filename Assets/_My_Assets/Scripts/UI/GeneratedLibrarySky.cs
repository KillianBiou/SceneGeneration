using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class GeneratedLibrarySky : MonoBehaviour
{

    public string libraryName;
    public GameObject buttonPrefab;

    [HideInInspector]
    public Texture2D texture;

    // Start is called before the first frame update
    void Start()
    {
        LoadLibrary();


        StableHandler sdh;
        if ((sdh = FindFirstObjectByType<StableHandler>()) != null)
            sdh.FinishedGenerating.AddListener(ReloadLibrary);
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
        DirectoryInfo dir = new DirectoryInfo(GlobalVariables.Instance.GetSkyPath());
        FileInfo[] info = dir.GetFiles("*_SKY.png");

        foreach (FileInfo f in info)
        {
            GameObject last = Instantiate(buttonPrefab);

            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(File.ReadAllBytes(f.FullName));
            tex.name = f.Name;

            last.GetComponent<ButtonMaterial>().SetupButton(tex);

            last.transform.SetParent(transform, false);
        }
    }
}
