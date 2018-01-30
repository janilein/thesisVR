using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetGenerator : Generator {

    private Vector3 spawnPosition;
    private GenericStreet previousStreetScript;
    private Transform previousStreet;
    //toegevoegd voot T, maar werkt niet
    private string previousOrientation;

    public StreetGenerator() {

    }

    public override void GenerateWorldObject(WorldObject obj, Vector3 currentDirection, ref Vector3 currentPosition, string pointDirection) {

        //For now
        spawnPosition = new Vector3(0, 0.2f, 0);

        //Obj is the 'lot' root obj
        int streetNumber;
        if (obj.directAttributes.ContainsKey("streetID")) {
            string streetString = (string)obj.directAttributes["streetID"];
            Int32.TryParse(streetString, out streetNumber);
        } else {
            System.Random random = new System.Random();
            streetNumber = random.Next(1000, 1000000);
        }
        obj = obj.GetChildren()[0];

        //Only 5 types of streets allowed, so switch case them all
        string type = obj.GetObjectValue();
        Debug.Log("Street type: " + type);
        Transform parent = (new GameObject("streetID:" + streetNumber)).transform;
        parent.position = spawnPosition;
        switch (type) {
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
                GenerateRoundabout(obj, parent, currentDirection);
                break;
            case "turn":
                GenerateTurn(obj, parent, currentDirection);
                break;
        }
        previousStreet = parent.transform;
    }

    private void GenerateStraightStreet(WorldObject obj, Transform parent, Vector3 currentDirection, ref Vector3 currentPosition, string pointDirection) {
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
        if (lotsLeft > 0) {
            float lotSpacing = streetLength / lotsLeft;
            float lotLength = lotSpacing;
            float lotWidth = 20f;

            //Get the starting point (left bottom corner of the street)
            Vector3 spawnPointLot = new Vector3(streetWidth / 2, 0f, streetLength / 2);
            spawnPointLot += new Vector3(lotWidth / 2, 0f, lotLength / 2);
            SpawnLot(lotsLeft, spawnPointLot, lotWidth, lotLength, parent);
        }

        //Then right
        if (lotsRight > 0) {
            float lotSpacing = streetLength / lotsRight;
            float lotLength = lotSpacing;
            float lotWidth = 20f;

            //Get the starting point (right bottom corner of the street)
            Vector3 spawnPointLot = new Vector3(-streetWidth / 2, 0f, streetLength / 2);
            spawnPointLot += new Vector3(-lotWidth / 2, 0f, lotLength / 2);
            SpawnLot(lotsRight, spawnPointLot, lotWidth, lotLength, parent);
        }

        //Spawn the street itself
        GameObject.Instantiate(street, spawnPosition, Quaternion.identity, parent);
        //Rotate to match our "walking" direction along x or z axis
        CheckOrientationStreet(currentDirection, parent);

        //Quaternion rotation = CheckOrientationStraight(currentDirection, parent);
        //Debug.Log("Street width: " + streetWidth);
        //Debug.Log("Street length: " + streetLength);
        //Debug.Log("Street height: " + streetHeight);

        //Update current position & stuff
        GenericStreet script = street.GetComponent<GenericStreet>();
        if (!script) {
            Debug.Log("Script null");
        }

        Debug.Log("Current position: " + currentPosition);
        
        if(currentPosition.x == float.PositiveInfinity) { //no street placed
            currentPosition = Vector3.zero; /*rotation * top;*/
        } else {
            //Left, right, or top?
            string type = previousStreetScript.GetTypePoint();
            Debug.Log("######type: " + type);
            Vector3 point;
            if (type.Equals("intersectionT")) {
                Debug.Log("hierzoooo");
                //checkPoints(previousOrientation, previousStreetScript, currentDirection);
                point = previousStreetScript.GetBottomPoint();
            } else
            {
                point = GetCorrectPoint(pointDirection);
                Debug.Log("Correct point: " + point);

                //Onze transform is gerotate door onze walking direction (previous), point moet even veel gerotate worden
                //Quaternion previousRotation = previousStreetScript.gameObject.transform.localRotation;
                Vector3 previousRotation = previousStreet.localEulerAngles;
                Debug.Log("Previous rotation: " + previousRotation);

                Quaternion rotation = Quaternion.Euler(previousRotation);
                Vector3 rotationEuler = rotation.eulerAngles;
                Vector3 inverseEuler = new Vector3(rotationEuler.x * -1, rotationEuler.y * -1, rotationEuler.z * -1);
                Quaternion inverseRotation = Quaternion.Euler(inverseEuler);
                point = inverseRotation * point;
                Debug.Log("Correct point after rotation: " + point);
            }
            //tot hier
            

            //Neem nu top deel van de nieuwe straat
            Vector3 topPoint = script.GetTopPoint();
            Debug.Log("Top point: " + topPoint);

            //Rotate het volgens walking direction
            RotateVector(ref topPoint, currentDirection);
            Debug.Log("Top point after rotation: " + topPoint);

            //Bereken spawnpoint
            Vector3 centerToSpawn = currentPosition + topPoint + point;

            //Zet parent
            parent.position = centerToSpawn + spawnPosition;
            currentPosition = centerToSpawn;
        }

        previousStreetScript = script;
        previousOrientation = null;
        Debug.Log("Current position set to: " + currentPosition);
    }

    //toegevoegd voor T, maar werkt nog niet
    private void checkPoints(string orientation, GenericStreet script, Vector3 currentDirection) {
        switch (orientation) {
            //case "leftStraight":
            //    script.SetLeftPoint(new Vector3(-15f, 0, 0));
            //    script.SetRightPoint(new Vector3(4.5f, 0, 0));
            //    script.SetTopPoint(new Vector3(0, 0, 15f));
            //    script.SetBottomPoint(new Vector3(0, 0, -15f));
            //    break;
            //case "rightStraight":    
            //    script.SetLeftPoint(new Vector3(-4.5f, 0, 0));
            //    script.SetRightPoint(new Vector3(15f, 0, 0));
            //    script.SetTopPoint(new Vector3(0, 0, 15f));
            //    script.SetBottomPoint(new Vector3(0, 0, -15f));
            //    break;
            //case "leftRight":
            //    Debug.Log("hierzo");
            //    script.SetLeftPoint(new Vector3(0, 0, -15f));
            //    script.SetRightPoint(new Vector3(0, 0, 4.5f));
            //    script.SetTopPoint(new Vector3(-15f, 0, 0));
            //    script.SetBottomPoint(new Vector3(15f, 0, 0));
            //    break;

            case "leftStraight":
                if (currentDirection.x == 1) {
                    script.SetLeftPoint(new Vector3(0, 0, 15f));
                    script.SetRightPoint(new Vector3(0, 0, -4.5f));
                    script.SetTopPoint(new Vector3(15f, 0, 0));
                    script.SetBottomPoint(new Vector3(-15f, 0, 0));
                } else if (currentDirection.x == -1) {
                    script.SetLeftPoint(new Vector3(0, 0, -15f));
                    script.SetRightPoint(new Vector3(0, 0, 4.5f));
                    script.SetTopPoint(new Vector3(-15f, 0, 0));
                    script.SetBottomPoint(new Vector3(15f, 0, 0));
                } else if (currentDirection.z == -1) {
                    script.SetLeftPoint(new Vector3(15f, 0, 0));
                    script.SetRightPoint(new Vector3(-4.5f, 0, 0));
                    script.SetTopPoint(new Vector3(0, 0, -15f));
                    script.SetBottomPoint(new Vector3(0, 0, 15f));
                } //nothing changes when z = 1
                break;
            case "rightStraight":
                if (currentDirection.x == 1) {
                    script.SetLeftPoint(new Vector3(0, 0, 4.5f));
                    script.SetRightPoint(new Vector3(0, 0, -15f));
                    script.SetTopPoint(new Vector3(15f, 0, 0));
                    script.SetBottomPoint(new Vector3(-15f, 0, 0));
                } else if (currentDirection.x == -1) {
                    script.SetLeftPoint(new Vector3(0, 0, -4.5f));
                    script.SetRightPoint(new Vector3(0, 0, 15f));
                    script.SetTopPoint(new Vector3(-15f, 0, 0));
                    script.SetBottomPoint(new Vector3(15f, 0, 0));
                } else if (currentDirection.z == 1) {
                    script.SetLeftPoint(new Vector3(-4.5f, 0, 0));
                    script.SetRightPoint(new Vector3(15f, 0, 0));
                    script.SetTopPoint(new Vector3(0, 0, 15f));
                    script.SetBottomPoint(new Vector3(0, 0, -15f));
                } //nothing changes when z = -1
                break;
            case "leftRight":
                if (currentDirection.x == -1) {
                    script.SetLeftPoint(new Vector3(0, 0, -15f));
                    script.SetRightPoint(new Vector3(0, 0, 15f));
                    script.SetTopPoint(new Vector3(-4.5f, 0, 0));
                    script.SetBottomPoint(new Vector3(15f, 0, 0));
                } else if (currentDirection.z == 1) {
                    script.SetLeftPoint(new Vector3(-15f, 0, 0));
                    script.SetRightPoint(new Vector3(15f, 0, 0));
                    script.SetTopPoint(new Vector3(0, 0, 4.5f));
                    script.SetBottomPoint(new Vector3(0, 0, -15f));
                } else if (currentDirection.z == -1) {
                    script.SetLeftPoint(new Vector3(15f, 0, 0));
                    script.SetRightPoint(new Vector3(-15f, 0, 0));
                    script.SetTopPoint(new Vector3(0, 0, -4.5f));
                    script.SetBottomPoint(new Vector3(0, 0, 15f));
                } // nothing changes when x = 1
                break;
        }
    }

    private void CheckOrientationStreet(Vector3 currentDirection, Transform parent) {
        //By default, prefabs are orientated along positive Z axis
        Vector3 rotationAxis = new Vector3(0, 1, 0);   //Rotation is always along y-axis

        if (currentDirection.x == 1) {          //Positive X axis --> -90°
            parent.transform.Rotate(rotationAxis, -90f);
            Debug.Log("Rotated transform -90°");
        } else if (currentDirection.x == -1) {  //Negative X axis --> 90°
            parent.transform.Rotate(rotationAxis, 90f);
            Debug.Log("Rotated transform 90°");
        } else if (currentDirection.z == -1) {  //Negative Z axis --> 180°
            parent.transform.Rotate(rotationAxis, 180f);
            Debug.Log("Rotated transform 180°");
        } else if (currentDirection.z == 1) {  //Positive Z axis --> 0°
            parent.transform.Rotate(rotationAxis, 0f);
            Debug.Log("Rotated transform 0°");
        }
    }

    private void RotateVector(ref Vector3 point, Vector3 currentDirection) {
        Quaternion rotation;

        if (currentDirection.x == 1) {          //Positive X axis --> -90°
            rotation = Quaternion.Euler(0, 90, 0);
            Debug.Log("Rotated vector -90°");
        } else if (currentDirection.x == -1) {  //Negative X axis --> 90°
            rotation = Quaternion.Euler(0, -90, 0);
            Debug.Log("Rotated transform 90°");
        } else if (currentDirection.z == -1) {  //Negative Z axis --> 180°
            rotation = Quaternion.Euler(0, 180, 0);
            Debug.Log("Rotated transform 180°");
        } else if (currentDirection.z == 1) {  //Positive Z axis --> 0°
            rotation = Quaternion.Euler(0, 0, 0);
            Debug.Log("Rotated transform 0°");
        } else {
            rotation = Quaternion.Euler(0, 0, 0);
        }
        point = rotation * point;
    }

    private void GenerateIntersectionT(WorldObject obj, Transform parent, Vector3 currentDirection, ref Vector3 currentPosition, string pointDirection) {
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
        if (lotsStraight > 0) {
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

        //Update current position & stuff
        GenericStreet script = street.GetComponent<GenericStreet>();
        if (!script)
        {
            Debug.Log("Script null");
        }
        //reset the points everytime
        script.SetTopPoint(new Vector3(0, 0, 15f));
        script.SetLeftPoint(new Vector3(-15f, 0, 0));
        script.SetRightPoint(new Vector3(4.5f, 0, 0));
        script.SetBottomPoint(new Vector3(0, 0, -15f));
        GameObject.Instantiate(street, spawnPosition, Quaternion.identity, parent);
        CheckOrientationIntersectionT(orientation, currentDirection, parent, script);

        if (currentPosition.x == float.PositiveInfinity)
        { //no street placed
            currentPosition = Vector3.zero; /*rotation * top;*/
        }
        else
        {
            //Left, right, or top?
            string type = previousStreetScript.GetTypePoint();
            Debug.Log("######type: " + type);
            Vector3 point;
            if (type.Equals("intersectionT"))
            {
                Debug.Log("hierzoooo");
                checkPoints(previousOrientation, previousStreetScript, currentDirection);
                point = previousStreetScript.GetTopPoint();
            }
            else
            {
                point = GetCorrectPoint(pointDirection);
                Debug.Log("Correct point: " + point);

                //Onze transform is gerotate door onze walking direction (previous), point moet even veel gerotate worden
                //Quaternion previousRotation = previousStreetScript.gameObject.transform.localRotation;
                Vector3 previousRotation = previousStreet.localEulerAngles;
                Debug.Log("Previous rotation: " + previousRotation);

                Quaternion rotation = Quaternion.Euler(previousRotation);
                Vector3 rotationEuler = rotation.eulerAngles;
                Vector3 inverseEuler = new Vector3(rotationEuler.x * -1, rotationEuler.y * -1, rotationEuler.z * -1);
                Quaternion inverseRotation = Quaternion.Euler(inverseEuler);
                point = inverseRotation * point;
                Debug.Log("Correct point after rotation: " + point);
            }

            //Neem nu top deel van de nieuwe straat
            Vector3 topPoint = script.GetTopPoint();
            Debug.Log("Top point: " + topPoint);

            //Rotate het volgens walking direction
            RotateVector(ref topPoint, currentDirection);
            Debug.Log("Top point after rotation: " + topPoint);

            //Bereken spawnpoint
            Vector3 centerToSpawn = currentPosition + topPoint + point;

            //Zet parent
            parent.position = centerToSpawn + spawnPosition;
            currentPosition = centerToSpawn;
        }


        previousStreetScript = script;
        previousOrientation = orientation;
        Debug.Log("Current position set to: " + currentPosition);
    }

    private void GenerateIntersectionX(WorldObject obj, Transform parent, Vector3 currentDirection, ref Vector3 currentPosition, string pointDirection) {
        Debug.Log("Generating Intersection-x");

        //Fetch the x intersection from resources
        GameObject street = Resources.Load("ProceduralBlocks/Streets/Intersection-x") as GameObject;

        GameObject spawnedStreet = GameObject.Instantiate(street, spawnPosition, Quaternion.identity, parent) as GameObject;
        //Quaternion rotation = CheckOrientationStraight(currentDirection, parent);
        CheckOrientationStreet(currentDirection, parent);
        GenericStreet script = spawnedStreet.GetComponent<GenericStreet>();

        if (currentPosition.x == float.PositiveInfinity) { //no street placed
            currentPosition = Vector3.zero; /*rotation * top;*/
        } else {
            //Left, right, or top?
            string type = previousStreetScript.GetTypePoint();
            Debug.Log("######type: " + type);
            Vector3 point;
            if (type.Equals("intersectionT"))
            {
                Debug.Log("hierzoooo");
                checkPoints(previousOrientation, previousStreetScript, currentDirection);
                point = previousStreetScript.GetTopPoint();
            }
            else
            {
                point = GetCorrectPoint(pointDirection);
                Debug.Log("Correct point: " + point);

                //Onze transform is gerotate door onze walking direction (previous), point moet even veel gerotate worden
                //Quaternion previousRotation = previousStreetScript.gameObject.transform.localRotation;
                Vector3 previousRotation = previousStreet.localEulerAngles;
                Debug.Log("Previous rotation: " + previousRotation);

                Quaternion rotation = Quaternion.Euler(previousRotation);
                Vector3 rotationEuler = rotation.eulerAngles;
                Vector3 inverseEuler = new Vector3(rotationEuler.x * -1, rotationEuler.y * -1, rotationEuler.z * -1);
                Quaternion inverseRotation = Quaternion.Euler(inverseEuler);
                point = inverseRotation * point;
                Debug.Log("Correct point after rotation: " + point);
            }

            //Neem nu top deel van de nieuwe straat
            Vector3 topPoint = script.GetTopPoint();
            Debug.Log("Top point: " + topPoint);

            //Rotate het volgens walking direction
            RotateVector(ref topPoint, currentDirection);
            Debug.Log("Top point after rotation: " + topPoint);

            //Bereken spawnpoint
            Vector3 centerToSpawn = currentPosition + topPoint + point;

            //Zet parent
            parent.position = centerToSpawn + spawnPosition;
            currentPosition = centerToSpawn;
        }


        previousStreetScript = script;
        previousOrientation = null;
        Debug.Log("Current position set to: " + currentPosition);

    }

    /*
    * TODO, deze werkt nog niet. Straight, X  en T wel al
    */

    private void GenerateRoundabout(WorldObject obj, Transform parent, Vector3 currentDirection) {
        Debug.Log("Generating Roundabout");
    }

    /*
    * TODO, deze werkt nog niet. Straight, X  en T wel al
    */

    private void GenerateTurn(WorldObject obj, Transform parent, Vector3 currentDirection) {
        Debug.Log("Generating Turn");
        string left = (string)obj.directAttributes["lotsLeft"];
        string right = (string)obj.directAttributes["lotsRight"];

        int lotsLeft, lotsRight;
        Int32.TryParse(left, out lotsLeft);
        Int32.TryParse(right, out lotsRight);

        //Fetch the straight street from resources
        GameObject street = Resources.Load("ProceduralBlocks/Streets/Turn") as GameObject;

        //Width, length & height of street
        Mesh mesh = street.GetComponentInChildren<MeshFilter>().sharedMesh;
        Vector3 meshSize = mesh.bounds.size;
        float streetWidth, streetLength, streetHeight;
        Vector3 scale = street.transform.lossyScale;
        Debug.Log("Scale: " + scale);
        Debug.Log("Meshsize: " + meshSize);
        streetWidth = scale.x * meshSize.x;
        streetLength = scale.z * meshSize.y;    //Swapping these is not by accident (street is turned 90 degrees)
        streetHeight = scale.y * meshSize.z;

        Debug.Log("Streetwidth: " + streetWidth);
        Debug.Log("Streetlength: " + streetLength);
        Debug.Log("Streetheight: " + streetHeight);

        //Create lots equally spaced along both sides of the straight street nigga
        //First left side
        if (lotsLeft > 0) {
            float lotSpacing = streetLength / lotsLeft;
            float lotLength = lotSpacing;
            float lotWidth = 20f;

            //Get the starting point (left bottom corner of the street)
            Vector3 spawnPointLot = new Vector3(streetWidth / 2, 0f, streetLength / 2);
            spawnPointLot += new Vector3(lotWidth / 2, 0f, lotLength / 2);
            SpawnLot(lotsLeft, spawnPointLot, lotWidth, lotLength, parent);
        }

        //Then right
        if (lotsRight > 0) {
            float lotSpacing = streetLength / lotsRight;
            float lotLength = lotSpacing;
            float lotWidth = 20f;

            //Get the starting point (right bottom corner of the street)
            Vector3 spawnPointLot = new Vector3(-streetWidth / 2, 0f, streetLength / 2);
            spawnPointLot += new Vector3(-lotWidth / 2, 0f, lotLength / 2);
            SpawnLot(lotsRight, spawnPointLot, lotWidth, lotLength, parent);
        }

        //Spawn the street itself
        GameObject.Instantiate(street, spawnPosition, Quaternion.identity, parent);
        CheckOrientationStraight(currentDirection, parent);
        Debug.Log("Street width: " + streetWidth);
        Debug.Log("Street length: " + streetLength);
        Debug.Log("Street height: " + streetHeight);
    }

    private void SpawnLot(int nbLots, Vector3 spawnPointLot, float lotWidth, float lotLength, Transform parent) {
        GameObject prefabLot = Resources.Load("ProceduralBlocks/Lot") as GameObject;
        GameObject[] spawnedLots = new GameObject[nbLots];

        for (int i = 0; i < nbLots; i++) {
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
        if (nbLots > 1) {
            for (int i = 0; i < nbLots; i++) {
                GameObject currentLot = spawnedLots[i];
                if (InBounds(i - 1, nbLots)) {
                    GameObject neighborLot = spawnedLots[i - 1];
                    SetNeighboringLots(currentLot, neighborLot);
                }
            }
        }
    }

    private static bool InBounds(int i, int length) {
        if (i < 0)
            return false;
        if (i >= length)
            return false;
        return true;
    }

    private static void SetNeighboringLots(GameObject lotLeft, GameObject lotRight) {
        //Debug.Log("lotLeft: " + lotLeft.transform);
        //Debug.Log("lotRight: " + lotRight.transform);
        lotLeft.GetComponent<LotResizer>().SetRightNeighbor(lotRight);
        lotRight.GetComponent<LotResizer>().SetLeftNeighbor(lotLeft);
    }

    private Quaternion CheckOrientationStraight(Vector3 currentDirection, Transform parent) {
        Quaternion rotation = Quaternion.identity;
        Vector3 rot = parent.rotation.eulerAngles;
        if (currentDirection.x == 1) {
            rot = new Vector3(rot.x, rot.y - 90, rot.z);
            parent.rotation = Quaternion.Euler(rot);
            rotation = Quaternion.Euler(0, -90, 0);
        } else if (currentDirection.x == -1) {
            rot = new Vector3(rot.x, rot.y + 90, rot.z);
            parent.rotation = Quaternion.Euler(rot);
            rotation = Quaternion.Euler(0, 90, 0);
        } else if (currentDirection.z == 1) {
            rot = new Vector3(rot.x, rot.y + 180, rot.z);
            parent.rotation = Quaternion.Euler(rot);
            rotation = Quaternion.Euler(0, 180, 0);
        } else if(currentDirection.z == -1) {
            rotation = Quaternion.Euler(0, 0, 0);
        }
        return rotation;
    }

    private void CheckOrientationIntersectionT(string orientation, Vector3 currentDirection, Transform parent, GenericStreet script) {
        Vector3 rot = parent.rotation.eulerAngles;
        switch (orientation) {
            case "leftStraight":
                if (currentDirection.x == 1) {
                    rot = new Vector3(rot.x, rot.y + 90, rot.z);
                    parent.rotation = Quaternion.Euler(rot);
                } else if (currentDirection.x == -1) {
                    rot = new Vector3(rot.x, rot.y - 90, rot.z);
                    parent.rotation = Quaternion.Euler(rot);
                } else if (currentDirection.z == -1) {
                    rot = new Vector3(rot.x, rot.y + 180, rot.z);
                    parent.rotation = Quaternion.Euler(rot);
                } //nothing changes when z = 1
                //top,bottom,left and right stays the same
                break;
            case "rightStraight":
                if (currentDirection.x == 1) {
                    rot = new Vector3(rot.x, rot.y - 90, rot.z);
                    parent.rotation = Quaternion.Euler(rot);
                } else if (currentDirection.x == -1) {
                    rot = new Vector3(rot.x, rot.y + 90, rot.z);
                    parent.rotation = Quaternion.Euler(rot);
                } else if (currentDirection.z == 1) {
                    rot = new Vector3(rot.x, rot.y + 180, rot.z);
                    parent.rotation = Quaternion.Euler(rot);
                } //nothing changes when z = -1
                break;
            case "leftRight":
                if (currentDirection.x == -1) {
                    rot = new Vector3(rot.x, rot.y + 180, rot.z);
                    parent.rotation = Quaternion.Euler(rot);
                } else if (currentDirection.z == 1) {
                    rot = new Vector3(rot.x, rot.y - 90, rot.z);
                    parent.rotation = Quaternion.Euler(rot);
                } else if (currentDirection.z == -1) {
                    rot = new Vector3(rot.x, rot.y + 90, rot.z);
                    parent.rotation = Quaternion.Euler(rot);
                } // nothing changes when x = 1
                break;
        }
    }

    private Vector3 GetCorrectPoint(string pointDirection) {
        Vector3 point = Vector3.zero;
        Debug.Log("Switch direction: " + pointDirection);
        switch (pointDirection) {
            case "top":
                point = previousStreetScript.GetTopPoint();
                break;
            case "left":
                point = previousStreetScript.GetLeftPoint();
                break;
            case "right":
                point = previousStreetScript.GetRightPoint();
                break;
            case "bottom":
                point = previousStreetScript.GetBottomPoint();
                break;
        }
        return point;
    }
}
