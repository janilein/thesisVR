using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionX : GenericStreet {

    public Vector3 topPoint = new Vector3(0, 0, 15f);
    public Vector3 leftPoint = new Vector3(-15f, 0, 0f);
    public string type = "intersectionX";

    public override void SetAllowedPoints(List<string> allowedDirections = null)
    {
        foreach (string direction in allowedDirections)
        {
            switch (direction)
            {
                case "left":
                    allowedPoints.Add(new KeyValuePair<string, Vector3>(direction, leftPoint));
                    break;
                case "right":
                    allowedPoints.Add(new KeyValuePair<string, Vector3>(direction, -leftPoint));
                    break;
                case "straight":
                    allowedPoints.Add(new KeyValuePair<string, Vector3>(direction, topPoint));
                    break;
            }
        }
    }

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
