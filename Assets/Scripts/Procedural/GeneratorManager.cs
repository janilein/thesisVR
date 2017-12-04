using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager : MonoBehaviour{

	public GameObject GenerateWorldObject(WorldObject obj) {

        if (obj.GetObjectValue().Equals("buildings")) {
            Debug.Log("parent is buildings");
            BuildingGenerator generator = new BuildingGenerator();
            generator.GenerateWorldObject(obj.GetChildren()[0]);
        } else {
            Debug.Log("ne marche pas");
        }

        return null;
    }
}
