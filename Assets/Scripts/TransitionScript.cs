using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionScript : MonoBehaviour
{

    public Transform cameraRig;
    private GameObject room;
    private bool isInRoom = true;
    private Transform world;
    private float scalingFactor;
    private Vector3 roomPosition;
    private float transitionTime = 1.25f;
    //private bool firstTime = true;

    private DoorScript doorScript;

    [Range(0, 10f)]
    public float tableOffset;

    private void Start()
    {
        world.localScale = world.localScale / scalingFactor;
        world.localPosition = roomPosition;
        //GoToRoom();
    }

    private void Awake()
    {
        doorScript = GameObject.Find("TheRoom/door").GetComponent<DoorScript>();
        scalingFactor = 250;
        world = GameObject.Find("World").transform;
        room = GameObject.Find("TheRoom");
        roomPosition = room.transform.Find("table1").transform.position + new Vector3(0.02f, tableOffset, 0.02f); //to get it just right
        //roomPosition = room.transform.position + new Vector3(0.05f, 1.31f, 0.05f);
    }

    public void Teleport()
    {
        if (isInRoom)
        {
            GoToWorld();
        } else
        {
            GoToRoom();
        }
    }

    public void OpenDoor()
    {
        doorScript.OpenDoor();
        StartCoroutine(WaitForDoor());
    }

    public void GoToRoom()
    {
        if (!isInRoom)
        {
            StartCoroutine(WaitForSpawnInRoom());
            SteamVR_Fade.Start(Color.clear, 0f);
            SteamVR_Fade.Start(Color.white, transitionTime);
            StartCoroutine(Teleport(true));
            isInRoom = true;
        }

    }

    public void GoToWorld()
    {
        if (isInRoom)
        {
            SteamVR_Fade.Start(Color.clear, 0f);
            SteamVR_Fade.Start(Color.white, transitionTime);
            StartCoroutine(Teleport(false));

        }
        isInRoom = false;
    }

    private IEnumerator WaitForDoor()
    {
        yield return new WaitForSeconds(1f);
        GoToWorld();
    }

    private IEnumerator WaitForSpawnInRoom()
    {
        yield return new WaitForSeconds(3f);
        doorScript.CloseDoor();
    }

    private IEnumerator Teleport(bool active)
    {
        yield return new WaitForSeconds(transitionTime);
        room.gameObject.SetActive(active);
        if (active)
        {
            world.localScale = world.localScale / scalingFactor;
            world.localPosition = roomPosition;

        }
        else
        {
            world.localScale = world.localScale * scalingFactor;
            world.localPosition = Vector3.zero;
        }
        cameraRig.position = Vector3.zero;
        SteamVR_Fade.Start(Color.white, 0f);
        SteamVR_Fade.Start(Color.clear, transitionTime);
    }

    public bool IsInRoom()
    {
        return isInRoom;
    }

    public void SetInRoom(bool value)
    {
        isInRoom = value;
    }

    public float GetScalingFactor()
    {
        return scalingFactor;
    }
}
