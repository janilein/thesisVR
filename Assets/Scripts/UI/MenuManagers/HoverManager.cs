namespace menumanager {
    using callback;
    using Hover.Core.Items.Types;
    using Hover.InterfaceModules.Cast;
    using UnityEngine;

    public class HoverManager : MenuManager {

        public GameObject hoverCast;
        public GameObject hoverCastRows;

        private void Awake() {
            hoverCast = GameObject.Find("Hovercast");
            if (!hoverCast) {
                Debug.Log("Did not find a Hovercast game object!");
            } else {
                hoverCastRows = hoverCast.transform.Find("TransformAdjuster").gameObject;
                if (!hoverCast) {
                    Debug.Log("Did not find a Hovercast transformadjuster game object!");
                } else {
                    hoverCastRows = hoverCastRows.transform.Find("Rows").gameObject;
                    if (!hoverCast) {
                        Debug.Log("Did not find a Hovercast rows game object!");
                    } else {
                        //Debug.Log("found all");
                    }
                }
            }
        }


        private void Start() {
            if (hoverCast) {
                hoverCast.SetActive(false);
            }
        }

        public override void UnlockButtons() {
            //Lock certain buttons on Hover UI
            if (hoverCastRows) {
                hoverCastRows.transform.Find("MyRoot/Undo").GetComponent<HoverItemDataSelector>().IsEnabled = false;
                hoverCastRows.transform.Find("MyRoot/Delete").GetComponent<HoverItemDataSelector>().IsEnabled = false;
                hoverCastRows.transform.Find("MyRoot/Change").GetComponent<HoverItemDataSelector>().IsEnabled = false;
                hoverCastRows.transform.Find("MyRoot/Spawn").GetComponent<HoverItemDataSelector>().IsEnabled = true;
            }
        }

        public override void LockButtons() {
            //Enable certain buttons on the Hover UI
            if (hoverCastRows) {
                hoverCastRows.transform.Find("MyRoot/Undo").GetComponent<HoverItemDataSelector>().IsEnabled = true;
                hoverCastRows.transform.Find("MyRoot/Delete").GetComponent<HoverItemDataSelector>().IsEnabled = true;
                hoverCastRows.transform.Find("MyRoot/Change").GetComponent<HoverItemDataSelector>().IsEnabled = true;
                hoverCastRows.transform.Find("MyRoot/Spawn").GetComponent<HoverItemDataSelector>().IsEnabled = false;

            }
        }

        public override void ToggleObjectEnabled(string path, bool state, bool isCheckbox) {
            if (isCheckbox) {
                hoverCast.transform.Find(path).GetComponent<HoverItemDataCheckbox>().IsEnabled = state;
             } else {
                hoverCast.transform.Find(path).GetComponent<HoverItemDataSelectable>().IsEnabled = state;
            }

        }

        public override bool GetObjectValue(string path) {
            return hoverCast.transform.Find(path).GetComponent<HoverItemDataCheckbox>().Value;
        }

        public override void SetObjectValue(string path, bool state) {
            hoverCast.transform.Find(path).GetComponent<HoverItemDataCheckbox>().Value = state;
        }

        public override void ToggleObjectActive(string path, bool state) {
            throw new System.NotImplementedException();
        }

        public override void ToggleObjectInteractable(string path, bool state) {
            throw new System.NotImplementedException();
        }

        public override void ChangeStatus() {
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

                    GameObject.Find("ObjectManager/GameObject").gameObject.GetComponent<CallbackDefaultHover>().ToggleMode();
                    editButton.GetComponent<HoverItemDataCheckbox>().Value = false;
                }


                hoverCast.SetActive(false);
                //Debug.Log("Set hovercast inactive");
                
            } else {
                hoverCast.SetActive(true);
                //Debug.Log("Set hovercast active");
            }
        }

        public bool GetStatus()
        {
            return hoverCast.activeSelf;
        }

    }
}
