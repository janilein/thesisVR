﻿using Newtonsoft.Json.Linq;
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

        //Save string to SaveManager
        SaveManager.AddJSON(rss.ToString());
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
            case "turn":
                type = "turn";
                break;
            default:
                type = "straight";
                break;
        }

        //In case of a straight street, check for the length as well
        string length = null;
        if (type.Equals("straight"))
        {
            foreach (Entity entity in entities)
            {
                if (entity.type.ToLower().Equals("length"))
                {
                    length = entity.form;
                    break;
                }
            }
        }

        //In case of an intersection-t, check for the orientation
        string orientation = null;
        if (type.Equals("intersection-t"))
        {
            foreach (Entity entity in entities)
            {
                if (entity.type.Equals("TDirection"))
                {
                    if (entity.form.ToLower().Equals("left and right"))
                    {
                        orientation = "leftRight";
                        break;
                    }
                    else if (entity.form.ToLower().Equals("right and straight"))
                    {
                        orientation = "rightStraight";
                        break;
                    }
                    else if (entity.form.ToLower().Equals("left and straight"))
                    {
                        orientation = "leftStraight";
                        break;
                    }

                }
            }
        }

        //In case of a turn, check for the direction & if it's sharp of slight
        string direction = null;
        int degrees = 0;
        if (type.Equals("turn"))
        {
            foreach (Entity entity in entities)
            {
                if (entity.type.Equals("Direction"))
                {
                    direction = entity.form;
                    break;
                }
            }
            foreach(Entity entity in entities)
            {
                if (entity.type.Equals("TurnAmount")){
                    switch (entity.form.ToLower())
                    {
                        case "slight":
                            degrees = 45;
                            break;
                        case "sharp":
                            degrees = 90;
                            break;
                    }
                    break;
                }
            }
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
        if(direction != null)
        {
            obj2.Add(new JProperty("direction", direction));
        }
        if(degrees != 0)
        {
            obj2.Add(new JProperty("angle", degrees.ToString()));
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
            if (entity.form.ToLower().Equals("on the left"))
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
            else if (entity.form.ToLower().Equals("on the right"))
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

        JObject rss =
            new JObject(
                    new JProperty("type", "streets"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("streetID", "1")),
                        obj
                        )));

        Debug.Log(rss.ToString());
        bool successParse = true;
        Hashtable o = (Hashtable)JSON.JsonDecode(rss.ToString(), ref successParse);
        hashParser.PrintHashTable(o);     //Convert the hashtable to WorldObjects
        WorldObject root = hashParser.getRootObject();
        manager.GenerateWorldObject(root);

        //Save string to SaveManager
        SaveManager.AddJSON(rss.ToString());
    }

    public void CreateHouseJSON(List<Entity> entityList, List<Quantity> quantityList, List<Relation> relationList)
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

            //------------------------------ Get the specification for 'all floors color'  ------------------------------
            Entity allFloors = null;
            Entity color = null;
            foreach(Entity entity in entityList)
            {
                if (entity.type.ToLower().Equals("color"))
                    color = entity;
                else if (entity.form.ToLower().Equals("all floors"))
                    allFloors = entity;
            }


            //------------------------------ Generate the JSON string ------------------------------

            JObject buildingObj = new JObject(
                new JProperty("type", typeOfBuilding)
                );

            List<JProperty> properties = new List<JProperty>();
            if(nbFloors > 0)
            {
                properties.Add(new JProperty("floors", nbFloors.ToString()));
            }
            if(color != null && allFloors != null)
            {
                properties.Add(new JProperty("color", color.form.ToLower().ToString()));
            }

            if (properties.Count > 0)   //Do we have a property to add?
            {
                JObject obj = new JObject();
                foreach(JProperty prop in properties)
                {
                    obj.Add(prop);
                }

                JProperty attr = new JProperty("attr", new JArray(
                    obj
                    )
                    );

                buildingObj.Add(attr);
            }

            //If both 'all floors' and a color are specified, add them as well

            specifiedDescription =
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
                        )
                    )
                );

            GenerateWorldObject(specifiedDescription.ToString());

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
            foreach (Entity entity in entityList)
            {
                if (entity.form.ToLower().Equals("roof"))   //Specify roof?
                {
                    specificationEntity = entity;
                    SpecifyRoof(ref specificationEntity, ref entityList);
                    break;
                }
                else if (entity.form.ToLower().Equals("floor")) //Specify floor?
                {
                    specificationEntity = entity;
                    SpecifyFloor(ref specificationEntity, ref entityList, ref relationList);
                    break;
                }
                else if (entity.form.ToLower().Equals("floors"))    //Respecify # floors?
                {
                    foreach(Quantity quantity in quantityList)
                    {
                        if (quantity.unit.ToLower().Equals("floors"))
                        {
                            SpecifyNumberOfFloors(quantity);
                            break;
                        }
                    }
                    break;
                } else if (entity.form.ToLower().Equals("all floors"))  //Set color for all floors?
                {
                    SpecifyFloorColors(ref entityList);
                }
            }

            Debug.Log("New JSON string: " + specifiedDescription);

            GenerateWorldObject(specifiedDescription.ToString());
        }


        //Voor floor specification:
        //Wat wel werkt: "floor 2 is green"
        //Wat niet werkt: "floor number 2 is green"



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

    private void SpecifyFloorColors(ref List<Entity> entityList) {
        Entity colorEntity = null;
        foreach(Entity entity in entityList)
        {
            if (entity.type.ToLower().Equals("color"))
            {
                colorEntity = entity;
                break;
            }
        }

        //Is floor color given?
        if(colorEntity == null)
        {
            Debug.Log("No floor color given");
            return;
        }

        //Add all floor color to floors

        //Als de first key net type is, dan weet ge dat er al een specification is toegevoegd

        JArray array3 = GetAttrArray();
        bool addedFloorsColor = false;

        foreach(JObject jsonObject in array3)
        {
            JProperty first = (JProperty)jsonObject.First;
            string key = (string)first.Name;

            if (key.Equals("type"))
                continue;
            else
            {
                //Dit is het JObject waarin ge de specification wilt toevoegen
                addedFloorsColor = true;
                jsonObject.Remove("color");
                jsonObject.Add("color", colorEntity.form.ToLower());
            }
        }

        if (!addedFloorsColor)
        {
            JObject floorObject = new JObject();
            floorObject.Add(new JProperty("color", colorEntity.form.ToLower()));
            array3.Add(floorObject);
            Debug.Log("Added new floorcolorobject");
        }

    }

    private void SpecifyRoof(ref Entity roofEntity, ref List<Entity> entityList)
    {
        //entityList.Remove(roofEntity);

        //Check for possible type of roof & color
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
            return;
        }

        //Did we already add a roof object to this JSON string?
        JArray array3 = GetAttrArray();

        JObject roofAttr = null;
        foreach (JObject jsonObject in array3)
        {
            JProperty first = (JProperty)jsonObject.First;
            //Debug.Log("First: " + first);
            if (((string)first.Value).Equals("roof"))
            {
                JProperty roofAttrs = (JProperty)first.Next;
                //Debug.Log("Roof attrs: " + roofAttrs);

                JArray roofAttrArray = (JArray)roofAttrs.Value;
                roofAttr = (JObject)roofAttrArray.First;
            }
        }

        Debug.Log("roofObject: " + roofAttr);

        //If we already have a roofObject, add stuff to that, otherwise create new roofObject and add stuff to that
        if (roofAttr != null)
        {
            if (rooftypeEntity != null)
            {
                //First make sure that there is no rooftype jproperty in the array
                roofAttr.Remove("roofType");
                roofAttr.Add("roofType", rooftypeEntity.form);
            }
            if (colorEntity != null)
            {
                //First make sure that there is no color jproperty in the array
                roofAttr.Remove("color");
                roofAttr.Add("color", colorEntity.form);
            }
        }

        else
        {
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

            array3.Add(roofObject);
        }

        return;
    }

    private void SpecifyNumberOfFloors(Quantity quantity)
    {
        //entityList.Remove(floorEntity);
        int nbFloors = quantity.amount;

        JArray array3 = GetAttrArray();
        bool addedFloors = false;

        foreach (JObject jsonObject in array3)
        {
            JProperty first = (JProperty)jsonObject.First;
            string key = (string)first.Name;

            if (key.Equals("type"))
                continue;
            else
            {
                //Dit is het JObject waarin ge de specification wilt toevoegen
                addedFloors = true;
                jsonObject.Remove("floors");
                jsonObject.Add("floors", nbFloors.ToString());
            }
        }

        if (!addedFloors)
        {
            JObject floorObject = new JObject();
            floorObject.Add(new JProperty("floors", nbFloors.ToString()));
            array3.Add(floorObject);
            Debug.Log("Added new floorobject");
        }

    }

    private void SpecifyFloor(ref Entity floorEntity, ref List<Entity> entityList, ref List<Relation> relationList)
    {
        //entityList.Remove(floorEntity);

        //Check for floor color
        Entity colorEntity = null;
        foreach (Entity entity in entityList)
        {
            if (entity.type.ToLower().Equals("color"))
            {
                colorEntity = entity;
                break;
            }
        }

        if (colorEntity == null)
        {
            throw new Exception("No floor specification given");
        }

        //Get the number of the floor
        int level = 0;

        foreach (Relation relation in relationList)
        {
            if (relation.form.Equals("floor"))
            {
                level = relation.Amount;
                break;

            }
        }

        if (level == 0)
            throw new Exception("No floor number given");

        // Debug.Log("Level to string: " + level.ToString());

        //Do we already have a specification for this floor level?
        JArray array3 = GetAttrArray();
        bool addedFloor = false;

        foreach (JObject jsonObject in array3)
        {
            JProperty first = (JProperty)jsonObject.First;
            //Debug.Log("First: " + first);
            if (((string)first.Value).Equals("floor"))
            {
                JProperty roofAttrs = (JProperty)first.Next;
                //Debug.Log("Floor attrs: " + roofAttrs);

                JArray roofAttrArray = (JArray)roofAttrs.Value;
                JObject floorAttr = (JObject)roofAttrArray.First;

                //Check the level JPropterty in this floorAttr
                JProperty levelAttr = (JProperty)floorAttr.First;
                //Debug.Log("LevelAttr: " + levelAttr);
                while(levelAttr != null)
                {
                    string value = (string)levelAttr.Value;
                    //Debug.Log("LevelAttr: " + levelAttr);
                    //Debug.Log("LevelAttr name: " + (string)levelAttr.Name);
                    //Debug.Log("LevelAttr value: " + value);
                    if (value.Equals(level.ToString()))
                        break;
                    else
                        levelAttr = (JProperty) levelAttr.Next;
                }

                if (levelAttr != null)
                {
                    //Debug.Log("In if...");
                    //Debug.Log("Value: " + levelAttr.Value);
                    //Debug.Log("LEvel to string: " + level.ToString());
                    if (((string)levelAttr.Value).Equals(level.ToString()))
                    {
                        //Debug.Log("Removing color...");
                        floorAttr.Remove("color");
                        floorAttr.Add("color", colorEntity.form);
                        addedFloor = true;
                        break;
                    }
                }
                else
                {
                    Debug.Log("Level attr is null");
                }
            }
            }

            if (!addedFloor)
            {
                JObject specifiedFloor = new JObject();

                specifiedFloor.Add(new JProperty("color", colorEntity.form));
                specifiedFloor.Add(new JProperty("level", level.ToString()));

                //Create the floor JObject
                JObject floorObject = new JObject(
                new JProperty("type", "floor"),
                new JProperty("attr", new JArray(
                    specifiedFloor
                    ))
                );

                array3.Add(floorObject);
            }
        }

        private JArray GetAttrArray()
        {
            //print ff de specified ding
            JProperty type1 = (JProperty)specifiedDescription.First;
            //Debug.Log("Type1: " + type1);
            JProperty attr1 = (JProperty)type1.Next;
            //Debug.Log("Attr1: " + attr1);

            JArray array1 = (JArray)attr1.Value;
            //Debug.Log("Array1: " + array1);

            JToken lotObject = array1.First;
            //Debug.Log("lotObject: " + lotObject);

            JObject otherObject = (JObject)lotObject.Next;
            //Debug.Log("other object: " + otherObject);

            JProperty typeBuilding = (JProperty)otherObject.First;
            //Debug.Log("typeBuilding: " + typeBuilding);

            JProperty otherAttr = (JProperty)typeBuilding.Next;
            //Debug.Log("Otherattr: " + otherAttr);

            JArray array2 = (JArray)otherAttr.Value;
            //Debug.Log("Array2: " + array2);

            JObject attrObject = (JObject)array2.First;
            //Debug.Log("attrObject: " + attrObject);

            JProperty typeHouse = (JProperty)attrObject.First;
            //Debug.Log("Typehouse: " + typeHouse);

            JProperty attr3 = (JProperty)typeHouse.Next;
            //Debug.Log("attr3: " + attr3);

        if (attr3 == null)
        {
            //Debug.Log("attr3 is null");
            JArray array3 = new JArray();
            JProperty attrPropNew = new JProperty("attr", array3);
            attrObject.Add(attrPropNew);
            return array3;
        }
        else
        {
            //Debug.Log("attr3 is not null");
            JArray array3 = (JArray)attr3.Value;
            //Debug.Log("Array3: " + array3);
            if(array3 == null)
            {
                //Debug.Log("array3 is null");
                array3 = new JArray();
                attr3.Value = array3;
            }
            return array3;
        }
        }

        public void disabledSpecifyDescription()
        {
            if (specifiedDescription != null)
            {
                //Debug.Log(specifiedDescription.ToString());
                //bool successParse = true;
                //Hashtable o = (Hashtable)JSON.JsonDecode(specifiedDescription.ToString(), ref successParse);
                //hashParser.PrintHashTable(o);     //Convert the hashtable to WorldObjects
                //WorldObject root = hashParser.getRootObject();
                //manager.GenerateWorldObject(root, specifiedDescription.ToString());


                //Now the house that we were specifying should get the 'highlightable' tag

                specifiedDescription = null;
                manager.stopSpecifying();
            }
            else
            {
                Debug.Log("Specified description null");
                return;
            }
        }

    private void GenerateWorldObject(string JSONstring)
    {
        if (JSONstring != null)
        {
            bool successParse = true;
            Hashtable o = (Hashtable)JSON.JsonDecode(JSONstring, ref successParse);
            hashParser.PrintHashTable(o);     //Convert the hashtable to WorldObjects
            WorldObject root = hashParser.getRootObject();
            manager.GenerateWorldObject(root, JSONstring);
        }
        else
        {
            Debug.Log("Specified description null");
            return;
        }
    }
    }