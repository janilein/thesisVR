using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Procurios.Public;
using Newtonsoft.Json.Linq;
using System.Globalization;
using SFB;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class SaveManager : MonoBehaviour
{
    public static bool loadingGame = false;

    private static SaveManager Instance;
    private List<string> commandList;
    private static string path;
    private static bool setDontDestroy = false;
    private static string loadPath = "";
    public GameObject floppy;
    public Transform floppies;
    public Transform room;
    public spawning.SpawningManager spawningManager;
    private static Vector2 xRange = new Vector2(0.065f, 0.839f);
    private static Vector2 yRange = new Vector2(-0.6f, 0.75f);
    private static float zValue = 0.5f; 
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
        path = Application.dataPath + "/Savegames";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        InitializeFloppies();
    }

    private void InitializeFloppies()
    {
        DirectoryInfo dir = new DirectoryInfo(path);
        try
        {
            FileInfo[] info = dir.GetFiles("*.txt");
            //Debug.Log(info.Length);
            foreach (FileInfo f in info)
            {
                SpawnFloppy(f.Name);
            }
        } catch(DirectoryNotFoundException exc)
        {
            Debug.LogError(exc.Message);
        }

        //Spawn some empty floppies as well
        SpawnFloppy();
    }

    public static void SpawnFloppy(string text = null)
    {
        GameObject spawnedFloppy = Instantiate(Instance.floppy);
        Vector3 scale = spawnedFloppy.transform.localScale;
        spawnedFloppy.transform.SetParent(Instance.floppies);
        spawnedFloppy.transform.localPosition = new Vector3(UnityEngine.Random.Range(xRange.x, xRange.y), UnityEngine.Random.Range(yRange.x, yRange.y), zValue);
        spawnedFloppy.transform.SetParent(Instance.room, true);
        spawnedFloppy.transform.localScale = scale;
        spawnedFloppy.transform.localRotation = UnityEngine.Random.rotation;

        if (text != null)
        {
            spawnedFloppy.transform.GetComponentInChildren<Text>().text = text.Substring(0, text.Length - 4);
            spawnedFloppy.transform.GetComponent<FloppyDisk>().SetState(FloppyEnum.usedsave);
        } else
        {
            spawnedFloppy.transform.GetComponentInChildren<Text>().text = "New save";
            spawnedFloppy.transform.GetComponent<FloppyDisk>().SetState(FloppyEnum.newsave);
        }

        zValue += 0.05f;
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
        SaveToFile(newPath);  
    }

    public static void SaveGameFloppy(string floppyName)
    {
        //Debug.LogError("SaveGameFloppy: " + floppyName);
        Instance.SaveToFile(path + "/" + floppyName + ".txt");
    }

    public static void DeleteSave(string floppyName)
    {
        Instance.DeleteSaveFile(path + "/" + floppyName + ".txt");
    }

    public static void NewWorld()
    {
        Instance.ResetGame();
    }

    private void SaveToFile(string path) {
        //Debug.LogError("Saving...");
        //Save all commands to a file
        if (File.Exists(path))
        {
            File.Delete(path);
        } else
        {
            //If file didn't exist, spawn new empty floppy
            SpawnFloppy();
        }

        // Create a file to write to.
        using (StreamWriter sw = File.CreateText(path))
        {
            foreach (string s in Instance.commandList)
            {
                sw.WriteLine(s);
            }
        }
        //Debug.LogError("Done saving");
    }

    private void DeleteSaveFile(string path)
    {
        if (File.Exists(path))
        {
            File.Delete(path);
        }
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
		
	public static void SpawnObjectHoverPanel(string activeParent, string resourceName, string usedName, Vector3 position){
		Debug.Log("Called SpawnObjectHoverPanel, activeParent: " + activeParent + " , resourceName: " + resourceName);
		
		JObject command = new JObject(
            new JObject(
                    new JProperty("type", "command"),
                    new JProperty("attr", new JArray(
                        new JObject(
                            new JProperty("type", "hoverspawn"),
                            new JProperty("attr", new JArray(
                                new JObject(
                                    new JProperty("activeparent", activeParent),
									new JProperty("resourcename", resourceName),
									new JProperty("usedname", usedName)
                                    ),
								new JObject(
									new JProperty("type", "position"),
									new JProperty("attr", new JArray(
										new JObject(
											new JProperty("x", position.x.ToString("G9")),
                                            new JProperty("y", position.y.ToString("G9")),
                                            new JProperty("z", position.z.ToString("G9"))
										)
									))
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

    public static void LoadSaveGame(string saveGameName)
    {
        //Debug.LogError("Load save game");
        loadPath = path + "/" + saveGameName;
        //Debug.LogError(loadPath);
        if (File.Exists(loadPath))
        {
            //Debug.LogError("Loading: " + loadPath);
            loadingGame = true;
            //SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
            Instance.ResetGame();
            Instance.StartCoroutine(Instance.ExecuteSaveFile());
        }
    }

    public void LoadGame()
    {
        //Read file & execute all commands
        //Debug.LogError("Loading...");

        if (!loadingGame)
        {

            loadPath = WriteResult(StandaloneFileBrowser.OpenFilePanel("Open File", path, "txt", false));

            if (File.Exists(loadPath))
            {
                loadingGame = true;
                //SceneManager.LoadScene("TestScene", LoadSceneMode.Single);
                ResetGame();
                StartCoroutine(ExecuteSaveFile());
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
        //Disable edit mode in ObjectManager
		ObjectManager.DisableEditMode();


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
		spawning.SpawningManager.hoverpanelCounter = 1;
        Speech.specifyDescription = false;

        //Reset shaders
        GameObject.Find("ObjectManager").GetComponent<InitializeShaders>().UpdateShaders();

        //Respawn ground
        GameObject.Find("GroundSpawner").GetComponent<GroundSpawner>().SpawnGround();

        //Go to room
        GameObject.Find("TransitionManager").GetComponent<TransitionScript>().GoToRoom();
		
		Instance.commandList.Clear();
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
					case "hoverspawn":
						string resourceName = (string)root.directAttributes["resourcename"];
						string usedName = (string)root.directAttributes["usedname"];
						string activeParent = (string)root.directAttributes["activeparent"];
						Vector3 position = GetVector(root.GetChildWithTypeValue("position"));
                        spawningManager.SpawnFromSaveFile(resourceName, activeParent, usedName, position);
						break;
                }
            } else
            {
                manager.GenerateWorldObject(root);
            }
            yield return new WaitForEndOfFrame();
        }
        loadingGame = false;
        //Debug.LogError("Done loading");
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if(loadingGame)
            StartCoroutine(ExecuteSaveFile());
    }

}
