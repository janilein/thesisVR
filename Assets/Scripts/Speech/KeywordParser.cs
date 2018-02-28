using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KeywordParser
{
    private TextToJSON jsonConverter;

    private List<Quantity> quantityList;
    private List<Relation> relationList;
    //private List<KeyValuePair<string, string>> entityList;      //Key : semenity (ontologytype), Value : form
    private List<KeyValuePair<string, string>> otherEntitiesList;      //Key : semenity (ontologytype), Value : form
    private List<Entity> entityList;
    private List<KeyValuePair<string, string>> otherConceptsList;     //Key : semenity (ontologytype), Value : form 

    public KeywordParser()
    {
        Debug.Log("Created a keyword parser");
        quantityList = new List<Quantity>();
        //entityList = new List<KeyValuePair<string, string>>();
        otherEntitiesList = new List<KeyValuePair<string, string>>();
        entityList = new List<Entity>();
        //otherConceptsList = new List<KeyValuePair<string, string>>();
        relationList = new List<Relation>();

        //PrintOntologyTypes();
        jsonConverter = new TextToJSON();
    }

    public void ConvertHashtable(Hashtable o)
    {
        quantityList.Clear();
        //entityList.Clear();
        otherEntitiesList.Clear();
        entityList.Clear();
        //otherConceptsList.Clear();
        relationList.Clear();

        foreach (DictionaryEntry entry in o)
        {
            if (entry.Key.Equals("entity_list"))
            {
                CheckEntityList((ArrayList)entry.Value);
            }
            else if (entry.Key.Equals("quantity_expression_list"))
            {
                CheckQuantityExpressionList((ArrayList)entry.Value);
            }
            else if (entry.Key.Equals("relation_list"))
            {
                CheckRelationList((ArrayList)entry.Value);
            }
            //else if (entry.Key.Equals("concept_list")) {
            //    CheckConceptList((ArrayList)entry.Value);
            //} 
        }
        ConvertToJSON();
    }

    private void ConvertToJSON()
    {
        Entity entity = null;

        //Eerste mogelijkheid: geen quantities, 1 direction (voor verandering van direction ofzo)
        entity = GetEntityFromEntityList("Direction");
        if (entity != null && quantityList.Count == 0) //no quantities + 1 direction concept for a direction change
        {
            Debug.Log("Found direction pair");
            jsonConverter.CreateDirectionJSON(entity);
            return;
        }

        //Tweede mogelijkheid: een straat beschrijving
        entity = GetEntityFromEntityList("StreetType");
        if (entity != null)
        {
            Debug.Log("Found StreetType pair");

            ////List to add pairs to: StreetType and possibly length
            //List<Concept> concepts = new List<Concept>();
            //concepts.Add(concept);

            ////Check for optional length specification
            //Concept lengthPair = GetKeyValuePairFromConceptList("length");
            //if(lengthPair != null)
            //{
            //    //Length pair found
            //    concepts.Add(lengthPair);
            //}
            jsonConverter.CreateStreetJSON(entityList, quantityList);
            return;
        }

        entity = GetEntityFromEntityList("HouseType");
        if (entity != null || Speech.specifyDescription)                          //Voorlopig gewoon kijken of specify aanstaat en er van uit gaan dat het dan over een huis gaat
        {
            Debug.Log("Found HouseType pair");
            jsonConverter.CreateHouseJSON(entityList, quantityList, relationList);
            return;
        }
    }

    private Entity GetEntityFromEntityList(string key)
    {
        key = key.ToLower();
        foreach (Entity entity in entityList)
        {
            if (entity.type.ToLower().Equals(key))
            {
                return entity;
            }
        }
        return null;
    }

    private void CheckRelationList(ArrayList list)
    {
        Debug.Log("-------- Relations --------");
        if (list.Count == 0)
        {
            Debug.Log("This list is empty");
        }
        else
        {
            string form = "";
            int amount = 0;
            foreach (Hashtable table in list)
            {
                foreach (DictionaryEntry entry in table)
                {
                    if (entry.Key.Equals("subject"))
                    {
                        //Find the 'subject' part
                        Debug.Log("entry type: " + entry.Value.GetType());
                        Hashtable relationTable = (Hashtable)entry.Value;
                        foreach (DictionaryEntry relation in relationTable)
                        {
                            if (relation.Key.Equals("form"))
                            {
                                form = ((string)relation.Value).ToLower();
                            }
                            else if (relation.Key.Equals("lemma_list"))
                            {
                                //The lemma list, get the number from this bad boy
                                ArrayList lemmaList = (ArrayList)relation.Value;
                                //Should only have 1 entry
                                if (lemmaList.Count > 0)
                                {
                                    Int32.TryParse((string)lemmaList[0], out amount);
                                }
                            }
                        }
                    }
                }

                //Add it as a new Relation to the relationList
                if (!form.Equals("") && !(amount == 0))
                {
                    Debug.Log("Added to relation list: Unit = " + form + " Amount = " + amount);
                    Relation relation = new Relation(form, amount);
                    relationList.Add(relation);
                    form = "";
                    amount = 0;
                }
                else
                {
                    Debug.Log("No pair added to quantity list");
                    Debug.Log("Unit: " + form + " Quantity: " + amount);
                }
            }
        }
        Debug.Log("--------------------------");
    }

    private void CheckQuantityExpressionList(ArrayList list)
    {
        Debug.Log("-------- Quantities --------");
        if (list.Count == 0)
        {
            Debug.Log("This list is empty");
        }
        else
        {
            string unit = "";
            int amount = 0;
            int inip = 0;
            int endp = 0;
            foreach (Hashtable table in list)
            {
                foreach (DictionaryEntry entry in table)
                {
                    if (entry.Key.Equals("unit"))
                    {
                        //Debug.Log((string)entry.Value);
                        unit = (string)entry.Value;
                    }
                    else if (entry.Key.Equals("numeric_value"))
                    {
                        //Debug.Log((string)entry.Value);
                        try
                        {
                            Int32.TryParse((string)entry.Value, out amount);
                        }
                        catch
                        {
                            Debug.Log("Could not convert quantity to an int");
                        }
                    }
                    else if (entry.Key.Equals("inip"))
                    {
                        Int32.TryParse((string)entry.Value, out inip);
                    }
                    else if (entry.Key.Equals("endp"))
                    {
                        Int32.TryParse((string)entry.Value, out endp);
                    }
                }

                //Add it as a new KeyValuePair to the quantityList
                if (!unit.Equals("") && !(amount == 0))
                {
                    Debug.Log("Added to quantity list: Unit = " + unit + " Amount = " + amount);
                    Quantity quantity = new Quantity(unit, amount, inip, endp);
                    quantityList.Add(quantity);
                    unit = "";
                    amount = 0;
                }
                else
                {
                    Debug.Log("No pair added to quantity list");
                    Debug.Log("Unit: " + unit + " Quantity: " + amount);
                }
            }
        }
        Debug.Log("--------------------------");
    }

    //private void CheckConceptList(ArrayList list) {
    //    Debug.Log("-------- Concepts --------");
    //    if (list.Count == 0) {
    //        Debug.Log("This list is empty");
    //    } else {
    //        string form = "";
    //        string ontologyType = "";
    //        int inip = 0;
    //        int endp = 0;
    //        List<int> inips = new List<int>();
    //        List<int> endps = new List<int>();
    //        bool fromDictionary = false;
    //        foreach (Hashtable table in list) {
    //            foreach (DictionaryEntry entry in table) {
    //                //Debug.Log("Key: " + entry.Key + " Value: " + entry.Value.ToString());
    //                if (entry.Key.Equals("form")) {
    //                    //Debug.Log((string)entry.Value);
    //                    form = (string) entry.Value;
    //                }
    //                 else if (entry.Key.Equals("sementity")) {
    //                    ontologyType = GetTypeOfKeyword((Hashtable)entry.Value);
    //                    //Debug.Log(ontologyType);
    //                }
    //                else if (entry.Key.Equals("dictionary"))
    //                {
    //                    fromDictionary = true;
    //                }
    //                else if (entry.Key.Equals("variant_list"))
    //                {
    //                    foreach(Hashtable variant in (ArrayList)entry.Value)
    //                    {
    //                        foreach(DictionaryEntry variantEntry in variant)
    //                        {
    //                            if (variantEntry.Key.Equals("inip"))
    //                            {
    //                                Int32.TryParse((string) variantEntry.Value, out inip);
    //                                inips.Add(inip);
    //                            }
    //                            else if (variantEntry.Key.Equals("endp"))
    //                            {
    //                                Int32.TryParse((string)variantEntry.Value, out endp);
    //                                endps.Add(endp);
    //                            }
    //                        }
    //                    }
    //                }
    //            }

    //            //Check if this semenity is in our ontologyTypes
    //            if (fromDictionary) {
    //                //Add it to the normal list
    //                for(int i = 0; i < inips.Count; i++)
    //                {
    //                    Concept concept = new Concept(form, ontologyType, inips[i], endps[i]);
    //                    conceptList.Add(concept);
    //                    //Debug.Log("Added type : " + ontologyType + " form: " + form + " to concept list" + "inip:" + inips[i] + "endp:" + endps[i]);
    //                } 
    //                fromDictionary = false;
    //            } else {
    //                //Add it to the other list
    //                otherConceptsList.Add(new KeyValuePair<string, string>(ontologyType, form));
    //                //Debug.Log("Added type : " + ontologyType + " form: " + form + " to other concepts list");
    //            }
    //            inips.Clear();
    //            endps.Clear();
    //        }
    //    }
    //    Debug.Log("--------------------------");
    //}

    private void CheckEntityList(ArrayList list)
    {
        Debug.Log("-------- Entities --------");
        if (list.Count == 0)
        {
            Debug.Log("This list is empty");
        }
        else
        {
            string form = "";
            string ontologyType = "";
            int inip = 0;
            int endp = 0;
            List<int> inips = new List<int>();
            List<int> endps = new List<int>();
            bool fromDictionary = false;
            foreach (Hashtable table in list)
            {
                foreach (DictionaryEntry entry in table)
                {
                    //Debug.Log("Key: " + entry.Key + " Value: " + entry.Value.ToString());
                    if (entry.Key.Equals("form"))
                    {
                        //Debug.Log((string)entry.Value);
                        form = ((string)entry.Value).ToLower();
                    }
                    else if (entry.Key.Equals("sementity"))
                    {
                        ontologyType = GetTypeOfKeyword((Hashtable)entry.Value);
                        //Debug.Log(ontologyType);
                    }
                    else if (entry.Key.Equals("dictionary"))
                    {
                        fromDictionary = true;
                    }
                    else if (entry.Key.Equals("variant_list"))
                    {
                        foreach (Hashtable variant in (ArrayList)entry.Value)
                        {
                            foreach (DictionaryEntry variantEntry in variant)
                            {
                                if (variantEntry.Key.Equals("inip"))
                                {
                                    Int32.TryParse((string)variantEntry.Value, out inip);
                                    inips.Add(inip);
                                }
                                else if (variantEntry.Key.Equals("endp"))
                                {
                                    Int32.TryParse((string)variantEntry.Value, out endp);
                                    endps.Add(endp);
                                }
                            }
                        }
                    }
                }

                //Check if this semenity is in our ontologyTypes
                if (fromDictionary)
                {
                    //Add it to the normal list
                    for (int i = 0; i < inips.Count; i++)
                    {
                        Entity entity = new Entity(form, ontologyType, inips[i], endps[i]);
                        entityList.Add(entity);
                        Debug.Log("Added type : " + ontologyType + " form: " + form + " to concept list" + "inip:" + inips[i] + "endp:" + endps[i]);
                    }
                    fromDictionary = false;
                }
                else
                {
                    //Add it to the other list
                    otherEntitiesList.Add(new KeyValuePair<string, string>(ontologyType, form));
                    Debug.Log("Added type : " + ontologyType + " form: " + form + " to other concepts list");
                }
                inips.Clear();
                endps.Clear();
            }
        }
        Debug.Log("--------------------------");
    }

    internal void disabledSpecifyDescription()
    {
        jsonConverter.disabledSpecifyDescription();
    }

    private string GetTypeOfKeyword(Hashtable value)
    {
        foreach (DictionaryEntry entry in value)
        {
            if (entry.Key.Equals("type"))
            {
                string type = (string)entry.Value;
                type = type.Substring(4, type.Length - 4);
                return type;
            }
        }
        return null;
    }
}
