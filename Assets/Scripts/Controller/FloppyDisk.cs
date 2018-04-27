using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloppyDisk : MonoBehaviour {
	
	private bool inComputerRange;
    private bool inTrashCanRange;
    private FloppyEnum state;
	
	private void Awake(){
		inComputerRange = false;
        inTrashCanRange = false;
    }


	//Enter with collider of PC
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.name.Equals("Computer"))
        {
            inComputerRange = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name.Equals("Computer"))
        {
            inComputerRange = true;
        }
        if (other.transform.name.Equals("trash_can"))
        {
            SaveManager.DeleteSave(this.transform.GetComponentInChildren<Text>().text);
            SetState(FloppyEnum.newsave);
            this.transform.GetComponentInChildren<Text>().text = "New save";
        }
	}

    public void SetState(FloppyEnum newState)
    {
        this.state = newState;
    }

    public FloppyEnum GetState()
    {
        return state;
    }

    public void OnRelease(){
		if(inComputerRange){
			//Tell the script on the PC to insert the floppy disk
			GameObject.Find("TheRoom/Computer").transform.GetComponent<PCScript>().InsertFloppy(this.transform);
		}
	}
}
