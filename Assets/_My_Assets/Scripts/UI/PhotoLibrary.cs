using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.Events;
using Unity.VisualScripting.Dependencies.Sqlite;

public class PhotoLibrary : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject holder;
    [SerializeField]
    private GameObject buttonPrefab;


    public static PhotoLibrary Instance;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        LoadLibrary();
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
        if (!Directory.Exists(GlobalVariables.Instance.GetPhotoPath()))
            return;

        DirectoryInfo dir = new DirectoryInfo(GlobalVariables.Instance.GetPhotoPath());
        FileInfo[] info = dir.GetFiles("*.jpg");
        foreach (FileInfo f in info)
        {
            CreateButton(f);
        }
    }

    public int AddNew(string path)
    {
        if (path == "")
            return 0;

        Debug.Log("has just been generated : " + path);
        CreateButton(new FileInfo(path));
        return 0;
    }

    public void AddNew(string path, int i)
    {
        CreateButton(new FileInfo(path));
    }

    private void CreateButton(FileInfo f)
    {
        GameObject last = Instantiate(buttonPrefab);

        last.GetComponent<ButtonPhoto>().SetupButton(f.FullName);
        last.transform.SetParent(holder.transform, false);
    }
}
