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
    private static List<string> GetFileLines(string path) { 
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
        ParseTable(o);
    }

    private void ParseTable(Hashtable o, WorldObject parent = null) {
        bool isLeaf = o.ContainsKey("attr") ? false : true;

        if (isLeaf) {
            if (parent != null) {
                foreach (DictionaryEntry entry in o) {
                    parent.AddDirectAttribute((string)entry.Key, (string)entry.Value);
                }
            } else {
                Debug.Log("Leaf has no parent");
            }
        } else {    //Not a leaf
            WorldObject newParent = null; //Parent to add children to in next recursion

            foreach (DictionaryEntry entry in o) {  //Loop over entries in hashtable
                if (!entry.Key.Equals("attr")) {    //Only consider whatever is not attr
                    WorldObject worldObject = new WorldObject((string)entry.Key, (string)entry.Value);
                    newParent = worldObject;
                    worldObject.SetParent(parent);
                    if (parent != null) { 
                        parent.AddChild(worldObject);   //Add the newly made worldObject as child to its parent
                    } else {
                        rootObject = worldObject;
                        Debug.Log("new worldobject has no parent");
                    }
               }
            }

            //Consider the attr part
            System.Object attr = o["attr"];         //Returns value for key = "attr", is ArrayList with Hashtables
            ArrayList attrList = (ArrayList)attr;   //Cast to arraylist
            foreach (Hashtable table in attrList) { //Loop over all hashtables in the arraylist
                ParseTable(table, newParent);
            }
        }
    }

    //Is called as last step in Main (Program) to show all constructed WorldObject
    public void PrintWorldObjects() {
        if (this.rootObject != null)
            //rootObject.PrintWorldObject();
            rootObject.PrintObjectInfo();
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
