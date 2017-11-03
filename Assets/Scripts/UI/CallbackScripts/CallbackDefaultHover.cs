namespace callback {

    using Hover.Core.Items.Types;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CallbackDefaultHover : CallbackDefault {
        private GameObject hoverCast;

        private void Awake() {
            Manager = GameObject.Find("HoverKit").transform.GetComponent<menumanager.HoverManager>();
        }

        public void Delete() {
            ObjectManager.DeleteCurrentObject();
            //Debug.Log("Callback Delete of object called");
        }

        private void LockObject() {
            Manager.LockButtons();
            ObjectManager.LockCurrentObject();
            //Debug.Log("Callback lock called");
        }

        private void UnlockObject() {
            ObjectManager.UnlockCurrentObject();
            Manager.UnlockButtons();
            //Debug.Log("Callback unlock called");
        }

        public void UndoChanges() {
            ObjectManager.UndoChanges();
            //Debug.Log("Callback undo changes called");
        }

        public void HoverUILockUnlock() {
            if (Manager.GetObjectValue("TransformAdjuster/Rows/MyRoot/Lock-Unlock")) {
                if (ObjectManager.CanLock()) {
                    LockObject();
                } else {
                    Manager.SetObjectValue("TransformAdjuster/Rows/MyRoot/Lock-Unlock", false);
                }
            } else {
                UnlockObject();
            }
        }

        public void ToggleMode() {
            ObjectManager.ToggleMode();

            if (ObjectManager.GetMode()) {
                    Manager.ToggleObjectEnabled("TransformAdjuster/Rows/MyRoot/Lock-Unlock", true, true);
                    Manager.ToggleObjectEnabled("TransformAdjuster/Rows/MyRoot/Spawn", true, false);
            } else {
                Manager.ToggleObjectEnabled("TransformAdjuster/Rows/MyRoot/Lock-Unlock", false, true);
                Manager.SetObjectValue("TransformAdjuster/Rows/MyRoot/Lock-Unlock", false);
                Manager.UnlockButtons();
                Manager.ToggleObjectEnabled("TransformAdjuster/Rows/MyRoot/Spawn", false, false);
            }
        }

    }

}