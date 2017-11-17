using Newtonsoft.Json.Linq;
using Procurios.Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCase : MonoBehaviour {

	public void Test() {
        Hashtable o;
        //JObject rss =
        //        new JObject(
        //            new JProperty("type", "buildings"),
        //            new JProperty("attr", new JArray(
        //                new JObject(
        //                    new JProperty("type", "huis"),
        //                    new JProperty("attr", new JArray(
        //                        new JObject(
        //                            new JProperty("other", "floor"),
        //                            new JProperty("attr", new JArray(
        //                                new JObject(
        //                                    new JProperty("kleur", "blauw"))))),
        //                        new JObject(
        //                            new JProperty("other", "floor"),
        //                            new JProperty("attr", new JArray(
        //                                new JObject(
        //                                    new JProperty("kleur", "geel"))))),
        //                        new JObject(
        //                            new JProperty("other", "dak"),
        //                            new JProperty("attr", new JArray(
        //                                new JObject(
        //                                  new JProperty("kleur", "groen"))))),
        //                        new JObject(
        //                            new JProperty("other", "dak"),
        //                            new JProperty("attr", new JArray(
        //                                new JObject(
        //                                    new JProperty("kleur", "groen"),
        //                                    new JProperty("grootte", "klein")))))))))));

        JObject rss =
                new JObject(
                    new JProperty("type", "buildings"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("type", "huis"),
                            new JProperty("attr", new JArray(
                                new JObject(
                                    new JProperty("kleur", "bruin"))))))));

        Debug.Log(rss.ToString());
        bool successParse = true;
        o = (Hashtable)JSON.JsonDecode(rss.ToString(), ref successParse);

        HashtableParser hashParser = new HashtableParser();
        hashParser.PrintHashTable(o);   //Convert the hashtable to WorldObjects
        hashParser.LinkWorldObjects();  //Link the WorldObjects (set correct children)
        hashParser.PrintWorldObjects();
        WorldObject root = hashParser.getRootObject();
        string path = root.SearchCompatibleGameObject();
        Debug.Log(path);
        GameObject instance = Instantiate(Resources.Load(path, typeof(GameObject)), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        //hashParser.PrintWorldObjects(); //Print all WorldObjects to proofread
    }
}
