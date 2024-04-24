using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GenerativeChoice : MonoBehaviour
{

    public string folderName;

    public GameObject uiContainer, buttonChoice;


    private StableHandler sdh;
    private int amount;
    private bool pending;

    public SdRequest req;

    // Start is called before the first frame update
    void Start()
    {
        pending = false;

        sdh = FindFirstObjectByType<StableHandler>();
    }


    public void ClearPicker()
    {
        for (int i = uiContainer.transform.childCount; i > 0; i--)
        {
            Destroy(transform.GetChild(i - 1).gameObject);
        }

        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/GeneratedData/" + folderName);
        FileInfo[] info = dir.GetFiles("*.*");

        foreach (FileInfo f in info)
        {
            f.Delete();
        }
    }



    public void GenerateChoices(SdRequest rq, int n)
    {
        if (!sdh)
            return;


        rq.directory = folderName +"/";
        for (int i = 0; i < n; i++)
        {
            req.filename.Replace(".png", "_" + i + ".png");
            sdh.RequestGeneration(rq);
        }

        sdh.FinishedGenerating.AddListener(CountingResults);
        amount = n;
        pending = true;
    }



    public void Test(int n)
    {
        if (!sdh)
            return;


        req.directory = "/GeneratedData/" + folderName + "/";
        for (int i = 0; i < n; i++)
        {
            req.filename = req.filename.Replace(".png", "_" + i + ".png");
            sdh.RequestGeneration(req);
        }

        sdh.FinishedGenerating.AddListener(CountingResults);
        amount = n;
        pending = true;
    }

    public void CountingResults()
    {

        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/GeneratedData/" +  folderName);
        FileInfo[] info = dir.GetFiles("*.png");

        if (info.Length >= amount)
        {
            //generate buttons
            foreach (FileInfo f in info)
            {
                GameObject last = Instantiate(buttonChoice);

                Texture2D tex = new Texture2D(2, 2);
                tex.LoadImage(File.ReadAllBytes(Application.dataPath + "/GeneratedData/" + folderName + "/" + f.Name));

                last.transform.GetChild(0).GetComponent<RawImage>().texture = tex;
                last.transform.SetParent(uiContainer.transform);

            }

            pending = false;
            sdh.FinishedGenerating.RemoveListener(CountingResults);
        }
    }


}
