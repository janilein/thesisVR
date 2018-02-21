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

    public void CreateDirectionJSON(Concept keyValuePair)
    {
        JObject rss = new JObject(
            new JObject(
                    new JProperty("type", "orientation"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty(keyValuePair.type.ToLower(), keyValuePair.form))))));
        Debug.Log(rss.ToString());
        bool successParse = true;
        Hashtable o = (Hashtable)JSON.JsonDecode(rss.ToString(), ref successParse);
        hashParser.PrintHashTable(o);     //Convert the hashtable to WorldObjects
        WorldObject root = hashParser.getRootObject();
        manager.GenerateWorldObject(root);
    }

    public void CreateStreetJSON(List<Concept> concepts, List<Quantity> quantities)
    {
        //Check the type of street
        Concept streetConcept = null;
        foreach(Concept concept in concepts)
        {
            if (concept.type.ToLower().Equals("streettype"))
            {
                streetConcept = concept;
                break;
            }
        }
        string streetType = streetConcept.form;
        concepts.Remove(streetConcept);

        string length = null;
        foreach (Concept concept in concepts)
        {
            if (concept.type.ToLower().Equals("length"))
            {
                length = concept.form;
                concepts.Remove(concept);
                break;
            }
        }

        string type = null;
        Debug.Log("StreetType: " + streetType);
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

        //Check if we have a length attribute
        if (length != null)
        {
           obj.Add(new JProperty("attr", new JArray(new JObject(new JProperty("length",length)))));
        }

        Debug.Log("$$$$$$$$$$$$$$$$$$" + concepts.Count);
        foreach (Concept concept in concepts)
        {
            //JProperty property = FindBestQuantityMatch(concept, ref quantities);
            Debug.Log("$$$$$$$$$$$$$$$$$$$$$$$$$$$$$" + concept.form);
        }

        JObject rss = new JObject(
            new JObject(
                    new JProperty("type", "streets"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("streetID", "1")),
                        obj
                        ))));

        Debug.Log(rss.ToString());
        bool successParse = true;
        Hashtable o = (Hashtable)JSON.JsonDecode(rss.ToString(), ref successParse);
        hashParser.PrintHashTable(o);     //Convert the hashtable to WorldObjects
        WorldObject root = hashParser.getRootObject();
        manager.GenerateWorldObject(root);
    }

    private JProperty FindBestQuantityMatch(Concept concept, ref List<Quantity> quantities)
    {
        foreach(Quantity quantity in quantities)
        {
            if(quantity.inip - concept.endp == 1)
            {

            }
        }

        return null;
    }
}
