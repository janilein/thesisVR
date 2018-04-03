﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericStreet : MonoBehaviour
{

    public Vector3 centerPoint;

	public Dictionary<string, Vector3> colliderAllowedPoints = new Dictionary<string, Vector3>();
	public Dictionary<string, Vector3> colliderRotatedPoints = new Dictionary<string, Vector3>();
	public Dictionary<string, Vector3> centerOffset = new Dictionary<string, Vector3>();

	public bool keepPosition = false;

    public Vector3 spawnStart;

    public virtual void SetAllowedPoints(List<string> allowedDirections = null) { }

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
		foreach (KeyValuePair<string, Vector3> colliderAllowedPoint in colliderAllowedPoints)
        {
            //Spawn a collider
            GameObject newCollider = GameObject.Instantiate(streetCollider);
            newCollider.transform.localPosition = colliderAllowedPoint.Value;
			newCollider.transform.SetParent(colliders.transform, keepPosition);			//worldPosition to true so orientation of the street model does not change the position
			Vector3 localPos = newCollider.transform.localPosition;
			newCollider.transform.localPosition = new Vector3(localPos.x, 0f, localPos.z);
            newCollider.name = colliderAllowedPoint.Key;
			newCollider.GetComponent<BoxCollider>().center = centerOffset [colliderAllowedPoint.Key];
        }
    }

    public void RemoveCollider(string direction)
    {
        //Debug.LogError("Remove collider: " + direction);
        //Remove collider from this specific direction
		colliderAllowedPoints.Remove(direction);
		centerOffset.Remove (direction);

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


    public void SetCorrectPoint(string pointDirection)
    {
		//For all colliders, check their position
		Transform colliderChild = this.transform.parent.Find("Colliders");
		if (colliderChild != null) {
			int nbChildren = colliderChild.childCount;
			Transform child;
			for (int i = 0; i < nbChildren; i++) {
				child = colliderChild.GetChild (i);
				if (child.name.Equals (pointDirection)) {
					spawnStart = child.position;
					spawnStart.y = 0f;
					return;
				}
			}

		}

        throw new Exception("Given direction not found in allowedPairs");

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
