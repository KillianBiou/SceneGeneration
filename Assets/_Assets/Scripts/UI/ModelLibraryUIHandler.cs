using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

[RequireComponent(typeof(ModelLibraryDragDropHandler))]
public class ModelLibraryUIHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject entryHolder;
    [SerializeField]
    private GameObject entryGameobject;
    [SerializeField]
    private Texture2D placeholder;

    private ModelLibraryDragDropHandler dragDropHandler;

    private void Start()
    {
        GenerationDatabase.OnDatabaseUpdated += DatabaseUpdatedReceiver;
        dragDropHandler = GetComponent<ModelLibraryDragDropHandler>();
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Alpha3)) { 
            DatabaseUpdatedReceiver();
        }
    }

    public void DatabaseUpdatedReceiver()
    {
        Debug.Log("Refresh Database UI");

        // Clear View (Not optimal, but sufficient for now as they wont be more than 20 models)
        ClearView();

        foreach((string, string) path in GenerationDatabase.Instance.GetAssetsDict())
        {
            AddEntry(path.Item1, path.Item2);
        }
    }

    private void AddEntry(string objName, string jsonPath)
    {
        string imagePath = jsonPath.Replace(".json", ".png");
        Texture2D showcaseImage = new Texture2D(2, 2);
        if (File.Exists(imagePath))
            ImageConversion.LoadImage(showcaseImage, File.ReadAllBytes(imagePath), false);
        else
            showcaseImage = placeholder;

        GameObject modelView = Instantiate(entryGameobject, entryHolder.transform);
        modelView.GetComponentInChildren<RawImage>().texture = showcaseImage;
        modelView.name = objName;

        EventTrigger trigger = modelView.GetComponent<EventTrigger>();

        // Add Begin Drag Trigger
        EventTrigger.Entry beginDrag = new EventTrigger.Entry();
        beginDrag.eventID = EventTriggerType.BeginDrag;
        beginDrag.callback.AddListener((data) => { dragDropHandler.BeginDrag((PointerEventData)data); });

        // Add Drag Trigger
        EventTrigger.Entry drag = new EventTrigger.Entry();
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener((data) => { dragDropHandler.Drag((PointerEventData)data); });

        // Add end Drag Trigger
        EventTrigger.Entry endDrag = new EventTrigger.Entry();
        endDrag.eventID = EventTriggerType.EndDrag;
        endDrag.callback.AddListener((data) => { dragDropHandler.EndDrag((PointerEventData)data); });

        trigger.triggers.Add(beginDrag);
        trigger.triggers.Add(drag);
        trigger.triggers.Add(endDrag);
    }

    private void ClearView()
    {
        foreach (Transform item in entryHolder.transform)
        {
            if(item.name != "AddModel")
                Destroy(item.gameObject);
        }
        Debug.Log("Clear database View");
    }
}
