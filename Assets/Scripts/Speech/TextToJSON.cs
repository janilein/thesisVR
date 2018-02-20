using Newtonsoft.Json.Linq;
using Procurios.Public;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToJSON {
    private GeneratorManager manager;
    private HashtableParser hashParser;

    public TextToJSON()
    {
        manager = new GeneratorManager();
        hashParser = new HashtableParser();
    }

    public void CreateDirectionJSON(KeyValuePair<string, string> keyValuePair)
    {
        JObject rss = new JObject(
            new JObject(
                    new JProperty("type", "orientation"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty(keyValuePair.Key.ToLower(), keyValuePair.Value))))));
        Debug.Log(rss.ToString());
        bool successParse = true;
        Hashtable o = (Hashtable)JSON.JsonDecode(rss.ToString(), ref successParse);
        hashParser.PrintHashTable(o);     //Convert the hashtable to WorldObjects
        WorldObject root = hashParser.getRootObject();
        manager.GenerateWorldObject(root);
    }

    public void CreateStreetJSON(List<KeyValuePair<string, string>> list)
    {
        //Check the type of street
        string streetType = list[0].Value.ToLower();
        string type = null;
        switch(streetType){
            case "straight street":
                type = "straight";
                break;
            case "three-way intersection":
                type = "intersection-t";
                break;
            case "four-way intersection":
                type = "intersection-x";
                break;
            default:
                type = "straight";
                break;
        }

        JObject obj = new JObject();
        obj.Add(new JProperty("type", type));

        //JObject rss = new JObject(
        //    new JObject(
        //            new JProperty("type", "streets"),
        //            new JProperty("attr", new JArray(
        //                new JObject(
        //                    new JProperty("streetID", "1")),
        //                new JObject(
        //                    new JProperty("type", type),
        //                    new JProperty("attr", new JArray(
        //                        new JObject(
        //                            new JProperty("length", "large")
        //                            ))))))));

        JObject rss = new JObject(
            new JObject(
                    new JProperty("type", "streets"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("streetID", "1")),
                        obj
                        ))));

        //Check if we have a length attribute
        if (list.Count > 1)
        {
            KeyValuePair<string, string> length = list[1];
            Debug.Log("Key: " + length.Key + " Value: " + length.Value);

            if (length.Key.ToLower().Equals("length"))
            {
                obj.Add(new JProperty("attr", new JArray(new JObject(new JProperty("length", length.Value.ToLower())))));
            }
        }

        

        Debug.Log(rss.ToString());
        bool successParse = true;
        Hashtable o = (Hashtable)JSON.JsonDecode(rss.ToString(), ref successParse);
        hashParser.PrintHashTable(o);     //Convert the hashtable to WorldObjects
        WorldObject root = hashParser.getRootObject();
        manager.GenerateWorldObject(root);
    }
}
