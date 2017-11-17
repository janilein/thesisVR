using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class HashtableParser {
    private Dictionary<string, List<string>> dictionary;
    private int objectID = 0;
    private List<WorldObject> worldObjects;
    private WorldObject rootObject = null;

    public HashtableParser() {
        ConstructDictionary();
    }

    public WorldObject getRootObject() {
        return rootObject;
    }

    //This function finds all TXT files in the Dictionary folder. Get called from ConstructDictionary();
    private static Hashtable FindFiles() {
        Hashtable table = new Hashtable();  //Hashtable with key = path, and value = name

        DirectoryInfo dir = new DirectoryInfo("Assets/Dictionaries");
        FileInfo[] info = dir.GetFiles("*.txt");
        Debug.Log(info.Length);
        foreach (FileInfo f in info) {
            table[f.FullName] = f.Name.Substring(0, f.Name.Length - 4); ;
        }
        return table;
    }

    //This function reads all lines in a file given a certain (relative) path to the file, get called from ConstructDictionary();
    public List<string> GetFileLines(string path) { //CHANGED PRIVATE STATIC -> LOOK IN TO THIS
        try {
            string[] stringArray = System.IO.File.ReadAllLines(path);
            List<string> stringList = new List<string>(stringArray);  //Convert string[] to List<string>
                                                                      //Check for whitelines and stuff
            List<string> stringsToRemove = new List<string>();  //Keep in a separate list the strings we want to remove (can't do it in foreach loop)
            foreach (string s in stringList) {
                if (IsNullOrWhiteSpace(s) || s.Trim().Length == 0) {
                    stringsToRemove.Add(s);
                }
            }
            //Remove those strings
            foreach (string s in stringsToRemove) {
                stringList.Remove(s);
            }
            return stringList;
        } catch (Exception exc) {
            Debug.Log("Could not get file lines: " + exc.Message);
            return null;
        }

    }

    //This is called in the constructor, initializes the dictionary and calls FindFiles() and GetFileLines()
    private void ConstructDictionary() {

        Hashtable files = FindFiles();
        if (files == null) {
            Debug.Log("Files was null");
            return;
        }

        //Maak dictionary met de gebruikte en aanvaarde woorden
        dictionary = new Dictionary<string, List<string>>();

        //Console.WriteLine("Txt Files--------------------");
        foreach (DictionaryEntry entry in files) {
            List<string> list = GetFileLines((string)entry.Key);
            if (list == null) {
                Debug.Log("List was null");
                return;
            } else {    //Add the strings to the dictionary
                if (list.Count > 0) {
                    try {
                        if (!IsNullOrWhiteSpace((string)entry.Value) && !(((string)entry.Value).Trim().Length == 0)) {
                            dictionary.Add((string)entry.Value, list);
                        }
                    } catch (Exception exc) {
                        Debug.Log('\t' + "Could not add new list to dictionary: " + exc.Message);
                        return;
                    }
                } else {
                    Debug.Log('\t' + "Txt file " + entry.Value + " is empty.");
                }
            }
        }
    }

    //Is the first function that is called from Main (Program) to convert the converted JSON (which has been converted to Hashtable) to the WorldObjects
    public void PrintHashTable(Hashtable o) {
        this.objectID = 0;
        worldObjects = new List<WorldObject>();
        PrintObject(o);
    }

    private void PrintObject(Hashtable o, int levelOfDepth = 0, int parentID = -1, WorldObject parentObject = null) {   //We use levelOfDepth to see how deep we are in the tree structure (or in your mom)
                                                                                                                        //parentID is to link leaf to branch or branch to branch, root has parentID = -1 and parentObject null
        bool isLeaf = true;
        int myID = objectID++;
        WorldObject newParent = null;

        //Check if hashtable contains an attribute (meaning it is a branch)
        if (o.ContainsKey("attr")) {
            isLeaf = false;
        }

        foreach (DictionaryEntry entry in o) { //Wait with doing the attributes till the last moment because the order is inversed (see above)
                                               //Reason for this is that we first need to have the parent for all attr children, so attr should be dealt with latest

            if (!entry.Key.Equals("attr")) {    //Key is not attr, check what it is then, see if dictionary contains the key
                List<string> valueList = new List<string>();

                //Check if key is in dictionary
                if (!dictionary.TryGetValue((string)entry.Key, out valueList)) { //Dictionary does not contain key
                    Debug.Log("#### Key " + "\"" + (string)entry.Key + "\"" + " not found in dictionary ####");
                }

                //For now, we don't take into account whether or not the dictionary contains/allows a certain Key / Value, just put it in the WorldObject
                newParent = new WorldObject((string)entry.Key, (string)entry.Value, myID, isLeaf, parentObject, parentID);
                worldObjects.Add(newParent);

                if (parentObject == null) {
                    this.rootObject = newParent; //Root does not have a parent
                }
            }
        }
        if (!isLeaf) {  //If it's not a leaf, means it still has attributes.
                        //If we get key = "attr" it means we have to go 1 level deeper. In the arraylist is just another hashtable, so get it out
            System.Object entry = null;
            ArrayList list;
            try {
                entry = o["attr"];
                list = (ArrayList)entry;    //Get ArrayList out of the Hashtable
            } catch (Exception exc) {
                Debug.Log("Could not cast attr to arraylist, information:");
                Debug.Log('\t' + "Value: " + entry.ToString());
                Debug.Log('\t' + "Level of depth: " + levelOfDepth);
                Debug.Log('\t' + exc.Message);
                return;
            }

            foreach (System.Object obj in list) {  //ArrayList containt Hashtable(s)
                try {   //try to cast the objects in it to Hashtable
                    Hashtable table = (Hashtable)obj;
                    PrintObject(table, levelOfDepth + 1, myID, newParent);
                } catch (Exception exc) { //Cast failed, what object it is then?
                    Debug.Log("Cast to hashtable failed, object: " + obj.ToString());
                    Debug.Log(exc.Message);
                }
            }
        }
    }

    /*For each WorldObject, set the correct children.
     * At time of creating the WorldObjects, only the parents are known,
     * so know the children must be added to the parents
     */
    public void LinkWorldObjects() {
        if (worldObjects == null || worldObjects.Count == 0) {
            Debug.Log("No WorldObjects exist");
            return;
        }

        foreach (WorldObject obj in worldObjects) {
            //Set children correctly
            WorldObject parentObject = obj.GetParent();
            if (parentObject == null)   //Root does not have a parent object
                continue;

            parentObject.AddChild(obj);
        }
    }

    //Is called as last step in Main (Program) to show all constructed WorldObject
    public void PrintWorldObjects() {
        if (this.rootObject != null)
            rootObject.PrintWorldObject();
        else
            Debug.Log("rootObject is null");
    }

    public static bool IsNullOrWhiteSpace(string value) {
        if (value != null) {
            for (int i = 0; i < value.Length; i++) {
                if (!char.IsWhiteSpace(value[i])) {
                    return false;
                }
            }
        }
        return true;
    }

}
