using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MediumStraightStreet : GenericStreet {

    public Vector3 topPoint = new Vector3(0, 0, 30f);
    public string type = "straightstreet";

    public Vector3 colliderTopPoint;

    public MediumStraightStreet()
    {
        colliderTopPoint = new Vector3(0, 0, 1.5f);
    }

    public override void SetAllowedPoints(List<string> allowedDirections = null)
    {
		colliderAllowedPoints.Add("straight", topPoint);
		centerOffset.Add ("straight", colliderTopPoint);
    }

    public override string GetTypePoint()
    {
        return type;
    }

    public override Vector3 GetOffsetPoint()
    {
        return topPoint;
    }

    public override Vector3 GetTopPoint() {
        return topPoint;
    }

    public override Vector3 GetBottomPoint()
    {
        return -topPoint;
    }
}
