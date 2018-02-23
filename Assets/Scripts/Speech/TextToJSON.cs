using Newtonsoft.Json.Linq;
using Procurios.Public;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextToJSON
{
    private GeneratorManager manager;
    private HashtableParser hashParser;

    private JObject specifiedDescription = null;

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
        foreach (Entity entity in entities)
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
                if (entity.form.ToLower().Equals("left and right"))
                {
                    orientation = "leftRight";
                    entities.Remove(entity);
                    break;
                }
                else if (entity.form.ToLower().Equals("right and straight"))
                {
                    orientation = "rightStraight";
                    entities.Remove(entity);
                    break;
                }
                else if (entity.form.ToLower().Equals("left and straight"))
                {
                    orientation = "leftStraight";
                    entities.Remove(entity);
                    break;
                }

            }
        }

        string type = null;
        Debug.Log("StreetType: " + streetType);
        switch (streetType)
        {
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
        if (orientation != null)
        {
            obj2.Add(new JProperty("orientation", orientation));
        }

        List<KeyValuePair<Entity, int>> linkedEntities = new List<KeyValuePair<Entity, int>>();
        foreach (Entity entity in entities)
        {
            if (entity.form.Equals("lot"))
            {
                foreach (Quantity quan in quantities)
                {
                    if (entity.endp == quan.endp)
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
            }
            else if (entity.form.ToLower().Contains("right"))
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

    public void CreateHouseJSON(List<Entity> entityList, List<Quantity> quantityList)
    {
        Debug.Log("Specify: " + Speech.specifyDescription);

        //Case: first sentence describing the type of house and number of floors
        if (Speech.specifyDescription == false)
        {
            //------------------------------ Get the type of building ------------------------------
            Entity building = null;
            foreach (Entity entity in entityList)
            {
                if (entity.type.ToLower().Equals("housetype"))
                {
                    building = entity;
                    break;
                }
            }
            if (building == null)
                return;

            entityList.Remove(building);
            string typeOfBuilding = building.form;

            //------------------------------ Get the number of floors ------------------------------
            Quantity floors = null;
            int nbFloors = 0;
            foreach (Quantity quantity in quantityList)
            {
                if (quantity.unit.ToLower().Equals("floors"))
                {
                    floors = quantity;
                }
            }
            if (floors != null)
            {
                nbFloors = floors.amount;
            }

            //------------------------------ Generate the JSON string ------------------------------

            JObject buildingObj = new JObject(
                new JProperty("type", typeOfBuilding)
                );

            if (nbFloors > 0)
            {
                JProperty attr = new JProperty("attr", new JArray(
                    new JObject(
                        new JProperty("floors", nbFloors.ToString())
                        )
                    )
                    );

                buildingObj.Add(attr);
            }

            specifiedDescription = new JObject(
                new JObject(
                    new JProperty("type", "lots"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("lotID", "1")
                            ),
                        new JObject(
                            new JProperty("type", "buildings"),
                            new JProperty("attr", new JArray(
                                buildingObj
                               ))
                            )
                        ))
                    )
                );

            //Set the specify bool to true
            Speech.SetSpecification(true);

        }

        //Other case: specify the house; color for floors or whatever
        else
        {
            if (specifiedDescription == null)
            {
                Debug.Log("Nothing to specify");
                return;
            }

            //Check what we are specifying
            Entity specificationEntity = null;
            JObject specificationObject = null;
            foreach (Entity entity in entityList)
            {
                if (entity.form.ToLower().Equals("roof"))
                {
                    specificationEntity = entity;
                    specificationObject = SpecifyRoof(ref specificationEntity, ref entityList);
                    break;
                }
                else if (entity.form.ToLower().Equals("floor"))
                {
                    specificationEntity = entity;
                    SpecifyFloor(ref specificationEntity, ref entityList);
                    break;
                }
            }

            if (specificationObject == null)
                return;

            //print ff de specified ding
            JProperty type1 = (JProperty)specifiedDescription.First;
            Debug.Log("Type1: " + type1);
            JProperty attr1 = (JProperty)type1.Next;
            Debug.Log("Attr1: " + attr1);

            JArray array1 = (JArray)attr1.Value;
            Debug.Log("Array1: " + array1);

            JToken lotObject = array1.First;
            Debug.Log("lotObject: " + lotObject);

            JObject otherObject = (JObject)lotObject.Next;
            Debug.Log("other object: " + otherObject);

            JProperty typeBuilding = (JProperty)otherObject.First;
            Debug.Log("typeBuilding: " + typeBuilding);

            JProperty otherAttr = (JProperty)typeBuilding.Next;
            Debug.Log("Otherattr: " + otherAttr);

            JArray array2 = (JArray)otherAttr.Value;
            Debug.Log("Array2: " + array2);

            JObject attrObject = (JObject)array2.First;
            Debug.Log("attrObject: " + attrObject);

            JProperty typeHouse = (JProperty)attrObject.First;
            Debug.Log("Typehouse: " + typeHouse);

            JProperty attr3 = (JProperty)typeHouse.Next;
            Debug.Log("attr3: " + attr3);

            JArray array3 = (JArray)attr3.Value;
            Debug.Log("Array3: " + array3);

            Debug.Log("Specified: " + specifiedDescription.ToString());

            Debug.Log("Adding spec entity: " + specificationObject);

            try
            {
                array3.Add(specificationObject);
                Debug.Log("New JSON string: " + specifiedDescription);
            } catch(Exception e)
            {
                Debug.Log("Error adding specification: " + e.Message);
            }
        }



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

    private JObject SpecifyRoof(ref Entity roofEntity, ref List<Entity> entityList)
    {
        entityList.Remove(roofEntity);

        //Check for possible type of roof
        Entity colorEntity = null;
        Entity rooftypeEntity = null;
        foreach (Entity entity in entityList)
        {
            if (entity.type.ToLower().Equals("color"))
            {
                colorEntity = entity;
            }
            else if (entity.type.ToLower().Equals("rooftype"))
            {
                rooftypeEntity = entity;
            }
        }

        if (rooftypeEntity == null && colorEntity == null)
        {
            Debug.Log("No roof specification details given");
            return null;
        }

        JObject specifiedRoof = new JObject();

        if (rooftypeEntity != null)
        {
            specifiedRoof.Add(new JProperty("roofType", rooftypeEntity.form));
        }
        if (colorEntity != null)
        {
            specifiedRoof.Add(new JProperty("color", colorEntity.form));
        }

        //Create the roof JObject
        JObject roofObject = new JObject(
            new JProperty("type", "roof"),
            new JProperty("attr", new JArray(
                specifiedRoof
                ))
            );

        return roofObject;
    }

    private void SpecifyFloor(ref Entity floorEntity, ref List<Entity> entityList)
    {

    }


    //From https://stackoverflow.com/questions/11278081/convert-words-string-to-int

    static int ParseEnglish(string number)
    {
        string[] words = number.ToLower().Split(new char[] { ' ', '-', ',' }, StringSplitOptions.RemoveEmptyEntries);
        string[] ones = { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        string[] teens = { "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
        string[] tens = { "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };
        Dictionary<string, int> modifiers = new Dictionary<string, int>() {
        {"billion", 1000000000},
        {"million", 1000000},
        {"thousand", 1000},
        {"hundred", 100}
    };

        if (number == "eleventy billion")
            return int.MaxValue; // 110,000,000,000 is out of range for an int!

        int result = 0;
        int currentResult = 0;
        int lastModifier = 1;

        foreach (string word in words)
        {
            if (modifiers.ContainsKey(word))
            {
                lastModifier *= modifiers[word];
            }
            else
            {
                int n;

                if (lastModifier > 1)
                {
                    result += currentResult * lastModifier;
                    lastModifier = 1;
                    currentResult = 0;
                }

                if ((n = Array.IndexOf(ones, word) + 1) > 0)
                {
                    currentResult += n;
                }
                else if ((n = Array.IndexOf(teens, word) + 1) > 0)
                {
                    currentResult += n + 10;
                }
                else if ((n = Array.IndexOf(tens, word) + 1) > 0)
                {
                    currentResult += n * 10;
                }
                else if (word != "and")
                {
                    throw new ApplicationException("Unrecognized word: " + word);
                }
            }
        }

        return result + currentResult * lastModifier;
    }

    internal void disabledSpecifyDescription()
    {
        if (specifiedDescription != null)
        {
            Debug.Log(specifiedDescription.ToString());
            bool successParse = true;
            Hashtable o = (Hashtable)JSON.JsonDecode(specifiedDescription.ToString(), ref successParse);
            hashParser.PrintHashTable(o);     //Convert the hashtable to WorldObjects
            WorldObject root = hashParser.getRootObject();
            manager.GenerateWorldObject(root);

            specifiedDescription = null;
        }
        else
        {
            Debug.Log("Specified description null");
            return;
        }
    }
}