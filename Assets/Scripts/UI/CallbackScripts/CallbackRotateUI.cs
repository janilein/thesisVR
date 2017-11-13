namespace callback {
    using System;
    using System.Collections;
    using UnityEngine;
    using UnityEngine.UI;

    public class CallbackRotateUI : CallbackRotate {

        private Text degreesTextRoll, degreesTextPitch, degreesTextYaw;
        private Slider sliderPitch, sliderYaw, sliderRoll;
        Vector3 originalPitchVector, previousPitch, originalYawVector, previousYaw, originalRollVector, previousRoll;

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

        public void SliderYawChanged() {
            if (!sliderYaw) {
                Debug.Log("Could not find the attached yaw slider");
            } else {
                degreesTextYaw.text = sliderYaw.value + " degree(s)";
                StartCoroutine("Yaw");
            }
        }

        public void SliderRollChanged() {
            if (!sliderRoll) {
                Debug.Log("Could not find the attached roll slider");
            } else {
                degreesTextRoll.text = sliderRoll.value + " degree(s)";
                StartCoroutine("Roll");
            }
        }

        public void SliderPitchChanged() {
            if (!sliderPitch) {
                
                Debug.Log("Could not find the attached pitch slider");
            } else {
                degreesTextPitch.text = sliderPitch.value + " degree(s)";
                StartCoroutine("Pitch");
            }
        }

        private IEnumerator Pitch() {
            while (true) {
                originalPitchVector = Vector3.Lerp(previousPitch, new Vector3(sliderPitch.value, 0f, 0f), Time.deltaTime);
                ObjectManager.RotateObject(originalPitchVector - previousPitch);
                previousPitch = originalPitchVector;

                    if (Math.Abs(sliderPitch.value - previousPitch.x) < 0.01f) {
                        StopCoroutine("Pitch");
                    }
                yield return null;
            }
        }

        private IEnumerator Roll() {
            while (true) {
                originalRollVector = Vector3.Lerp(previousRoll, new Vector3(0f, 0f, sliderRoll.value), Time.deltaTime);
                ObjectManager.RotateObject(originalRollVector - previousRoll);
                previousRoll = originalRollVector;

                    if (Math.Abs(sliderRoll.value - previousRoll.z) < 0.01f) {
                        StopCoroutine("Roll");

                    }
                yield return null;
            }
        }

        private IEnumerator Yaw() {
            while (true) {
                originalYawVector = Vector3.Lerp(previousYaw, new Vector3(0f, sliderYaw.value, 0f), Time.deltaTime);
                ObjectManager.RotateObject(originalYawVector - previousYaw);
                previousYaw = originalYawVector;

                    if (Math.Abs(sliderYaw.value - previousYaw.y) < 0.01f) {
                        StopCoroutine("Yaw");
                    }
                yield return null;
            }
        }

        public void Back() {

            Manager.ToggleObjectActive("GUI/Canvas/RotateButtons", false);
            Manager.ToggleObjectActive("GUI/Canvas/DefaultButtons", true);
        }
    }
}
