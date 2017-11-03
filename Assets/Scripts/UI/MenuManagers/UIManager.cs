namespace menumanager {

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    public class UIManager : MenuManager {

        public override void ToggleObjectActive(string path, bool state) {
            GameObject defaultButtons = GameObject.Find(path);
            if (defaultButtons) {
                defaultButtons.SetActive(state);
            } else {
                Debug.Log("Couldn't find object at: " + path);
            }
        }

        public override void ToggleObjectInteractable(string path, bool state) {
            GameObject defaultButtons = GameObject.Find(path);
            if (defaultButtons) {
                defaultButtons.GetComponent<Button>().interactable = state;
            } else {
                Debug.Log("Couldn't find object at: " + path);
            }
        }

        public override void UnlockButtons() {
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Unlock GameObject", false);
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Lock GameObject", true);
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Delete GameObject", false);
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Move GameObject", false);
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Rotate GameObject", false);
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Undo", false);
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Spawn GameObject", true);
        }

        public override void LockButtons() {
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Unlock GameObject", true);
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Lock GameObject", false);
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Delete GameObject", true);
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Move GameObject", true);
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Rotate GameObject", true);
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Undo", true);
            ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Spawn GameObject", false);
        }

        public override void ToggleObjectEnabled(string path, bool state, bool isCheckbox) {
            throw new System.NotImplementedException();
        }

        public override bool GetObjectValue(string path) {
            throw new System.NotImplementedException();
        }

        public override void SetObjectValue(string path, bool state) {
            throw new System.NotImplementedException();
        }

        public void BackFromSpawn() {
            Debug.Log("Called back from spawn");
            ClearScrollView();

            ToggleObjectActive("GUI/Canvas/SpawnButtons", false);
            ToggleObjectActive("GUI/Canvas/DefaultButtons", true);
        }

        private void ClearScrollView() {
            GameObject spawnButtons = GameObject.Find("GUI/Canvas/SpawnButtons/Scroll View/Viewport/Content");
            foreach (Transform t in spawnButtons.transform) {
                //Debug.Log("Destroyed " + t.name);
                Destroy(t.gameObject);
            }
        }

        public override void ChangeStatus() {
            if (GameObject.Find("GUI/Canvas/DefaultButtons/Toggle").activeSelf) {                //UI Active

                GameObject canvas = GameObject.Find("GUI/Canvas");
                foreach (Transform t in canvas.transform) {
                    if (!t.name.Equals("RawImage")) {
                        t.gameObject.SetActive(false);
                    }
                }
                GameObject defaultButtons = canvas.transform.Find("DefaultButtons").gameObject;
                defaultButtons.SetActive(true);
                if (defaultButtons.transform.Find("Toggle").GetComponent<Toggle>().isOn) {
                    defaultButtons.transform.Find("Toggle").GetComponent<Toggle>().isOn = false;
                }
                ToggleObjectActive("GUI/Canvas/DefaultButtons/Toggle", false);

            } else {                                                                             //UI Inactive
                ToggleObjectActive("GUI/Canvas/DefaultButtons/Toggle", true);
            }
        }
    }

}