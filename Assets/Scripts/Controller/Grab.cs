﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//From https://www.raywenderlich.com/149239/htc-vive-tutorial-unity

public class Grab : MonoBehaviour
{

    private SteamVR_TrackedObject trackedObj;
    private GameObject grabbedObject;
    private GameObject collidedObject;
    private TransitionScript transitionScript;

    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        transitionScript = GameObject.Find("TransitionManager").GetComponent<TransitionScript>();
    }

    private void Update()
    {
        if (Controller.GetHairTriggerDown())
        {
            Debug.Log("Pressed trigger");
            if (collidedObject)
            {
                if (collidedObject.transform.name.ToLower().Contains("doorhandle"))
                {
                    transitionScript.OpenDoor();
                }
                else if (collidedObject.transform.name.ToLower().Contains("okean"))
                {
                    collidedObject.GetComponent<Radio>().ToggleRadio();
                }
                else
                {
                    GrabObject(collidedObject);
                }
            }
        }
        else if (Controller.GetHairTriggerUp())
        {
            Debug.Log("Released trigger");
            if (grabbedObject)
            {
                Debug.Log("Releasing object");
                ReleaseObject();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (collidedObject == other.gameObject)
        {
            Debug.Log("Set collided object to null");
            collidedObject = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        //Check if other is grabbable, and trigger is pressed
        if (other.transform.tag.Equals("Grabbable"))
        {
            Debug.Log("Collided with grabbable object");
            if (grabbedObject == null)
            {
                collidedObject = other.gameObject;
            }
        } else if (other.transform.name.Equals("okean"))
        {
            collidedObject = other.gameObject;
        }
    }

    private void GrabObject(GameObject obj)
    {
        Debug.Log("Grabbing object");
        grabbedObject = obj;
        var joint = AddFixedJoint();
        joint.connectedBody = grabbedObject.GetComponent<Rigidbody>();
        grabbedObject.GetComponent<Rigidbody>().useGravity = false;
    }

    private void ReleaseObject()
    {
        if (GetComponent<FixedJoint>())
        {
            GetComponent<FixedJoint>().connectedBody = null;
            Destroy(GetComponent<FixedJoint>());
            grabbedObject.GetComponent<Rigidbody>().velocity = Controller.velocity;
            grabbedObject.GetComponent<Rigidbody>().angularVelocity = Controller.angularVelocity;
            grabbedObject.GetComponent<Rigidbody>().useGravity = true;
        }

        if (grabbedObject.transform.name.Contains("floppy"))
        {
            //Debug.LogError("release floppy");
            grabbedObject.transform.GetComponent<FloppyDisk>().OnRelease();
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

    //Gets called automatically when the joint breaks
    private void OnJointBreak(float breakForce)
    {
        grabbedObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        grabbedObject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        grabbedObject.GetComponent<Rigidbody>().useGravity = true;
        grabbedObject = null;
    }
}
