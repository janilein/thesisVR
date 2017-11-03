using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using VRTK;

public class WalkController : MonoBehaviour {

    public Transform cameraRigTransform;
    public Transform headTransform;
    [Range(0.01f, 2f)]
    public float speed;

    public Quaternion headRotation;

    //rivate VRTK_ControllerEvents VRTKController;                       //For VRTK

    //private UnityAction enableAction, disableAction;
    private bool canWalk = true;

    private SteamVR_TrackedObject trackedObj;                           //For legacy
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

    void Awake() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();           //For legacy
        //VRTKController = GetComponent<VRTK_ControllerEvents>();         //For VRTK

        //enableAction = new UnityAction(EnableWalk);
        //disableAction = new UnityAction(DisableWalk);
    }

    // Update is called once per frame
    void Update () {


        //if (VRTKController.GetTouchpadAxis() != Vector2.zero) {        //For VRTK
        if (Controller.GetAxis() != Vector2.zero) {                   //For legacy
            if (canWalk) {
                //Debug.Log(gameObject.name + Controller.GetAxis());

                Vector2 movement = Controller.GetAxis();              //For legacy
                //Vector2 movement = VRTKController.GetTouchpadAxis();    //For VRTK
                MovePlayer(ref movement);
            }
        }
    }

    private void MovePlayer(ref Vector2 movement) {

        headRotation = headTransform.rotation;
        Vector3 moveDirection = headRotation * Vector3.forward * movement.y + headRotation * Vector3.right * movement.x;
        moveDirection.y = 0f;

        cameraRigTransform.position = cameraRigTransform.position + moveDirection * Time.deltaTime * speed;


    }

    //private void OnEnable() {
    //    EventManager.StartListening("EnableControls", enableAction);
    //    EventManager.StartListening("DisableControls", disableAction);
    //}

    //private void OnDisable() {
    //    EventManager.StopListening("EnableControls", enableAction);
    //    EventManager.StopListening("DisableControls", disableAction);
    //}

    private void DisableWalk() {
        canWalk = false;
        Debug.Log("CanWalk = false");
    }

    private void EnableWalk() {
        canWalk = true;
        Debug.Log("CanWalk = true");
    }
}
