using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionT : GenericStreet
{

    public Vector3 topPoint = new Vector3(0, 0, 15f);
    public Vector3 leftPoint = new Vector3(-15f, 0, 0);
    public Vector3 rightPoint = new Vector3(4.5f, 0, 0);
    public Vector3 bottomPoint = new Vector3(0, 0, -15f);

    public override Vector3 GetTopPoint()
    {
        return topPoint;
    }

    public override Vector3 GetBottomPoint()
    {
        return bottomPoint;
    }

    public override Vector3 GetLeftPoint()
    {
        return leftPoint;
    }

    public override Vector3 GetRightPoint()
    {
        return rightPoint;
    }

    //public override void SetTopPoint(Vector3 point) {
    //    topPoint = point;
    //}

    //public override void SetBottomPoint(Vector3 point) {
    //    bottomPoint = point;
    //}

    //public override void SetLeftPoint(Vector3 point) {
    //    leftPoint = point;
    //}

    //public override void SetRightPoint(Vector3 point) {
    //    rightPoint = point;
    //}
}
