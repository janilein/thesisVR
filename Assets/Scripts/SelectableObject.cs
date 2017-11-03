﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableObject : MonoBehaviour {

    public void HightlightObject() {
        MeshRenderer[] shaderCollection = GetComponentsInChildren<MeshRenderer>();

        foreach (MeshRenderer renderer in shaderCollection) {
            foreach (Material material in renderer.materials) {
                if (material.shader.name.Equals("Custom/Outline")) {
                    material.SetFloat("_Outline", 0.1f);
                }
            }
        }
    }

    public void RemoveHighlight() {
        MeshRenderer[] shaderCollection = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer renderer in shaderCollection) {
            foreach (Material material in renderer.materials) {
                if (material.shader.name.Equals("Custom/Outline")) {
                    material.SetFloat("_Outline", 0f);
                }
            }
        }

    }

}
