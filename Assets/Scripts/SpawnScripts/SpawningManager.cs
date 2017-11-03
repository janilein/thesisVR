namespace spawning {
    using Hover.Core.Items.Types;
    using Hover.Core.Layouts.Rect;
    using Hover.InterfaceModules.Panel;
    using menumanager;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    public class SpawningManager : MonoBehaviour {

        protected MenuManager Manager;

        public Transform cameraRig;

        protected Object[] activeObjects;

        protected string activePart;
        protected string[] resourceFolders;
        protected bool generatedFileInfo = false;


    protected void InstantiateSpawnButtons() {
            DirectoryInfo dir = new DirectoryInfo("Assets/Resources");
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
            activePart = newActivePart;
            LoadAllObjects();
        }

        protected virtual void LoadNextButtons(bool down) {
       
        }

        protected virtual void LoadNextObjects(bool down) {

        }

        protected void SpawnObject(int index) {
            //GameObject objectToInstantiate = Resources.Load(activePart + "/" + name, typeof(GameObject)) as GameObject;
            GameObject parent = GameObject.Find(activePart);
            if (!parent) {
                Debug.Log("Parent was null");
                new GameObject(activePart);
            } else {
                Debug.Log("Parent was not null");
            }
            GameObject instance = Instantiate(activeObjects[index], cameraRig.transform.position + cameraRig.transform.forward * 10, Quaternion.identity, GameObject.Find(activePart).transform) as GameObject;

            InitializeShaders shaderScript = GameObject.Find("ObjectManager").gameObject.GetComponent<InitializeShaders>();
            shaderScript.UpdateShaders();

            ObjectManager.SetAndLockGameObject(instance);

        }

        

    }
}
