using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour {

    public GameObject ground;

	// Use this for initialization
	void Awake () {
        //Parent Gameobject
        GameObject parent = new GameObject("FloorParent");

        int width = 20;
        int length = 20;
        float totalWidth = width * ground.GetComponent<Renderer>().bounds.size.x;
        float totalLength = width * ground.GetComponent<Renderer>().bounds.size.z;
        float widthOffset = -totalWidth / 2;
        float lengthOffset = -totalLength / 2;
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < length; y++) {
                GameObject groundObject = Instantiate(ground, new Vector3(widthOffset + x * ground.GetComponent<Renderer>().bounds.size.x, 0f, lengthOffset + y * ground.GetComponent<Renderer>().bounds.size.z), Quaternion.identity);
                groundObject.transform.parent = parent.transform;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}


}
