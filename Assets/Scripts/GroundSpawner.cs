using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour{

    public GameObject ground;

	// Use this for initialization
	void Awake () {
        //Parent Gameobject
        GameObject parent = new GameObject("FloorParent");
        parent.transform.parent = GameObject.Find("World").transform;

        int nbOfX = 40;
        int nbOfY = 40;
        float totalWidth = nbOfX * ground.GetComponent<Renderer>().bounds.size.x;
        float totalLength = nbOfY * ground.GetComponent<Renderer>().bounds.size.z;
        float widthOffset = -totalWidth / 2;
        float lengthOffset = -totalLength / 2;
        for (int x = 0; x < nbOfX; x++) {
            for (int y = 0; y < nbOfY; y++) {
                GameObject groundObject = Instantiate(ground, new Vector3(widthOffset + x * ground.GetComponent<Renderer>().bounds.size.x, 0f, lengthOffset + y * ground.GetComponent<Renderer>().bounds.size.z), Quaternion.identity);
                groundObject.transform.parent = parent.transform;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}


}
