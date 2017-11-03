using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRayCaster : MonoBehaviour {

    private float sightLength = 30;
    private Transform camTransform;

    private void Awake() {
        camTransform = GetComponent<Transform>();
        //Debug.Log("Transform: " + transform);
    }
	
	// Update is called once per frame
	void Update () {

        RaycastHit seen;
        //Debug.DrawRay(camTransform.position, camTransform.forward * sightLength, Color.red);
        if (Physics.Raycast(camTransform.position, camTransform.forward, out seen, sightLength)) {
            //Debug.Log("Hit a collider");
            //Debug.Log("Hit " + seen.collider.name);
            ObjectManager.SetGameObject(seen.collider.gameObject);
        } else ObjectManager.SetGameObject(null);

	}
}
