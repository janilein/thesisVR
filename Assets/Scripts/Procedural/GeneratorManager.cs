using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager {

    private BuildingGenerator buildingGen;
    //private StreetGenerator streetGen;
    private static StreetGeneratorV2 streetGenV2;
    //private Vector3 currentDirection;
    private static Vector2 currentDirection;
    private Vector3 currentPosition;
    private static string pointDirection;

    public GameObject previousObject = null;

    private InitializeShaders shaderScript;

    //The active lot on which we want to spawn a building
    public GameObject activeLot;

    public GeneratorManager() {
        buildingGen = new BuildingGenerator();
        //streetGen = new StreetGenerator();
        streetGenV2 = new StreetGeneratorV2();

        //currentDirection = new Vector3(0, 0, 1);
        currentDirection = new Vector2(0, 0);   //ALong positive Z-axis
        //currentPosition = Vector3.positiveInfinity;
        currentPosition = Vector3.zero;
        pointDirection = "straight";

        shaderScript = GameObject.Find("ObjectManager").GetComponent<InitializeShaders>();
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
            //streetGen.GenerateWorldObject(obj, currentDirection, ref currentPosition, pointDirection);
            streetGenV2.GenerateWorldObject(obj, ref currentDirection, ref currentPosition, pointDirection);
            pointDirection = "straight";

			//In street, also automatically arrow rotating
			streetGenV2.SelectDirectionArrow(pointDirection, false);

        } else if (obj.GetObjectValue().Equals("orientation")) {
            ChangeDirection((string)obj.directAttributes["direction"], true);
            //Debug.Log("currentDir x: " + currentDirection.x);
            //Debug.Log("currentDir z: " + currentDirection.z);
        } else {
            Debug.Log("ne marche pas");
        }

        shaderScript.UpdateShaders();
        return;
    }

	private static void ChangeDirection(string direction, bool selectArrow) {
        Debug.Log("direction switch: " + direction);

        switch (direction)
        {
            case "left":
				if(pointDirection.Equals("right")){
					currentDirection.x = (currentDirection.x - 180) % 360;
				} else if(pointDirection.Equals("straight"))  {
					currentDirection.x = (currentDirection.x - 90) % 360;
				}
				pointDirection = "left";
				Debug.Log("Point direction set to " + pointDirection);
                break;
            case "right":
				if(pointDirection.Equals("left")){
					currentDirection.x = (currentDirection.x + 180) % 360;
				} else if(pointDirection.Equals("straight"))  {	
					currentDirection.x = (currentDirection.x + 90) % 360;
				}
				pointDirection = "right";
				Debug.Log("Point direction set to " + pointDirection);
				break;
            case "straight":
				if(pointDirection.Equals("left")){
					currentDirection.x = (currentDirection.x + 90) % 360;
				} else if(pointDirection.Equals("right")){	
					currentDirection.x = (currentDirection.x - 90) % 360;
				}
				pointDirection = "straight";
				Debug.Log("Point direction set to " + pointDirection);
                break;
        }

		if (selectArrow) {
			streetGenV2.SelectDirectionArrow (pointDirection, true);
		}
    }

    public static void ChangeDirectionFromCollider(string direction, Vector2 dir)
    {
        GeneratorManager.currentDirection = dir;
		GeneratorManager.pointDirection = "straight";	//Start from a 'default' straight direction again
		GeneratorManager.ChangeDirection (direction, false);
    }

    public void SetActiveLot(GameObject newLot)
    {
        activeLot = newLot;
    }

    public void stopSpecifying()
    {
        if(previousObject != null)
        {
            //Previous object is the lot, get the house in the lot
            Transform house = previousObject.transform.Find("Building");
            if(house != null)
            {
                house.tag = "Highlightable";
            } else
            {
                Debug.Log("No house found in lot");
            }
            LotManager.DeselectLot();
            previousObject = null;
        }
    }
}
