using Hover.Core.Items.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallbackMoveHover : MonoBehaviour {

    private HoverItemDataSlider slider;

    public void SelectedUpDownSlider(IItemDataSelectable data) {
        slider = data.gameObject.GetComponent<HoverItemDataSlider>();
        if (!slider) {
            Debug.Log("Could not find the attached up/down slider");
        } else {
            StartCoroutine("MoveUpDown");
            //Debug.Log("Started coroutine up/down");
        }
    }

    public void DeselectedUpDownSlider(IItemDataSelectable data) {
        slider = data.gameObject.GetComponent<HoverItemDataSlider>();
        if (!slider) {
            Debug.Log("Could not find the attached up/down slider");
        } else {
            StopCoroutine("MoveUpDown");
            slider.Value = 0.5f;
            //Debug.Log("Stopped coroutine up/down");
        }
    }

    public void SelectedLeftRightSlider(IItemDataSelectable data) {
        slider = data.gameObject.GetComponent<HoverItemDataSlider>();
        if (!slider) {
            Debug.Log("Could not find the attached left/right slider");
        } else {
            StartCoroutine("MoveLeftRight");
            //Debug.Log("Started coroutine left/right");
        }
    }

    public void DeselectedLeftRightSlider(IItemDataSelectable data) {
        slider = data.gameObject.GetComponent<HoverItemDataSlider>();
        if (!slider) {
            Debug.Log("Could not find the attached left/right slider");
        } else {
            StopCoroutine("MoveLeftRight");
            slider.Value = 0.5f;
            //Debug.Log("Stopped coroutine left/right");
        }
    }

    public void SelectedNearFarSlider(IItemDataSelectable data) {
        slider = data.gameObject.GetComponent<HoverItemDataSlider>();
        if (!slider) {
            Debug.Log("Could not find the attached near/far slider");
        } else {
            StartCoroutine("MoveNearFar");
            //Debug.Log("Started coroutine near/far");
        }
    }

    public void DeselectedNearFarSlider(IItemDataSelectable data) {
        slider = data.gameObject.GetComponent<HoverItemDataSlider>();
        if (!slider) {
            Debug.Log("Could not find the attached near/far slider");
        } else {
            StopCoroutine("MoveNearFar");
            slider.Value = 0.5f;
            //Debug.Log("Stopped coroutine near/far");
        }
    }

    private IEnumerator MoveNearFar() {
        while (true) {
            //Debug.Log("Move near/far: " + slider.SnappedRangeValue * Time.deltaTime);
            ObjectManager.MoveObject(slider.SnappedRangeValue * Time.deltaTime, 0f, 0f);
            yield return null;
        }
    }

    private IEnumerator MoveUpDown() {
        while (true) {
            //Debug.Log("Move up/down: " + slider.SnappedRangeValue * Time.deltaTime);
            ObjectManager.MoveObject(0f ,slider.SnappedRangeValue * Time.deltaTime, 0f);
            yield return null;
        }
    }

    private IEnumerator MoveLeftRight() {
        while (true) {
            //Debug.Log("Rotate left/right: " + slider.SnappedRangeValue * Time.deltaTime);
            ObjectManager.MoveObject(0f, 0f, slider.SnappedRangeValue * Time.deltaTime);
            yield return null;
        }
    }
}
