using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class KeywordParser {

    private List<KeyValuePair<string, int>> quantityList;       //Key : unit, Value : amount
    private List<KeyValuePair<string, string>> entityList;      //Key : semenity (ontologytype), Value : form
    private List<KeyValuePair<string, string>> otherEntitiesList;      //Key : semenity (ontologytype), Value : form
    private List<KeyValuePair<string, string>> conceptList;     //Key : semenity (ontologytype), Value : form 
    private List<KeyValuePair<string, string>> otherConceptsList;     //Key : semenity (ontologytype), Value : form 

    private List<string> ontologyTypes;     //Contains ontologyTypes as defined in ThesisVR dictionary in MeaningCloud

    public KeywordParser() {
        Debug.Log("Created a keyword parser");
        quantityList = new List<KeyValuePair<string, int>>();
        entityList = new List<KeyValuePair<string, string>>();
        otherEntitiesList = new List<KeyValuePair<string, string>>();
        conceptList = new List<KeyValuePair<string, string>>();
        otherConceptsList = new List<KeyValuePair<string, string>>();

        ontologyTypes = new List<string>();
        GetOntologyTypes();
        //PrintOntologyTypes();
    }

    private void GetOntologyTypes() {
        //Read the OntologyTypes.txt file
        DirectoryInfo dir = new DirectoryInfo("Assets/Scripts/Speech");
        try {
            FileInfo[] info = dir.GetFiles("OntologyTypes.txt");
            FileInfo f = info[0];
            string path = f.FullName;

            //Only read strings that have actual content
            string[] stringArray = System.IO.File.ReadAllLines(path);
            List<string> stringList = new List<string>(stringArray);
            List<string> stringsToRemove = new List<string>();
            foreach (string s in stringList) {
                if (IsNullOrWhiteSpace(s) || s.Trim().Length == 0) {
                    stringsToRemove.Add(s);
                }
            }
            foreach (string s in stringsToRemove) {
                stringList.Remove(s);
            }

            //Copy to ontologyTypes
            foreach(string s in stringList) {
                ontologyTypes.Add(s);
            }

        } catch (DirectoryNotFoundException exc) {
            Debug.Log("The ontology types could not be loaded");
        } catch (Exception exc) {
            Debug.Log("An exception occured reading the ontology types");
        }

    }

    private void PrintOntologyTypes() {
        Debug.Log("-------- Ontology types --------");
        foreach(string s in ontologyTypes) {
            Debug.Log(s);
        }
        Debug.Log("----------------------------------");
    }

    private static bool IsNullOrWhiteSpace(string value) {
        if (value != null) {
            for (int i = 0; i < value.Length; i++) {
                if (!char.IsWhiteSpace(value[i])) {
                    return false;
                }
            }
        }
        return true;
    }

    public void ConvertHashtable(Hashtable o) {
        quantityList.Clear();
        entityList.Clear();
        otherEntitiesList.Clear();
        conceptList.Clear();
        otherConceptsList.Clear();

        foreach (DictionaryEntry entry in o) {
            if (entry.Key.Equals("entity_list")) {
                CheckEntityList((ArrayList)entry.Value);
            } else if (entry.Key.Equals("concept_list")) {
                CheckConceptList((ArrayList)entry.Value);
            } else if (entry.Key.Equals("quantity_expression_list")) {
                CheckQuantityExpressionList((ArrayList)entry.Value);
            }
        }
    }

    private void CheckQuantityExpressionList(ArrayList list) {
        Debug.Log("-------- Quantities --------");
        if (list.Count == 0) {
            Debug.Log("This list is empty");
        } else {
            string unit = "";
            int quantity = 0;
            foreach (Hashtable table in list) {
                foreach (DictionaryEntry entry in table) {
                    if (entry.Key.Equals("unit")) {
                        //Debug.Log((string)entry.Value);
                        unit = (string) entry.Value;
                    }
                    if (entry.Key.Equals("amount_form")) {
                        //Debug.Log((string)entry.Value);
                        try {
                            Int32.TryParse((string)entry.Value, out quantity);
                        } catch {
                            Debug.Log("Could not convert quantity to an int");
                        }
                    }
                }

                //Add it as a new KeyValuePair to the quantityList
                if (!unit.Equals("") && !(quantity == 0)) {
                    Debug.Log("Added to quantity list: Unit = " + unit + " Amount = " + quantity);
                    quantityList.Add(new KeyValuePair<string, int>(unit, quantity));
                    unit = "";
                    quantity = 0;
                 } else {
                    Debug.Log("No pair added to quantity list");
                    Debug.Log("Unit: " + unit + " Quantity: " + quantity);
                }
            }
        }
        Debug.Log("--------------------------");
    }

    private void CheckConceptList(ArrayList list) {
        Debug.Log("-------- Concepts --------");
        if (list.Count == 0) {
            Debug.Log("This list is empty");
        } else {
            string form = "";
            string ontologyType = "";
            foreach (Hashtable table in list) {
                foreach (DictionaryEntry entry in table) {
                    //Debug.Log("Key: " + entry.Key + " Value: " + entry.Value.ToString());
                    if (entry.Key.Equals("form")) {
                        //Debug.Log((string)entry.Value);
                        form = (string) entry.Value;
                    }
                    if (entry.Key.Equals("sementity")) {
                        ontologyType = GetTypeOfKeyword((Hashtable)entry.Value);
                        //Debug.Log(ontologyType);
                    }
                }

                //Check if this semenity is in our ontologyTypes
                if (ontologyTypes.Contains(ontologyType)) {
                    //Add it to the normal list
                    conceptList.Add(new KeyValuePair<string, string>(ontologyType, form));
                    Debug.Log("Added type : " + ontologyType + " form: " + form + " to concept list");
                } else {
                    //Add it to the other list
                    otherConceptsList.Add(new KeyValuePair<string, string>(ontologyType, form));
                    Debug.Log("Added type : " + ontologyType + " form: " + form + " to other concepts list");
                }
            }
        }
        Debug.Log("--------------------------");
    }

    private void CheckEntityList(ArrayList list) {
        Debug.Log("-------- Entities --------");
        if (list.Count == 0) {
            Debug.Log("This list is empty");
        } else {
            string form = "";
            string ontologyType = "";
            foreach (Hashtable table in list) {
                foreach (DictionaryEntry entry in table) {
                    //Debug.Log("Key: " + entry.Key + " Value: " + entry.Value.ToString());
                    if (entry.Key.Equals("form")) {
                        //Debug.Log((string)entry.Value);
                        form = (string)entry.Value;
                    }
                    if (entry.Key.Equals("sementity")) {
                        ontologyType = GetTypeOfKeyword((Hashtable)entry.Value);
                        //Debug.Log(ontologyType);
                    }
                }

                //Check if this semenity is in our ontologyTypes
                if (ontologyTypes.Contains(ontologyType)) {
                    //Add it to the normal list
                    entityList.Add(new KeyValuePair<string, string>(ontologyType, form));
                    Debug.Log("Added type : " + ontologyType + " form: " + form + " to entity list");
                } else {
                    //Add it to the other list
                    otherEntitiesList.Add(new KeyValuePair<string, string>(ontologyType, form));
                    Debug.Log("Added type : " + ontologyType + " form: " + form + " to other entities list");
                }
            }
        }
        Debug.Log("--------------------------");
    }

    private string GetTypeOfKeyword(Hashtable value) {
        foreach (DictionaryEntry entry in value) {
            if (entry.Key.Equals("type")) {
                string type = (string)entry.Value;
                type = type.Substring(4, type.Length - 4);
                return type;
            }
        }
        return null;
    }
}
