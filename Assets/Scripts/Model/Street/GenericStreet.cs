using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericStreet : MonoBehaviour
{

    public Vector3 centerPoint;
	public Dictionary<string, Vector3> colliderAllowedPoints = new Dictionary<string, Vector3>();
	public Dictionary<string, Vector3> colliderRotatedPoints = new Dictionary<string, Vector3>();
	public Dictionary<string, Vector3> centerOffset = new Dictionary<string, Vector3>();
    public static int arrowID = 1;

    public Vector3 spawnStart;
	protected Vector2 direction = Vector2.zero;	//Direction of the street
	private Dictionary<string, string> connectedStreets = new Dictionary<string, string>();	//Dictionary that contains all other streets connected to this one along with the direction
	//First string is name of the direction, second one is the other street
	
	private static bool quitting = false;
	protected bool rotateBackCollider = true;	//Back collider should rotate, except for IntersectionT
	
	private List<GenericStreet> connectedGenericStreetScripts = new List<GenericStreet>();
	
    public virtual void SetAllowedPoints(List<string> allowedDirections = null) { }

    public virtual void SetBackCollider() { }

    //    public void RotateColliderPositions(int degrees)
    //    {
    //        Debug.Log("In call---------------");
    //        Vector3 point;
    //        Debug.Log("Allowed points length: " + colliderAllowedPoints.Count);
    //        foreach (KeyValuePair<string, Vector3> pair in colliderAllowedPoints)
    //            {
    //                point = pair.Value;
    //                Vector3 rotatedVector = Quaternion.AngleAxis(degrees, Vector3.up) * point;
    //                Debug.Log("Collider Rotated vector " + point.ToString() + " by " + degrees + " resulting in " + rotatedVector.ToString());
    //                point = rotatedVector;
    //                colliderRotatedPoints.Add(new KeyValuePair<string, Vector3>(pair.Key, point));
    //            }
    //        colliderAllowedPoints.Clear();
    //        foreach (KeyValuePair<string, Vector3> pair in colliderRotatedPoints)
    //        {
    //            colliderAllowedPoints.Add(new KeyValuePair<string, Vector3>(pair.Key, pair.Value));
    //        }
    //        colliderRotatedPoints.Clear();
    //    }
	
	public void AddConnectedStreet(string direction, string name){
		//Debug.LogError("Add to " + gameObject.transform.parent.name + " : Name = " + name + " Direction = " + direction);
		//if(connectedStreets.ContainsKey(direction)){
		//	connectedStreets.Remove(direction);
		//}
		
		connectedStreets[direction] = name;	//other is connected to this street at "direction" point
		//Debug.LogError("Add to street " + gameObject.transform.parent.name + ": " + direction + " and street " + name);
		
		//PrintConnectedStreets();
		
	}
	
	public void AddConnectedStreetScript(GenericStreet other){
		//Debug.LogError("Add CSC to " + gameObject.transform.parent.name + ": " + other.transform.parent.name); 
		if(connectedGenericStreetScripts.Contains(other)){
			connectedGenericStreetScripts.Remove(other);
		}
		connectedGenericStreetScripts.Add(other);
	}
	
	public void ConnectedStreetDestroyed(string otherStreetName){
		//Debug.LogError(gameObject.transform.parent.name + " destroyed street: " + otherStreetName);
		
		//Get the direction of the arrow this street should respawn
		string direction = null;
		foreach(KeyValuePair<string, string> connectedStreet in connectedStreets){
			if(connectedStreet.Value.Equals(otherStreetName)){
				direction = connectedStreet.Key;
				break;
			}
		}
		//PrintConnectedStreets();
		
		if(direction == null){
			//Debug.LogError("Direction for " + otherStreetName + " in " + gameObject.transform.parent.name + " not found");
			return;
		}
		
		connectedStreets.Remove(direction);
		
		//Copy all colliderAllowedPoint values to a backup
		Dictionary<string, Vector3> backup = new Dictionary<string, Vector3>();
		foreach (KeyValuePair<string, Vector3> colliderAllowedPoint in colliderAllowedPoints) {
			backup.Add(colliderAllowedPoint.Key, colliderAllowedPoint.Value);
		}
		
		//Clear the dictionary, only leaving the direction that is given
		colliderAllowedPoints.Clear();
		foreach (KeyValuePair<string, Vector3> backupPoint in backup) {
			if(backupPoint.Key.Equals(direction)){
				colliderAllowedPoints.Add(backupPoint.Key, backupPoint.Value);
			}
		}
		
		//Now do spawnColliders()
		/*Debug.LogError("Spawning colliders, dictionary entries: ");
		foreach (KeyValuePair<string, Vector3> colliderAllowedPoint in colliderAllowedPoints) {
			Debug.LogError(colliderAllowedPoint.Key);
		}
		*/
		
		SpawnColliders();
		
		//Put all values back in colliderAllowedPoints
		colliderAllowedPoints.Clear();
		foreach (KeyValuePair<string, Vector3> backupPoint in backup) {
			colliderAllowedPoints.Add(backupPoint.Key, backupPoint.Value);
		}
	}
	
	private void PrintConnectedStreets(){
		Debug.Log(" ------------Printing connected streets for " + gameObject.transform.parent.name + "------------------");
		foreach(KeyValuePair<string, string> connectedStreet in connectedStreets){
				Debug.Log("Key: " + connectedStreet.Key + " Value: " + connectedStreet.Value);
		}
		
		Debug.Log("--------------");
	}
	

    public void SpawnColliders()
    {

        //Hier bij elk allowedpoint: plaats collider iets buiten de straat (punt kunt ge berekenen door allowedpoint & centerpoint, kunt ge een soort richtingscoefficient uit halen, en voila)
        //Bij elke collider die collide met andere straat: verwijder deze allowedpoint
        //Voor elke collider die niet collide met een andere straat, spawn zo een selectable pijl van waaruit de gebruiker ook verder kan bouwen
        //Deze pijl houdt bij bij welke straat ie hoort alsook de allowedpoint (dus spawnpoint + direction)
        //Ook belangrijk: de straat waarmee gecollide wordt moet ook ff bij al z'n allowedpoints een collider plaatsen en ook de nodige verwijderen, zodat het in beide richtingen klopt
        //Als iemand dan toch een straat op dit punt wilt spawnen, staat die punt niet meer in de allowedpoints en zal het dus niet lukken
        //Nieuwe straat spawnen kan dan of door eerst van richting te veranderen, of bij een andere straat verder gaan door pijlen te selecteren

        //Als ge gewoon ergens een nieuwe straat tegen spawnt, moet bij de straat die er eerst stond dan ook ff die pijl verwijderd worden samen met de allowedpoint

        //Load streetCollider resource
        GameObject streetCollider = Resources.Load("ProceduralBlocks/StreetCollider") as GameObject;
		GameObject arrow = Resources.Load ("ProceduralBlocks/Arrow") as GameObject;

        Transform colliderChild = this.transform.parent.Find("Colliders");
        GameObject colliders;
        if (colliderChild == null)
        {
            colliders = new GameObject("Colliders");
			colliders.transform.localPosition = Vector3.zero;
			colliders.transform.SetParent(this.transform.parent, false);
        }
        else colliders = colliderChild.gameObject;

        //Alle colliders plaatsen
		foreach (KeyValuePair<string, Vector3> colliderAllowedPoint in colliderAllowedPoints) {
			//Debug.LogError ("Foreach, key: " + colliderAllowedPoint.Key + ", Value: " + colliderAllowedPoint.Value.ToString ());
			//Spawn a collider
			GameObject newCollider = GameObject.Instantiate (streetCollider);
			//newCollider.transform.position = colliderAllowedPoint.Value;
			newCollider.transform.localPosition = new Vector3(colliderAllowedPoint.Value.x, 0f, colliderAllowedPoint.Value.z);
			newCollider.transform.SetParent(colliders.transform, false);			//worldPosition to true so orientation of the street model does not change 		the position
			//newCollider.transform.localPosition = new Vector3(colliderAllowedPoint.Value.x, 0f, colliderAllowedPoint.Value.z);
			//Vector3 localPos = newCollider.transform.localPosition;
			//newCollider.transform.localPosition = new Vector3(localPos.x, 0f, localPos.z);
            newCollider.name = colliderAllowedPoint.Key;
			Vector3 centerOffsetPosition = centerOffset [colliderAllowedPoint.Key];
			newCollider.GetComponent<BoxCollider>().center = centerOffsetPosition;

			//Bepaal richting van de arrow
			//Vector3 firstPoint = newCollider.transform.localPosition; //TransformPoint (centerOffsetPosition);
			//Vector3 secondPoint = firstPoint + centerOffsetPosition;
			Vector3 firstPoint = Vector3.zero;
			Vector3 secondPoint = centerOffsetPosition;
			//Debug.LogError ("Second point: " + secondPoint.ToString ());

			//Vector3 secondPoint = newCollider.transform.position;
			//Debug.LogError (colliderAllowedPoint.Key);
			//Debug.LogError("TP: " + firstPoint.ToString());
			//Debug.LogError ("TP2: " + secondPoint.ToString());

			double angle = Mathf.Rad2Deg * Mathf.Atan((secondPoint.x  - firstPoint.x) / (  secondPoint.z -  firstPoint.z));
            //Debug.Log("##########colliderAllowedpoint:" + colliderAllowedPoint.Key + " en de rico is " + Mathf.Atan((secondPoint.x - firstPoint.x) / (secondPoint.z - firstPoint.z)));
            if (colliderAllowedPoint.Key.Equals("back") && rotateBackCollider)
            {
                angle = 180;
            }
			
			GameObject spawnedArrow = GameObject.Instantiate (arrow);
			spawnedArrow.transform.SetParent (newCollider.transform, false);
			spawnedArrow.transform.localPosition = new Vector3 (centerOffsetPosition.x, 1f, centerOffsetPosition.z);
			spawnedArrow.transform.Rotate (new Vector3 (0, 1, 0) * (float) angle);
            spawnedArrow.transform.name = "arrowID:" + arrowID++;
			//spawnedArrow.transform.localScale = new Vector3 (1, 1, 1);
        }
    }

    public void RemoveCollider(string direction)
    {
        //Debug.LogError("Remove collider: " + direction);
        //Remove collider from this specific direction
		//colliderAllowedPoints.Remove(direction);
		//centerOffset.Remove (direction);

        Transform colliderChild = this.transform.parent.Find("Colliders");
        if(colliderChild != null)
        {
            Transform collider = colliderChild.Find(direction);
            if(collider != null)
            {
                MonoBehaviour.Destroy(collider.gameObject);
            }
        }
    }

    public void CheckColliders(bool checkOther)
    {
        //Check for all colliders whether or not they collided with a street
        Transform colliderChild = this.transform.parent.Find("Colliders");
        if(colliderChild != null)
        {
            //Check all children
            Transform colliderTransform;
            ColliderScript colliderScript;
            List<Transform> childrenToDestroy = new List<Transform>();
            int nbChildren = colliderChild.childCount;
            for(int i = 0; i < nbChildren; i++)
            {
                colliderTransform = colliderChild.GetChild(i);
                colliderScript = colliderTransform.GetComponent<ColliderScript>();
                if (colliderScript.CheckCollision())
                {
                    childrenToDestroy.Add(colliderTransform);
					
					//Debug.LogError("Add 2 to " + gameObject.transform.parent.name + " : Name = " + colliderScript.GetNameCollidedStreet() + " Direction = " + colliderTransform.name); 
					string name = colliderScript.GetNameCollidedStreet();
					//if(connectedStreets.ContainsKey(name)){
					//	connectedStreets.Remove(name);
					//}
					
					connectedStreets[colliderTransform.name] = name;
					//PrintConnectedStreets();
					colliderScript.SetConnectedStreet(this);
                }
            }

            //Destroy all these children, also important: the street they collided with has to check their points again as well
            foreach(Transform child in childrenToDestroy)
            {
                if (checkOther)
                {
                    child.GetComponent<ColliderScript>().CheckOtherStreet();
                }
                RemoveCollider(child.name);
                //MonoBehaviour.Destroy(child.gameObject);
            }
        }
    }


	public void SetCorrectPoint(Orientation pointDirection)
    {
		Transform world = GameObject.Find ("World").transform;
		//For all colliders, check their position
		Transform colliderChild = this.transform.parent.Find("Colliders");
		if (colliderChild != null) {
			int nbChildren = colliderChild.childCount;
			Transform child;
			for (int i = 0; i < nbChildren; i++) {
				child = colliderChild.GetChild (i);
				if (child.name.Equals (pointDirection.ToString())) {
					spawnStart = child.position;
					//Debug.LogError ("Pos: " + spawnStart.ToString ());
					//spawnStart.y = 0f;
					//spawnStart = world.TransformPoint(spawnStart);
					//spawnStart.y = 0;
					//spawnStart = world.TransformPoint(spawnStart);
					//spawnStart.y = 0f;

					//Debug.LogError("TP1: " + world.TransformPoint(spawnStart));
					//Debug.LogError ("TP2: " + world.InverseTransformPoint (spawnStart));
					//Debug.LogError("TP3: " + this.transform.TransformPoint(spawnStart));
					//Debug.LogError ("TP4: " + this.transform.InverseTransformPoint (spawnStart));
					//Debug.LogError("TP5: " + colliderChild.TransformPoint(spawnStart));
					//Debug.LogError ("TP6: " + colliderChild.InverseTransformPoint (spawnStart));
					spawnStart = world.InverseTransformPoint(spawnStart);
					spawnStart.y = 0f;

					return;
				}
			}

		}

        throw new Exception("Given direction not found in allowedPairs");

    }

	public bool DirectionInAllowedPoints(string direction){
		return colliderAllowedPoints.ContainsKey (direction);
	}

    public Vector3 GetSpawnPoint()
    {
        return spawnStart;
    }

    public Vector3 GetCenterPoint()
    {
        return centerPoint;
    }

    public void SetCenterPoint(Vector3 newCenterPoint)
    {
        centerPoint = newCenterPoint;
    }
	
	public void SetDirection(Vector2 dir)
    {
        direction = dir;
    }
	
	public virtual Vector2 GetDirection(bool back){
		return direction;
	}
	
	private void OnDestroy(){	//When this street gets destroyed, the streets it is connected to should re-allow this direction 
		//Since this will spawn stuff @ OnDestroy, which is also called when the application quits, we should check if the application is quitting when this gets called
		if(quitting)
			return;
		
		foreach (GenericStreet connectedGenericStreet in connectedGenericStreetScripts) {
				//Inform this GenericStreet that this street has been deleted, respawn this arrow
				if(connectedGenericStreet != null){
					//Debug.LogError("Not null");
					connectedGenericStreet.ConnectedStreetDestroyed(this.gameObject.transform.parent.name);
				}
		}
	}
	
	private void OnApplicationQuit(){
		quitting = true;
	}

    public virtual Vector3 GetTopPoint() { return Vector3.zero; }
    public virtual Vector3 GetBottomPoint() { return Vector3.zero; }
    public virtual Vector3 GetLeftPoint() { return Vector3.zero; }
    public virtual Vector3 GetRightPoint() { return Vector3.zero; }
    public virtual Vector3 GetOffsetPoint() { return Vector3.zero; }
    public virtual string GetTypePoint() { return null; }
    

    public virtual void SetTopPoint(Vector3 point) { }
    public virtual void SetBottomPoint(Vector3 point) { }
    public virtual void SetLeftPoint(Vector3 point) { }
    public virtual void SetRightPoint(Vector3 point) { }

    public virtual void SetRightTurn() { }
}
