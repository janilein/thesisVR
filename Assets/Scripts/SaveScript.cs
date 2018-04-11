using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveScript : MonoBehaviour {

    public GameObject world;

    public void Save()
    {
        Debug.Log("Object saved");
    }

    public void Load()
    {
        Debug.Log("Object loaded");
    }
}
