using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeShaders : MonoBehaviour {

    public Color colorRGB = new Color32( 0xE7, 0xFF, 0x06, 0xFF);

    private GameObject[] highlightableObjects;

    private void Awake() {
        UpdateShaders();
    }

    public void ChangeShaderColor(Color color) {
        colorRGB = color;
        UpdateShaders();
    }

    public void UpdateObjects() {
        highlightableObjects = GameObject.FindGameObjectsWithTag("Highlightable");
        //Debug.Log("Found " + highlightableObjects.Length + " objects");
    }

    public void UpdateShaders() {
        UpdateObjects();
        foreach (GameObject highlightableObject in highlightableObjects) {
            MeshRenderer[] shaderCollection = highlightableObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in shaderCollection) {
                foreach (Material material in renderer.materials) {
                    if (material.shader.name.Equals("Custom/Outline")) {

                        material.SetColor("_OutlineColor", colorRGB);
                    }
                }
            }
        }
    }
}
