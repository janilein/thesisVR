using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericStreet : MonoBehaviour{

    public Vector3 centerPoint;

    public Vector3 GetCenterPoint() {
        return centerPoint;
    }

    public void SetCenterPoint(Vector3 newCenterPoint) {
        centerPoint = newCenterPoint;
    }

    public virtual Vector3 GetTopPoint() { return Vector3.zero; }
    public virtual Vector3 GetBottomPoint() { return Vector3.zero; }
    public virtual Vector3 GetLeftPoint() { return Vector3.zero; }
    public virtual Vector3 GetRightPoint() { return Vector3.zero; }

    //public virtual void SetTopPoint(Vector3 point) {  }
    //public virtual void SetBottomPoint(Vector3 point) {  }
    //public virtual void SetLeftPoint(Vector3 point) {  }
    //public virtual void SetRightPoint(Vector3 point) { }
}
