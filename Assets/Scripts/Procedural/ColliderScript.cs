using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ColliderScript : MonoBehaviour
{
    private bool collidedWithStreet = false;
    private Transform collidedStreet;
    private string orientation = "";    //In case this colliderSCriptis attached to a t-intersection
    private Vector3 position = Vector3.zero;
    private Vector2 direction = Vector2.zero;

    private void OnTriggerEnter(Collider other)
    {
        //Did we collide with a street?
        EditorGUIUtility.PingObject(this.gameObject);
        if (other.gameObject.layer == LayerMask.NameToLayer("CanTeleport"))
        {
            collidedWithStreet = true;
            collidedStreet = other.transform;

            //We collided with a street, so make parent do the CheckCollideers()
            this.transform.GetComponentInParent<GenericStreet>().CheckColliders(true);
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

    public void SetVars(Vector3 pos, string orient, Vector2 dir)
    {
        Debug.LogError("Set orientation to" + orient + " , position: " + pos.ToString() + " , direction: " + dir.ToString());
        orientation = orient;
        position = pos;
        direction = dir;
    }

    public void SelectedCollider()
    {
        //Update the position and orientation to match the selected collider

        //Get the (world) position of the street
        //Vector3 streetPosition = this.transform.parent.parent.parent.position; //First parent is 'Colliders', second parent is the street, third parent is general parent
        string dir = this.transform.name;
        Debug.LogError("Calling change direction with position: " + position + " , direction: " + dir + " , orientation: " + orientation + " , direction vector: " + direction.ToString());
        StreetGeneratorV2.SetPreviousStreetScript(this.transform.GetComponentInParent<GenericStreet>());
        GeneratorManager.ChangeDirectionFromCollider(position, dir, orientation, direction);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }

}