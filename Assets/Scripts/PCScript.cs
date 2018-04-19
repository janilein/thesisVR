using System.Collections;
using UnityEngine;

public class PCScript : MonoBehaviour {
	
	private bool floppyInserted;
	private Transform floppy;
	
	private Vector3 wantedRotation = Vector3.zero;
	private float rotationSpeed = 1;
	private Vector3 wantedPosition = Vector3.zero;
	private float positionSpeed = 1;
	
	private void Awake(){
		floppyInserted = false;
	}
	
	public void InsertFloppy(Transform floppy){
		if(floppyInserted)
			return;
		
		this.floppy = floppy;
		floppyInserted = true;
			//Start coroutine which moves & rotates the floppy disk
			
			//Moving to center of pc, whatever
			StartCoroutine(MoveFloppy());
			
			//Rotating to normal stuff
			StartCoroutine(RotateFloppy());
	}
	
	private IEnumerable RotateFloppy(){
		
		yield break;
	}
	
	private IEnumerable MoveFloppy(){
		double i = 0;
		while(i < 1){
			i += Time.deltaTime * positionSpeed;
			floppy.localPosition = Vector3.Lerp(floppy.localPosition, wantedPosition, i);
			yield return null;
		}
		yield break;
	}
}
