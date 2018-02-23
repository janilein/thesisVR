using Newtonsoft.Json.Linq;
using Procurios.Public;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCase : MonoBehaviour {

    public void Test() {

        //JObject rss =
        //        new JObject(
        //            new JProperty("type", "buildings"),
        //            new JProperty("attr", new JArray(
        //                new JObject(
        //                    new JProperty("type", "appartement"),
        //                    new JProperty("attr", new JArray(
        //                        new JObject(
        //                            new JProperty("verdiepingen", "2"),
        //                            new JProperty("kleur", "blauw")),
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


        //building case

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

        JObject rss = new JObject(
            new JObject(
                    new JProperty("type", "lots"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("lotID", "1")
                            ),
                        new JObject(
                                new JProperty("type", "buildings"),
                                new JProperty("attr", new JArray(
                                    new JObject(
                                        new JProperty("type", "apartment"),
                                        new JProperty("attr", new JArray(
                                            new JObject(
                                                new JProperty("floors", "5")
                                                ),
                                            new JObject(
                                                new JProperty("type", "roof"),
                                                new JProperty("attr", new JArray(
                                                    new JObject(
                                                        new JProperty("color", "yellow"),
                                                        new JProperty("roofType", "pointy")
                                                        )
                                                    ))
                                                )
                                            
                                            )
                                        )
                                    ))
                                    )

                        ))))
        );



        //JObject rss = new JObject(
        //    new JObject(
        //            new JProperty("type", "streets"),
        //            new JProperty("attr", new JArray(
        //                new JObject(
        //                    new JProperty("streetID", "1")),
        //                new JObject(
        //                    new JProperty("type", "straight"),
        //                    new JProperty("attr", new JArray(
        //                        new JObject(
        //                            new JProperty("length", "small"),
        //                            new JProperty("lotsLeft", "2"),
        //                            new JProperty("lotsRight", "6"),
        //                            new JProperty("orientation", "rightStraight")
        //                            ))))))));

        bool successParse = true;
        Hashtable o = (Hashtable)JSON.JsonDecode(rss.ToString(), ref successParse);
        HashtableParser hashParser = new HashtableParser();
        hashParser.PrintHashTable(o);     //Convert the hashtable to WorldObjects
        //hashParser.LinkWorldObjects();  //Link the WorldObjects (set correct children)
        //hashParser.PrintWorldObjects();
        ////string path = hashParser.SearchBestFit();
        WorldObject root = hashParser.getRootObject();
        //Debug.Log("Starting to generate");
        GeneratorManager manager = new GeneratorManager();
        manager.GenerateWorldObject(root);

        //JObject rss2 = new JObject(
        //    new JObject(
        //            new JProperty("type", "streets"),
        //            new JProperty("attr", new JArray(
        //                new JObject(
        //                    new JProperty("streetID", "1")),
        //                new JObject(
        //                    new JProperty("type", "straight"),
        //                    new JProperty("attr", new JArray(
        //                        new JObject(
        //                            new JProperty("length", "long"),
        //                            new JProperty("lotsLeft", "2"),
        //                            new JProperty("lotsRight", "6"),
        //                            new JProperty("orientation", "rightStraight")
        //                            ))))))));

        //Hashtable o2 = (Hashtable)JSON.JsonDecode(rss2.ToString(), ref successParse);
        //hashParser.PrintHashTable(o2);     //Convert the hashtable to WorldObjects
        //WorldObject root2 = hashParser.getRootObject();

        //JObject rss3 = new JObject(
        //    new JObject(
        //            new JProperty("type", "orientation"),
        //            new JProperty("attr", new JArray(
        //                new JObject(
        //                    new JProperty("direction", "left"))
        //                ))));

        //Hashtable o3 = (Hashtable)JSON.JsonDecode(rss3.ToString(), ref successParse);
        //hashParser.PrintHashTable(o3);     //Convert the hashtable to WorldObjects
        //WorldObject root3 = hashParser.getRootObject();

        //JObject rss4 = new JObject(
        //    new JObject(
        //            new JProperty("type", "streets"),
        //            new JProperty("attr", new JArray(
        //                new JObject(
        //                    new JProperty("streetID", "1")),
        //                new JObject(
        //                    new JProperty("type", "intersection-x"),
        //                    new JProperty("attr", new JArray(
        //                        new JObject(
        //                            new JProperty("length", "long"),
        //                            new JProperty("lotsLeft", "2"),
        //                            new JProperty("lotsRight", "6"),
        //                            new JProperty("orientation", "rightStraight")
        //                            ))))))));

        //JObject rss5 = new JObject(
        //    new JObject(
        //            new JProperty("type", "orientation"),
        //            new JProperty("attr", new JArray(
        //                new JObject(
        //                    new JProperty("direction", "right"))
        //                ))));

        //JObject rss6 = new JObject(
        // new JObject(
        //         new JProperty("type", "streets"),
        //         new JProperty("attr", new JArray(
        //             new JObject(
        //                 new JProperty("streetID", "1")),
        //             new JObject(
        //                 new JProperty("type", "intersection-t"),
        //                 new JProperty("attr", new JArray(
        //                     new JObject(
        //                         new JProperty("length", "long"),
        //                         new JProperty("lotsLeft", "2"),
        //                         new JProperty("lotsRight", "6"),
        //                         new JProperty("orientation", "leftRight")
        //                         ))))))));

        //Hashtable o5 = (Hashtable)JSON.JsonDecode(rss5.ToString(), ref successParse);
        //hashParser.PrintHashTable(o5);     //Convert the hashtable to WorldObjects
        //WorldObject root5 = hashParser.getRootObject();

        //Hashtable o4 = (Hashtable)JSON.JsonDecode(rss4.ToString(), ref successParse);
        //hashParser.PrintHashTable(o4);     //Convert the hashtable to WorldObjects
        //WorldObject root4 = hashParser.getRootObject();

        //Hashtable o6 = (Hashtable)JSON.JsonDecode(rss6.ToString(), ref successParse);
        //hashParser.PrintHashTable(o6);     //Convert the hashtable to WorldObjects
        //WorldObject root6 = hashParser.getRootObject();

        //4 small streets
        //manager.GenerateWorldObject(root);
        //manager.GenerateWorldObject(root);
        //manager.GenerateWorldObject(root);
        //manager.GenerateWorldObject(root);

        ////t-intersection
        //manager.GenerateWorldObject(root6);

        ////Take a left
        //manager.GenerateWorldObject(root3);

        ////1 long street
        //manager.GenerateWorldObject(root2);

        ////Intersection
        //manager.GenerateWorldObject(root6);

        ////Take a left
        //manager.GenerateWorldObject(root3);

        ////2 long streets
        //manager.GenerateWorldObject(root2);
        //manager.GenerateWorldObject(root2);
        //manager.GenerateWorldObject(root2);
        //manager.GenerateWorldObject(root2);

        ////Intersection
        //manager.GenerateWorldObject(root4);

        ////Take another left
        //manager.GenerateWorldObject(root3);

        ////Another 2 small streets
        //manager.GenerateWorldObject(root);
        //manager.GenerateWorldObject(root);
        //manager.GenerateWorldObject(root2);
        //manager.GenerateWorldObject(root2);

        ////Intersection
        //manager.GenerateWorldObject(root4);

        ////Take another left
        //manager.GenerateWorldObject(root3);

        ////1 small street
        //manager.GenerateWorldObject(root);
        //manager.GenerateWorldObject(root);

        ////Intersection
        //manager.GenerateWorldObject(root4);

        ////Take a right
        //manager.GenerateWorldObject(root5);

        ////1 small street
        //manager.GenerateWorldObject(root);

        ////Intersection
        //manager.GenerateWorldObject(root4);

        ////Take a right
        //manager.GenerateWorldObject(root5);

        ////1 long street
        //manager.GenerateWorldObject(root2);

        ////Intersection
        //manager.GenerateWorldObject(root4);

        ////Take a left
        //manager.GenerateWorldObject(root3);

        ////1 small street
        //manager.GenerateWorldObject(root);

        ////t-intersection
        //manager.GenerateWorldObject(root6);

        ////Take a left
        //manager.GenerateWorldObject(root3);

        ////1 long street
        //manager.GenerateWorldObject(root2);

        ////t-intersection
        //manager.GenerateWorldObject(root6);

        ////Take a left
        //manager.GenerateWorldObject(root3);

        ////Intersection
        //manager.GenerateWorldObject(root4);

        //Debug.Log(rss.ToString());
        //hashParser.PrintWorldObjects(); //Print all WorldObjects to proofread

    }


}
