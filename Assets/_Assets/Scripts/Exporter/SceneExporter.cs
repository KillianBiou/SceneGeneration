using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityGLTF;
using UnityGLTF.Plugins;

public class SceneExporter : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            StartCoroutine(TestExport());
        }
    }

    private IEnumerator TestExport()
    {
        Debug.Log("Export scene");
        //GLTFSceneExporter.Export

        Transform gameObjectHolder = GameObject.FindGameObjectWithTag("Playground").transform;
        Transform wallsHolder = RoomMap.Instance.ReturnOnlyActivatedTile().transform;

        yield return new WaitForEndOfFrame();

        foreach (Renderer t in wallsHolder.GetComponentsInChildren<Renderer>(false))
        {
            if (t.name.ToLower() == "shadow")
                Debug.Log("Shadows Remains");
                    //Destroy(t.gameObject);
        }
        Debug.Log("Name @ " + wallsHolder.name);
        //Transform lightHolder = GameObject.FindGameObjectWithTag("Playground").transform;

        Transform gameObjectHolderMem = gameObjectHolder.transform.parent;
        Transform wallsHolderMem = wallsHolder.transform.parent;

        GameObject tempParent = new GameObject("TempHolder");

        gameObjectHolder.parent = tempParent.transform;
        wallsHolder.parent = tempParent.transform;

        var settings = GLTFSettings.GetOrCreateSettings();
        settings.SaveFolderPath = Path.Combine(Application.dataPath);
        var exportOptions = new ExportContext(settings) { TexturePathRetriever = RetrieveTexturePath };
        var exporter = new GLTFSceneExporter(tempParent.transform, exportOptions);

        var path = settings.SaveFolderPath;
        if (!Directory.Exists(path))
            path = EditorUtility.SaveFolderPanel("glTF Export Path", settings.SaveFolderPath, "");

        if (!string.IsNullOrEmpty(path))
        {
            var ext = ".glb";
            var resultFile = GLTFSceneExporter.GetFileName(path, "TestScene", ext);
            settings.SaveFolderPath = path;
            exporter.SaveGLB(path, "TestScene");

            Debug.Log("Exported to " + resultFile);
            EditorUtility.RevealInFinder(resultFile);
        }

        gameObjectHolder.parent = gameObjectHolderMem;
        wallsHolder.parent = wallsHolderMem;

        Destroy(tempParent.gameObject);
        //Destroy(wallsHolder.gameObject);
    }

    public static string RetrieveTexturePath(Texture texture)
    {
        var path = AssetDatabase.GetAssetPath(texture);
        // texture is a subasset
        if (AssetDatabase.GetMainAssetTypeAtPath(path) != typeof(Texture2D))
        {
            var ext = Path.GetExtension(path);
            if (string.IsNullOrWhiteSpace(ext)) return texture.name + ".png";
            path = path.Replace(ext, "-" + texture.name + ext);
        }
        return path;
    }
}
