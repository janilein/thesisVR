using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn45 : GenericStreet {

    public Vector3 topPoint = new Vector3(-22.08f, 0, 33.1f);
    public Vector3 bottomPoint = new Vector3(7.8f, 0, -38.9f);
    public Vector3 offsetPoint = new Vector3(-7.8f, 0f, 38.9f);

    public Vector3 colliderTopPoint, colliderBottomPoint;

    public Turn45()
    {
        colliderTopPoint = new Vector3(-1.5f, 0, 1.5f);
        colliderBottomPoint = new Vector3(-1.5f, 0, 0);
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

		topPoint = new Vector3(22.08f, 0, 33.1f);
		bottomPoint = new Vector3(-7.8f, 0, -38.9f);
		offsetPoint = new Vector3(7.8f, 0f, 38.9f);

		colliderTopPoint = new Vector3(1.5f, 0, 1.5f);
		colliderBottomPoint = new Vector3(1.5f, 0, 0);

        //topPoint = new Vector3(22.05f, 0f, 33.16f);
        //bottomPoint = new Vector3(-7.78f, 0, -39f);
        //offsetPoint = new Vector3(7.78f, 0f, 39f);

        //colliderTopPoint = new Vector3(1.5f, 0, 1.5f);
        //colliderBottomPoint = new Vector3(0, 0, -1.5f);
    }
}
