using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RelationManager {

    //private static RelationManager relationManager;

    //public static RelationManager instance {
    //    get {
    //        if (!relationManager)
    //        {
    //            relationManager = FindObjectOfType(typeof(RelationManager)) as RelationManager;

    //            if (!relationManager)
    //            {
    //                Debug.LogError("There needs to be one active RelationManager script on a GameObject in your scene.");
    //            }
    //        }

    //        return relationManager;
    //    }
    //}

    private static GameObject GetRelatedObject(GameObject obj)
    {
        return obj.GetComponent<ObjectRelation>().GetRelatedObject();
    }

	public static void Delete(GameObject obj)
    {
        GameObject other = GetRelatedObject(obj);
        MonoBehaviour.Destroy(obj);
        MonoBehaviour.Destroy(other);
    }

    public static void MoveObject(Vector3 movement, GameObject obj)
    {
        Transform otherTransform = GetRelatedObject(obj).transform;
        obj.transform.position = obj.transform.position + movement;
        otherTransform.position = otherTransform.position + movement;
    }

    public static void RotateObject(Vector3 rotateValue, GameObject obj)
    {
        Transform otherTransform = GetRelatedObject(obj).transform;
        obj.transform.Rotate(rotateValue);
        otherTransform.Rotate(rotateValue);
    }

    //public static void InstantiateCopy(GameObject obj)
    //{
    //    GameObject other = GetRelatedObject(obj);

    //    GameObject otherOriginalLockedObject = MonoBehaviour.Instantiate(other, other.GetComponent<Transform>().position, other.GetComponent<Transform>().rotation);
    //    otherOriginalLockedObject.transform.SetParent(other.transform.parent);
    //    otherOriginalLockedObject.SetActive(false);

    //    obj.GetComponent<ObjectRelation>().SetRelatedOriginalObject(otherOriginalLockedObject);
    //}

    public static void UndoChanges(GameObject obj)
    {
        GameObject other = GetRelatedObject(obj);
        other.SetActive(true);
        obj.SetActive(true);


    }

}
