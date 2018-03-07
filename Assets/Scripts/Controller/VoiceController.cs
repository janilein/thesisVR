using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceController : MonoBehaviour {

    private Speech speechManager;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

	// Use this for initialization
	void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();

        speechManager = GameObject.Find("SpeechObject").transform.GetComponent<Speech>();
        if (!speechManager)
        {
            Debug.Log("Didn't find speech script!");
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("Pressed grip");
            speechManager.BtnRecordVoice_Click();
        } else if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            Debug.Log("Released grip");
            speechManager.EndAndProcessRecording();
        }
    }
}
