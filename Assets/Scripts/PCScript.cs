using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PCScript : MonoBehaviour
{

    public Transform firstMovePoint;
    public Transform secondMovePoint;

    private bool floppyInserted;
    private Transform floppy;

    private Quaternion wantedRotation;
    private float rotationSpeed = 0.5f;
    private float positionSpeed = 0.5f;

    private bool rotationFinished;
    private bool movingFinished;

    private void Awake()
    {
        floppyInserted = false;
        rotationFinished = false;
        movingFinished = false;
    }

    public void InsertFloppy(Transform floppy)
    {
        Debug.LogError("Insert floppy");
        if (floppyInserted)
            return;

        this.floppy = floppy;
        floppyInserted = true;

        floppy.GetComponent<Rigidbody>().velocity = Vector3.zero;
        floppy.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        floppy.GetComponent<Rigidbody>().useGravity = false;
        floppy.GetComponent<BoxCollider>().enabled = false;
        //Start coroutine which moves & rotates the floppy disk

        //Moving to center of pc, whatever
        StartCoroutine(MoveFloppyToFirstPoint());

        //Rotating to normal stuff
        StartCoroutine(RotateFloppy());

        //SaveManager.LoadSaveGame(floppy.GetComponentInChildren<Text>().text + ".txt");
    }

    private IEnumerator RotateFloppy()
    {
        wantedRotation = this.transform.rotation;
        Quaternion originalRotation = floppy.rotation;
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * positionSpeed;
            floppy.rotation = Quaternion.Lerp(originalRotation, wantedRotation, i);
            yield return null;
        }
        if (movingFinished)
        {
            Debug.LogError("go to second, from rotate");
            StartCoroutine(MoveFloppyToSecondPoint());
        }
        rotationFinished = true;
        yield break;
    }

    private IEnumerator MoveFloppyToFirstPoint()
    {
        float i = 0;
        Vector3 originalPosition = floppy.position;
        while (i < 1)
        {
            i += Time.deltaTime * positionSpeed;
            floppy.position = Vector3.Lerp(originalPosition, firstMovePoint.position, i);
            yield return null;
        }
        if (rotationFinished)
        {
            Debug.LogError("go to second, from move");
            StartCoroutine(MoveFloppyToSecondPoint());
        }
        movingFinished = true;
        yield break;
    }

    private IEnumerator MoveFloppyToSecondPoint()
    {
        float i = 0;
        Vector3 originalPosition = floppy.position;
        while (i < 1)
        {
            i += Time.deltaTime * positionSpeed;
            floppy.position = Vector3.Lerp(originalPosition, secondMovePoint.position, i);
            yield return null;
        }
        movingFinished = false;
        rotationFinished = false;
        string saveGameName = floppy.name;
        SaveManager.LoadSaveGame(floppy.GetComponentInChildren<Text>().text + ".txt");
        floppyInserted = false;

        floppy.GetComponent<Rigidbody>().velocity = new Vector3(-1f, 0, -1f);
        floppy.GetComponent<BoxCollider>().enabled = true;
        floppy.GetComponent<Rigidbody>().useGravity = true;
    }
}
