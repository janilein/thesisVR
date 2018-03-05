using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn90 : GenericStreet {

    public Vector3 topPoint = new Vector3(-36.5f, 0, 14.8f);
    public Vector3 bottomPoint = new Vector3(14.8f, 0, -36.5f);

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
        topPoint = new Vector3(36.5f, 0, 14.8f);
        bottomPoint = new Vector3(-14.8f, 0, -36.5f);
    }

}
