﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Procurios.Public;
using Newtonsoft.Json.Linq;
using System.Globalization;
using SFB;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static bool loadingGame = false;

    private static SaveManager Instance;
    private List<string> commandList;
    private static string path;
    private static bool setDontDestroy = false;
    private static string loadPath = "";

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            Instance.commandList = new List<string>(); 
        }
        if (!setDontDestroy)
        {
            setDontDestroy = true;
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }
        path = Application.dataPath;        
    }

    public static void AddJSON(string s)
    {
        //Add command to the commandList
        s = s.Replace(System.Environment.NewLine, "");
        s = s.Replace(" ", "");
        Instance.commandList.Add(s);
    }

    public static void AddCommand(string s)
    {
        s = s.Replace(System.Environment.NewLine, "");
        s = s.Replace(" ", "");
        Instance.commandList.Add(s);
    }

    public void SaveGame()
    {
        string newPath = StandaloneFileBrowser.SaveFilePanel("Save File", path, "", "txt");

        Debug.LogError("Saving...");
        //Save all commands to a file
        if (File.Exists(newPath))
        {
            File.Delete(newPath);
        }

        // Create a file to write to.
        using (StreamWriter sw = File.CreateText(newPath))
        {
            foreach (string s in Instance.commandList)
            {
                sw.WriteLine(s);
            }
        }
        Debug.LogError("Done saving");
    }

    public static void CreateSelectArrowCommand(string arrowID)
    {
        Debug.LogError("CreateSelectArrowCommand called");
        JObject command = new JObject(
            new JObject(
                    new JProperty("type", "command"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("type", "selectarrow"),
                            new JProperty("attr", new JArray(
                                new JObject(
                                    new JProperty("arrowid", arrowID)
                                    )
                                )
                        ))))));

        AddCommand(command.ToString());
    }

    public static void CreateSelectLotCommand(string lotID)
    {
        Debug.LogError("CreateSelectLotCommand called");
        JObject command = new JObject(
            new JObject(
                    new JProperty("type", "command"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("type", "selectlot"),
                            new JProperty("attr", new JArray(
                                new JObject(
                                    new JProperty("lotid", lotID)
                                    )
                                )
                        ))))));

        AddCommand(command.ToString());
    }

    public static void CreateDeleteObjectCommand(string objectID)
    {
        Debug.LogError("CreateDeleteObjectCommand called");
        JObject command = new JObject(
            new JObject(
                    new JProperty("type", "command"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("type", "delete"),
                            new JProperty("attr", new JArray(
                                new JObject(
                                    new JProperty("objectid", objectID)
                                    )
                                )
                        ))))));

        AddCommand(command.ToString());
    }

    public static void CreateTransformCommand(string objectID, Vector3 localPosition, Quaternion localRotation)
    {
        Vector3 rotation = localRotation.eulerAngles;

        Debug.LogError("CreateTransformCommand called");
        JObject command = new JObject(
            new JObject(
                    new JProperty("type", "command"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("type", "transform"),
                            new JProperty("attr", new JArray(
                                new JObject(
                                    new JProperty("objectid", objectID)
                                    ),
                                new JObject(
                                    new JProperty("type", "position"),
                                    new JProperty("attr", new JArray(
                                        new JObject(
                                            new JProperty("x", localPosition.x.ToString("G9")),
                                            new JProperty("y", localPosition.y.ToString("G9")),
                                            new JProperty("z", localPosition.z.ToString("G9"))
                                            )
                                        ))
                                    ),
                                new JObject(
                                    new JProperty("type", "rotation"),
                                    new JProperty("attr", new JArray(
                                        new JObject(
                                            new JProperty("x", rotation.x.ToString("G9")),
                                            new JProperty("y", rotation.y.ToString("G9")),
                                            new JProperty("z", rotation.z.ToString("G9"))
                                            )
                                        ))
                                    )
                                )
                        ))))));

        AddCommand(command.ToString());
    }

    public void LoadGame()
    {
        //Read file & execute all commands
        Debug.LogError("Loading...");

        if (!loadingGame)
        {

            loadPath = WriteResult(StandaloneFileBrowser.OpenFilePanel("Open File", path, "txt", false));

            if (File.Exists(loadPath))
            {
                loadingGame = true;
                //SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
                ResetGame();
            }
        } else
        {
            Debug.Log("Nothing to load");
        }
    }

    private Vector3 GetVector(WorldObject obj)
    {
        Hashtable direcAttributes = obj.directAttributes;
        float x = float.Parse((string)direcAttributes["x"], CultureInfo.InvariantCulture.NumberFormat);
        float y = float.Parse((string)direcAttributes["y"], CultureInfo.InvariantCulture.NumberFormat);
        float z = float.Parse((string)direcAttributes["z"], CultureInfo.InvariantCulture.NumberFormat);
        return new Vector3(x, y, z);
    }

    private string WriteResult(string[] paths)
    {
        if (paths.Length == 0)
        {
            return null;
        }
        return paths[0];
    }

    private void ResetGame()
    {
        //Reload Scene


        //Destroy everything in World
        Transform world = GameObject.Find("World").transform;
        int childCount = world.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Destroy(world.GetChild(i).gameObject);
        }

        //Reset static stuff
        StreetGeneratorV2.streetID = 1;
        StreetGeneratorV2.lotID = 1;
        StreetGeneratorV2.previousStreetScript = null;
        BuildingGenerator.lotID = 1;
        GeneratorManager.currentDirection = new Vector2(0, 0);
        GeneratorManager.pointDirection = Orientation.straight;
        GenericStreet.arrowID = 1;
        Speech.specifyDescription = false;

        //Reset shaders
        GameObject.Find("ObjectManager").GetComponent<InitializeShaders>().UpdateShaders();

        //Respawn ground
        GameObject.Find("GroundSpawner").GetComponent<GroundSpawner>().SpawnGround();

        //Go to room
        GameObject.Find("TransitionManager").GetComponent<TransitionScript>().GoToRoom();

        StartCoroutine(ExecuteSaveFile());

    }

    IEnumerator ExecuteSaveFile()
    {
        GeneratorManager manager = new GeneratorManager();

        HashtableParser hashParser = new HashtableParser();
        bool successParse = true;

        string[] commands = File.ReadAllLines(loadPath);
        for (int i = 0; i < commands.Length; i++)
        {
            //Debug.LogError("I: " + i);
            //Debug.LogError(commands[i]);
            Instance.commandList.Add(commands[i]);

            Hashtable ht = (Hashtable)JSON.JsonDecode(commands[i], ref successParse);
            hashParser.PrintHashTable(ht);     //Convert the hashtable to WorldObjects
            WorldObject root = hashParser.getRootObject();
            if (root.GetObjectValue().Equals("command"))
            {
                root = root.GetChildren()[0];
                switch (root.GetObjectValue())
                {
                    case "selectarrow":
                        OrientationManager.Instance.SetSelectedArrow(GameObject.Find((string)root.directAttributes["arrowid"]).transform, true);
                        break;
                    case "selectlot":
                        LotManager.setLot(GameObject.Find((string)root.directAttributes["lotid"]));
                        break;
                    case "delete":
                        Destroy(GameObject.Find((string)root.directAttributes["objectid"]));
                        break;
                    case "transform":
                        Transform objectToSet = GameObject.Find((string)root.directAttributes["objectid"]).transform;
                        Vector3 localPosition = GetVector(root.GetChildWithTypeValue("position"));
                        Vector3 localRotation = GetVector(root.GetChildWithTypeValue("rotation"));
                        objectToSet.localPosition = localPosition;
                        objectToSet.localRotation = Quaternion.Euler(localRotation.x, localRotation.y, localRotation.z);
                        break;
                }
            }
            else
            {
                manager.GenerateWorldObject(root);
            }
            yield return new WaitForEndOfFrame();
        }
        loadingGame = false;
        Debug.LogError("Done loading");
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if(loadingGame)
            StartCoroutine(ExecuteSaveFile());
    }

}
