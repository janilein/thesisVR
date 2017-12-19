using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongStraightStreet : GenericStreet {

    public Vector3 topPoint = new Vector3(0, 0, 45f);
    public Vector3 bottomPoint = new Vector3(0, 0, -45f);

    public override Vector3 GetTopPoint() {
        return topPoint;
    }

    public override Vector3 GetBottomPoint() {
        return bottomPoint;
    }
}
