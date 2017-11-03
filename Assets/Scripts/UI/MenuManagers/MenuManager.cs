namespace menumanager {

    using UnityEngine;

    public abstract class MenuManager : MonoBehaviour {

        public abstract void LockButtons();

        public abstract void UnlockButtons();

        public abstract void ToggleObjectActive(string path, bool state);

        public abstract void ToggleObjectInteractable(string path, bool state);

        public abstract void ToggleObjectEnabled(string path, bool state, bool isCheckbox);

        public abstract bool GetObjectValue(string path);

        public abstract void SetObjectValue(string path, bool state);

        public abstract void ChangeStatus();
    }
}
