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
        colliderTopPoint = topPoint + new Vector3(-1.5f, 0, 1.5f);
        colliderBottomPoint = bottomPoint + new Vector3(-1.5f, 0, -1.5f);
    }

    public override void SetAllowedPoints(List<string> allowedDirections = null)
    {
        Debug.Log("Called set allowed points @@@@@@@@@@@@@@@@");
        allowedPoints.Add(new KeyValuePair<string, Vector3>("straight", topPoint));
        colliderAllowedPoints.Add(new KeyValuePair<string, Vector3>("straight", colliderTopPoint));
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
        //topPoint = new Vector3(22.08f, 0, 33.1f);
        //topPoint = new Vector3(-7.8f, 0, 38.9f);
        //bottomPoint = new Vector3(22.08f, 0, -33.1f);
        //offsetPoint = new Vector3(-22.08f, 0, -33.1f);
        topPoint = new Vector3(22.08f, 0f, 33.1f);
        bottomPoint = new Vector3(-7.8f, 0, -38.9f);
        offsetPoint = new Vector3(7.8f, 0f, 38.9f);

        colliderTopPoint = topPoint + new Vector3(1.5f, 0, 1.5f);
        colliderBottomPoint = bottomPoint + new Vector3(-1.5f, 0, -1.5f);
    }
}
