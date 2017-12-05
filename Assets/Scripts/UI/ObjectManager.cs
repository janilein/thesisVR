using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour {

    public Transform headTransform;

    public GameObject previousObject;
    public GameObject currentObject;
    public GameObject lockedObject;
    public GameObject originalLockedObject;    //To undo changes
    private Quaternion headRotation;

    public bool editMode = false;

    private bool lockedObjectDeleted = false;

    private static ObjectManager objectManager;

    public static ObjectManager instance {
        get {
            if (!objectManager) {
                objectManager = FindObjectOfType(typeof(ObjectManager)) as ObjectManager;

                if (!objectManager) {
                    Debug.LogError("There needs to be one active ObjectManger script on a GameObject in your scene.");
                }
            }

            return objectManager;
        }
    }

    public static void ToggleMode() {
        Debug.Log("In toggle mode");
        instance.editMode = !instance.editMode;
        if (instance.editMode == false) {
            UnlockCurrentObject();
        }
        Debug.Log("Done with toggle mode");
    }

    public static bool GetMode() {
        return instance.editMode;
    }

    public static void DeleteCurrentObject() {
        //instance.currentObject.SetActive(false);
        Destroy(instance.lockedObject);
        instance.lockedObject = null;
        instance.lockedObjectDeleted = true;
    }

    public static void SetAndLockGameObject(GameObject newObject) {
        instance.previousObject = null;
        if (instance.currentObject && instance.currentObject.tag.Equals("Highlightable")) {
            instance.currentObject.GetComponent<SelectableObject>().RemoveHighlight();
        }
        instance.currentObject = newObject;
        LockCurrentObject();

        if (instance.currentObject && instance.currentObject.tag.Equals("Highlightable")) {
            instance.currentObject.GetComponent<SelectableObject>().HightlightObject();
        }
    }

    public static void SetGameObject(GameObject newObject) {
        if (instance.editMode) {
            instance.previousObject = instance.currentObject;
            instance.currentObject = newObject;
            //Debug.Log("GameObject set");

            if (!instance.lockedObject && !instance.lockedObjectDeleted) {
                //Debug.Log("lockedObject not null");
                if (instance.currentObject != instance.previousObject) {
                    //Debug.Log("current not null & not equal to previous");
                    if (instance.previousObject && instance.previousObject.tag.Equals("Highlightable")) {
                        instance.previousObject.GetComponent<SelectableObject>().RemoveHighlight();
                    }
                    if (instance.currentObject && instance.currentObject.tag.Equals("Highlightable")) {
                        instance.currentObject.GetComponent<SelectableObject>().HightlightObject();
                    }
                }
            }
        }
    }

    public static GameObject GetGameObject() {
        return instance.currentObject;
    }

    public static bool CanLock() {
        return (instance.currentObject) ? true : false;
    }

    public static void LockCurrentObject() {
        Debug.Log("Called lock current object");
        if (instance.currentObject) {
            if (!instance.lockedObject) {
                instance.headRotation = instance.headTransform.rotation;
                instance.lockedObject = instance.currentObject;

                InstantiateCopy();

                instance.lockedObjectDeleted = false;
            }
        }
    }

    public static void UnlockCurrentObject() {
        Debug.Log("Called unlock current object");
        if (instance.lockedObject || instance.originalLockedObject) {
            if (instance.lockedObject && instance.lockedObject.tag.Equals("Highlightable")) {
                instance.lockedObject.GetComponent<SelectableObject>().RemoveHighlight();
            }
            instance.lockedObject = null;
            Destroy(instance.originalLockedObject);
        }
        if (instance.currentObject && instance.currentObject.tag.Equals("Highlightable")) {
            instance.currentObject.GetComponent<SelectableObject>().RemoveHighlight();
        }
        instance.currentObject = null;
        if (instance.lockedObjectDeleted) {
            instance.gameObject.GetComponent<InitializeShaders>().UpdateObjects();
        }
        instance.lockedObjectDeleted = false;
    }

    public static void MoveObject(float x, float y, float z) {
        Vector3 movement;
        
        movement= instance.headRotation * Vector3.forward * x + instance.headRotation * Vector3.left * z;
        movement.y = y;
        instance.lockedObject.transform.position = instance.lockedObject.transform.position + movement;
    }

    public static void RotateObject(Vector3 rotateValue) {
        if (instance.lockedObject) {
            instance.lockedObject.transform.Rotate(rotateValue);
        }
    }

    public static void UndoChanges() {
        Debug.Log("Called undo changes");
        if (instance.lockedObject || instance.lockedObjectDeleted) {

            Destroy(instance.lockedObject);
            instance.lockedObject = instance.originalLockedObject;
            instance.currentObject = instance.originalLockedObject;
            instance.lockedObject.SetActive(true);
            string name = instance.originalLockedObject.name;
            instance.lockedObject.name = name.Substring(0, name.Length - 7);

            InstantiateCopy();

            if (instance.currentObject && instance.currentObject.tag.Equals("Highlightable")) {
                        instance.currentObject.GetComponent<SelectableObject>().HightlightObject();
                    }

            instance.lockedObjectDeleted = false;
        }
    }

    private static void InstantiateCopy() {
        instance.originalLockedObject = Instantiate(instance.lockedObject, instance.lockedObject.GetComponent<Transform>().position, instance.lockedObject.GetComponent<Transform>().rotation);
        instance.originalLockedObject.transform.SetParent(instance.lockedObject.transform.parent);
        instance.originalLockedObject.SetActive(false);
    }

    
}
