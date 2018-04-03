using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionT : GenericStreet
{
    public Vector3 topPoint = new Vector3(0, 0, 15f);
    public Vector3 leftPoint = new Vector3(15f, 0, 0);
    public Vector3 rightPoint = new Vector3(-15f, 0, 0);
	public Vector3 offsetPoint;

    public Vector3 colliderTopPoint, colliderLeftPoint, colliderRightPoint;

    public string type = "intersectionT";

    public IntersectionT()
    {
        colliderTopPoint = new Vector3(0, 0, 1.5f);
        colliderLeftPoint = new Vector3(1.5f, 0, 0);
        colliderRightPoint = new Vector3(-1.5f, 0, 0);
		offsetPoint = topPoint;
    }

    public override void SetAllowedPoints(List<string> allowedDirections = null) {
        foreach(string direction in allowedDirections)
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

	public void ChangeOrientation(string orientation){
		Debug.LogError ("Chaning T orientation to " + orientation);
		switch (orientation)
		{
		case "leftRight":
			break;
		case "leftStraight":
			topPoint = new Vector3 (15f, 0, 0);
			leftPoint = new Vector3 (0, 0, 15f);

			colliderTopPoint = new Vector3 (1.5f, 0, 0);
			colliderLeftPoint = new Vector3 (0, 0, 1.5f);
			break;
		case "rightStraight":
			rightPoint = new Vector3 (0, 0, 15f);
			topPoint = new Vector3 (-15f, 0, 0);

			colliderRightPoint = new Vector3 (0, 0, 1.5f);
			colliderTopPoint = new Vector3 (-1.5f, 0, 0);

			break;
		}
	}

    public override string GetTypePoint()
    {
        return type;
    }

    public override Vector3 GetOffsetPoint() {
		return offsetPoint;
    }

    public override Vector3 GetTopPoint()
    {
        return topPoint;
    }

    public override Vector3 GetLeftPoint()
    {
        return leftPoint;
    }

    public override Vector3 GetRightPoint()
    {
        return rightPoint;
    }

    public override void SetTopPoint(Vector3 point)
    {
        topPoint = point;
    }

    public override void SetLeftPoint(Vector3 point)
    {
        leftPoint = point;
    }

    public override void SetRightPoint(Vector3 point)
    {
        rightPoint = point;
    }
}
