using Newtonsoft.Json.Linq;
using Procurios.Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCase : MonoBehaviour {

	public void Test() {
        Hashtable o;
        /*
        JObject rss =
                new JObject(
                    new JProperty("type", "buildings"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("type", "appartement"),
                            new JProperty("attr", new JArray(
                                new JObject(
                                    new JProperty("verdiepingen", "2"),
                                    new JProperty("kleur", "blauw")),
                                new JObject(
                                    new JProperty("other", "floor"),
                                    new JProperty("attr", new JArray(
                                        new JObject(
                                            new JProperty("kleur", "blauw"))))),
                                new JObject(
                                    new JProperty("other", "floor"),
                                    new JProperty("attr", new JArray(
                                        new JObject(
                                            new JProperty("kleur", "geel"))))),
                                new JObject(
                                    new JProperty("other", "dak"),
                                    new JProperty("attr", new JArray(
                                        new JObject(
                                          new JProperty("kleur", "groen"))))),
                                new JObject(
                                    new JProperty("other", "dak"),
                                    new JProperty("attr", new JArray(
                                        new JObject(
                                            new JProperty("kleur", "groen"),
                                            new JProperty("grootte", "klein")))))))))));
        */
        JObject rss = new JObject(
            new JObject(
                    new JProperty("type", "buildings"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("type", "appartement"),
                            new JProperty("attr", new JArray(
                                new JObject(
                                    new JProperty("verdiepingen", "2"),
                                    new JProperty("kleur", "blauw")
                                    ),
                                new JObject(
                                    new JProperty("type", "verdieping"),
                                    new JProperty("attr", new JArray(
                                        new JObject(
                                            new JProperty("etage", "1"),
                                            new JProperty("kleur", "groen")
                                            )
                                    )
                                    )
                                ),
                                new JObject(
                                    new JProperty("type", "verdieping"),
                                    new JProperty("attr", new JArray(
                                        new JObject(
                                            new JProperty("etage", "2"),
                                            new JProperty("kleur", "geel")
                                            )
                                        ))
                                    )
                                )
                            )
                        ))
                        )

                        )
        );

        //JObject rss =
        //        new JObject(
        //            new JProperty("type", "buildings"),
        //            new JProperty("attr", new JArray(
        //                new JObject(
        //                    new JProperty("type", "huis"),
        //                    new JProperty("attr", new JArray(
        //                        new JObject(
        //                            new JProperty("kleur", "blauw"))))))));

        Debug.Log(rss.ToString());
        bool successParse = true;
        o = (Hashtable)JSON.JsonDecode(rss.ToString(), ref successParse);

        HashtableParser hashParser = new HashtableParser();
        hashParser.PrintHashTable(o);   //Convert the hashtable to WorldObjects
        //hashParser.LinkWorldObjects();  //Link the WorldObjects (set correct children)
        hashParser.PrintWorldObjects();
        string path = hashParser.SearchBestFit();
        //WorldObject root = hashParser.getRootObject();
        //string path = root.SearchCompatibleGameObject();
        if (path != null) {
            //Debug.Log("Spawn path: " + path);
            GameObject instance = Instantiate(Resources.Load(path, typeof(GameObject)), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        }
        //hashParser.PrintWorldObjects(); //Print all WorldObjects to proofread
    }
}
