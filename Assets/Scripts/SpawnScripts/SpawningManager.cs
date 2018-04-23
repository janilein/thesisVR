namespace spawning {
    using Hover.Core.Items.Types;
    using Hover.Core.Layouts.Rect;
    using Hover.InterfaceModules.Panel;
    using menumanager;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.UI;

    public class SpawningManager : MonoBehaviour {

        protected MenuManager Manager;

        public Transform cameraRig;

        protected Object[] activeObjects;

        protected string activePart;
        protected string activeParent;
        protected string[] resourceFolders;
        protected bool generatedFileInfo = false;
		public static int hoverpanelCounter = 1;


    protected void InstantiateSpawnButtons() {
            DirectoryInfo dir = new DirectoryInfo("Assets/Resources/SpawnResources");
            FileInfo[] info = dir.GetFiles("*.*");
            resourceFolders = new string[info.Length];

            int index = 0;
            foreach (FileInfo f in info) {
                resourceFolders[index++] = f.Name.Substring(0, f.Name.Length - 5);
                //Debug.Log(resourceFolders[index - 1]);
            }
            generatedFileInfo = true;
        }

        protected void LoadAllObjects() {
            activeObjects = Resources.LoadAll(activePart, typeof(GameObject));
            Debug.Log("Objects size: " + activeObjects.Length);
        }

        protected void SetActivePart(string newActivePart) {
            activePart = "SpawnResources/" + newActivePart;
            activeParent = newActivePart;
            LoadAllObjects();
        }

        public string GetActivePart() {
            return this.activePart;
        }

        protected virtual void LoadNextButtons(bool down) {
       
        }

        protected virtual void LoadNextObjects(bool down) {

        }

        protected void SpawnObject(int index) {
            //GameObject objectToInstantiate = Resources.Load(activePart + "/" + name, typeof(GameObject)) as GameObject;
			/*
            GameObject parent = GameObject.Find(activeParent);
            if (!parent) {
                Debug.Log("Parent was null");
                new GameObject(activeParent);
            } else {
                Debug.Log("Parent was not null");
            }
			*/
            //GameObject instance = Instantiate(activeObjects[index], cameraRig.transform.position + cameraRig.transform.forward * 10, Quaternion.identity, GameObject.Find(activeParent).transform) as GameObject;
			
			//Full name of object to spawn (starting from SpawnResources) = activeParent + resourceFolders[index];
			Object objectToSpawn = activeObjects[index];
            Vector3 position = cameraRig.transform.position + cameraRig.transform.forward * 10;
            string usedName = SpawnObject(objectToSpawn, position);
		
			//Send info to SaveManager
			SaveManager.SpawnObjectHoverPanel(activePart, objectToSpawn.name, usedName, position);
		
        }
		
		public string SpawnObject(Object obj, Vector3 position, string name = null){
			GameObject instance = Instantiate(obj, position, Quaternion.identity) as GameObject;
			
            InitializeShaders shaderScript = GameObject.Find("ObjectManager").gameObject.GetComponent<InitializeShaders>();
            shaderScript.UpdateShaders();
			
			//Are we spawning a building? If so, set the position and orientation on a lot (if selected)
			if(activePart.ToLower().Contains("buildings")){
				//Building
				GameObject lot = LotManager.getLot();
				if(lot){
					instance.transform.position = Vector3.zero;
                    //Quaternion rotation = instance.transform.rotation;
					instance.transform.SetParent(lot.transform, false);
                    //instance.transform.rotation = rotation;
                    instance.transform.localRotation = Quaternion.Euler(0, LotManager.GetDirection(), 0);
                } else {
					instance.transform.SetParent(GameObject.Find("World").transform, false);
				}
				
				//Automatically deselect the lot
				LotManager.DeselectLot();
				
			} else {
				instance.transform.SetParent(GameObject.Find("World").transform, false);
			}
			
			//If given name, use it, else use counter
            //Given name also means we are loading. In this case, do not set and lock object in objectmanager
			if(name == null){
				instance.transform.name = instance.transform.name + hoverpanelCounter++;
                ObjectManager.SetAndLockGameObject(instance);
            } else {
				instance.transform.name = name;
			}

			return instance.transform.name;
		}
		
		public void SpawnFromSaveFile(string objectName, string loadedActivePart, string name, Vector3 position){
			activePart = loadedActivePart;
            string loadPath = loadedActivePart + "/" + objectName;
            Object loadedObject = Resources.Load(loadPath, typeof(GameObject));

			SpawnObject(loadedObject, position, name);
			activePart = "SpawnResources/";
			
		}

        

    }
}
