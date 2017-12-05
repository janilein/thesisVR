namespace spawning {
    using callback;
    using menumanager;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class SpawningManagerUI : SpawningManager {
        public GameObject button;
        //EventTrigger eventTrigger = null;

        private void Awake() {
            Manager = GameObject.Find("GUI").transform.GetComponent<UIManager>();
        }

        public void LoadButtons() {
            if (!generatedFileInfo) {
                InstantiateSpawnButtons();
            }
            LoadNextButtons(true);
        }

        protected override void LoadNextButtons(bool down) {
            FillContent(resourceFolders.Length, true); 
        }

        private void FillContent(int size, bool category) {
            GameObject scrollView = GameObject.Find("GUI/Canvas/SpawnButtons/Scroll View/Viewport/Content").gameObject;
            if (scrollView) {
                for (int y = 0; y < size; y++) {
                    GameObject buttonObj = Instantiate(button) as GameObject;
                    buttonObj.transform.SetParent(scrollView.transform, false);
                    buttonObj.transform.localScale = new Vector3(1, 1, 1);
                    Button tempButton = buttonObj.GetComponent<Button>();
                    int temp = y;
                    if (category) {
                        buttonObj.GetComponentInChildren<Text>().text = resourceFolders[y];
                        buttonObj.transform.name = resourceFolders[y];
                        buttonObj.GetComponent<PointerEventsForButton>().enabled = false;

                        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/SpawnResources/" + resourceFolders[y]);
                        FileInfo[] info = dir.GetFiles("*.*");
                        if (info.Length != 0) {
                            tempButton.onClick.AddListener(() => CategoryButtonClicked(resourceFolders[temp]));
                        } else {
                            tempButton.GetComponent<Button>().interactable = false;
                        }
             
                    } else {
                        buttonObj.GetComponentInChildren<Text>().text = activeObjects[y].name;
                        buttonObj.transform.name = y.ToString();
                        tempButton.onClick.AddListener(() => ObjectButtonClicked(temp));
                        //tempButton.OnPointerEnter
                    }
                }
            } else {
                Debug.Log("No scrollview found");
            }
        }

        private void CategoryButtonClicked(string name) {
            SetActivePart(name);
            GameObject scrollView = GameObject.Find("GUI/Canvas/SpawnButtons/Scroll View/Viewport/Content").gameObject;
            foreach (Transform t in scrollView.transform) {
                Destroy(t.gameObject);
            }
            FillContent(activeObjects.Length, false);
        }

        private void ObjectButtonClicked(int index) {
            Debug.Log("Going to call");
            ((UIManager)Manager).BackFromSpawn();
            Debug.Log("Called it");
            SpawnObject(index);

            Manager.LockButtons();
        }

        protected override void LoadNextObjects(bool down) {
        }

        public void SpawnScrollView() {
            if (!generatedFileInfo) {
                InstantiateSpawnButtons();
            }
            Manager.ToggleObjectActive("GUI/Canvas/DefaultButtons", false);
            //GameObject defaultButtons = GameObject.Find("GUI/Canvas/DefaultButtons");
            //defaultButtons.SetActive(false);
            Manager.ToggleObjectActive("GUI/Canvas/SpawnButtons", true);
            //GameObject spawnButtons = GameObject.Find("GUI/Canvas/SpawnButtons");
            //spawnButtons.SetActive(true);
            LoadNextButtons(true);
        }
    }
}
