using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn90 : GenericStreet {

    public Vector3 topPoint = new Vector3(-36.5f, 0, 14.8f);
    public Vector3 bottomPoint = new Vector3(14.8f, 0, -36.5f);
    public Vector3 offsetPoint = new Vector3(-14.8f, 0, 36.5f);

    public Vector3 colliderTopPoint, colliderBottomPoint;

    public Turn90()
    {
        colliderTopPoint = topPoint + new Vector3(-1.5f, 0, 1.5f);
        colliderBottomPoint = bottomPoint + new Vector3(-1.5f, 0, -1.5f);
    }

    public override void SetAllowedPoints(List<string> allowedDirections = null)
    {
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
        //topPoint = new Vector3(-14.8f, 0, 36.5f);
        //bottomPoint = new Vector3(-36.5f, 0, 14.8f);
        //offsetPoint = new Vector3(14.8f, 0, 35.5f);

        topPoint = new Vector3(36.5f, 0f, 14.8f);
        bottomPoint = new Vector3(-14.8f, 0f, -36.5f);
        offsetPoint = new Vector3(14.8f, 0f, 36.5f);

        //topPoint *= -1;
        //bottomPoint *= -1;
        //offsetPoint *= -1;

        colliderBottomPoint = topPoint;// + new Vector3(1.5f, 0, 1.5f);
        colliderTopPoint = bottomPoint;// + new Vector3(-1.5f, 0, -1.5f);
    }

}
