﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager {

	public void GenerateWorldObject(WorldObject obj) {

        if (obj.GetObjectValue().Equals("lots")) {
            WorldObject child = obj.GetChildren()[0];
            if (child.GetObjectValue().Equals("buildings")) {
                Debug.Log("parent is buildings");
                BuildingGenerator generator = new BuildingGenerator();
                generator.GenerateWorldObject(child.GetChildren()[0]);
            } else if (obj.GetObjectValue().Equals("gardens")) {
                Debug.Log("parent is garden");
                //BuildingGenerator generator = new BuildingGenerator();
                //generator.GenerateWorldObject(obj.GetChildren()[0]);
            } else {
                Debug.Log("ne marche pas");
            }
        } else if (obj.GetObjectValue().Equals("streets")) {
            WorldObject child = obj.GetChildren()[0];
            Debug.Log("parent is streets");
            StreetGenerator generator = new StreetGenerator();
            generator.GenerateWorldObject(child.GetChildren()[0]);
        } else {
            Debug.Log("ne marche pas");
        }
        return;
    }
}
