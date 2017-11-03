namespace callback {

    using Hover.Core.Items.Types;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class CallbackRotateHover : CallbackRotate {

        private HoverItemDataSlider slider;

        public void SelectedPitchSlider(IItemDataSelectable data) {
            slider = data.gameObject.GetComponent<HoverItemDataSlider>();
            if (!slider) {
                Debug.Log("Could not find the attached slider");
            } else {
                StartCoroutine("Pitch");
            }
        }

        public void DeselectedPitchSlider(IItemDataSelectable data) {
            slider = data.gameObject.GetComponent<HoverItemDataSlider>();
            if (!slider) {
                Debug.Log("Could not find the attached slider");
            } else {
                StopCoroutine("Pitch");
                slider.Value = 0.5f;
            }
        }

        public void SelectedRollSlider(IItemDataSelectable data) {
            slider = data.gameObject.GetComponent<HoverItemDataSlider>();
            if (!slider) {
                Debug.Log("Could not find the attached slider");
            } else {
                StartCoroutine("Roll");
            }
        }

        public void DeselectedRollSlider(IItemDataSelectable data) {
            slider = data.gameObject.GetComponent<HoverItemDataSlider>();
            if (!slider) {
                Debug.Log("Could not find the attached slider");
            } else {
                StopCoroutine("Roll");
                slider.Value = 0.5f;
            }
        }

        public void SelectedYawSlider(IItemDataSelectable data) {
            slider = data.gameObject.GetComponent<HoverItemDataSlider>();
            if (!slider) {
                Debug.Log("Could not find the attached slider");
            } else {
                StartCoroutine("Yaw");
            }
        }

        public void DeselectedYawSlider(IItemDataSelectable data) {
            slider = data.gameObject.GetComponent<HoverItemDataSlider>();
            if (!slider) {
                Debug.Log("Could not find the attached slider");
            } else {
                StopCoroutine("Yaw");
                slider.Value = 0.5f;
            }
        }

        private IEnumerator Roll() {
            while (true) {
                //Debug.Log("Rotate degrees: " + slider.SnappedRangeValue * Time.deltaTime);
                ObjectManager.RotateObject(new Vector3(slider.SnappedRangeValue, 0f, 0f) * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator Pitch() {
            while (true) {
                //Debug.Log("Rotate degrees: " + slider.SnappedRangeValue * Time.deltaTime);
                ObjectManager.RotateObject(new Vector3(0f, slider.SnappedRangeValue, 0f) * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator Yaw() {
            while (true) {
                //Debug.Log("Rotate degrees: " + slider.SnappedRangeValue * Time.deltaTime);
                ObjectManager.RotateObject(new Vector3(0f, 0f, slider.SnappedRangeValue) * Time.deltaTime);
                yield return null;
            }
        }
    }

}