using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRelation : MonoBehaviour {

    private GameObject relatedObject;
    //private GameObject relatedOriginalObject; //Bij undo changes enz

    public void SetRelatedObject(GameObject obj)
    {
        relatedObject = obj;
    }

    public GameObject GetRelatedObject()
    {
        return relatedObject;
    }

    //public void SetRelatedOriginalObject(GameObject obj)
    //{
    //    relatedOriginalObject = obj;
    //}

    //public GameObject getRelatedOriginalObject()
    //{
    //    return relatedOriginalObject;
    //}

    public void ApplyChanges()
    {
        //GameObject biggestObject = (this.transform.localScale.x > relatedObject.transform.localScale.x ? this.gameObject : relatedObject);
        float scalingFactor = GetScalingFactor();
        relatedObject.transform.rotation = this.transform.rotation;
        relatedObject.transform.position = transform.position / scalingFactor;
    }

    public void DeleteOther()
    {
        Destroy(relatedObject);
    }

    public void CreateOther(GameObject obj)
    {
        GameObject otherRelated = obj.GetComponent<ObjectRelation>().GetRelatedObject();

        relatedObject = Instantiate(otherRelated, otherRelated.transform.position, otherRelated.transform.rotation);
        relatedObject.transform.SetParent(otherRelated.transform.parent);
        relatedObject.SetActive(false);
    }

    public float GetScalingFactor()
    {
        return transform.localScale.x / relatedObject.transform.localScale.x;
    }

    public void ActivateOther()
    {
        relatedObject.SetActive(true);
    }

    public void CreateGameObject()
    {
        Transform newTrans = (new GameObject(this.transform.name + " ////TEST////")).transform;
        newTrans.gameObject.AddComponent<ObjectRelation>();
        newTrans.gameObject.GetComponent<ObjectRelation>().SetRelatedObject(this.gameObject);
        this.relatedObject = newTrans.gameObject;

        Debug.Log("Original position: " + this.transform.position.ToString());

         //newTrans.position = ObjectManager.GetScalingFactor() * this.transform.position;
       //  newTrans.localScale = this.transform.localScale / ObjectManager.GetScalingFactor();
         Debug.Log("Is in room, new position: " + newTrans.position.ToString());
    }

    public void Duplicate()
    {
        GameObject newGameObject = Instantiate(this.gameObject);
        newGameObject.GetComponent<ObjectRelation>().SetRelatedObject(this.gameObject);
        newGameObject.transform.parent = this.transform.parent.GetComponent<ObjectRelation>().GetRelatedObject().transform;
        this.relatedObject = newGameObject;
    }
}
