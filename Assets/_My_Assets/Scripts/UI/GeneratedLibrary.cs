using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEditor;
using Unity.VisualScripting;

public class GeneratedLibrary : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject holder;
    [SerializeField]
    private GameObject buttonPrefab;
    [SerializeField]
    private MatInfo selectedMaterial;

    [HideInInspector]
    public Texture2D texture, normal;

    public static GeneratedLibrary Instance;
    private void Awake()
    {
        Instance = this;
    }

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
        for (int i = holder.transform.childCount; i > 1; i--)
        {
            Destroy(holder.transform.GetChild(i - 1).gameObject);
        }

        LoadLibrary();
    }


    public void LoadLibrary()
    {
        if (!Directory.Exists(GlobalVariables.Instance.GetImagePath()))
            return;

        DirectoryInfo dir = new DirectoryInfo(GlobalVariables.Instance.GetImagePath());
        FileInfo[] info = dir.GetFiles("*_T.png");
        //Debug.Log("jen ai trouve " + info.Length);
        foreach (FileInfo f in info)
        {
            CreateButton(f);
        }
    }

    public string AddNew(string path)
    {
        if (path == "")
            return "";

        Debug.Log("has just been generated : " + path);
        CreateButton(new FileInfo(path));
        return "";
    }


    private void CreateButton(FileInfo f)
    {
        GameObject last = Instantiate(buttonPrefab);

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(File.ReadAllBytes(f.FullName));
        tex.name = f.Name;

        last.GetComponent<MaterialSetter>().tex = tex;
        if (File.Exists((f.FullName).Replace("_T.png", "_N.png")))
        {
            Texture2D norm = new Texture2D(2, 2);
            norm.LoadImage(File.ReadAllBytes((f.FullName).Replace("_T.png", "_N.png")));

            last.GetComponent<MaterialSetter>().SetupButton(selectedMaterial, tex, norm);
        }
        else
            last.GetComponent<MaterialSetter>().SetupButton(selectedMaterial, tex, null);

        last.transform.SetParent(holder.transform, false);
    }
}
