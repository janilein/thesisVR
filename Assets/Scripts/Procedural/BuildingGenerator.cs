using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuildingGenerator : Generator {

    private List<GameObject> generatedFloors;

    public BuildingGenerator() {
        generatedFloors = new List<GameObject>();
    }

    public override void GenerateWorldObject(WorldObject obj) {
        //Check nb floors
        int nbFloors = 0;
        try {
            Hashtable attributes = obj.directAttributes;
            string stringFloors = (string) attributes["floors"];      
            Int32.TryParse(stringFloors, out nbFloors);
        } catch(Exception e) {
            Debug.Log(e.Message);
            return;
        }

        List<int> floorNumbersToGenerate = new List<int>();
        for(int i = 1; i <= nbFloors; i++) {
            floorNumbersToGenerate.Add(i);
        }

        //Count fully & partially defined floors
        List<WorldObject> partiallyDefinedFloors = new List<WorldObject>();
        foreach (WorldObject child in obj.GetChildren()) {
            if (child.GetObjectValue().Equals("floor")) {
                Hashtable childAttributes = child.directAttributes;
                if (childAttributes.ContainsKey("level")) {
                    int floorNumber = 0;
                    Int32.TryParse((string) childAttributes["level"], out floorNumber);
                    if (floorNumbersToGenerate.Contains(floorNumber)) {
                        floorNumbersToGenerate.Remove(floorNumber);
                        GenerateFloor(child);
                    }
                } else {
                    partiallyDefinedFloors.Add(child);
                }
            }
        }

        //Generate partially defined floors
        foreach(WorldObject child in partiallyDefinedFloors) {
            child.directAttributes["level"] = floorNumbersToGenerate[0];
            floorNumbersToGenerate.Remove(floorNumbersToGenerate[0]);
            GenerateFloor(child);
        }

        //check if there are still fully default
        if(floorNumbersToGenerate.Count != 0) {
            for (int i = 0; i < floorNumbersToGenerate.Count; i++) {
                GenerateDefaultFloor(floorNumbersToGenerate[i]);
            }
        }

    }

    //private bool SetDefaults(string objValue) {
    //    DirectoryInfo dir;
    //    try {
    //        dir = new DirectoryInfo("Assets/Categories");

    //        FileInfo[] info = dir.GetFiles(objValue + ".txt");
    //        if (info.Length > 0) {
    //            string[] lines = File.ReadAllLines(info[0].FullName);
    //            foreach (string s in lines) {
    //                int index = s.IndexOf(":");
    //                if (index == -1) {
    //                    Debug.Log("Description file is corrupt. No use of ':'. Name: " + info[0].FullName);
    //                } else {
    //                    string key = s.Substring(0, index);
    //                    string value = s.Substring(index + 1, s.Length - index - 1);
    //                    Debug.Log("Key: " + key);
    //                    Debug.Log("Value: " + value);
    //                    if (defaultValues.Contains(key)) {
    //                        defaultValues[key] = value;
    //                    } else {
    //                        Debug.Log("Default values has no key for: " + key);
    //                    }
    //                }

    //            }

    //            foreach (DictionaryEntry entry in defaultValues) {
    //                if (entry.Value == null) {
    //                    Debug.Log("No default value set for key: " + entry.Key);
    //                    return false;
    //                }
    //            }
    //        } else {
    //            Debug.Log("sumting wong");
    //            return false;
    //        }
    //    } catch (Exception e) {
    //        Debug.Log(e.Message);
    //        return false;
    //    }
    //    return true;
    //}

    private void GenerateFloor(WorldObject obj) {

    }

    private void GenerateDefaultFloor(int level) {
        throw new NotImplementedException();
    }

    private void GenerateRoof(WorldObject obj) {

    }
}
