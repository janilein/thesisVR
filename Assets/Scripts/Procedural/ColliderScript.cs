using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderScript : MonoBehaviour
{
    private bool collidedWithStreet = false;
    private Transform collidedStreet;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ColliderScript called OnTriggerEnter");
        //Did we collide with a street?
        if(other.gameObject.layer.Equals("CanTeleport"))
        {
            Debug.Log("ColliderScript collided with street");
            collidedWithStreet = true;
            collidedStreet = other.transform;
        }
    }

    public bool CheckCollision()
    {
        return collidedWithStreet;
    }

    public void CheckOtherStreet()
    {
        collidedStreet.GetComponentInParent<GenericStreet>().CheckColliders(false);
    }

}