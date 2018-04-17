namespace callback
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CallbackTest : Callback
    {
        void Awake()
        {
            Manager = GameObject.Find("GUI").transform.GetComponent<menumanager.UIManager>();
        }

        public void Back()
        {
            Manager.ToggleObjectActive("GUI/Canvas/TestButtons", false);
            Manager.ToggleObjectActive("GUI/Canvas/DefaultButtons", true);
        }
    }
}
