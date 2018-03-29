using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{

    private Animator _animator;

    void Start()
    {
        _animator = transform.Find("Door").GetComponent<Animator>();
    }

    public void OpenDoor()
    {
        _animator.SetBool("open", true);
    }

    public void CloseDoor()
    {
        _animator.SetBool("open", false);
        Debug.Log("BOOOOOOOOOOOOOOOOOOOOOL geset");
    }
}
