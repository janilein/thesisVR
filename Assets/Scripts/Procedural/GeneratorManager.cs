using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager {

    private BuildingGenerator buildingGen;
    //private StreetGenerator streetGen;
    private static StreetGeneratorV2 streetGenV2;
    //private Vector3 currentDirection;
    public static Vector2 currentDirection;
    //private Vector3 currentPosition;
	public static Orientation pointDirection;

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
        //currentPosition = Vector3.zero;
		pointDirection = Orientation.straight;

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

				GameObject newPreviousObject = buildingGen.GenerateWorldObject(obj, currentDirection, JSON); //Obj, niet child.getChildren()[0], want generator moet aan het 'lot' kunnen
                if(newPreviousObject != null)
					previousObject = newPreviousObject;
				
				if (SaveManager.loadingGame)
                {
                    Speech.SetSpecification(false);
                    LotManager.DeselectLot();
                }
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
            streetGenV2.GenerateWorldObject(obj, ref currentDirection, pointDirection);
			pointDirection = Orientation.straight;

			//In street, also automatically arrow rotating
			streetGenV2.SelectDirectionArrow(pointDirection);

        } else if (obj.GetObjectValue().Equals("orientation")) {
			Orientation orient = OrientationEnumFunctions.GetOrientationFromString ((string)obj.directAttributes["direction"]);
            ChangeDirection(orient, true);
            //Debug.Log("currentDir x: " + currentDirection.x);
            //Debug.Log("currentDir z: " + currentDirection.z);
        } else {
            Debug.Log("ne marche pas");
        }

        shaderScript.UpdateShaders();
        return;
    }

	private static void ChangeDirection(Orientation direction, bool selectArrow) {
		Debug.Log("direction switch: " + direction.ToString());
        if (!streetGenV2.DirectionAllowed(direction)) {
            Debug.Log("Orienation change not allowed");
            return;
        }

        switch (direction)
        {
			case Orientation.left:
				if(pointDirection == Orientation.right){
					currentDirection.x = (currentDirection.x - 180) % 360;
				} else if(pointDirection == Orientation.straight)  {
					currentDirection.x = (currentDirection.x - 90) % 360;
				}
				pointDirection = Orientation.left;
				Debug.Log("Point direction set to " + pointDirection.ToString());
                break;
			case Orientation.right:
				if(pointDirection == Orientation.left){
					currentDirection.x = (currentDirection.x + 180) % 360;
				} else if(pointDirection == Orientation.straight)  {	
					currentDirection.x = (currentDirection.x + 90) % 360;
				}
				pointDirection = Orientation.right;
				Debug.Log("Point direction set to " + pointDirection.ToString());
				break;
			case Orientation.straight:
				if(pointDirection == Orientation.left){
					currentDirection.x = (currentDirection.x + 90) % 360;
				} else if(pointDirection == Orientation.right){	
					currentDirection.x = (currentDirection.x - 90) % 360;
				}
				pointDirection = Orientation.straight;
				Debug.Log("Point direction set to " + pointDirection.ToString());
                break;
            case Orientation.back:
                if (pointDirection == Orientation.straight)
                {
                    currentDirection.x = (currentDirection.x + 180) % 360;
                }
                pointDirection = Orientation.back;
                Debug.Log("Point direction set to " + pointDirection.ToString());
                break;
        }

		if (selectArrow) {
			streetGenV2.SelectDirectionArrow (pointDirection);
		}
    }

	public static void ChangeDirectionFromCollider(Orientation direction, Vector2 dir)
    {
        GeneratorManager.currentDirection = dir;
		GeneratorManager.pointDirection = Orientation.straight;	//Start from a 'default' straight direction again
		GeneratorManager.ChangeDirection(direction, false);
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
		
		if (!SaveManager.loadingGame)
        {
            SaveManager.AddJSON(previousObject.GetComponent<JSONHolder>().JSON);
        }
		
    }
}
