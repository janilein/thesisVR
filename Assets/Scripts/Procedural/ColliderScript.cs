using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class ColliderScript : MonoBehaviour
{
    private bool collidedWithStreet = false;
    private Transform collidedStreet;
    private Vector2 direction = Vector2.zero;
	private string nameCollidedStreet = "";

    private void OnTriggerEnter(Collider other)
    {
        //Did we collide with a street?
        if (other.gameObject.layer == LayerMask.NameToLayer("CanTeleport"))
        {
			//Just to make sure the "collision" didn't happen with its own street
			if(other.gameObject.transform.parent == gameObject.transform.parent.parent)
				return;
			
            collidedWithStreet = true;
            collidedStreet = other.transform;
			nameCollidedStreet = other.transform.parent.name;

            //We collided with a street, so make parent do the CheckColliders()
			this.transform.parent.parent.GetComponentInChildren<GenericStreet>().CheckColliders(true);
        }
    }

    public bool CheckCollision()
    {
        return collidedWithStreet;
    }
	
	public string GetNameCollidedStreet(){
		return nameCollidedStreet;
	}

    public void CheckOtherStreet()
    {
        collidedStreet.GetComponentInParent<GenericStreet>().CheckColliders(false);
    }
	
	public void SetConnectedStreet(GenericStreet street){
		collidedStreet.GetComponent<GenericStreet>().AddConnectedStreetScript(street);
	}

    public void SelectedCollider()
    {
        //Update the direction to match the selected collider
        string dir = this.transform.name;
		StreetGeneratorV2.SetPreviousStreetScript(this.transform.parent.parent.GetComponentInChildren<GenericStreet>());
		//Get the GenericStreet script.
		/* Layout:
		 * 	Parent
		 * 		Street with attached GenericStreet
		 * 		Colliders
		 * 			Left, Right, ... whatever exist, this script is attached at this point
		 * 
		 */
		 
		//Get direction from the street. Bool = backcollider yes or no
		bool back = false;
		if(gameObject.transform.name.ToLower().Contains("back")){
			back = true;
		}
		Vector2 direction = gameObject.transform.parent.parent.GetComponentInChildren<GenericStreet>().GetDirection(back);
		 
		Orientation orient = OrientationEnumFunctions.GetOrientationFromString (dir);
        GeneratorManager.ChangeDirectionFromCollider(orient, direction);


    }

}