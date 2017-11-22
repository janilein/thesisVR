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
    private Hashtable directAttributes { get; set; }     //Children without children
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
        this.directAttributes.Add(key, value);
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
        string bestFit = null;
        Hashtable poss = null;
        if (path != null) {
            poss = SearchTopTypeInRoot(path);
            if(poss != null) {
                WorldObject child = children[0];
                bestFit = child.SearchBestFitFromPossibilities(poss);
            } else {
                Debug.Log("No top type possibilities found.");
            }
        } else {
            Debug.Log("No compatible root found.");
        }
        return bestFit;
    }

    private string SearchCompatibleRoots() {
        string path = null;

        //search top for possible matches
        DirectoryInfo dir = new DirectoryInfo("Assets/Resources");
        FileInfo[] info = dir.GetFiles("*.*");

        foreach (FileInfo f in info) {
            if (f.Name.Substring(0, f.Name.Length - 5).ToLower().Equals(this.GetObjectValue())) {
                path = f.FullName;
                break;
            }
        }
        return path;
    }

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
            string[] lines = File.ReadAllLines(f.FullName);
            string firstLine = lines[1];
            int index = firstLine.IndexOf(":");
            if (index == -1) {
                Debug.Log("Description file is corrupt. No use of ':'.");
                return null;
            }
            if (firstLine.Substring(0, index).Equals("type")) {
                string childValue = "\"" + child.GetObjectValue() + "\"";
                Debug.Log("Root child type value: " + childValue);
                if (firstLine.Substring(index + 1, firstLine.Length - index - 1).Equals(childValue)) {
                    possibilities.Add(f.FullName);
                }
            } else {
                Debug.Log("Description file is corrupt. Not started with type.");
                return null;
            }
        }

        Hashtable pathJSON = new Hashtable();
        foreach (string s in possibilities) {
            string text = File.ReadAllText(s);
            bool success = false;
            Hashtable o = (Hashtable)JSON.JsonDecode(text, ref success);
            if (!success) {
                Debug.Log("Parse failed");
            } else {
                List<string> list = new List<string>();
                list.Add(s);
                pathJSON.Add(list, o);
            }
        }

        return pathJSON;
    }

    private string SearchBestFitFromPossibilities(Hashtable poss){

        List<DictionaryEntry> entriesToRemove = new List<DictionaryEntry>();

        foreach (DictionaryEntry possJSON in poss) {
            bool shnioké = false;
            Hashtable jsonString = (Hashtable) possJSON.Value;
            Hashtable standaloneTable = (Hashtable)jsonString["standalone"];

            if (standaloneTable != null) {
                foreach (DictionaryEntry entry in standaloneTable) {
                    Debug.Log("Entry key: " + entry.Key + " value: " + entry.Value);
                    if (directAttributes.Contains(entry.Key)) {
                        string entryValue = (string)entry.Value;
                        string myValue = (string)directAttributes[entry.Key];
                        if (!entryValue.Equals(myValue)) {
                            shnioké = true;
                            break;
                        }
                    }
                }
            } else {
                Debug.Log("Standalone table null");
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
        }

        if (HasChildren()) {
            Debug.Log("has children");
        } else {
            Debug.Log("has no children");
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
