using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn45 : GenericStreet {

    public Vector3 topPoint = new Vector3(-22.08f, 0, 33.1f);
    public Vector3 bottomPoint = new Vector3(7.8f, 0, -38.9f);

    public override void SetAllowedPoints(List<string> allowedDirections = null)
    {
        allowedPoints.Add(new KeyValuePair<string, Vector3>("straight", topPoint));
    }

    public override Vector3 GetTopPoint()
    {
        return -bottomPoint;
    }

    public override Vector3 GetBottomPoint()
    {
        return bottomPoint;
    }

    public override void SetRightTurn()
    {
        topPoint = new Vector3(22.08f, 0, 33.1f);
        bottomPoint = new Vector3(-7.8f, 0, -38.9f);
    }
}
