namespace callback {
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEngine;
    using UnityEngine.UI;

    public class CallbackDefaultUI : CallbackDefault {

        private void Awake() {
            Manager = GameObject.Find("GUI").transform.GetComponent<menumanager.UIManager>();
        }

        public void Delete() {
            ObjectManager.DeleteCurrentObject();
            //Debug.Log("Callback Delete of object called");
        }

        public void LockObject() {
            if (ObjectManager.CanLock()) {
                Manager.LockButtons();

                ObjectManager.LockCurrentObject();
            }
            //Debug.Log("Callback lock called");
        }

        public void UnlockObject() {
            Manager.UnlockButtons();

            ObjectManager.UnlockCurrentObject();
            //Debug.Log("Callback unlock called");
        }

        public void UndoChanges() {
            ObjectManager.UndoChanges();
            //Debug.Log("Callback undo changes called");
        }

        public void MoveObject() {
            Debug.Log("Move object: enable");
            Manager.ToggleObjectActive("GUI/Canvas/DefaultButtons", false);
            Manager.ToggleObjectActive("GUI/Canvas/MoveButtons", true);

        }

        public void RotateObject() {
            Manager.ToggleObjectActive("GUI/Canvas/DefaultButtons", false);
            Manager.ToggleObjectActive("GUI/Canvas/RotateButtons", true);
        }

        public void ToggleMode() {
            ObjectManager.ToggleMode();

            if (ObjectManager.GetMode()) {
                Manager.ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Lock GameObject", true);
                Manager.ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Spawn GameObject", true);
            } else {
                Manager.UnlockButtons();
                Manager.ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Lock GameObject", false);
                Manager.ToggleObjectInteractable("GUI/Canvas/DefaultButtons/Spawn GameObject", false);
            }
        }

        public void SpawnObject() {
            Manager.ToggleObjectActive("GUI/Canvas/DefaultButtons", false);
            Manager.ToggleObjectActive("GUI/Canvas/SpawnButtons", true);
        }
    }

}