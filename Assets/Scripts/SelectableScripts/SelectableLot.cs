using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableLot : SelectableObject {

    private Texture2D defaultTex, highlightTex;

    public void Awake()
    {
        defaultTex = generateTexture(Color.green);
        highlightTex = generateTexture(Color.yellow);
    }

    private Texture2D generateTexture(Color col)
    {
        Texture2D tex = new Texture2D(256, 256, TextureFormat.ARGB32, false);

        Color fillColor = Color.clear;
        Color[] fillPixels = new Color[tex.width * tex.height];

        for (int i = 0; i < fillPixels.Length; i++)
        {
            fillPixels[i] = fillColor;
        }

        tex.SetPixels(fillPixels);

        for (int i = 0; i < 256; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                tex.SetPixel(i, j, col);
                tex.SetPixel(i, 255 - j, col);
            }
        }

        for (int j = 0; j < 256; j++)
        {
            for (int i = 0; i < 10; i++)
            {
                tex.SetPixel(i, j, col);
                tex.SetPixel(255 - i, j, col);
            }
        }

        tex.Apply();
        return tex;
    }

    public override void HighlightObject()
    {
        //Debug.LogError("Highlight lot");
        this.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", highlightTex);
    }

    public override void RemoveHighlight()
    {
        //Debug.LogError("Remove highlight lot");
        this.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex", defaultTex);
    }
}
