using Hover.Core.Items.Types;
using Hover.InterfaceModules.Key;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PCScript : MonoBehaviour
{

    public Transform firstMovePoint;
    public Transform secondMovePoint;
    public Text visibleText;
    public HoverItemDataSelector ejectButton, loadButton, saveButton, okButton;
    public HoverkeyTextInput hoverTextInput;

    private bool floppyInserted;
    private Transform floppy;

    private Quaternion wantedRotation;
    private float rotationSpeed = 0.5f;
    private float positionSpeed = 0.5f;

    private bool rotationFinished;
    private bool movingFinished;
    private string textCommand;
    //private bool saving;

    private FloppyEnum insertedFloppyState;

    private void Awake()
    {
        floppyInserted = false;
        rotationFinished = false;
        movingFinished = false;
        textCommand = "Insert a floppy";
        visibleText.text = textCommand;
        //saving = false;
    }

    //Listener for hoverkey events
    public void KeyPressed()
    {
        //In coroutine so it can wait 1 frame, otherwise reading happens too soon
        StartCoroutine(ReadHoverText());
    }

    private IEnumerator ReadHoverText()
    {
        yield return new WaitForEndOfFrame();
        visibleText.text = textCommand + hoverTextInput.TextInput;
        yield break;
    }

    public void EjectFloppy()
    {
        if (floppyInserted)
        {
            floppyInserted = false;

            floppy.GetComponent<Rigidbody>().velocity = new Vector3(-2f, 0, -2f);
            floppy.GetComponent<BoxCollider>().enabled = true;
            floppy.GetComponent<Rigidbody>().useGravity = true;
            floppy = null;

            DisableSpecialButtons();

            textCommand = "Insert a floppy";
            visibleText.text = textCommand;
        }
    }

    public void LoadFloppy()
    {
        if (floppyInserted)
        {
            SaveManager.LoadSaveGame(floppy.GetComponentInChildren<Text>().text + ".txt");
            EjectFloppy();
        }
    }

    public void SaveFloppy()
    {
        if (floppyInserted)
        {
            //New save floppy or existing overwrite?
            if (insertedFloppyState == FloppyEnum.newsave)
            {
                //New save, first enter a name
                visibleText.text = "Enter save name \n";
                hoverTextInput.TextInput = "";
                okButton.IsEnabled = true;
            }
            else
            {
                //Overwrite older
                SaveManager.SaveGameFloppy(floppy.GetComponentInChildren<Text>().text);
                EjectFloppy();
            }
        }
    }

    public void OkButtonPressed()
    {
        //Only allow save if text input is not empty
        if(hoverTextInput.TextInput.Length > 0)
        {
            //Save to new file
            SaveManager.SaveGameFloppy(hoverTextInput.TextInput);
            floppy.GetComponentInChildren<Text>().text = hoverTextInput.TextInput;
            floppy.GetComponent<FloppyDisk>().SetState(FloppyEnum.usedsave);
            EjectFloppy();
        } else
        {
            textCommand = "Enter a valid save name \n";
            visibleText.text = textCommand;
        }
    }

    private void CheckInsertedFloppy()
    {
        insertedFloppyState = floppy.GetComponent<FloppyDisk>().GetState();
        if (insertedFloppyState == FloppyEnum.newsave)
        {
            //Debug.LogError("New save inserted");
            //Inserted a new save floppy: disable load
            loadButton.IsEnabled = false;
        } else
        {
            //Debug.LogError("Old save inserted");
            loadButton.IsEnabled = true;
        }

        okButton.IsEnabled = false;
        ejectButton.IsEnabled = true;
        saveButton.IsEnabled = true;

        textCommand = "Select a command \n";
        visibleText.text = textCommand;
    }

    public void InsertFloppy(Transform floppy)
    {
        //Debug.LogError("Insert floppy");
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
    }

    private IEnumerator RotateFloppy()
    {
        wantedRotation = this.transform.rotation;
        Quaternion originalRotation = floppy.rotation;
        float i = 0;
        while (i < 1)
        {
            i += Time.deltaTime * rotationSpeed;
            floppy.rotation = Quaternion.Lerp(originalRotation, wantedRotation, i);
            yield return null;
        }
        if (movingFinished)
        {
            //Debug.LogError("go to second, from rotate");
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
            //Debug.LogError("go to second, from move");
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

        CheckInsertedFloppy();
    }

    private void DisableSpecialButtons()
    {
        ejectButton.IsEnabled = false;
        loadButton.IsEnabled = false;
        okButton.IsEnabled = false;
        saveButton.IsEnabled = false;
        hoverTextInput.TextInput = "";
        visibleText.text = "";
        hoverTextInput.CursorIndex = 0;
    }
}
