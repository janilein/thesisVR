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
}
