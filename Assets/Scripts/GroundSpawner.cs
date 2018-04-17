using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpawner : MonoBehaviour{

    public GameObject ground;

	// Use this for initialization
	void Awake () {
        SpawnGround();
    }

    public void SpawnGround()
    {
        //Parent Gameobject
        Transform parent = new GameObject("FloorParent").transform;
        parent.SetParent(GameObject.Find("World").transform, false);

        int nbOfX = 20;
        int nbOfY = 20;
        float totalWidth = nbOfX * ground.GetComponent<Renderer>().bounds.size.x;
        float totalLength = nbOfY * ground.GetComponent<Renderer>().bounds.size.z;
        float widthOffset = -totalWidth / 2;
        float lengthOffset = -totalLength / 2;
        for (int x = 0; x < nbOfX; x++)
        {
            for (int y = 0; y < nbOfY; y++)
            {
                GameObject groundObject = Instantiate(ground, new Vector3(widthOffset + x * ground.GetComponent<Renderer>().bounds.size.x, 0f, lengthOffset + y * ground.GetComponent<Renderer>().bounds.size.z), Quaternion.identity);
                groundObject.transform.SetParent(parent, false);
            }
        }
    }


}
