using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionScript : MonoBehaviour {

    private GameObject room;
    private static bool isInRoom = true;
    private Transform world;
    private static float scalingFactor;
    private Vector3 roomPosition;

    [Range(0, 10f)]
    public float tableOffset;

    private void Start()
    {
        GoToRoom();
    }

    private void Awake()
    {
        scalingFactor = 100;
        world = GameObject.Find("World").transform;
        room = GameObject.Find("TheRoom");
        roomPosition = room.transform.Find("table1").transform.position + new Vector3(0.05f, tableOffset, 0.05f); //to get it just right
    }

    public void GoToRoom()
    {
        room.gameObject.SetActive(true);
        world.localScale = world.localScale / scalingFactor;
        world.position = roomPosition;
    }

    public void GoToWorld()
    {
        room.gameObject.SetActive(false);
        world.localScale = world.localScale * scalingFactor;
        world.position = Vector3.zero;
    }

    public static bool IsInRoom()
    {
        return isInRoom;
    }

    public static void SetInRoom(bool value)
    {
        isInRoom = value;
    }

    public static float GetScalingFactor()
    {
        return scalingFactor;
    }
}
