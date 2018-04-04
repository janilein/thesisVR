using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationManager : MonoBehaviour {

	[Range(1, 300)]
	float rotationSpeed = 100f;

	public Transform selectedArrow = null;
	public Transform draggedArrowInEditor = null;
	
	// Update is called once per frame
	void Update () {
		if (draggedArrowInEditor) {
			SetSelectedArrow (draggedArrowInEditor.gameObject);
			draggedArrowInEditor = null;
		}

		if (selectedArrow) {
			//Rotate the chosen arrow around it's Z-axis
			selectedArrow.Rotate (Vector3.forward * rotationSpeed * Time.deltaTime);
		}
	}

	public void SetSelectedArrow(GameObject arrow){
		if (arrow.gameObject.layer != LayerMask.NameToLayer ("CanSelect")) {
			return;
		}

		if (selectedArrow == arrow) {
			return;
		}

		if (!selectedArrow) {
			selectedArrow = arrow.transform;
		} else {
			//Reset original orientation of the previously selected arrow
			Vector3 rotation = selectedArrow.localEulerAngles;
			selectedArrow.localEulerAngles = new Vector3 (rotation.x, rotation.y, 0f);
			selectedArrow = arrow.transform;
		}

		selectedArrow.parent.GetComponent<ColliderScript>().SelectedCollider();
	}

}
