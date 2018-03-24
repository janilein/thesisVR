using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BuildingGenerator : Generator {

    Hashtable bounds;
    string typeOfBuilding;

    private static int lotID = 1;
 
    public BuildingGenerator() {
        bounds = new Hashtable();
        if(worldTransform == null)
        {
            worldTransform = GameObject.Find("World").transform;
        }
    }

    public override GameObject GenerateWorldObject(WorldObject obj, Vector3 currentDirection, string JSON = null) {
        //Obj is the 'lot' root obj
        //int lotNumber;
        //if (obj.directAttributes.ContainsKey("lotID")) {
        //    string lotString = (string)obj.directAttributes["lotID"];
        //    Int32.TryParse(lotString, out lotNumber);
        //} else {
        //    System.Random random = new System.Random();
        //    lotNumber = random.Next(1000, 1000000);
        //}

        //Create the 'lot' parent
        GameObject lot = new GameObject("lotID:" + lotID++);
        lot.AddComponent<JSONHolder>();
        lot.GetComponent<JSONHolder>().JSON = JSON;
        //Now go to the children
        obj = obj.GetChildren()[0].GetChildren()[0];

        //Spawning appartment or house or ...? (For defaults)
        typeOfBuilding = obj.GetObjectValue();
        Debug.Log("Type of building: " + typeOfBuilding);

        Hashtable specifiedDefaults = obj.directAttributes;

        bounds.Clear();

        //Check nb floors
        int nbFloors = 0;
        try {
            Hashtable attributes = obj.directAttributes;
            string stringFloors = (string) attributes["floors"];      
            Int32.TryParse(stringFloors, out nbFloors);
        } catch(Exception e) {
            Debug.Log(e.Message);
            throw new Exception("No floors given");
        }

        List<int> floorNumbersToGenerate = new List<int>();
        for(int i = 1; i <= nbFloors; i++) {
            floorNumbersToGenerate.Add(i);
        }

        bool generateRoof = false;

        //Make a house parent in it
        GameObject house = Resources.Load("ProceduralBlocks/House") as GameObject;
        GameObject parent = GameObject.Instantiate(house, new Vector3(0, 0, 0), Quaternion.identity);
        parent.transform.name = "Building";
		parent.transform.SetParent (lot.transform, false);
       // parent.transform.parent = lot.transform;
        Transform parentTransform = parent.transform;

        //Count fully & partially defined floors
        List<WorldObject> partiallyDefinedFloors = new List<WorldObject>();
        foreach (WorldObject child in obj.GetChildren()) {
            if (child.GetObjectValue().Equals("floor")) {
                Hashtable childAttributes = child.directAttributes;
                if (childAttributes.ContainsKey("level")) {
                    int floorNumber = 0;
                    Int32.TryParse((string)childAttributes["level"], out floorNumber);
                    if (floorNumbersToGenerate.Contains(floorNumber)) {
                        floorNumbersToGenerate.Remove(floorNumber);
                        Hashtable floorBounds = GenerateFloor(child, parentTransform, specifiedDefaults);
                        UpdateBounds(floorBounds);
                    }
                } else {
                    partiallyDefinedFloors.Add(child);
                }
            } else if (child.GetObjectValue().Equals("roof")) {
                //Roof should be generated as last (height has to be known)
                generateRoof = true;
            }
        }

        //Generate partially defined floors
        foreach(WorldObject child in partiallyDefinedFloors) {
            child.directAttributes["level"] = floorNumbersToGenerate[0];
            floorNumbersToGenerate.Remove(floorNumbersToGenerate[0]);
            Debug.Log("Generationg partially defined floor");
            Hashtable floorBounds = GenerateFloor(child, parentTransform, specifiedDefaults);
            UpdateBounds(floorBounds);
        }

        //check if there are still fully default
        if(floorNumbersToGenerate.Count != 0) {
            for (int i = 0; i < floorNumbersToGenerate.Count; i++) {
                Hashtable floorBounds = GenerateDefaultFloor(floorNumbersToGenerate[i], parentTransform, specifiedDefaults);
                UpdateBounds(floorBounds);
            }
        }

        if (generateRoof) {
            //Generate the roof
            foreach (WorldObject child in obj.GetChildren()) {
                if (child.GetObjectValue().Equals("roof")) {
                    float height = ((Vector2)bounds["yBounds"]).y; //Max height reached
                    Hashtable roofBounds = GenerateRoof(child, parentTransform, specifiedDefaults, height);
                    UpdateBounds(roofBounds);
                }
            }
        } else {
            //Generate default roof, ah ja he
            float height = ((Vector2)bounds["yBounds"]).y; //Max height reached
            Hashtable roofBounds = GenerateDefaultRoof(parentTransform, height, specifiedDefaults);
            UpdateBounds(roofBounds);
        }

        //Update the parent (house) box collider using the bounds
        //BoxCollider coll = house.GetComponent<BoxCollider>();
        Vector2 xBounds = (Vector2)bounds["xBounds"];
        float xSize = xBounds.y - xBounds.x;
        Vector2 yBounds = (Vector2)bounds["yBounds"];
        float ySize = yBounds.y - yBounds.x;
        Vector2 zBounds = (Vector2)bounds["zBounds"];
        float zSize = zBounds.y - zBounds.x;
        //Debug.Log("xSize: " + xSize);
        //Debug.Log("ySize: " + ySize);
        //Debug.Log("ZSize: " + zSize);
        parent.GetComponent<BoxCollider>().size = new Vector3(xSize, ySize, zSize);
        parent.GetComponent<BoxCollider>().center = new Vector3(0, ySize / 2, 0);

        //Move the spawned house to the active lot
        GameObject lotToSpawn = LotManager.getLot();
        if(lotToSpawn != null)
        {
            Debug.Log("Lot to spawn position: " + lotToSpawn.transform.position.ToString());
            lot.transform.localPosition = lotToSpawn.transform.position;
            lot.transform.rotation = lotToSpawn.transform.parent.transform.localRotation; 

            //Set the LotToSpawn as the lot's parent
			lot.transform.SetParent(lotToSpawn.transform, false);

            //The lots rotation
            //Debug.Log("Lot euler: " + lotToSpawn.transform.localEulerAngles.ToString());

            //The street rotation: 
            //Debug.Log("Street euler: " + lotToSpawn.transform.parent.transform.localEulerAngles.ToString());
        } else
        {
			lot.transform.SetParent(worldTransform);
        }

        return lot;

    }

    private void UpdateBounds(Hashtable newBounds) {
        if(bounds.Count == 0) {
            //No bounds yet
            foreach(DictionaryEntry entry in newBounds) {
                bounds.Add(entry.Key, entry.Value);
            }
        } else {
            //Check the new bounds
            Vector2 finalBounds = FindBoundValues((Vector2)bounds["xBounds"], (Vector2)newBounds["xBounds"]);
            bounds["xBounds"] = finalBounds;

            finalBounds = FindBoundValues((Vector2)bounds["yBounds"], (Vector2)newBounds["yBounds"]);
            bounds["yBounds"] = finalBounds;

            finalBounds = FindBoundValues((Vector2)bounds["zBounds"], (Vector2)newBounds["zBounds"]);
            bounds["zBounds"] = finalBounds;
            //Debug.Log("Bounds: ");
            //foreach(DictionaryEntry entry in bounds)
            //{
            //    Debug.Log(entry.Key + " : " + entry.Value.ToString());
            //}
        }
    }

    private Vector2 FindBoundValues(Vector2 firstBound, Vector2 secondBound) {
        Vector2 finalBounds = new Vector2(Math.Min(firstBound.x, secondBound.x), Math.Max(firstBound.y, secondBound.y));
        return finalBounds;
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

    private Hashtable GenerateFloor(WorldObject obj, Transform parent, Hashtable specifiedDefaults) {
        
        int level = 0;
        Int32.TryParse(((string)obj.directAttributes["level"]), out level);    //Level 1 = ground floor
        level -= 1;
        GameObject block = Resources.Load("ProceduralBlocks/Block") as GameObject;
        if(block == null) {
            Debug.Log("block is null");
            return null;
        }

        //Height of a block        
        Mesh mesh = block.GetComponent<MeshFilter>().sharedMesh;
        Vector3 meshSize = mesh.bounds.size;
        float width, length, height;
        Vector3 scale = block.transform.localScale;
        width = scale.x * meshSize.x;
        length = scale.z * meshSize.z;
        height = scale.y * meshSize.y;

        //Vector3 scale = block.transform.localScale;
        //float height = scale.y;
        //float width = scale.x;
        //float length = scale.z;
        //Debug.Log("Width: " + width);
        //Debug.Log("Height: " + height);
        //Debug.Log("Length: " + length);

        float xPos = 0;
        float yPos = height/2 + level*height;
        float zPos = 0;

        //Instantiate this block
        GameObject instantiatedBlock = UnityEngine.Object.Instantiate(block, new Vector3(xPos, yPos, zPos), Quaternion.identity, parent);

        //Set the color of the floor
        string color = (string) obj.directAttributes["color"];
        //Debug.Log("Color to set: " + color);
        //Debug.Log("Level: " + (string)obj.directAttributes["level"]);
        Material material = GetMaterial(color);
        if (material != null) {
            MeshRenderer renderer = instantiatedBlock.GetComponent<MeshRenderer>();
            renderer.material = material;
        }

        //Return the bounds for this spawned block
        // (-x, +x), (-y, +y), (-z, +z)
        Hashtable bounds = new Hashtable();
        bounds["xBounds"] = new Vector2(xPos - width/2, xPos + width/2);
        bounds["yBounds"] = new Vector2(yPos - height/2, yPos + height/2);
        bounds["zBounds"] = new Vector2(zPos - length/2, zPos + length/2);
        //Debug.Log("Spawned floor");

        return bounds;
    }

    private Hashtable GenerateDefaultFloor(int level, Transform parent, Hashtable specifiedDefaults) {
        Hashtable defaults = FindDefaults("floor");
        string type = "type";
        string value = "floor";
        WorldObject floor = new WorldObject(type, value);
        foreach (DictionaryEntry entry in defaults) {
            string key = (string)entry.Key;
            if (!specifiedDefaults.Contains(key)) {
                string attributeValue = (string)entry.Value;
                floor.AddDirectAttribute(key, attributeValue);
            } else {
                floor.AddDirectAttribute(key, (string)specifiedDefaults[key]);
            }
        }
        string levelText = level.ToString();
        floor.AddDirectAttribute("level", levelText);
            Int32.TryParse(((string)floor.directAttributes["level"]), out level);
        return GenerateFloor(floor, parent, specifiedDefaults);
        //level -= 1;

        //Debug.Log("Default floor");

        //GameObject block = Resources.Load("ProceduralBlocks/Block") as GameObject;
        //if(block == null) {
        //    Debug.Log("Block is null");
        //    return null;
        //}

        ////Height of a block
        //Vector3 scale = block.transform.localScale;
        //float height = scale.y;
        //float width = scale.x;
        //float length = scale.z;

        //float xPos = 0;
        //float yPos = height / 2 + level * height;
        //float zPos = 0;

        ////Instantiate this block
        //GameObject instantiatedBlock = UnityEngine.Object.Instantiate(block, new Vector3(xPos, yPos, zPos), Quaternion.identity, parent);

        ////Find default color
        //Hashtable defaults = FindDefaults("floor");
        //if (defaults.ContainsKey("color")) {
        //    //It should contain this key, but doesn't hurt to check
        //    string color = (string) defaults["color"];
        //    Material material = GetMaterial(color);
        //    if (material != null) {
        //        MeshRenderer renderer = instantiatedBlock.GetComponent<MeshRenderer>();
        //        renderer.material = material;
        //    } else {
        //        Debug.Log("Material is null");
        //    }
        //} else {
        //    Debug.Log("No color default found");
        //}

        ////Return the bounds for this spawned block
        //// (-x, +x), (-y, +y), (-z, +z)
        //Hashtable bounds = new Hashtable();
        //bounds["xBounds"] = new Vector2(xPos - width / 2, xPos + width / 2);
        //bounds["yBounds"] = new Vector2(yPos - height / 2, yPos + height / 2);
        //bounds["zBounds"] = new Vector2(zPos - length / 2, zPos + length / 2);
        //Debug.Log("Spawned floor");
        //return bounds;
    }

    private Hashtable GenerateDefaultRoof(Transform parentTransform, float height, Hashtable specifiedDefaults) {
        Hashtable defaults = FindDefaults("roof");
        string type = "type";
        string value = "roof";
        WorldObject roof = new WorldObject(type, value);
        foreach(DictionaryEntry entry in defaults) {
            string key = (string)entry.Key;
            if (!specifiedDefaults.Contains(key)) {
                Debug.Log("#############Specified roof does not contain key " + (string)key);
                string attributeValue = (string)entry.Value;
                roof.AddDirectAttribute(key, attributeValue);
            } else {
                Debug.Log("#############Specified roof contains key " + (string)key);
                roof.AddDirectAttribute(key, (string) specifiedDefaults[key]);
            }    
        }
        return GenerateRoof(roof, parentTransform, specifiedDefaults, height);
    }

    private Hashtable GenerateRoof(WorldObject obj, Transform parent, Hashtable specifiedDefaults, float height = 0) {

        Debug.Log("Generationg roof");

        //Check type of roof
        string type = (string) obj.directAttributes["roofType"];
        Debug.Log("RoofType: " + type);

        if (type.Equals("flat")) {
            float xPos = 0;
            float yPos = height;
            float zPos = 0;

            GameObject flatRoof = Resources.Load("ProceduralBlocks/FlatRoof") as GameObject;

            //Height of a block

            /*
            Mesh mesh = flatRoof.GetComponent<MeshFilter>().sharedMesh;
            Vector3 meshSize = mesh.bounds.size;
            float roofWidth, roofLength, roofHeight;
            Vector3 scale = flatRoof.transform.localScale;
            roofWidth = scale.x * meshSize.x;
            roofLength = scale.z * meshSize.z;
            roofHeight = scale.y * meshSize.y;
            */

            Vector3 scale = flatRoof.transform.localScale;
            float roofHeight = scale.y;
            //Debug.Log("Roofheight: " + roofHeight);
            float roofWidth = scale.x;
            float roofLength = scale.z;

            //Instantiate the roof
            GameObject roof = UnityEngine.Object.Instantiate(flatRoof, new Vector3(xPos, yPos + roofHeight / 2, zPos), Quaternion.identity, parent);

            //Get the color
            string color = (string)obj.directAttributes["color"];
            Material material = GetMaterial(color);
            if (material != null) {
                MeshRenderer[] renderers = roof.GetComponentsInChildren<MeshRenderer>();
                foreach(MeshRenderer renderer in renderers) {
                    renderer.material = material;
                }
            }

            Hashtable bounds = new Hashtable();
            bounds["xBounds"] = new Vector2(xPos - roofWidth / 2, xPos + roofWidth / 2);
            bounds["yBounds"] = new Vector2(yPos, yPos + roofHeight);
            bounds["zBounds"] = new Vector2(zPos - roofLength / 2, zPos + roofLength / 2);

            //Debug.Log("Roof yBounds: " + bounds["yBounds"].ToString());
            return bounds;

        }        else if (type.Equals("pointy")) {
            //Generate this roof using mesh generation #cool

            return GenerateMeshRoof(obj, parent, height);
        }
        return null;
        }

    private Hashtable GenerateMeshRoof(WorldObject obj, Transform parent, float height) {
        float totalHeight = 5f;
        //Pointy roof bestaat uit 4 delen: de 2 schuine delen, en de 2 driehoeken

        //Determine the positions for the roof
        Vector2 xValues = (Vector2)bounds["xBounds"];
        Vector2 yValues = (Vector2)bounds["yBounds"];
        Vector2 zValues = (Vector2)bounds["zBounds"];

        //Shortest axis?
        float xDiff = Math.Abs(xValues.y - xValues.x);
        float zDiff = Math.Abs(zValues.y - zValues.x);

        Debug.Log("xDiff: " + xDiff);
        Debug.Log("zDiff: " + zDiff);

        //Is the X axis the shortest?
        bool xShortest = xDiff > zDiff ? true : false;

        Vector3 highestPointA, highestPointB;
        Vector3 basePointA, basePointB, basePointC, basePointD ;

        basePointA = new Vector3(xValues.y, height, zValues.y);
        basePointB = new Vector3(xValues.y, height, zValues.x);
        basePointC = new Vector3(xValues.x, height, zValues.x);
        basePointD = new Vector3(xValues.x, height, zValues.y);

        if (xShortest == true) { //Points for X axis shortest
            Debug.Log("x shortest");
            float center = (xValues.y + xValues.x) / 2;
            highestPointA = new Vector3(center, height + totalHeight, zValues.x);
            highestPointB = new Vector3(center, height + totalHeight, zValues.y);
        } else {
            Debug.Log("z shortest");
            Debug.Log("z y: " + zValues.y);
            Debug.Log("z x: " + zValues.x);
            float center = (zValues.y + zValues.x) / 2;
            highestPointA = new Vector3(xValues.x, height + totalHeight, center);
            highestPointB = new Vector3(xValues.y, height + totalHeight, center);
        }

        Vector2 shortestAxis = xDiff > zDiff ? zValues : xValues;
        Vector2 longestAxis = xDiff <= zDiff ? zValues : xValues;

        //Center points
        float shortestCenter = (shortestAxis.y - shortestAxis.x)/2;
        float longestCenter = (longestAxis.y - longestAxis.x)/2;

        Vector3[] vertices = new Vector3[6];
        vertices[0] = highestPointA;
        vertices[1] = highestPointB;
        vertices[2] = basePointA;
        vertices[3] = basePointB;
        vertices[4] = basePointC;
        vertices[5] = basePointD;

        //In totaal bestaat het dak uit 6 driehoeken
        int numTriPoints = 3 * 6;
        int[] tri = new int[numTriPoints];

        if (xShortest == true) {
            //Eerste driehoek
            tri[0] = 2;
            tri[1] = 1;
            tri[2] = 5;
            //Tweede driehoek
            tri[3] = 4;
            tri[4] = 0;
            tri[5] = 3;
            //Derde driehoek
            tri[6] = 5;
            tri[7] = 1;
            tri[8] = 4;
            //Vierde driehoek
            tri[9] = 4;
            tri[10] = 1;
            tri[11] = 0;
            //Vijfde driehoek
            tri[12] = 3;
            tri[13] = 1;
            tri[14] = 2;
            //Zesde driehoek
            tri[15] = 3;
            tri[16] = 0;
            tri[17] = 1;
        } else {
            //Eerste driehoek
            tri[0] = 5;
            tri[1] = 0;
            tri[2] = 4;
            //Tweede driehoek
            tri[3] = 3;
            tri[4] = 1;
            tri[5] = 2;
            //Derde driehoek
            tri[6] = 4;
            tri[7] = 0;
            tri[8] = 3;
            //Vierde driehoek
            tri[9] = 3;
            tri[10] = 0;
            tri[11] = 1;
            //Vijfde driehoek
            tri[12] = 2;
            tri[13] = 0;
            tri[14] = 5;
            //Zesde driehoek
            tri[15] = 2;
            tri[16] = 1;
            tri[17] = 0;
        }

        GameObject meshRoof = Resources.Load("ProceduralBlocks/MeshRoofTriangle") as GameObject;

        GameObject roofObject = UnityEngine.Object.Instantiate(meshRoof, new Vector3(0, 0, 0), Quaternion.identity, parent);

        MeshFilter filter = roofObject.GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        filter.mesh = mesh;

        mesh.vertices = vertices;
        mesh.triangles = tri;
        mesh.RecalculateNormals();

        //Get the color
        string color = (string) obj.directAttributes["color"];
        Material material = GetMaterial(color);
        if(material != null) {
            MeshRenderer renderer = roofObject.GetComponent<MeshRenderer>();
            renderer.material = material;
        }

        Hashtable newBounds = new Hashtable();
        newBounds["xBounds"] = bounds["xBounds"];
        newBounds["zBounds"] = bounds["zBounds"];
        newBounds["yBounds"] = new Vector2(height, (height + totalHeight));
        return newBounds;
    }

    private Material GetMaterial(string color) {
        Material material = null;
        switch (color) {
            case "blue":
                material = Resources.Load("Materials/MaterialBlue") as Material;
                break;
            case "brown":
                material = Resources.Load("Materials/MaterialBrown") as Material;
                break;
            case "green":
                material = Resources.Load("Materials/MaterialGreen") as Material;
                break;
            case "purple":
                material = Resources.Load("Materials/MaterialPurple") as Material;
                break;
            case "red":
                material = Resources.Load("Materials/MaterialRed") as Material;
                break;
            case "yellow":
                material = Resources.Load("Materials/MaterialYellow") as Material;
                break;
        }
        return material;
    }

    private Hashtable FindDefaults(string tree) {
        Hashtable defaults = new Hashtable();
        string path = "Assets/Dictionaries/lots/buildings/" + typeOfBuilding + "/" + tree;
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("defaults.txt");

        List<string> possibilities = new List<string>(); //filenames of possible top type matches

        foreach (FileInfo f in info) {
            string[] lines = File.ReadAllLines(f.FullName);
            foreach(string s in lines) {
                int index = s.IndexOf(":");
                if (index == -1) {
                    Debug.Log("Description file is corrupt. No use of ':'. Name: " + f.FullName);
                } else {
                    string key = s.Substring(0, index);
                    string value = s.Substring(index + 1, s.Length - index - 1);
                    //Debug.Log("Adding Key: " + key + " Value: " + value);
                    defaults.Add(key, value);
                }
            }
        }
        return defaults;
    }

    public void DecreaseLotCounter()
    {
        lotID -= 1;
    }
}
