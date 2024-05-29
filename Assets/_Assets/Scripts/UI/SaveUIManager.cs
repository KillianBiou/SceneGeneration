using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SaveUIManager : MonoBehaviour
{
    [SerializeField]
    private string savePath;
    [SerializeField]
    private Transform holder;
    [SerializeField]
    private GameObject entryPrefab;

    private void OnEnable()
    {
        ClearHolder();
        FetchSaveList();
    }

    private void ClearHolder()
    {
        foreach (Transform item in holder)
        {
            Destroy(item.gameObject);
        }
    }

    private void FetchSaveList()
    {
        string fullPath = Path.Combine(Application.dataPath, savePath);
        Debug.Log(fullPath);

        if (!Directory.Exists(fullPath))
            return;

        DirectoryInfo directoryInfo = new DirectoryInfo(fullPath);

        foreach (FileInfo info in directoryInfo.GetFiles())
        {
            if(info.Extension == ".json")
                AddEntry(info.Name, File.GetLastWriteTimeUtc(info.FullName), info.FullName);
            
        }
    }

    private void AddEntry(string saveName, DateTime lastEdit, string fullPath)
    {
        GameObject temp = Instantiate(entryPrefab, holder);
        temp.GetComponentInChildren<TextMeshProUGUI>().text = saveName + " - " + lastEdit.ToString();

        temp.GetComponent<Button>().onClick.AddListener(() => SaveSelectionCallback(fullPath));
        temp.GetComponent<Button>().onClick.AddListener(() => gameObject.transform.parent.gameObject.SetActive(false));
        temp.GetComponent<Button>().onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void SaveSelectionCallback(string path)
    {
        Debug.Log("Trying to load :" + path);
        SceneSaver.Instance.LoadScene(path);
    }
}
