using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager {

    private BuildingGenerator buildingGen;
    private StreetGenerator streetGen;
    private Vector3 currentDirection;
    private Vector3 currentPosition;
    private string pointDirection;

    public GameObject previousObject = null;

    public GeneratorManager() {
        buildingGen = new BuildingGenerator();
        streetGen = new StreetGenerator();

        currentDirection = new Vector3(0, 0, 1);
        currentPosition = Vector3.positiveInfinity;
        pointDirection = "top";
    }

	public void GenerateWorldObject(WorldObject obj, string JSON = null) {
        if (obj.GetObjectValue().Equals("lots")) {
            WorldObject child = obj.GetChildren()[0];
            if (child.GetObjectValue().Equals("buildings")) {
                Debug.Log("parent is buildings");
                //BuildingGenerator generator = new BuildingGenerator(); //gaan veel van deze maken, misschien in een singleton steken die we gaan oproepen?
                if (Speech.specifyDescription)
                {
                    if(previousObject != null)
                    {
                        MonoBehaviour.Destroy(previousObject);
                        previousObject = null;
                        buildingGen.DecreaseLotCounter();
                    }
                } else
                {
                    previousObject = null;
                }

                previousObject = buildingGen.GenerateWorldObject(obj, currentDirection, JSON); //Obj, niet child.getChildren()[0], want generator moet aan het 'lot' kunnen
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
            streetGen.GenerateWorldObject(obj, currentDirection, ref currentPosition, pointDirection);
            pointDirection = "top";
        } else if (obj.GetObjectValue().Equals("orientation")) {
            ChangeDirection((string)obj.directAttributes["direction"]);
            //Debug.Log("currentDir x: " + currentDirection.x);
            //Debug.Log("currentDir z: " + currentDirection.z);
        } else {
            Debug.Log("ne marche pas");
        }
        return;
    }

    private void ChangeDirection(string direction) {
        Debug.Log("direction switch");
        switch (direction) {
            case "left":
                if (currentDirection.x == 1) {
                    currentDirection.x = 0;
                    currentDirection.z = 1;
                    //pointDirection = "left";
                } else if (currentDirection.x == -1) {
                    currentDirection.x = 0;
                    currentDirection.z = -1;
                    //pointDirection = "right";
                } else if (currentDirection.z == 1) {
                    currentDirection.x = -1;
                    currentDirection.z = 0;
                    //pointDirection = "bottom";
                } else if (currentDirection.z == -1) {
                    currentDirection.x = 1;
                    currentDirection.z = 0;
                    //pointDirection = "top";
                }
                pointDirection = "left";
                break;
            case "right":
                if (currentDirection.x == 1) {
                    currentDirection.x = 0;
                    currentDirection.z = -1;
                    //pointDirection = "right";
                } else if (currentDirection.x == -1) {
                    currentDirection.x = 0;
                    currentDirection.z = 1;
                    //pointDirection = "left";
                } else if (currentDirection.z == 1) {
                    currentDirection.x = 1;
                    currentDirection.z = 0;
                    //pointDirection = "top";
                } else if (currentDirection.z == -1) {
                    currentDirection.x = -1;
                    currentDirection.z = 0;
                    //pointDirection = "bottom";
                }
                pointDirection = "right";
                break;
            case "straigth":
                //don't do anything
                break;
        }
    }
}
