namespace spawning {
    using Hover.Core.Items.Types;
    using Hover.InterfaceModules.Panel;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.UI;

    public class SpawningManagerHover : SpawningManager {

        private GameObject hoverPanel;
        private GameObject hoverCast;
        private int objectIndex = 0;
        private int categoryIndex = 0;

        private void Awake() {
            hoverPanel = GameObject.Find("Hoverpanel");
            if (!hoverPanel) {
                Debug.Log("Did not find a hover panel");
            } else {
                hoverPanel.SetActive(false);
            }

            hoverCast = GameObject.Find("Hovercast");
            if (!hoverCast) {
                Debug.Log("Did not find a hovercast gameobject!");
            }

            Manager = GameObject.Find("HoverKit").transform.GetComponent<menumanager.HoverManager>();
        }

        public void SetActivePart(IItemDataSelectable item) {
            SetActivePart(item.Id);
            objectIndex = 0;
            LoadNextObjects(true);
        }

        public void SpawnPanel() {
            if (!generatedFileInfo) {
                InstantiateSpawnButtons();
            }
            hoverPanel.transform.position = cameraRig.transform.position + cameraRig.transform.forward * 0.65f;
            hoverPanel.transform.LookAt(cameraRig.transform);
            hoverPanel.transform.Rotate(Vector3.up, 180);
            hoverPanel.SetActive(true);
            hoverCast.SetActive(false);
            LoadNextButtons(true);
        }

        protected override void LoadNextButtons(bool down) {
            GameObject middleRow = hoverPanel.transform.Find("RowRoot/RowLayout/FirstColumn/MiddleRow").gameObject;
            if (!middleRow) {
                Debug.Log("Did not find middle row");
            } else {
                //Debug.Log("Found middle row");
            }
            int nbChildren = middleRow.transform.childCount;
            //Debug.Log("Nbbbbb of children: " + nbChildren);
            if (!down) {
                categoryIndex -= 2 * nbChildren;
            }

            foreach (Transform t in middleRow.transform) {
                HoverItemDataSelector dataSelector = t.gameObject.GetComponent<HoverItemDataSelector>();
                if (dataSelector) {
                    if (CategoryIndexInRange()) {
                        dataSelector.Id = resourceFolders[categoryIndex];
                        dataSelector.Label = resourceFolders[categoryIndex];
                        DirectoryInfo dir = new DirectoryInfo("Assets/Resources/" + resourceFolders[categoryIndex]);
                        FileInfo[] info = dir.GetFiles("*.*");
                        if (info.Length != 0) {
                            dataSelector.IsEnabled = true;
                        } else {
                            dataSelector.IsEnabled = false;
                        }

                    } else {
                        dataSelector.Id = "";
                        dataSelector.Label = "";
                        dataSelector.IsEnabled = false;

                    }
                    categoryIndex++;
                }
            }

            SelectorToggler(categoryIndex, nbChildren, resourceFolders.Length, "RowRoot/RowLayout/FirstColumn", down);
        }

        private void SelectorToggler(int index, int nbChildren, int arrayLength, string objectPath, bool down) {
            HoverItemDataSelector upSelector = hoverPanel.transform.Find(objectPath + "/Up").gameObject.GetComponent<HoverItemDataSelector>();
            if (upSelector) {
                //Debug.Log("found upselector");
            } else {
                Debug.Log("No up selector");
            }
            HoverItemDataSelector downSelector = hoverPanel.transform.Find(objectPath + "/Down").gameObject.GetComponent<HoverItemDataSelector>();
            if (downSelector) {
                //Debug.Log("found downselector");
            } else {
                Debug.Log("No down selector");
            }
            if (down) {
                if (index - nbChildren > 0) {
                    Debug.Log("Enabled up selector");
                    upSelector.IsEnabled = true;
                } else {
                    Debug.Log("Disabled up selector");
                    upSelector.IsEnabled = false;
                }

                if (index < arrayLength) {
                    Debug.Log("Enabled down selector");
                    downSelector.IsEnabled = true;
                } else {
                    Debug.Log("Disabled down selector");
                    downSelector.IsEnabled = false;
                }
            } else {
                if (index - nbChildren == 0) {
                    Debug.Log("Disabled up selector");
                    upSelector.IsEnabled = false;
                } else {
                    Debug.Log("Enabled up selector");
                    upSelector.IsEnabled = true;
                }

                if (index >= arrayLength) {
                    downSelector.IsEnabled = false;
                } else {
                    downSelector.IsEnabled = true;
                }
            }
        }

        public void ClosePanel() {
            if (hoverPanel.GetComponent<HoverpanelInterface>().ActiveRow.name.Equals("ObjectRow")) {
                GameObject activeRow = hoverPanel.GetComponent<HoverpanelInterface>().ActiveRow.gameObject;
                GameObject backButton = activeRow.transform.Find("RowLayout/ItemBack").gameObject;
                backButton.GetComponent<HoverItemDataSelector>().Select();
            }
            categoryIndex = 0;
            hoverPanel.SetActive(false);
            hoverCast.SetActive(true);
        }

        public void CategoryMenuGoUp() {
            LoadNextButtons(false);
        }

        public void CategoryMenuGoDown() {
            LoadNextButtons(true);
        }

        public void ObjectMenuGoUp() {
            LoadNextObjects(false);
        }

        public void ObjectMenuGoDown() {
            LoadNextObjects(true);
        }

        protected override void LoadNextObjects(bool down) {
            GameObject middleRow = hoverPanel.transform.Find("ObjectRow/RowLayout/FirstColumn/MiddleRow").gameObject;
            if (!middleRow) {
                Debug.Log("Did not find middle row");
            } else {
                //Debug.Log("Found middle row");
            }

            int nbChildren = middleRow.transform.childCount;
            //Debug.Log("Nbbbbb of children: " + nbChildren);
            if (!down) {
                objectIndex -= 2 * nbChildren;
            }

            foreach (Transform t in middleRow.transform) {
                HoverItemDataSelector dataSelector = t.gameObject.GetComponent<HoverItemDataSelector>();
                if (dataSelector) {
                    if (ObjectIndexInRange()) {
                        dataSelector.Id = objectIndex.ToString();
                        dataSelector.Label = ((GameObject)activeObjects[objectIndex]).name;
                        dataSelector.IsEnabled = true;

                        string loadPath = activePart + "/" + ((GameObject)activeObjects[objectIndex]).name;
                        var sprite = Resources.Load<Sprite>(loadPath);
                        if (!sprite) {
                            Debug.Log("Sprite null");
                        } else {
                            GameObject image = t.Find("HoverAlphaButtonRectRenderer-Default/Canvas/Image").gameObject;
                            image.SetActive(true);
                            image.GetComponent<Image>().sprite = sprite;
                            image.GetComponent<SpriteRenderer>().sprite = sprite;
                        }

                    } else {
                        dataSelector.Id = "";
                        dataSelector.Label = "";
                        dataSelector.IsEnabled = false;
                        GameObject image = t.Find("HoverAlphaButtonRectRenderer-Default/Canvas/Image").gameObject;
                        image.SetActive(false);

                    }
                    objectIndex++;
                }
            }

            SelectorToggler(objectIndex, nbChildren, activeObjects.Length, "ObjectRow/RowLayout/FirstColumn", down);
        }

        private bool ObjectIndexInRange() {
            return objectIndex < activeObjects.Length;
        }

        protected bool CategoryIndexInRange() {
            return categoryIndex < resourceFolders.Length;
        }

        public void SpawnObjectHover(IItemDataSelectable item) {
            SpawnObject(System.Int32.Parse(item.Id));

            Manager.SetObjectValue("TransformAdjuster/Rows/MyRoot/Edit Mode", true);
            Manager.SetObjectValue("TransformAdjuster/Rows/MyRoot/Lock-Unlock", true);
            //hoverCast.transform.Find("TransformAdjuster/Rows/MyRoot/Edit Mode").gameObject.GetComponent<HoverItemDataCheckbox>().Value = true;
            //hoverCast.transform.Find("TransformAdjuster/Rows/MyRoot/Lock-Unlock").gameObject.GetComponent<HoverItemDataCheckbox>().Value = true;

            ClosePanel();
            Manager.LockButtons();
            //CallbackFunctionsHover callback = GameObject.Find("ObjectManager/GameObject").gameObject.GetComponent<CallbackFunctionsHover>();
            //callback.LockButtons();
        }
    }
}
