using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PictureGeneratorScript : MonoBehaviour
{

    private string basePath = "Assets/Resources\\";
    private List<string> paths = new List<string>();

    public void GeneratePictures() {
        Debug.Log("Generating Pictures");

        TraverseMap("Assets/Resources");
    }

    private void TraverseMap(string path) {

        try {
            foreach(string s in Directory.GetDirectories(path)) {
                TraverseMap(s);
            }
            foreach(string s in Directory.GetFiles(path)) {
                if(s.EndsWith(".prefab")) {
                    //Debug.Log("Found a prefab at path: " + s);
                    GeneratePNG(s, path);
                }
            }
            ChangeSettings();
        } catch(Exception e) {
            Debug.Log("Caught exception: " + e.ToString());
        }
    }

    private void ChangeSettings() {
        AssetDatabase.Refresh();

        foreach(string relPath in paths) {
            AssetDatabase.ImportAsset(relPath);
            TextureImporter importer = AssetImporter.GetAtPath(relPath) as TextureImporter;
            if(importer) {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.SaveAndReimport();
            }
            AssetDatabase.WriteImportSettingsIfDirty(relPath);
        }
    }

    private void GeneratePNG(string path, string dir) {

        int subLength = basePath.Length;

        string sub = path.Remove(path.Length - 7, 7);
        string prefabName = sub.Remove(0, dir.Length + 1);
        sub = sub.Remove(0, subLength);
        GameObject previewObject = Resources.Load(sub, typeof(GameObject)) as GameObject;
        if(previewObject) {

            Texture2D preview = AssetPreview.GetAssetPreview(previewObject);
            if(preview) {
                preview.alphaIsTransparency = true;
                //Sprite sprite = Sprite.Create(preview, new Rect(0.0f, 0.0f, preview.width, preview.height), new Vector2(0.5f, 0.5f));

                byte[] bytes = preview.EncodeToPNG();

                string dataPath = Application.dataPath;
                dataPath = dataPath.Substring(0, dataPath.Length - 7);  //Remove "Assets" part
                dataPath += "/" + dir + "/" + prefabName + ".png";
                //dataPath += "/" + dir + "/" + prefabName + ".asset";

                ////Debug.Log("Final datapath: " + dataPath);
                System.IO.File.WriteAllBytes(dataPath, bytes);
                string relPath = dir + "/" + prefabName + ".png";
                paths.Add(relPath);
                //AssetDatabase.CreateAsset(sprite, dataPath);
            } else {
                Debug.Log("Preview texture is null");
            }
        } else {
            Debug.Log("Preview object is null");
        }
    }
}
