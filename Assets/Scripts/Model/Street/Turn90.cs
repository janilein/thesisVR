using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn90 : GenericStreet {

    public Vector3 topPoint = new Vector3(-36f, 0, 14.92f);
    public Vector3 bottomPoint = new Vector3(14.92f, 0, -36f);
    public Vector3 offsetPoint = new Vector3(-14.92f, 0, 36f);

	//public Vector3 topPoint = new Vector3 (-14.8f, -36.5f, 0);
	//public Vector3 bottomPoint = new Vector3(14.8f, 0, -36.5f);

    public Vector3 colliderTopPoint, colliderBottomPoint;

    public Turn90()
    {
        colliderTopPoint = new Vector3(-1.5f, 0, 0);
        colliderBottomPoint = new Vector3(0, 0, -1.5f);
    }

    public override void SetAllowedPoints(List<string> allowedDirections = null)
    {
		colliderAllowedPoints.Add("straight", topPoint);
		centerOffset.Add ("straight", colliderTopPoint);
    }

    public override void SetBackCollider()
    {
        colliderAllowedPoints.Add("back", bottomPoint);
        centerOffset.Add("back", colliderBottomPoint);
    }

    public override Vector3 GetTopPoint()
    {
        return topPoint;
    }

    public override Vector3 GetBottomPoint()
    {
        return bottomPoint;
    }

    public override Vector3 GetOffsetPoint()
    {
        return offsetPoint;
    }

    public override void SetRightTurn()
    {

        topPoint = new Vector3(36f, 0f, 14.92f);
        bottomPoint = new Vector3(-14.92f, 0f, -36f);
        offsetPoint = new Vector3(14.92f, 0f, 36f);

		colliderBottomPoint = new Vector3 (0, 0, 1.5f);
		colliderTopPoint = new Vector3 (1.5f, 0, 0);
    }

}
