using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LotResizer : MonoBehaviour {

    public float smallestValue = 0f;

    public GameObject leftNeighbor;
    public GameObject rightNeighbor;
    [Range(0f, 100f)]
    public float lotWidth;
    [Range(0f, 100f)]
    public float lotLength;

    private GameObject plane;

    //To test
    public float currentLength;
    public float currentWidth;

    private void Awake() {
        leftNeighbor = null;
        rightNeighbor = null;
    }

    private void Update() {
        if(lotLength != currentLength) {
            ChangeLength();
        }
        if(lotWidth != currentWidth) {
            ChangeWidth();
        }
    }

    private void ChangeLength() {
        float lengthDiff = lotLength - currentLength;
        //Growing or shrinking?
        bool growing = lengthDiff >= 0 ? true : false;

        //If you don't have a right neighbor, new center point should be relative to leftmost Z value (highest Z value)
        //Growing happens in 1 direction

        //If you don't have a left neighbor, new center point should be relative to rightmost Z value (smallest Z value)
        //Growing happens in 1 direction

        //If you have both neighbors, center point stays the same and growing happens in 2 directions
        //So what case are we?

        //If we are growing, the neighbors should shrink.
        //But first and foremost, check if the neighbors don't become smaller than 0 (or too small) !!!
        bool growthAllowed;
        Debug.Log("Length diff: " + lengthDiff);

        if (leftNeighbor == null && rightNeighbor != null) {
            Debug.Log("Left neighbor null, right exists");
            //Only 1 neighbor (right side)
            growthAllowed = !rightNeighbor.GetComponent<LotResizer>().TooSmall(lengthDiff);
            if (growthAllowed) {
                ChangeRightNeighbor(-lengthDiff);
                ChangeLengthChangingLot(-lengthDiff / 2);
            }
        } else if (leftNeighbor != null && rightNeighbor == null) {
            Debug.Log("Right neighbor null, left exists");
            //Only 1 neigbor (left side)
            growthAllowed = !leftNeighbor.GetComponent<LotResizer>().TooSmall(lengthDiff);
            if (growthAllowed) {
                ChangeLeftNeighbor(lengthDiff);
                ChangeLengthChangingLot(lengthDiff / 2);
            }
        } else if (leftNeighbor != null && rightNeighbor != null) {
            Debug.Log("Both neighbors exist");
            //We have both neighbors
            growthAllowed = !leftNeighbor.GetComponent<LotResizer>().TooSmall(lengthDiff/2);
            growthAllowed = !growthAllowed && rightNeighbor.GetComponent<LotResizer>().TooSmall(lengthDiff/2);
            if (growthAllowed) {
                ChangeLeftNeighbor(-lengthDiff/2);
                ChangeRightNeighbor(-lengthDiff/2);
                ChangeLengthSelf(-lengthDiff);
            }
        } else {
            Debug.Log("No neighbors exist");
            //We have no neighbors, can't grow
            lotLength = currentLength;
            return;
        }

        if (!growthAllowed) {
            Debug.Log("Growing no longer allowed");
            lotLength = currentLength;
            return;
        }
    }

    private void ChangeLeftNeighbor(float lengthDiff) {
        leftNeighbor.GetComponent<LotResizer>().ChangeLengthSelf(lengthDiff);
    }

    private void ChangeRightNeighbor(float lengthDiff) {
        rightNeighbor.GetComponent<LotResizer>().ChangeLengthSelf(lengthDiff);
    }

    private void ChangeLengthChangingLot(float lengthDiff) {
        this.lotLength += lengthDiff;
        currentLength = lotLength;
        Mesh planeMesh = plane.GetComponent<MeshFilter>().mesh;
        Vector3 planeSize = planeMesh.bounds.size;
        plane.transform.localScale = new Vector3(lotWidth / planeSize.x, 1f, lotLength / planeSize.z);
    }

    private void ChangeLengthSelf(float lengthDiff) {
        //Update lot position
        //if (transform.localPosition.z >= 0) {
            this.transform.localPosition = this.transform.localPosition + new Vector3(0f, 0f, lengthDiff / 2);
        //} else {
        //    this.transform.localPosition = this.transform.localPosition - new Vector3(0f, 0f, lengthDiff / 2);
        //}
        this.lotLength -= lengthDiff;
        currentLength = lotLength;
        Mesh planeMesh = plane.GetComponent<MeshFilter>().mesh;
        Vector3 planeSize = planeMesh.bounds.size;
        plane.transform.localScale = new Vector3(lotWidth / planeSize.x, 1f, lotLength / planeSize.z);
    }

    private void ChangeWidth() {
        //Update lot position
        float widthDiff = lotWidth - currentWidth;
        if (transform.localPosition.x >= 0) {
            this.transform.localPosition = this.transform.localPosition + new Vector3(widthDiff / 2, 0f, 0f);
        } else {
            this.transform.localPosition = this.transform.localPosition - new Vector3(widthDiff / 2, 0f, 0f);
        }
        //this.transform.position.x = lotWidth + widthDiff;
        currentWidth = lotWidth;
        Mesh planeMesh = plane.GetComponent<MeshFilter>().mesh;
        Vector3 planeSize = planeMesh.bounds.size;
        plane.transform.localScale = new Vector3(lotWidth / planeSize.x, 1f, lotLength / planeSize.z);
    }

    public bool TooSmall(float shrinkSize) {
        return currentLength - shrinkSize <= smallestValue;
    }

    public void SetLeftNeighbor(GameObject neighbor) {
        this.leftNeighbor = neighbor;
    }

    public void SetRightNeighbor(GameObject neighbor) {
        this.rightNeighbor = neighbor;
    }

    public void SetLotLength(float newLength) {
        this.lotLength = newLength;
        currentLength = newLength;
    }

    public void SetLotWidth(float newWidth) {
        this.lotWidth = newWidth;
        currentWidth = newWidth;
    }

    public void SpawnPlane() {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.parent = this.transform;
        plane.transform.localPosition = new Vector3(0f, 0f, 0f);
        Destroy(plane.GetComponent<Collider>());

        Mesh planeMesh = plane.GetComponent<MeshFilter>().mesh;
        plane.GetComponent<MeshRenderer>().material = Resources.Load("Outlined_Material", typeof(Material)) as Material;
        Vector3 planeSize = planeMesh.bounds.size;
        plane.transform.localScale = new Vector3(lotLength / planeSize.z, 1f, lotWidth / planeSize.x);
        this.plane = plane;
    }
}
