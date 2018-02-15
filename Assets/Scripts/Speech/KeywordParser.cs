using System;
using System.Collections;
using UnityEngine;

public class KeywordParser {
	
    public void ConvertHashtable(Hashtable o) {
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
        if (list.Count == 0) {
            Debug.Log("This list is empty");
        } else {
            foreach(DictionaryEntry entry in (Hashtable)list[0]) {
                if (entry.Key.Equals("unit")) {
                    Debug.Log((string)entry.Value);
                }
                if (entry.Key.Equals("amount_form")) {
                    Debug.Log((string)entry.Value);
                }
            }
        }
    }

    private void CheckConceptList(ArrayList list) {
        if (list.Count == 0) {
            Debug.Log("This list is empty");
        } else {
            foreach (Hashtable table in list) {
                foreach (DictionaryEntry entry in table) {
                    //Debug.Log("Key: " + entry.Key + " Value: " + entry.Value.ToString());
                    if (entry.Key.Equals("form")) {
                        Debug.Log((string)entry.Value);
                    }
                    if (entry.Key.Equals("sementity")) {
                        Debug.Log(GetTypeOfKeyword((Hashtable)entry.Value));
                    }
                }
            }
        }
    }

    private void CheckEntityList(ArrayList list) {
        if(list.Count == 0) {
            Debug.Log("This list is empty");
        } else {
            foreach (DictionaryEntry entry in (Hashtable)list[0]) {

            }
        }
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
