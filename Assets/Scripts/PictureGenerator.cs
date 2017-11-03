using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//[ExecuteInEditMode]
[CustomEditor(typeof(PictureGeneratorScript))]
public class PictureGenerator : Editor {

    public override void OnInspectorGUI() {

        DrawDefaultInspector();

        PictureGeneratorScript script = (PictureGeneratorScript)target;

        if(GUILayout.Button("Generate pictures")) {
            script.GeneratePictures();
        }
    }

}
