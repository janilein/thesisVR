using Procurios.Public;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldObject{

    private string objectType;
    private string objectValue;
    private List<WorldObject> children;     //Children with children
    public Hashtable directAttributes { get; set; }     //Children without children
    //private Hashtable defaultValues { get; set; }       //default values read in from dictionary file
    private WorldObject parent;

    public WorldObject(string objectType = null, string objectValue = null) {
        this.objectType = objectType;
        this.objectValue = objectValue;
        this.children = new List<WorldObject>();
        this.directAttributes = new Hashtable();
    }

    public void SetObjectValue(string value) {
        this.objectValue = value;
    }

    public string GetObjectValue() {
        return this.objectValue;
    }

    public void SetChildren(List<WorldObject> children) {
        this.children = children;
    }

    public List<WorldObject> GetChildren() {
        return this.children;
    }

    public void AddDirectAttribute(string key, string value) {
        this.directAttributes[key] = value;
    }

    public void AddChild(WorldObject child) {
        this.children.Add(child);
    }

    public string GetObjectType() {
        return this.objectType;
    }

    public void SetParent(WorldObject parent) {
        this.parent = parent;
    }

    public WorldObject GetParent() {
        return this.parent;
    }

    public string SearchBestFit() {
        string path = SearchCompatibleRoots();
        Debug.Log("Compatible roots: " + path);
        List<string> bestFit = null;
        List<string> bestFitRelative = new List<string>();
        Hashtable poss = null;
        if (path != null) {
            poss = SearchTopTypeInRoot(path);
            if(poss != null) {
                WorldObject child = children[0];
                bestFit = child.SearchBestFitFromPossibilities(poss);
                if (bestFit != null) {
                    Debug.Log("Best fit not null");
                    foreach (string s in bestFit) {
                        Debug.Log("Best fit possibility: " + s);
                        Debug.Log("Datapath: " + Application.dataPath);
                        bestFitRelative.Add(GetRelativePath(s));
                    }
                } else {
                    Debug.Log("Best fit null");
                    return null;
                }
            } else {
                Debug.Log("No top type possibilities found.");
                return null;
            }
        } else {
            Debug.Log("No compatible root found.");
            return null;
        }
        return bestFitRelative[0];  //For now
    }

    private string GetRelativePath(string s) {
        string absolutePath = Application.dataPath; //To assets folder
        int absolutePathLength = absolutePath.Length;
        //The "/Resources/" has to be added
        absolutePathLength += "/Resources/".Length;
        //Debug.Log("s length: " + s.Length + " abs length: " + absolutePathLength);
        string relativePath = s.Substring(absolutePathLength, s.Length - absolutePathLength - 1 - 3);
        //Debug.Log("Relativepath: " + relativePath);
        return relativePath;
    }

    //First filter, based on the main category (buildings, roads, ...). This is the very top element (root) in WorldObject
    private string SearchCompatibleRoots() {
        string path = null;

        //search top for possible matches
        //DirectoryInfo dir = new DirectoryInfo("Assets/Resources");
        string startPath = "Assets\\Resources";
        string[] dirs = Directory.GetDirectories(startPath);
        //FileInfo[] info = dir.GetFiles("*.*");

        //Debug.Log("dirs -------------------");
        foreach(string s in dirs) {
            //Get actual folder name, without path to it
            string folder = s.Substring(startPath.Length + 1, s.Length - startPath.Length - 1);
            if (folder.ToLower().Equals(this.GetObjectValue().ToLower())) {
                path = s;
            }
        }
        return path;
    }

    //Second filter, checks the type of the child of the root. This is e.g. 'house', 'appartment', ...
    private Hashtable SearchTopTypeInRoot(string path) {

        WorldObject child = null;
        try {
            child = children[0];
        } catch(IndexOutOfRangeException e) {
            Debug.Log(e.Message);
            return null;
        }
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.txt");

        List<string> possibilities = new List<string>(); //filenames of possible top type matches

        foreach (FileInfo f in info) {
            //Debug.Log("f name: " + f.Name);
            //Debug.Log("f fullname: " + f.FullName);
            //Debug.Log("Txt file: " + f.Name);
            string[] lines = File.ReadAllLines(f.FullName);
            string firstLine = lines[1];
            int index = firstLine.IndexOf(":");
            if (index == -1) {
                Debug.Log("Description file is corrupt. No use of ':'. Name: " + f.FullName);
            }
            string type = "\"type\"";
            try {
                string typeLine = firstLine.Substring(index - 6, 6);
                //Debug.Log("typeLine: " + typeLine);
                if (typeLine.Equals(type)) {
                    string childValue = "\"" + child.GetObjectValue() + "\"";
                    //Debug.Log("Root child type value: " + childValue);
                    string typeValue = firstLine.Substring(index + 1, firstLine.Length - index - 2);
                    //Debug.Log("typeValue: " + typeValue);
                    if (typeValue.Equals(childValue)) { 
                        possibilities.Add(f.FullName);
                    }
                } else {
                    Debug.Log("Description file is corrupt. Not started with type. Name: " + f.FullName);
                }
            } catch (Exception e) {
                Debug.Log(e.Message);
            }
        }
        Hashtable pathJSON = new Hashtable();
        foreach (string s in possibilities) {
            Debug.Log("Possibility: " + s);
            string text = File.ReadAllText(s);
            bool success = false;
            Debug.Log("Pllll");
            Hashtable o = (Hashtable)JSON.JsonDecode(text, ref success);
            if (!success) {
                Debug.Log("Parse failed");
            } else {
                List<string> list = new List<string>();
                list.Add(s);
                pathJSON.Add(list, o);
            }
        }
        Debug.Log("SearchTopTypeInRoot returned " + pathJSON.Count + "possibilities");
        return pathJSON;
    }

    //Iterative step. Based on your own leafs, filter using the standalone variables. Then pass to children
    public List<string> SearchBestFitFromPossibilities(Hashtable poss){
        Debug.Log("Search best fit type: " + this.GetObjectType());
        Debug.Log("Search best fit value: " + this.GetObjectValue());

        List<DictionaryEntry> entriesToRemove = new List<DictionaryEntry>();

        foreach (DictionaryEntry possJSON in poss) {
            bool shnioké = false;
            Hashtable jsonString = (Hashtable) possJSON.Value;
            ArrayList standaloneList = (ArrayList)(jsonString["standalone"]);
            foreach (Hashtable standaloneTable in standaloneList) {
                if (standaloneTable != null) {
                    foreach (DictionaryEntry entry in standaloneTable) {
                        Debug.Log("Entry key: " + entry.Key + " value: " + entry.Value);
                        if (directAttributes.Contains(entry.Key)) {
                            Debug.Log("DirectAttributes contains key " + entry.Key);
                            string entryValue = (string)entry.Value;
                            string myValue = (string)directAttributes[entry.Key];
                            if (!entryValue.Equals(myValue)) {
                                Debug.Log("Shnioke");
                                Debug.Log("My value: " + myValue);
                                Debug.Log("Entry value: " + entryValue);
                                shnioké = true;
                                break;
                            }
                        } else {
                            Debug.Log("DirectAttributes does NOT contain key " + entry.Key);
                        }
                    }
                } else {
                    Debug.Log("Standalone table null");
                }
            }

            if (shnioké) {
                entriesToRemove.Add(possJSON);
            }
        }
        Debug.Log("Entries to remove: " + entriesToRemove.Count);
        foreach(DictionaryEntry entryToRemove in entriesToRemove) {
            poss.Remove(entryToRemove.Key);
        }

        if(poss.Count == 0) {
            Debug.Log("Poss count 0");
            return null;

        } else {

            if (HasChildren()) {
                Debug.Log("has children");

                Debug.Log("Poss count: " + poss.Count);

                //Create new poss list
                Hashtable newPoss = new Hashtable();
                List<string> pathList;
                foreach (DictionaryEntry entry in poss) {
                    Debug.Log("Entry key: " + entry.Key);
                    Debug.Log("Entry Value: " + entry.Value);
                    ArrayList attr = (ArrayList)(((Hashtable)entry.Value)["attr"]);
                    if (attr != null) {
                        foreach (Hashtable table in attr) {
                            pathList = new List<string>();

                            pathList.Add((string)((List<string>)entry.Key)[0]);
                            newPoss.Add(pathList, table);
                        }
                    } else {
                        Debug.Log("Attr is null");
                    }

                }
                //Print the newly made table
                Debug.Log("New poss ---------------");
                foreach (DictionaryEntry entry in newPoss) {
                    Debug.Log("Key: " + (((List<string>)entry.Key)[0]) + " Value: " + entry.Value);
                }

                //For all children, try it with all attrs
                List<string> possibilities = new List<string>();
                List<string> result;
                foreach (WorldObject child in GetChildren()) {
                    result = child.SearchBestFitFromPossibilities(newPoss);
                    foreach(string s in result) {
                        possibilities.Add(s);
                    }
                    return possibilities;
                }
            } else {
                Debug.Log("has no children");
                List<string> possibilities = new List<string>();
                List<string> result;
                foreach(DictionaryEntry entry in poss) {
                    result = (List<string>)entry.Key;
                    Debug.Log("Adding to result: " + result[0]);
                    possibilities.Add(result[0]);
                } 
                return possibilities;
            }
        }
        return null;
    }

    private string[] RuleOutFromDirectAttributes(string[] lines) {
        bool child = false;
        List<string> childStr = null;
        List<List<string>> children = null;
        foreach (string s in lines) {
            if (child) {

            } else {
                if (s.Contains("{")) {
                    child = true;
                } 
            }
        }
        return null;
    }

    public string SearchCompatibleGameObject() {
        bool random = false;
        //HashtableParser hashParser = new HashtableParser();
        string path = "Assets/Resources";
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.*");
        //Debug.Log(info.Length);
        List<string> possibilities = new List<string>();
        //List<Hashtable> tables = new List<Hashtable>();
        foreach (FileInfo f in info) {
            //Debug.Log(f.Name.Substring(0, f.Name.Length - 5).ToLower());
            //Debug.Log(this.GetObjectValue());
            if (f.Name.Substring(0, f.Name.Length - 5).ToLower().Equals(this.GetObjectValue())) {
                DirectoryInfo specifiedDir = new DirectoryInfo(path + "/" + this.GetObjectValue());
                FileInfo[] specifiedInfo = specifiedDir.GetFiles("*.txt");
                //Debug.Log(specifiedInfo.Length);
                foreach (FileInfo s in specifiedInfo) {
                    string[] lines = File.ReadAllLines(s.FullName);
                    //List<string> lines = hashParser.GetFileLines(s.FullName);
                    Hashtable hashtable = new Hashtable();
                    foreach (string l in lines) {
                        int index = l.IndexOf(":");
                        hashtable.Add(l.Substring(0, index), l.Substring(index + 1, l.Length - index - 1));
                        //Debug.Log(l.Substring(0, index) + ":" + hashtable[l.Substring(0, index)]);
                    }
                    if (hashtable["type"].Equals(children[0].GetObjectValue())){
                        
                        if (hashtable["kleur"].Equals(children[0].GetChildren()[0].GetObjectValue())){
                            possibilities.Add(s.Name.Substring(0, s.Name.Length - 4));
                        }
                    }
                }
                if(possibilities.Count > 1) {
                    random = true;
                }
            }
        }
        Debug.Log("aantal opties: " + possibilities.Count + " /random: " + random);
        if (random) {
            System.Random r = new System.Random();
            int rInt = r.Next(0, possibilities.Count);
            return this.GetObjectValue() + "/" + possibilities[rInt];
            //GameObject instance = Instantiate(Resources.Load(path + "/" + this.GetObjectValue() + "/" + possibilities[0], typeof(GameObject)), new Vector3(0,0,0), Quaternion.identity) as GameObject;
        }
        return this.GetObjectValue() + "/" + possibilities[0];
    }

    public void PrintObjectInfo() {
        Debug.Log("I am a " + "\"" + this.GetObjectType() + "\"" + " and my value is " + this.GetObjectValue());
        string value;
        if (this.GetParent() != null) {
            value = this.GetParent().GetObjectValue();
        } else {
            value = "null";
        }
        Debug.Log('\t' + "My parent is " + value);
        Debug.Log('\t' + "I have " + directAttributes.Count + " leafs");
        foreach (DictionaryEntry entry in directAttributes) {
            Debug.Log('\t' + "Key:" + entry.Key + " Value: " + entry.Value);
        }
        Debug.Log('\t' + "I have " + children.Count + " children");
        foreach (WorldObject child in children) {
            child.PrintObjectInfo();
        }
    }

    private bool HasChildren() {
        return this.children.Count != 0;
    }
}
