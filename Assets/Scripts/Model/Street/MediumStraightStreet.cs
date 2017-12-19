using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumStraightStreet : GenericStreet {

    public Vector3 topPoint = new Vector3(0, 0, 30f);
    public Vector3 bottomPoint = new Vector3(0, 0, -30f);

    public override Vector3 GetTopPoint() {
        return topPoint;
    }

    public override Vector3 GetBottomPoint() {
        return bottomPoint;
    }
}
