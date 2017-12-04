using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class HashtableParser {
    private Dictionary<string, List<string>> dictionary;
    private WorldObject rootObject = null;

    public HashtableParser() {
        dictionary = new Dictionary<string, List<string>>();
    }

    public WorldObject getRootObject() {
        return rootObject;
    }

    //This function finds all TXT files in the Dictionary folder. Get called from ConstructDictionary();
    private static Hashtable FindFiles(string basePath) {
        Hashtable table = new Hashtable();  //Hashtable with key = path, and value = name

        DirectoryInfo dir = new DirectoryInfo(basePath);
        FileInfo[] info = dir.GetFiles("*.txt");
        //Debug.Log(info.Length);
        foreach (FileInfo f in info) {
            if (!f.Name.Equals("defaults.txt")){
                table[f.FullName] = f.Name.Substring(0, f.Name.Length - 4); ;
            }
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
    private void ConstructDictionary(string basePath) {

        Hashtable files = FindFiles(basePath);
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
        ParseTable(o);
    }

    private void ParseTable(Hashtable o, WorldObject parent = null, string basePath = "Assets/Dictionaries") {
        LoadDictionary(basePath);
        bool isLeaf = o.ContainsKey("type") ? false : true;

        if (isLeaf) {
            if (parent != null) {
                foreach (DictionaryEntry entry in o) {

                    //Check if key-value exists in a certain dictionary
                    string key = (string)entry.Key;
                    string value = (string)entry.Value;
                    bool allowKeyValuePair = CheckAllowedPair(key, value);

                    if (allowKeyValuePair) {
                        Debug.Log("Key " + key + " Value " + value + " allowed by dictionary (isLeaf)");
                        parent.AddDirectAttribute(key, value);
                    } else {
                        Debug.Log("Key " + key + " Value " + value + " not allowed by dictionary (isLeaf)");
                    }
                }
            } else {
                Debug.Log("Leaf has no parent -------------------");
                foreach(DictionaryEntry entry in o) {
                    Debug.Log("Key: " + (string)entry.Key + " value: " + (string)entry.Value);
                }
                Debug.Log("-----------------");
            }
        } else {    //Not a leaf
            WorldObject newParent = null; //Parent to add children to in next recursion
            bool allowKeyValuePair = false;

            foreach (DictionaryEntry entry in o) {  //Loop over entries in hashtable
                if (!entry.Key.Equals("attr")) {    //Only consider whatever is not attr

                    string key = (string)entry.Key;
                    string value = (string)entry.Value;
                    allowKeyValuePair = allowKeyValuePair || CheckAllowedPair(key, value);
                    if (allowKeyValuePair) {

                        Debug.Log("Key " + key + " Value " + value + " allowed by dictionary (is no leaf)");
                        WorldObject worldObject = new WorldObject(key, value);
                        newParent = worldObject;
                        worldObject.SetParent(parent);
                        if (parent != null) {
                            parent.AddChild(worldObject);   //Add the newly made worldObject as child to its parent
                        } else {
                            rootObject = worldObject;
                            Debug.Log("new worldobject has no parent");
                        }
                    } else {
                        Debug.Log("Key " + key + " Value " + value + " NOT allowed by dictionary (is no leaf)");
                    }
               }
            }

            //Only consider the attr part if the main part was allowed
            if (allowKeyValuePair) {
                //Consider the attr part
                string newBasePath = basePath + "/" + newParent.GetObjectValue();
                SetDefaultValues(newParent, newBasePath);
                if (o.ContainsKey("attr")) {
                    System.Object attr = o["attr"];         //Returns value for key = "attr", is ArrayList with Hashtables
                    ArrayList attrList = (ArrayList)attr;   //Cast to arraylist
                    
                    foreach (Hashtable table in attrList) { //Loop over all hashtables in the arraylist
                        ParseTable(table, newParent, newBasePath);
                    }
                }
            }
        }
    }

    private void SetDefaultValues(WorldObject obj, string basePath) {
        basePath += "/defaults.txt";
        List<string> lines = null;
        lines = GetFileLines(basePath);

        if (lines != null) {
            foreach (string s in lines) {
                int index = s.IndexOf(":");
                if (index == -1) {
                    Debug.Log("Description file is corrupt. No use of ':'. Name: " + basePath);
                } else {
                    string key = s.Substring(0, index);
                    string value = s.Substring(index + 1, s.Length - index - 1);
                    Debug.Log("Key: " + key);
                    Debug.Log("Value: " + value);
                    obj.AddDirectAttribute(key, value);
                }

            }
        }
    }

    private void LoadDictionary(string basePath) {
        dictionary.Clear();
        ConstructDictionary(basePath);
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

    private bool CheckAllowedPair(string key, string value) {
        if (dictionary.ContainsKey(key)) {
            List<string> values = dictionary[key];
            if (values.Contains(value)) {
                return true;   //This key value pair is allowed by the dictionary
            }
        } else if (dictionary.ContainsKey("other")) {   //In the 'other' dict, the value doesn't matter, only the key (e.g. for floors, as to not create a 'floor' dict with values 1, 2, 3, ...
            Debug.Log("\"other\" dict contains key " + key);
            return true;
        }
        return false;
    }

    public string SearchBestFit() {
        if(rootObject != null) {
            return rootObject.SearchBestFit();
        } else {
            Debug.Log("Search best fit: rootobject is null");
            return null;
        }
    }

}
