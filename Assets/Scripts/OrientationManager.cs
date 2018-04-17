using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientationManager : MonoBehaviour {

	public static OrientationManager Instance{ get; set;}

	[Range(1, 300)]
	float rotationSpeed = 100f;

	public Transform selectedArrow = null;
	public Transform draggedArrowInEditor = null;

	public void Awake(){
		if (!Instance)
		{
			Instance = FindObjectOfType(typeof(OrientationManager)) as OrientationManager;

			if (!Instance)
			{
				Debug.LogError("There needs to be one active OrientationManager script on a GameObject in your scene.");
			}
		}
	}


	// Update is called once per frame
	void Update () {
		if (draggedArrowInEditor) {
			Instance.SetSelectedArrow (draggedArrowInEditor, true);
			draggedArrowInEditor = null;
		}

		if (Instance.selectedArrow) {
			//Rotate the chosen arrow around it's Z-axis
			Instance.selectedArrow.Rotate (Vector3.forward * rotationSpeed * Time.deltaTime);
		}
	}

	public void SetSelectedArrow(Transform arrow, bool selectCollider){
		if (arrow.gameObject.layer != LayerMask.NameToLayer ("CanSelect") || !LegitOrientation(arrow.parent.name.ToLower())) {
			//Debug.LogError ("Case 1");
			return;
		}
		if (selectedArrow == arrow) {
			//Debug.LogError ("Case 2");
			return;
		}
		if (!selectedArrow) {
			//Debug.LogError ("Case 3");
			selectedArrow = arrow;
		} else {
			//Debug.LogError ("Case 4");
			//Reset original orientation of the previously selected arrow
			Vector3 rotation = selectedArrow.localEulerAngles;
			selectedArrow.localEulerAngles = new Vector3 (rotation.x, rotation.y, 0f);
			selectedArrow = arrow.transform;
		}

        if (selectCollider)
        {
            if (!SaveManager.loadingGame)
            {
                string arrowFullName = selectedArrow.transform.name;
                Transform parent = selectedArrow.parent;
                while (parent != null)
                {
                    arrowFullName = parent.name + "/" + arrowFullName;
                    parent = parent.parent;
                }
                SaveManager.CreateSelectArrowCommand(arrowFullName);
            }
            selectedArrow.parent.GetComponent<ColliderScript>().SelectedCollider();
        }
	}

	private bool LegitOrientation(object orientationName){
		return System.Enum.IsDefined (typeof(Orientation), orientationName);
	}

}
