using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetGeneratorV2 : Generator {

    private Vector3 spawnPosition;
    private GenericStreet previousStreetScript = null;
    //private string newPointDirection = "";

    private static int streetID = 1;

    public StreetGeneratorV2()
    {
        spawnPosition = new Vector3(0, 0.2f, 0);
    }


    public override void GenerateWorldObject(WorldObject obj, ref Vector2 currentDirection, ref Vector3 currentPosition, string pointDirection) {

        obj = obj.GetChildren()[0];

        //Only 5 types of streets allowed, so switch case them all
        string type = obj.GetObjectValue();
        Debug.Log("Street type: " + type + " -----------------------------------");
        Transform parent = (new GameObject("streetID:" + streetID++)).transform;
        parent.position = spawnPosition;
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

    private void GenerateIntersectionX(WorldObject obj, Transform parent, Vector2 currentDirection, ref Vector3 currentPosition, string pointDirection)
    {
        Debug.Log("Generating Intersection-x");
        //Fetch the x intersection from resources
        GameObject street = Resources.Load("ProceduralBlocks/Streets/Intersection-x") as GameObject;
        street = GameObject.Instantiate(street, spawnPosition, Quaternion.identity, parent);
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

        //Create lots equally spaced along both sides of the straight street nigga
        //First left side
        if (lotsStraight > 0)
        {
            float lotSpacing = streetLength / lotsStraight;
            float lotLength = lotSpacing;
            float lotWidth = 20f;

            //Get the starting point (left bottom corner of the street)
            Vector3 spawnPointLot = new Vector3(streetWidth / 2, 0f, streetLength / 2);
            //Debug.Log("#1: " + spawnPointLot.x);
            spawnPointLot += new Vector3(lotWidth / 2, 0f, lotLength / 2);
            spawnPointLot -= new Vector3(2.675f, 0f, 0f); //correct offset due to different symmetry
            //Debug.Log("#2: " + spawnPointLot.x);
            SpawnLot(lotsStraight, spawnPointLot, lotWidth, lotLength, parent);
        }

        street = GameObject.Instantiate(street, spawnPosition, Quaternion.identity, parent);
        UpdateOrientationIntersectionT(parent, orientation, pointDirection);
        UpdateAllowedPointsIntersectionT(street, orientation);
        SpawnStreet(street, parent, currentDirection, ref currentPosition, "intersection-t", pointDirection);

    }

    private void UpdateOrientationIntersectionT(Transform parent, string orientation, string pointDirection)
    {
        switch (orientation)
        {
            case "leftRight":
                parent.transform.Rotate(new Vector3(0, 1, 0), 180);
                //newPointDirection = pointDirection;
                //Debug.Log("PointDir0 = " + pointDirection);
                //Debug.Log("NewPointDir0 = " + newPointDirection);
                break;
            case "leftStraight":
                parent.transform.Rotate(new Vector3(0, 1, 0), -90);
                //if (pointDirection.Equals("left"))
                //{
                //    newPointDirection = "straight";
                //} else if (pointDirection.Equals("straight"))
                //{
                //    newPointDirection = "left";
                //}
                break;
            case "rightStraight":
                parent.transform.Rotate(new Vector3(0, 1, 0), 90);
                //if (pointDirection.Equals("right"))
                //{
                //    newPointDirection = "straight";
                //}
                //else if (pointDirection.Equals("straight"))
                //{
                //    newPointDirection = "right";
                //}
                break;
        }
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
                spawnAngle = 45f;
                break;
            case "45":
                spawnAngle = 22.5f;
                break;
        }

        Debug.Log("Spawnangle: " + spawnAngle);

        if (direction.ToLower().Equals("right"))
        {
            street = GameObject.Instantiate(street, spawnPosition, Quaternion.Euler(-90f, spawnAngle, 0), parent);
            street.transform.localScale = new Vector3(-1f, 1f, 1f);
            street.GetComponent<GenericStreet>().SetRightTurn();
        } else
        {
            street = GameObject.Instantiate(street, spawnPosition, Quaternion.Euler(-90f, -spawnAngle, 0), parent);
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

        Debug.Log("Streetwidth: " + streetWidth);
        Debug.Log("Streetlength: " + streetLength);
        Debug.Log("Streetheight: " + streetHeight);

        //Create lots equally spaced along both sides of the straight street nigga
        //First left side
        if (lotsLeft > 0)
        {
            float lotSpacing = streetLength / lotsLeft;
            float lotLength = lotSpacing;
            float lotWidth = 20f;

            //Get the starting point (left bottom corner of the street)
            Vector3 spawnPointLot = new Vector3(-streetWidth / 2, 0f, streetLength / 2);
            spawnPointLot += new Vector3(-lotWidth / 2, 0f, lotLength / 2);
            SpawnLot(lotsLeft, spawnPointLot, lotWidth, lotLength, parent);
        }

        //Then right
        if (lotsRight > 0)
        {
            float lotSpacing = streetLength / lotsRight;
            float lotLength = lotSpacing;
            float lotWidth = 20f;

            //Get the starting point (right bottom corner of the street)
            Vector3 spawnPointLot = new Vector3(streetWidth / 2, 0f, streetLength / 2);
            spawnPointLot += new Vector3(lotWidth / 2, 0f, lotLength / 2);
            SpawnLot(lotsRight, spawnPointLot, lotWidth, lotLength, parent);
        }

        street = GameObject.Instantiate(street, spawnPosition, Quaternion.identity, parent);
        UpdateAllowedPointsStraightStreet(street);
        //Debug.Log("PointDir1 = " + pointDirection);
        //Debug.Log("NewPointDir1 = " + newPointDirection);
        //if (!newPointDirection.Equals("")){
        //    pointDirection = newPointDirection;
        //}
        //Debug.Log("PointDir2 = " + pointDirection);
        //Debug.Log("NewPointDir2 = " + newPointDirection);
        //newPointDirection = "";
        SpawnStreet(street, parent, currentDirection, ref currentPosition, "straight", pointDirection);
    }

    private void RotateStreet(Transform parent, Vector2 currentDirection)
    {
        parent.transform.Rotate(new Vector3(0, 1, 0), currentDirection.x);
    }

    private void SpawnStreet(GameObject street, Transform parent, Vector2 currentDirection, ref Vector3 currentPosition, string typeOfStreet, string pointDirection)
    {
        //street = GameObject.Instantiate(street, spawnPosition, Quaternion.identity, parent);
        RotateStreet(parent, currentDirection);
        street.GetComponent<GenericStreet>().RotateAllowedPoints((int) currentDirection.x);

        //Street is rotated, now update it's position
        //Depending on whether or not we have a previous street, spawning happens in currentPosition or an updated currentPosition
        if (previousStreetScript == null)        //We have no previous street, so spawning is in currentPosition
        {
            Debug.Log("Spawning new first street");
            parent.position = currentPosition + spawnPosition;
            previousStreetScript = street.GetComponent<GenericStreet>();

            return;
        }
        else
        {
            Debug.Log("Spawning new street, previous street exists");
            previousStreetScript.SetCorrectPoint(pointDirection);

            Debug.Log("Previous turned point: " + previousStreetScript.GetSpawnPoint());

            currentPosition += previousStreetScript.GetSpawnPoint();
            Debug.Log("Currentposition set to: " + currentPosition.ToString());

            Vector3 rotatedTopPoint = Quaternion.AngleAxis(currentDirection.x, Vector3.up) * street.GetComponent<GenericStreet>().GetTopPoint();
            Debug.Log("Rotated top point: " + rotatedTopPoint.ToString());

            currentPosition += rotatedTopPoint;
            Debug.Log("Currentposition set to: " + currentPosition.ToString());

            parent.position = currentPosition + spawnPosition;
            previousStreetScript = street.GetComponent<GenericStreet>();

            //Debug.Log("Spawning new street, previous street exists");

            //Update the current position first

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
        int degrees = 0;

        switch (orientation)
        {
            case "leftRight":
                allowedDirections.Add("left");
                allowedDirections.Add("right");
                degrees = 180;
                break;
            case "leftStraight":
                allowedDirections.Add("left");
                allowedDirections.Add("straight");
                degrees = -90;
                break;
            case "rightStraight":
                allowedDirections.Add("straight");
                allowedDirections.Add("right");
                degrees = 90;
                break;
        }

        street.GetComponent<GenericStreet>().SetAllowedPoints(allowedDirections);
        street.GetComponent<GenericStreet>().RotateAllowedPoints(degrees);
    }

    private void SpawnLot(int nbLots, Vector3 spawnPointLot, float lotWidth, float lotLength, Transform parent)
    {
        GameObject prefabLot = Resources.Load("ProceduralBlocks/Lot") as GameObject;
        GameObject[] spawnedLots = new GameObject[nbLots];

        for (int i = 0; i < nbLots; i++)
        {
            //Calculate the center of the lot that we will spawn
            spawnPointLot += new Vector3(0f, 0f, -lotLength);

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
            GameObject lot = GameObject.Instantiate(prefabLot, parent);
            lot.transform.localPosition = spawnPointLot;
            lot.transform.rotation = Quaternion.identity;
            //Set the collider from the lot
            BoxCollider coll = lot.transform.GetComponent<BoxCollider>();
            coll.size = new Vector3(lotWidth, 0.05f, lotLength);

            //Set width & height in the lot script
            LotResizer resizer = lot.GetComponent<LotResizer>();
            resizer.SetLotLength(lotLength);
            resizer.SetLotWidth(lotWidth);

            resizer.SpawnPlane();

            spawnedLots[i] = lot;

            //lot.transform.parent = parent;
            //lot.transform.localScale = new Vector3(lotWidth / planeSize.z, 1f, lotLength / planeSize.x);
            //lot.transform.localScale = new Vector3(lotWidth, 1f, lotLength);
            //lot.transform.position = spawnPointLot;

            //plane.transform.parent = lot.transform;

            //plane.transform.localScale = new Vector3(lotWidth / planeSize.z, 1f, lotLength / planeSize.x);

            //plane.transform.parent = parent;
            //plane.transform.localPosition = spawnPointLot;


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
}
