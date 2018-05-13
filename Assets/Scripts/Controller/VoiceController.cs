using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceController : MonoBehaviour {

    private Speech speechManager;
    private bool enabled;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device Controller {
        get { return SteamVR_Controller.Input((int)trackedObj.index); }
    }

	// Use this for initialization
	void Awake () {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        enabled = true;

        speechManager = GameObject.Find("SpeechObject").transform.GetComponent<Speech>();
        if (!speechManager)
        {
            Debug.Log("Didn't find speech script!");
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (enabled)
        {
            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                Debug.Log("Pressed grip");
                StartCoroutine(Vibrate(0.2f, 1500));
                speechManager.BtnRecordVoice_Click();
            }
            else if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
            {
                Debug.Log("Released grip");
                StartCoroutine(Vibrate(0.2f, 1500));
                speechManager.EndAndProcessRecording();
            }
        }
        if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu))  //Teleport automatically
        {
            GameObject.Find("TransitionManager").GetComponent<TransitionScript>().Teleport();
        }
    }

    public void SetEnabled(bool value)
    {
        enabled = value;
    }

    IEnumerator Vibrate(float length, ushort strength)
    {
        for (float i = 0; i < length; i += Time.deltaTime)
        {
            Controller.TriggerHapticPulse(strength);
            yield return null;
        }
    }
}
