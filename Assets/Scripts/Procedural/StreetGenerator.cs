using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreetGenerator : Generator {

    Vector3 spawnPosition;

    public override void GenerateWorldObject(WorldObject obj) {

        //For now
        spawnPosition = new Vector3(0, 10f, 0);

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

        //Only 4 types of streets allowed, so switch case them all
        string type = obj.GetObjectValue();
        Debug.Log("Street type: " + type);
        Transform parent = (new GameObject("streetID:" + streetNumber)).transform;
        parent.position = spawnPosition;
        switch (type) {
            case "straight":
                GenerateStraightStreet(obj, parent);
                break;
            case "intersection":
                GenerateIntersection(obj, parent);
                break;
            case "roundabout":
                GenerateRoundabout(obj, parent);
                break;
            case "turn":
                GenerateTurn(obj, parent);
                break;
        }
    }

    private void GenerateStraightStreet(WorldObject obj, Transform parent) {
        Debug.Log("Generating straight street");

        //Okay uhm boom
        string left = (string) obj.directAttributes["lotsLeft"];
        string right = (string)obj.directAttributes["lotsRight"];

        int lotsLeft, lotsRight;
        Int32.TryParse(left, out lotsLeft);
        Int32.TryParse(right, out lotsRight);

        //Fetch the straight street from resources
        GameObject street = Resources.Load("ProceduralBlocks/Streets/Straight") as GameObject;

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
            spawnPointLot += new Vector3(lotWidth/2, 0f, lotLength/2);
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

        Debug.Log("Street width: " + streetWidth);
        Debug.Log("Street length: " + streetLength);
        Debug.Log("Street height: " + streetHeight);

    }

    private void GenerateIntersection(WorldObject obj, Transform parent) {
        Debug.Log("Generating Intersection");
    }

    private void GenerateRoundabout(WorldObject obj, Transform parent) {
        Debug.Log("Generating Roundabout");
    }

    private void GenerateTurn(WorldObject obj, Transform parent) {
        Debug.Log("Generating Turn");
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
            for(int i = 0; i < nbLots; i++) {
                GameObject currentLot = spawnedLots[i];
                if(InBounds(i - 1, nbLots)) {
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
        Debug.Log("lotLeft: " + lotLeft.transform);
        Debug.Log("lotRight: " + lotRight.transform);
        lotLeft.GetComponent<LotResizer>().SetRightNeighbor(lotRight);
        lotRight.GetComponent<LotResizer>().SetLeftNeighbor(lotLeft);
    }

}
