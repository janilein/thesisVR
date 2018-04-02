using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderSelectTester : MonoBehaviour {

    public GameObject selectedDirection = null;
	
	// Update is called once per frame
	void Update () {
        if (selectedDirection)
        {
            selectedDirection.GetComponent<ColliderScript>().SelectedCollider();
            selectedDirection = null;
        }
	}
}
