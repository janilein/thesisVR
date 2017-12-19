using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager {

    private BuildingGenerator buildingGen;
    private StreetGenerator streetGen;
    private Vector3 currentDirection;
    private Vector3 currentPosition;

    public GeneratorManager() {
        buildingGen = new BuildingGenerator();
        streetGen = new StreetGenerator();

        currentDirection = new Vector3(1, 0, 0);
        currentPosition = Vector3.zero;
    }

	public void GenerateWorldObject(WorldObject obj) {
        if (obj.GetObjectValue().Equals("lots")) {
            WorldObject child = obj.GetChildren()[0];
            if (child.GetObjectValue().Equals("buildings")) {
                Debug.Log("parent is buildings");
                //BuildingGenerator generator = new BuildingGenerator(); //gaan veel van deze maken, misschien in een singleton steken die we gaan oproepen?
                buildingGen.GenerateWorldObject(obj, currentDirection); //Obj, niet child.getChildren()[0], want generator moet aan het 'lot' kunnen
            } else if (child.GetObjectValue().Equals("gardens")) {
                Debug.Log("parent is garden");
                //BuildingGenerator generator = new BuildingGenerator();
                //generator.GenerateWorldObject(obj.GetChildren()[0]);
            } else {
                Debug.Log("ne marche pas");
            }
        } else if (obj.GetObjectValue().Equals("streets")) {
            //WorldObject child = obj.GetChildren()[0];
            Debug.Log("parent is streets");
            //StreetGenerator generator = new StreetGenerator(); //gaan veel van deze maken, misschien in een singleton steken die we gaan oproepen?
            streetGen.GenerateWorldObject(obj, currentDirection, ref currentPosition);
        } else if (obj.GetObjectValue().Equals("orientation")) {
            ChangeDirection((string)obj.directAttributes["direction"]);
            Debug.Log("currentDir x: " + currentDirection.x);
            Debug.Log("currentDir z: " + currentDirection.z);
        } else {
            Debug.Log("ne marche pas");
        }
        return;
    }

    private void ChangeDirection(string direction) {
        switch (direction) {
            case "left":
                if (currentDirection.x == 1) {
                    currentDirection.x = 0;
                    currentDirection.z = 1;
                } else if (currentDirection.x == -1) {
                    currentDirection.x = 0;
                    currentDirection.z = -1;
                } else if (currentDirection.z == 1) {
                    currentDirection.x = -1;
                    currentDirection.z = 0;
                } else if (currentDirection.z == -1) {
                    currentDirection.x = 1;
                    currentDirection.z = 0;
                }
                break;
            case "right":
                if (currentDirection.x == 1) {
                    currentDirection.x = 0;
                    currentDirection.z = -1;
                } else if (currentDirection.x == -1) {
                    currentDirection.x = 0;
                    currentDirection.z = 1;
                } else if (currentDirection.z == 1) {
                    currentDirection.x = 1;
                    currentDirection.z = 0;
                } else if (currentDirection.z == -1) {
                    currentDirection.x = -1;
                    currentDirection.z = 0;
                }
                break;
            case "straigth":
                //don't do anything
                break;
        }
    }
}
