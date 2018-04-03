using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionX : GenericStreet {

    public Vector3 topPoint = new Vector3(0, 0, 15f);
    public Vector3 leftPoint = new Vector3(-15f, 0, 0);
    public Vector3 rightPoint = new Vector3(15f, 0, 0);
    public Vector3 bottomPoint = new Vector3(0, 0, -15f);
    public string type = "intersectionX";

    public Vector3 colliderTopPoint, colliderLeftPoint, colliderRightPoint, colliderBottomPoint;

    public IntersectionX()
    {
        colliderTopPoint = new Vector3(0, 0, 1.5f);
        colliderLeftPoint = new Vector3(-1.5f, 0, 0);
        colliderRightPoint = new Vector3(1.5f, 0, 0);
        colliderBottomPoint = new Vector3(0, 0, -1.5f);
    }

    public override void SetAllowedPoints(List<string> allowedDirections = null)
    {
        foreach (string direction in allowedDirections)
        {
            switch (direction)
            {
                case "left":
					colliderAllowedPoints.Add(direction, leftPoint);
					centerOffset.Add(direction, colliderLeftPoint);
                    break;
                case "right":
					colliderAllowedPoints.Add(direction, rightPoint);
					centerOffset.Add(direction, colliderRightPoint);
                    break;
                case "straight":
					colliderAllowedPoints.Add(direction, topPoint);
					centerOffset.Add(direction, colliderTopPoint);
                    break;
            }
        }
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

    public override Vector3 GetLeftPoint() {
        return leftPoint;
    }

    public override Vector3 GetRightPoint()
    {
        return -leftPoint;
    }
}
