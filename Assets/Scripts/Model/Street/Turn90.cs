using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn90 : GenericStreet {

    public Vector3 topPoint = new Vector3(-36f, 0, 14.92f);
    public Vector3 bottomPoint = new Vector3(14.92f, 0, -36f);
    public Vector3 offsetPoint = new Vector3(-14.92f, 0, 36f);
	
	private bool isRightTurn = false;

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
	
	public override Vector2 GetDirection(bool back){
		if(back == false)
			return direction;
		
		//Change direction to match the original direction
		Vector2 newDirection = direction;
		int sign = isRightTurn ? -1 : 1;
		newDirection.x = newDirection.x + sign * 90;
		
		return newDirection;
	}

    public override void SetRightTurn()
    {
		isRightTurn = true;

        topPoint = new Vector3(36f, 0f, 14.92f);
        bottomPoint = new Vector3(-14.92f, 0f, -36f);
        offsetPoint = new Vector3(14.92f, 0f, 36f);

		colliderBottomPoint = new Vector3 (0, 0, -1.5f);
		colliderTopPoint = new Vector3 (1.5f, 0, 0);
    }

}
