namespace callback {
    using menumanager;
    using UnityEngine;

    public class CallbackSpawnFunctions : Callback {

        private GameObject[] buttons;

        private void Awake() {
            Manager = GameObject.Find("GUI").transform.GetComponent<UIManager>();
        }

        public void Back() {
            ((UIManager)Manager).BackFromSpawn();
        }
    }
}
