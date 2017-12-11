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
        //building case
        //JObject rss = new JObject(
        //    new JObject(
        //            new JProperty("type", "lots"),
        //            new JProperty("attr", new JArray(
        //    new JObject(
        //            new JProperty("type", "buildings"),
        //            new JProperty("attr", new JArray(
        //                new JObject(
        //                    new JProperty("type", "house"),
        //                    new JProperty("attr", new JArray(
        //                        new JObject(
        //                            new JProperty("floors", "4"),
        //                            new JProperty("color", "blue")
        //                            ),
        //                        new JObject(
        //                            new JProperty("type", "floor"),
        //                            new JProperty("attr", new JArray(
        //                                new JObject(
        //                                    new JProperty("level", "1")//,
        //                                                               //new JProperty("color", "green")
        //                                    )
        //                            )
        //                            )
        //                        ),
        //                        new JObject(
        //                            new JProperty("type", "floor"),
        //                            new JProperty("attr", new JArray(
        //                                new JObject(
        //                                    new JProperty("level", "2"),
        //                                    new JProperty("color", "yellow")
        //                                    )
        //                                ))
        //                            )
        //                        )
        //                    )
        //                ))
        //                )

        //                ))))
        //);

        JObject rss = new JObject(
            new JObject(
                    new JProperty("type", "streets"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("type", "straight"),
                            new JProperty("attr", new JArray(
                                new JObject(
                                    new JProperty("lots", "4")
                                    ))))))));

        Debug.Log(rss.ToString());
        bool successParse = true;
        o = (Hashtable)JSON.JsonDecode(rss.ToString(), ref successParse);

        HashtableParser hashParser = new HashtableParser();
        hashParser.PrintHashTable(o);     //Convert the hashtable to WorldObjects
        //hashParser.LinkWorldObjects();  //Link the WorldObjects (set correct children)
        hashParser.PrintWorldObjects();
        //string path = hashParser.SearchBestFit();
        WorldObject root = hashParser.getRootObject();
        //root.GenerateWorldObject();
        //string path = root.SearchCompatibleGameObject();
        //if (path != null) {
        //    //Debug.Log("Spawn path: " + path);
        //    GameObject instance = Instantiate(Resources.Load(path, typeof(GameObject)), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        //}
        //hashParser.PrintWorldObjects(); //Print all WorldObjects to proofread
        Debug.Log("Starting to generate");
        GeneratorManager manager = new GeneratorManager();
        manager.GenerateWorldObject(root);
    }
}
