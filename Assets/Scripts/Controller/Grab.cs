using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//From https://www.raywenderlich.com/149239/htc-vive-tutorial-unity

public class Grab : MonoBehaviour {

    private SteamVR_TrackedObject trackedObj;
    private GameObject grabbedObject;

    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    private void Update()
    {
        if (Controller.GetHairTriggerUp()) {
            if (grabbedObject)
            {
                ReleaseObject();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        //Check if other is grabbable, and trigger is pressed
        if (other.transform.tag.Equals("Grabbable"))
        {
            //Debug.Log("Collided with grabbable object");
            if (grabbedObject == null)
            {
                //Debug.Log("Grabbed object is null");
                if (Controller.GetHairTriggerDown())
                {
                    //Debug.Log("Pressed trigger");
                    GrabObject(other.gameObject);
                }
            }
        }
    }

    private void GrabObject(GameObject obj)
    {
        grabbedObject = obj;
        var joint = AddFixedJoint();
        joint.connectedBody = grabbedObject.GetComponent<Rigidbody>();
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            grabbedObject.GetComponent<Rigidbody>().velocity = Controller.velocity;
            grabbedObject.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
        }
        grabbedObject = null;
    }

    private FixedJoint AddFixedJoint()
    {
        FixedJoint fx = gameObject.AddComponent<FixedJoint>();
        fx.breakForce = 20000;
        fx.breakTorque = 20000;
        return fx;
    }
}
