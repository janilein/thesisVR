using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionX : GenericStreet {

    public Vector3 topPoint = new Vector3(0, 0, 15f);
    public Vector3 leftPoint = new Vector3(-15f, 0, 0f);
    public string type = "intersectionX";

    public override string GetTypePoint()
    {
        return type;
    }

    public override Vector3 GetTopPoint() {
        return topPoint;
    }

    public override Vector3 GetBottomPoint()
    {
        return -topPoint;
    }

    public override Vector3 GetLeftPoint() {
        return leftPoint;
    }

    public override Vector3 GetRightPoint()
    {
        return -leftPoint;
    }
}
