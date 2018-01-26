using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallStraightStreet : GenericStreet {

    public Vector3 topPoint = new Vector3(0, 0, 15f);
    public string type = "straightstreet";

    public override string GetTypePoint() {
        return type;
    }

    public override Vector3 GetTopPoint() {
        return topPoint;
    }

    public override Vector3 GetBottomPoint()
    {
        return -topPoint;
    }
}
