using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using VRTK;

public class TurnController : MonoBehaviour {

    public Transform cameraRigTransform;
    [Range(15, 50)]
    public float rotationSpeed;

    private bool canTurn = true;
    //private UnityAction enableAction, disableAction;

    //private VRTK_ControllerEvents VRTKController;                       //For VRTK

    private SteamVR_TrackedObject trackedObj;                           //For legacy
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();           //For legacy
        //VRTKController = GetComponent<VRTK_ControllerEvents>();         //For VRTK

        //enableAction = new UnityAction(EnableTurn);
        //disableAction = new UnityAction(DisableTurn);
    }

    // Update is called once per frame
    void Update() {

        //if (VRTKController.GetTouchpadAxis() != Vector2.zero) {        //For VRTK
        if (Controller.GetAxis() != Vector2.zero) {                   //For legacy
            if (canTurn) {

                Vector2 turning = Controller.GetAxis();              //For legacy
                //Vector2 turning = VRTKController.GetTouchpadAxis();    //For VRTK
                TurnCamera(ref turning);
            }
        }
    }

    private void TurnCamera(ref Vector2 turning) {

        Vector3 rotation = Vector3.up * turning.x;
        cameraRigTransform.Rotate(rotation * Time.deltaTime * rotationSpeed);

    }
}
