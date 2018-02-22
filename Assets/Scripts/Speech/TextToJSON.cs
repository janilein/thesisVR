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

    public void CreateDirectionJSON(Entity keyValuePair)
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

    public void CreateStreetJSON(List<Entity> entities, List<Quantity> quantities)
    {
        //Check the type of street
        Entity streetEntity = null;
        foreach(Entity entity in entities)
        {
            if (entity.type.ToLower().Equals("streettype"))
            {
                streetEntity = entity;
                break;
            }
        }
        string streetType = streetEntity.form;
        entities.Remove(streetEntity);

        string length = null;
        foreach (Entity entity in entities)
        {
            if (entity.type.ToLower().Equals("length"))
            {
                length = entity.form;
                entities.Remove(entity);
                break;
            }
        }

        string orientation = null;
        foreach (Entity entity in entities)
        {
            if (entity.type.Equals("TDirection"))
            {
                if(entity.form.ToLower().Equals("left and right"))
                {
                    orientation = "leftRight";
                    entities.Remove(entity);
                    break;
                } else if(entity.form.ToLower().Equals("right and straight"))
                {
                    orientation = "rightStraight";
                    entities.Remove(entity);
                    break;
                } else if(entity.form.ToLower().Equals("left and straight"))
                {
                    orientation = "leftStraight";
                    entities.Remove(entity);
                    break;
                }
                
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

        JObject obj2 = new JObject();
        //Check if we have a length attribute
        if (length != null)
        {
            obj2.Add(new JProperty("length", length));
        }
        if(orientation != null)
        {
            obj2.Add(new JProperty("orientation", orientation));
        }

        List<KeyValuePair<Entity, int>> linkedEntities = new List<KeyValuePair<Entity, int>>();
        foreach (Entity entity in entities)
        {
            if (entity.form.Equals("lot"))
            {
                foreach(Quantity quan in quantities)
                {
                    if(entity.endp == quan.endp)
                    {
                        linkedEntities.Add(new KeyValuePair<Entity, int>(entity, quan.amount));
                    }
                }
            }
        }
        foreach (Entity entity in entities)
        {
            if (entity.form.ToLower().Contains("left"))
            {
                foreach (KeyValuePair<Entity, int> pair in linkedEntities)
                {
                    if (entity.inip - pair.Key.endp == 2)
                    {
                        Debug.Log("Adding lots left -------------------------");
                        obj2.Add(new JProperty("lotsLeft", pair.Value.ToString()));
                    }
                }
            } else if (entity.form.ToLower().Contains("right"))
            {
                foreach (KeyValuePair<Entity, int> pair in linkedEntities)
                {
                    if (entity.inip - pair.Key.endp == 2)
                    {
                        Debug.Log("Adding lots right -------------------------");
                        obj2.Add(new JProperty("lotsRight", pair.Value.ToString()));
                    }
                }
            }
        }

        obj.Add(new JProperty("attr", new JArray(obj2)));

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

    internal void CreateHouseJSON(List<Entity> entityList, List<Quantity> quantityList)
    {
        //JObject rss = new JObject(
        //    new JObject(
        //            new JProperty("type", "lots"),
        //            new JProperty("attr", new JArray(
        //                new JObject(
        //                    new JProperty("lotID", "1")
        //                    ),
        //                new JObject(
        //                        new JProperty("type", "buildings"),
        //                        new JProperty("attr", new JArray(
        //                            new JObject(
        //                                new JProperty("type", "house"),
        //                                new JProperty("attr", new JArray(
        //                                    new JObject(
        //                                        //new JProperty("floors", "5")
        //                                        ),
        //                                    new JObject(
        //                                        new JProperty("type", "floor"),
        //                                        new JProperty("attr", new JArray(
        //                                            new JObject(
        //                                                new JProperty("level", "1"),
        //                                                new JProperty("color", "red")
        //                                                )
        //                                        )
        //                                        )
        //                                    ),
        //                                    new JObject(
        //                                        new JProperty("type", "floor"),
        //                                        new JProperty("attr", new JArray(
        //                                            new JObject(
        //                                                new JProperty("level", "2"),
        //                                                new JProperty("color", "yellow")
        //                                                )
        //                                            ))
        //                                        )
        //                                    )
        //                                )
        //                            ))
        //                            )

        //                ))))
        //);
    }
}
