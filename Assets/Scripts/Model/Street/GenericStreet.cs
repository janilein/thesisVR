using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericStreet : MonoBehaviour
{

    public Vector3 centerPoint;
    public List<KeyValuePair<string, Vector3>> allowedPoints = new List<KeyValuePair<string, Vector3>>();
    public List<KeyValuePair<string, Vector3>> rotatedPoints = new List<KeyValuePair<string, Vector3>>();

    public Vector3 spawnStart;

    public virtual void SetAllowedPoints(List<string> allowedDirections = null) { }

    public void RotateAllowedPoints(int degrees)
    {
        Vector3 point;
        foreach (KeyValuePair<string, Vector3> pair in allowedPoints)
        {
            point = pair.Value;
            Vector3 rotatedVector = Quaternion.AngleAxis(degrees, Vector3.up) * point;
            Debug.Log("Rotated vector " + point.ToString() + " by " + degrees + " resulting in " + rotatedVector.ToString());
            point = rotatedVector;
            rotatedPoints.Add(new KeyValuePair<string, Vector3>(pair.Key, point));
        }
    }

    public void SetCorrectPoint(string pointDirection)
    {

        foreach (KeyValuePair<string, Vector3> pair in rotatedPoints)
        {
            if (pair.Key.Equals(pointDirection))
            {
                spawnStart = pair.Value;
                return;
            }
        }
        Debug.Log("PointDirection given: " + pointDirection);
        foreach (KeyValuePair<string, Vector3> pair in allowedPoints)
        {
            Debug.Log("Direction in list: " + pair.Key);
        }

        throw new Exception("Given direction not found in allowedPairs");

    }

    public Vector3 GetSpawnPoint()
    {
        return spawnStart;
    }

    public Vector3 GetCenterPoint()
    {
        return centerPoint;
    }

    public void SetCenterPoint(Vector3 newCenterPoint)
    {
        centerPoint = newCenterPoint;
    }

    public virtual Vector3 GetTopPoint() { return Vector3.zero; }
    public virtual Vector3 GetBottomPoint() { return Vector3.zero; }
    public virtual Vector3 GetLeftPoint() { return Vector3.zero; }
    public virtual Vector3 GetRightPoint() { return Vector3.zero; }
    public virtual string GetTypePoint() { return null; }

    public virtual void SetTopPoint(Vector3 point) { }
    public virtual void SetBottomPoint(Vector3 point) { }
    public virtual void SetLeftPoint(Vector3 point) { }
    public virtual void SetRightPoint(Vector3 point) { }

}
