using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WorldObject{

    private string objectType;
    private string objectValue;
    private int ID;
    private List<WorldObject> children;
    private WorldObject parent;
    private int parentID;
    private bool isLeaf;

    public WorldObject(string objectType = null, string objectValue = null, int ID = -1, bool isLeaf = false, WorldObject parent = null, int parentID = -1) {
        this.objectType = objectType;
        this.objectValue = objectValue;
        this.ID = ID;
        this.isLeaf = isLeaf;
        this.children = new List<WorldObject>();
        this.parent = parent;
        this.parentID = parentID;
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

    public int GetID() {
        return this.ID;
    }

    public int GetParentID() {
        return this.parentID;
    }

    public string SearchCompatibleGameObject() {
        bool random = false;
        HashtableParser hashParser = new HashtableParser();
        string path = "Assets/Resources";
        DirectoryInfo dir = new DirectoryInfo(path);
        FileInfo[] info = dir.GetFiles("*.*");
        Debug.Log(info.Length);
        List<string> possibilities = new List<string>();
        List<Hashtable> tables = new List<Hashtable>();
        foreach (FileInfo f in info) {
            Debug.Log(f.Name.Substring(0, f.Name.Length - 5).ToLower());
            Debug.Log(this.GetObjectValue());
            if (f.Name.Substring(0, f.Name.Length - 5).ToLower().Equals(this.GetObjectValue())) {
                DirectoryInfo specifiedDir = new DirectoryInfo(path + "/" + this.GetObjectValue());
                FileInfo[] specifiedInfo = specifiedDir.GetFiles("*.txt");
                Debug.Log(specifiedInfo.Length);
                foreach (FileInfo s in specifiedInfo) {
                    string[] lines = File.ReadAllLines(s.FullName);
                    //List<string> lines = hashParser.GetFileLines(s.FullName);
                    Hashtable hashtable = new Hashtable();
                    foreach (string l in lines) {
                        int index = l.IndexOf(":");
                        hashtable.Add(l.Substring(0, index), l.Substring(index + 1, l.Length - index - 1));
                        Debug.Log(l.Substring(0, index) + ":" + hashtable[l.Substring(0, index)]);
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

    public void PrintWorldObject() {
        Debug.Log("I am a " + "\"" + this.GetObjectType() + "\"" + " and my value is " + this.GetObjectValue());
        Debug.Log('\t' + "My ID is " + this.GetID() + ", my parent ID is " + this.GetParentID() + " and I have " + this.GetChildren().Count + " child(ren)");
        /*
        if (this.GetParent() == null)
            Console.WriteLine('\t' + "My parent object is null");
        else Console.WriteLine('\t' + "My parent object is not null");
        */

        foreach (WorldObject obj in this.GetChildren()) {
            obj.PrintWorldObject();
        }
    }
}
