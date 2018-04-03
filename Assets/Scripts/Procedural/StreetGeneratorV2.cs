using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetGeneratorV2 : Generator {

    private Vector3 spawnPosition;
    private static GenericStreet previousStreetScript = null;
    //private string newPointDirection = "";

    private static int streetID = 1;

    public StreetGeneratorV2()
    {
        //spawnPosition = new Vector3(0, 1.f, 0);
        if (worldTransform == null)
        {
            worldTransform = GameObject.Find("World").transform;
        }
        
    }


    public override void GenerateWorldObject(WorldObject obj, ref Vector2 currentDirection, ref Vector3 currentPosition, string pointDirection) {
        float yOffset = 0.2f;
        //spawnPosition = worldTransform.position + new Vector3(-0.05f, yOffset, -0.05f);
		spawnPosition = new Vector3(0f, yOffset, 0f);

        obj = obj.GetChildren()[0];

        //Only 5 types of streets allowed, so switch case them all
        string type = obj.GetObjectValue();
        Debug.Log("Street type: " + type + " -----------------------------------");
        Transform parent = (new GameObject("streetID:" + streetID++)).transform;
        parent.SetParent(worldTransform, false);
		parent.localPosition = spawnPosition;

        //parent.gameObject.GetComponent<ObjectRelation>().CreateGameObject();

        switch (type)
        {
            case "straight":
                GenerateStraightStreet(obj, parent, currentDirection, ref currentPosition, pointDirection);
                break;
            case "intersection-t":
                GenerateIntersectionT(obj, parent, currentDirection, ref currentPosition, pointDirection);
                break;
            case "intersection-x":
                GenerateIntersectionX(obj, parent, currentDirection, ref currentPosition, pointDirection);
                break;
            case "roundabout":
                //GenerateRoundabout(obj, parent, currentDirection);
                break;
            case "turn":
                GenerateTurn(obj, parent, ref currentDirection, ref currentPosition, pointDirection);
                break;
        }
        //previousStreet = parent.transform;

    }

    public GameObject InstantiateStreet(GameObject street, Vector3 spawnPosition, Quaternion rotation, Transform parent) {
		GameObject newStreet = GameObject.Instantiate (street);
		newStreet.transform.SetParent (parent, false);
		newStreet.transform.localPosition = Vector3.zero;
		return newStreet;

		//return GameObject.Instantiate(street, spawnPosition, rotation, parent);

        //float scale = ObjectManager.GetScalingFactor();
        //if (ObjectManager.IsInRoom()) {
        //    GameObject.Instantiate(street, spawnPosition * scale, rotation, parent.GetComponent<ObjectRelation>().GetRelatedObject().transform);
        //} else
        //{
        //    GameObject.Instantiate(street, spawnPosition / scale, rotation, parent.GetComponent<ObjectRelation>().GetRelatedObject().transform);
        //}
        //return newStreet;
    }

    private void GenerateIntersectionX(WorldObject obj, Transform parent, Vector2 currentDirection, ref Vector3 currentPosition, string pointDirection)
    {
        Debug.Log("Generating Intersection-x");
        //Fetch the x intersection from resources
        GameObject street = Resources.Load("ProceduralBlocks/Streets/Intersection-x") as GameObject;
        //street = GameObject.Instantiate(street, spawnPosition, Quaternion.identity, parent);
        street = InstantiateStreet(street, spawnPosition, Quaternion.identity, parent);


        UpdateAllowedPointsIntersectionX(street);
        SpawnStreet(street, parent, currentDirection, ref currentPosition, "intersection-x", pointDirection);
    }

    private void GenerateIntersectionT(WorldObject obj, Transform parent, Vector3 currentDirection, ref Vector3 currentPosition, string pointDirection)
    {
        Debug.Log("Generating Intersection-t");
        string straight = (string)obj.directAttributes["lotsStraight"];
        int lotsStraight;
        Int32.TryParse(straight, out lotsStraight);
        string orientation = (string)obj.directAttributes["orientation"];
        //parent.rotation = orientationCoor;
        GameObject street = Resources.Load("ProceduralBlocks/Streets/Intersection-t") as GameObject;

        //Width, length & height of street
        Mesh mesh = street.GetComponentInChildren<MeshFilter>().sharedMesh;
        Vector3 meshSize = mesh.bounds.size;
        float streetWidth, streetLength, streetHeight;
        Vector3 scale = street.transform.lossyScale;
        streetWidth = scale.x * meshSize.x;
        streetLength = scale.z * meshSize.y;    //Swapping these is not by accident (street is turned 90 degrees)
        streetHeight = scale.y * meshSize.z;

        //Create lots equally spaced along both sides of the straight street
        //First left side
        if (lotsStraight > 0)
        {
            float lotSpacing = streetLength / lotsStraight;
            float lotLength = 20f;
            float lotWidth = lotSpacing;

            Debug.Log("---------------------------------");
            Debug.Log("Street length: " + streetLength);
            Debug.Log("Street width: " + streetWidth);
            Debug.Log("Lot spacing: " + lotSpacing);
            Debug.Log("Lot length: " + lotLength);
            

            //Get the starting point (left bottom corner of the street)
            Vector3 spawnPointLot = new Vector3(-streetLength / 2, 0f, -10.35f);
            //spawnPointLot += new Vector3(0f, 0f, -1.275f); //correct offset due to different symmetry

            Debug.Log("SpawnpointLot: " + spawnPointLot.ToString());

            spawnPointLot += new Vector3(lotWidth / 2, 0f, -lotLength / 2);
            Debug.Log("Spawnpoint lot 2: " + spawnPointLot.ToString());
            Debug.Log("---------------------------------");

            SpawnLot(lotsStraight, spawnPointLot, lotLength, lotWidth, parent, Orientation.straight, new Vector3(lotWidth, 0f, 0f));
        }

        //street = GameObject.Instantiate(street, spawnPosition, Quaternion.identity, parent);
        street = InstantiateStreet(street, spawnPosition, Quaternion.identity, parent);
        UpdateOrientationIntersectionT(parent, orientation, pointDirection);
        //street.GetComponent<GenericStreet>().RotateColliderPositions(angle);
		street.GetComponent<IntersectionT> ().ChangeOrientation (orientation);

        UpdateAllowedPointsIntersectionT(street, orientation);
        SpawnStreet(street, parent, currentDirection, ref currentPosition, "intersection-t", pointDirection);

    }

    private void UpdateOrientationIntersectionT(Transform parent, string orientation, string pointDirection)
    {
        //int angle = 0;
        switch (orientation)
        {
            case "leftRight":
                parent.transform.Rotate(new Vector3(0, 1, 0), 180);
                //angle = 180;
                //newPointDirection = pointDirection;
                //Debug.Log("PointDir0 = " + pointDirection);
                //Debug.Log("NewPointDir0 = " + newPointDirection);
                break;
            case "leftStraight":
                parent.transform.Rotate(new Vector3(0, 1, 0), -90);
                //angle = -90;
                break;
            case "rightStraight":
                parent.transform.Rotate(new Vector3(0, 1, 0), 90);
                //angle = 90;
                break;
        }
        //return angle;
    }

    private void GenerateTurn(WorldObject obj, Transform parent, ref Vector2 currentDirection, ref Vector3 currentPosition, string pointDirection)
    {
        string angle = (string)obj.directAttributes["angle"];
        GameObject street = Resources.Load("ProceduralBlocks/Streets/Turn" + angle) as GameObject;

        string direction = (string)obj.directAttributes["direction"];
        Debug.Log("Direction: " + direction);

        float spawnAngle = 0f;
        switch (angle)
        {
            case "90":
                spawnAngle = 90f;
                break;
            case "45":
                spawnAngle = 45f;
                break;
        }

        Debug.Log("Spawnangle: " + spawnAngle);

        if (direction.ToLower().Equals("right"))
        {
			street = InstantiateStreet(street, spawnPosition, Quaternion.identity, parent);
			//Rotate street to make it a right one
			if (spawnAngle == 90)
				parent.Rotate (new Vector3 (0, 1, 0), 270);
			else if (spawnAngle == 45)
				parent.Rotate (new Vector3 (0, 1, 0), 225);

            street.GetComponent<GenericStreet>().SetRightTurn();
        } else
        {
			street = InstantiateStreet(street, spawnPosition, Quaternion.identity, parent);
        }
        UpdateAllowedPointsStraightStreet(street);

        SpawnStreet(street, parent, currentDirection, ref currentPosition, "turn", pointDirection);

        int sign = 1;
        if (direction.ToLower().Equals("left"))
        {
            sign = -1;
        } else
        {
            sign = +1;
        }

        switch (angle)
        {
            case "90":
                currentDirection.x = (currentDirection.x + sign * 90) % 360;
                break;
            case "45":
                currentDirection.x = (currentDirection.x + sign * 45) % 360;
                break;
        }
			
		foreach (ColliderScript collScript in previousStreetScript.transform.parent.GetComponentsInChildren<ColliderScript>())
        {
            collScript.SetDirection(currentDirection);
        }

    }

        private void GenerateStraightStreet(WorldObject obj, Transform parent, Vector2 currentDirection, ref Vector3 currentPosition, string pointDirection)
    {
        Debug.Log("Generating straight street");

        //Okay uhm boom
        string left = (string)obj.directAttributes["lotsLeft"];
        string right = (string)obj.directAttributes["lotsRight"];

        int lotsLeft, lotsRight;
        Int32.TryParse(left, out lotsLeft);
        Int32.TryParse(right, out lotsRight);

        //Fetch the straight street from resources
        string length = (string)obj.directAttributes["length"];
        length = length.Substring(0, 1).ToUpper() + length.Substring(1, length.Length - 1);
        GameObject street = Resources.Load("ProceduralBlocks/Streets/Straight" + length) as GameObject;

        //Width, length & height of street
        Mesh mesh = street.GetComponentInChildren<MeshFilter>().sharedMesh;
        Vector3 meshSize = mesh.bounds.size;
        float streetWidth, streetLength, streetHeight;
        Vector3 scale = street.transform.lossyScale;
        //Debug.Log("Scale: " + scale);
        //Debug.Log("Meshsize: " + meshSize);
        streetWidth = scale.x * meshSize.x;
        streetLength = scale.z * meshSize.y;    //Swapping these is not by accident (street is turned 90 degrees)
        streetHeight = scale.y * meshSize.z;

        //Debug.Log("Streetwidth: " + streetWidth);
        //Debug.Log("Streetlength: " + streetLength);
        //Debug.Log("Streetheight: " + streetHeight);

        //Create lots equally spaced along both sides of the straight street nigga
        //First left side
        if (lotsLeft > 0)
        {
            float lotSpacing = streetLength / lotsLeft;
            float lotLength = 20f;
            float lotWidth = lotSpacing;

            //Debug.Log("Lot Width: " + lotWidth.ToString());
            //Debug.Log("Lot length: " + lotLength.ToString());

            //Get the starting point (left bottom corner of the street)
            Vector3 spawnPointLot = new Vector3(-streetWidth / 2, 0f, -streetLength / 2);
            spawnPointLot += new Vector3(-lotLength / 2, 0f, lotWidth / 2);

            //Debug.Log("First spawnpoint lot: " + spawnPointLot.ToString());

            SpawnLot(lotsLeft, spawnPointLot, lotWidth, lotLength, parent, Orientation.left, new Vector3(0f, 0f, lotWidth));
        }

        //Then right
        if (lotsRight > 0)
        {
            float lotSpacing = streetLength / lotsRight;
            float lotLength = 20f;
            float lotWidth = lotSpacing;

            //Debug.Log("Lot Width: " + lotWidth.ToString());
            //Debug.Log("Lot length: " + lotLength.ToString());

            //Get the starting point (right bottom corner of the street)
            Vector3 spawnPointLot = new Vector3(streetWidth / 2, 0f, -streetLength / 2);
            spawnPointLot += new Vector3(lotLength / 2, 0f, lotWidth / 2);

            //Debug.Log("First spawnpoint lot: " + spawnPointLot.ToString());

            SpawnLot(lotsRight, spawnPointLot, lotWidth, lotLength, parent, Orientation.right, new Vector3(0f, 0f, lotWidth));
        }

        //street = GameObject.Instantiate(street, spawnPosition, Quaternion.identity, parent);
        street = InstantiateStreet(street, spawnPosition, Quaternion.identity, parent);
        UpdateAllowedPointsStraightStreet(street);

        SpawnStreet(street, parent, currentDirection, ref currentPosition, "straight", pointDirection);
    }

    private void RotateStreet(Transform parent, Vector2 currentDirection)
    {
        parent.transform.Rotate(new Vector3(0, 1, 0), currentDirection.x);
    }

    private void SpawnStreet(GameObject street, Transform parent, Vector2 currentDirection, ref Vector3 currentPosition, string typeOfStreet, string pointDirection)
    {
        //street = GameObject.Instantiate(street, spawnPosition, Quaternion.identity, parent);
        street.GetComponent<GenericStreet>().SpawnColliders();

		Debug.Log ("Rotate: " + currentDirection.ToString ());
        RotateStreet(parent, currentDirection);

        //Street is rotated, now update it's position
        //Depending on whether or not we have a previous street, spawning happens in currentPosition or an updated currentPosition
        if (previousStreetScript == null)        //We have no previous street, so spawning is in currentPosition
        {
            Debug.Log("Spawning new first street");
            parent.localPosition = currentPosition + spawnPosition;
            //EditorGUIUtility.PingObject(parent);

            previousStreetScript = street.GetComponent<GenericStreet>();
        }
        else
        {
            Debug.Log("Spawning new street, previous street exists");
            try
            {
                previousStreetScript.SetCorrectPoint(pointDirection);
            } catch(Exception e)
            {
                Debug.Log(e.Message);
                MonoBehaviour.Destroy(parent.gameObject);
                return;
            }

            Debug.Log("Previous turned point: " + previousStreetScript.GetSpawnPoint());

            currentPosition = previousStreetScript.GetSpawnPoint();
            Debug.Log("Currentposition set to: " + currentPosition.ToString());

            Debug.Log("Offset point: " + street.GetComponent<GenericStreet>().GetOffsetPoint().ToString());
            Vector3 rotatedTopPoint = Quaternion.AngleAxis(currentDirection.x, Vector3.up) * street.GetComponent<GenericStreet>().GetOffsetPoint();
            Debug.Log("Rotated offset point: " + rotatedTopPoint.ToString());

            currentPosition += rotatedTopPoint;
            Debug.Log("Currentposition set to: " + currentPosition.ToString());

            parent.localPosition = currentPosition + spawnPosition;

            //Remove collider from previous street on the end on which we spawned a new street
            previousStreetScript.RemoveCollider(pointDirection);

            previousStreetScript = street.GetComponent<GenericStreet>();

            //Debug.Log("Spawning new street, previous street exists");

        }

        foreach (ColliderScript collScript in previousStreetScript.transform.parent.GetComponentsInChildren<ColliderScript>())
        {
           collScript.SetDirection(currentDirection);
        }
    }

    private void UpdateAllowedPointsStraightStreet(GameObject street)
    {
        street.GetComponent<GenericStreet>().SetAllowedPoints();
    }

    private void UpdateAllowedPointsIntersectionX(GameObject street)
    {
        List<string> allowedDirections = new List<string>();
        allowedDirections.Add("left");
        allowedDirections.Add("right");
        allowedDirections.Add("straight");
        street.GetComponent<GenericStreet>().SetAllowedPoints(allowedDirections);
    }

    private void UpdateAllowedPointsIntersectionT(GameObject street, string orientation)
    {
        List<string> allowedDirections = new List<string>();
        //int degrees = 0;

        switch (orientation)
        {
            case "leftRight":
                allowedDirections.Add("left");
                allowedDirections.Add("right");
                //degrees = 180;
                break;
            case "leftStraight":
                allowedDirections.Add("left");
                allowedDirections.Add("straight");
                //degrees = -90;
                break;
            case "rightStraight":
                allowedDirections.Add("straight");
                allowedDirections.Add("right");
                //degrees = 90;
                break;
        }

        street.GetComponent<GenericStreet>().SetAllowedPoints(allowedDirections);
    }

    private void SpawnLot(int nbLots, Vector3 spawnPointLot, float lotWidth, float lotLength, Transform parent, Orientation orient, Vector3 growDirection)
    {
        //bool isLeft is used for the orientation of the lots

        GameObject prefabLot = Resources.Load("ProceduralBlocks/Lot") as GameObject;
        GameObject[] spawnedLots = new GameObject[nbLots];

        //If lot is on the left, should turn 90° along Y-axis
        //If on the right, 90° against Y-axis
        //Straight 180°
        //Thanks to this, the Z-axis of the lot will always point towards the street, which can be used for the house orientation later on
        //Because of the rotation, it is also (possibly) necessary to switch the width & length
        float rotate = 0f;
        float dummy;
        switch (orient)
        {
            case Orientation.left:
                rotate = 90f;
                dummy = lotWidth;
                lotWidth = lotLength;
                lotLength = dummy;
                Debug.Log("Left, swapped");
                break;
            case Orientation.right:
                rotate = -90f;
                dummy = lotWidth;
                lotWidth = lotLength;
                lotLength = dummy;
                Debug.Log("Right, swapped");
                break;
            case Orientation.straight:
                rotate = 0f;
                break;
            default:
                rotate = 0f;
                break;
        }

        for (int i = 0; i < nbLots; i++)
        {
            //Calculate the center of the lot that we will spawn
            //spawnPointLot += new Vector3(0f, 0f, -lotLength);

            //For now use a simple plane
            /*
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Mesh planeMesh = plane.GetComponent<MeshFilter>().mesh;
            Vector3 planeSize = planeMesh.bounds.size;  //This is the actual size, lotWidth & lotLength is wanted size --> change scale
            Vector3 planeScale = plane.transform.localScale;
            

            Debug.Log("Plane Size X: " + planeSize.x + " Y: " + planeSize.y + " Z: " + planeSize.z);
            Debug.Log("Plane Scale X: " + planeScale.x + " Y: " + planeScale.y + " Z: " + planeScale.z);

    */

            // GameObject lot = GameObject.Instantiate(prefabLot, spawnPointLot, Quaternion.identity, parent);
            GameObject lot = GameObject.Instantiate(prefabLot);
			lot.transform.SetParent (parent, false);
            lot.transform.localPosition = spawnPointLot;
            //Debug.Log("Spawned lot at position: " + spawnPointLot.ToString());
            lot.transform.rotation = Quaternion.identity; 

            //Set the collider from the lot
            BoxCollider coll = lot.transform.GetComponent<BoxCollider>();
            coll.size = new Vector3(lotLength, 0.05f, lotWidth);

            //Set width & height in the lot script
            LotResizer resizer = lot.GetComponent<LotResizer>();
            resizer.SetLotLength(lotLength);
            resizer.SetLotWidth(lotWidth);

            resizer.SpawnPlane();

            //Debug.Log("Rotate: " + rotate);
            lot.transform.Rotate(new Vector3(0, 1f, 0), rotate);

            spawnedLots[i] = lot;

            //lot.transform.parent = parent;
            //lot.transform.localScale = new Vector3(lotWidth / planeSize.z, 1f, lotLength / planeSize.x);
            //lot.transform.localScale = new Vector3(lotWidth, 1f, lotLength);
            //lot.transform.position = spawnPointLot;

            //plane.transform.parent = lot.transform;

            //plane.transform.localScale = new Vector3(lotWidth / planeSize.z, 1f, lotLength / planeSize.x);

            //plane.transform.parent = parent;
            //plane.transform.localPosition = spawnPointLot;

            spawnPointLot += growDirection;
        }

        //In the lot resizers, set the necessary neighbors
        if (nbLots > 1)
        {
            for (int i = 0; i < nbLots; i++)
            {
                GameObject currentLot = spawnedLots[i];
                if (InBounds(i - 1, nbLots))
                {
                    GameObject neighborLot = spawnedLots[i - 1];
                    SetNeighboringLots(currentLot, neighborLot);
                }
            }
        }
    }

    private static bool InBounds(int i, int length)
    {
        if (i < 0)
            return false;
        if (i >= length)
            return false;
        return true;
    }

    private static void SetNeighboringLots(GameObject lotLeft, GameObject lotRight)
    {
        //Debug.Log("lotLeft: " + lotLeft.transform);
        //Debug.Log("lotRight: " + lotRight.transform);
        lotLeft.GetComponent<LotResizer>().SetRightNeighbor(lotRight);
        lotRight.GetComponent<LotResizer>().SetLeftNeighbor(lotLeft);
    }

    public static void SetPreviousStreetScript(GenericStreet script)
    {
        previousStreetScript = script;
    }

    private enum Orientation { left, right, straight};

}