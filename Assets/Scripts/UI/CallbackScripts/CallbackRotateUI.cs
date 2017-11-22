namespace callback {
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class CallbackRotateUI : CallbackRotate {

        private Text degreesTextRoll, degreesTextPitch, degreesTextYaw;
        private Slider sliderPitch, sliderYaw, sliderRoll;
        Vector3 originalPitchVector, previousPitch, originalYawVector, previousYaw, originalRollVector, previousRoll;
        private bool goingBackPitch, goingBackRoll, goingBackYaw = false;
        private float yawValue, rollValue, pitchValue = 0f;

        private void Awake() {
            Manager = GameObject.Find("GUI").transform.GetComponent<menumanager.UIManager>();
            
            sliderPitch = GameObject.Find("GUI/Canvas/RotateButtons/SliderPitch/Slider").GetComponent<Slider>();
            sliderRoll = GameObject.Find("GUI/Canvas/RotateButtons/SliderRoll/Slider").GetComponent<Slider>();
            sliderYaw = GameObject.Find("GUI/Canvas/RotateButtons/SliderYaw/Slider").GetComponent<Slider>();

            degreesTextPitch = GameObject.Find("GUI/Canvas/RotateButtons/SliderPitch/Text").GetComponent<Text>();
            degreesTextRoll = GameObject.Find("GUI/Canvas/RotateButtons/SliderRoll/Text").GetComponent<Text>();
            degreesTextYaw = GameObject.Find("GUI/Canvas/RotateButtons/SliderYaw/Text").GetComponent<Text>();
            originalPitchVector = previousPitch = new Vector3(0f, 0f, 0f);
        }

        public void SliderYawChanged(float degree) {
            if (!sliderYaw) {
                Debug.Log("Could not find the attached yaw slider");
            } else {
                if (!goingBackYaw) {
                    degreesTextYaw.text = sliderYaw.value + " degree(s)";
                    yawValue = sliderYaw.value;
                    StartCoroutine("Yaw");
                } else {
                    //Debug.Log("yaw true");
                    goingBackYaw = false;
                }
            }
        }

        public void SliderRollChanged(float degree) {
            if (!sliderRoll) {
                Debug.Log("Could not find the attached roll slider");
            } else {
                if (!goingBackRoll) {
                    degreesTextRoll.text = sliderRoll.value + " degree(s)";
                    rollValue = sliderRoll.value;
                    StartCoroutine("Roll");
                } else {
                    //Debug.Log("roll true");
                    goingBackRoll = false;
                }
            }
        }

        public void SliderPitchChanged(float degree) {
            if (!sliderPitch) {
                
                Debug.Log("Could not find the attached pitch slider");
            } else {
                if (!goingBackPitch) {
                    //Debug.Log("false");
                    degreesTextPitch.text = sliderPitch.value + " degree(s)";
                    pitchValue = sliderPitch.value;
                    StartCoroutine("Pitch");
                } else {
                    //Debug.Log("pitch true");
                    goingBackPitch = false;
                }
            }
        }

        private IEnumerator Pitch() {
            while (true) {
                originalPitchVector = Vector3.Lerp(previousPitch, new Vector3(pitchValue, 0f, 0f), Time.deltaTime);
                ObjectManager.RotateObject(originalPitchVector - previousPitch);
                previousPitch = originalPitchVector;

                    if (Math.Abs(pitchValue - previousPitch.x) < 0.01f) {
                        StopCoroutine("Pitch");
                    }
                yield return null;
            }
        }

        private IEnumerator Roll() {
            while (true) {
                originalRollVector = Vector3.Lerp(previousRoll, new Vector3(0f, 0f, rollValue), Time.deltaTime);
                ObjectManager.RotateObject(originalRollVector - previousRoll);
                previousRoll = originalRollVector;

                    if (Math.Abs(rollValue - previousRoll.z) < 0.01f) {
                        StopCoroutine("Roll");

                    }
                yield return null;
            }
        }

        private IEnumerator Yaw() {
            while (true) {
                originalYawVector = Vector3.Lerp(previousYaw, new Vector3(0f, yawValue, 0f), Time.deltaTime);
                ObjectManager.RotateObject(originalYawVector - previousYaw);
                previousYaw = originalYawVector;

                    if (Math.Abs(yawValue - previousYaw.y) < 0.01f) {
                        StopCoroutine("Yaw");
                    }
                yield return null;
            }
        }

        public void Back() {

            //Reset all slider values to 0
            ResetSliders();

            Manager.ToggleObjectActive("GUI/Canvas/RotateButtons", false);
            Manager.ToggleObjectActive("GUI/Canvas/DefaultButtons", true);
        }

        private void ResetSliders() {
            //Roll slider
            if(sliderRoll.value != 0f) {
                goingBackRoll = true;
                sliderRoll.value = 0f;
                degreesTextRoll.text = "0 degrees";
            }

            //Pitch slider
            if(sliderPitch.value != 0f) {
                goingBackPitch = true;
                sliderPitch.value = 0f;
                degreesTextPitch.text = "0 degrees";
            }

            //Yaw slider
            if(sliderYaw.value != 0f) {
                goingBackYaw = true;
                sliderYaw.value = 0f;
                degreesTextYaw.text = "0 degrees";
            }
        }
    }
}
