using System.Collections;
using UnityEngine;

public class FloppyDisk : MonoBehaviour {
	
	private bool inComputerRange;
	
	private void Awake(){
		inComputerRange = false;
	}


	//Enter with collider of PC
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.name.Equals("PC"))
        {
            inComputerRange = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
		if (other.transform.name.Equals("PC"))
        {
            inComputerRange = false;
        }
	}
	
	public void OnRelease(){
		if(inComputerRange){
			//Tell the script on the PC to insert the floppy disk
			GameObject.Find("TheRoom/Computer").transform.GetComponent<PCScript>().InsertFloppy(this.transform);
		}
	}
}
