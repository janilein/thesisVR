using Hover.Core.Items.Types;
using Hover.InterfaceModules.Cast;
using menumanager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnableHovercastScripts : MonoBehaviour {

    private MenuManager hoverManager, uiManager;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        uiManager = GameObject.Find("GUI").transform.GetComponent<UIManager>();
        if (!uiManager) {
            Debug.Log("Didn't find UI Manager!");
        }
        hoverManager = GameObject.Find("HoverKit").transform.GetComponent<HoverManager>();
        if (!hoverManager) {
            Debug.Log("Didn't find hover Manager!");
        }
    }

    private void Update() {
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.ApplicationMenu)) {
            ChangeHoverStatus();
        }
    }

    public void DisableHoverMenu()
    {
        if (((HoverManager)hoverManager).GetStatus()) {
            ChangeHoverStatus();
        }
    }

    private void ChangeHoverStatus() {

        hoverManager.ChangeStatus();
        uiManager.ChangeStatus();

        /*
        if (hoverCast.activeSelf) {

            GameObject backButton;
            GameObject activeRow = hoverCast.GetComponent<HovercastInterface>().ActiveRow.gameObject;
            while (!activeRow.name.Equals("MyRoot")) {
                Debug.Log("Active row name: " + activeRow.name);
                backButton = activeRow.transform.Find("Back").gameObject;
                backButton.GetComponent<HoverItemDataSelector>().Select();
                activeRow = hoverCast.GetComponent<HovercastInterface>().ActiveRow.gameObject;
            }

            GameObject editButton = activeRow.transform.Find("Edit Mode").gameObject;
            if (editButton.GetComponent<HoverItemDataCheckbox>().Value) {

                GameObject.Find("ObjectManager/GameObject").gameObject.GetComponent<CallbackFunctionsHover>().ToggleMode();
                editButton.GetComponent<HoverItemDataCheckbox>().Value = false;
            }


            hoverCast.SetActive(false);
            GameObject.Find("GUI/Canvas/DefaultButtons/Toggle").SetActive(true);
        } else {
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
            defaultButtons.transform.Find("Toggle").gameObject.SetActive(false);

            hoverCast.SetActive(true);
        }
        */
    }
}
