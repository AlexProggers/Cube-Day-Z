//Ladder.cs by Azuline Studios© All Rights Reserved
//When attached to a trigger, this script can be used to create climbable surfaces.
using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {

	private GameObject playerObj;
	public bool playClimbingAudio = true;//if false, climbing footstep sounds won't be played
	
	void Start (){
		playerObj = Camera.main.transform.GetComponent<CameraKick>().playerObj;
	}

	void OnTriggerStay ( Collider other ){
		if(other.gameObject.tag == "Player"){
			FPSRigidBodyWalker FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
			FPSPlayer FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
			//on start of a collision with ladder trigger set climbing var to true on FPSRigidBodyWalker script
			FPSWalkerComponent.climbing = true;
			if(!playClimbingAudio){FPSWalkerComponent.noClimbingSfx = true;}//dont play climbing sounds if playClimbingAudio is false
			if(Mathf.Abs(FPSWalkerComponent.inputY) < 0.1f){
				if(Input.GetKey(FPSPlayerComponent.crouch)){
					//prevent player from crouching when leaving surface if they did so by holding crouch button
					FPSWalkerComponent.crouchState = false;
				}
			}
		}
	}
	
	void OnTriggerExit ( Collider other2  ){
		//on exit of a collision with ladder trigger set climbing var to false on FPSRigidBodyWalker script
		if(other2.gameObject.tag == "Player"){
			FPSRigidBodyWalker FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
			FPSWalkerComponent.climbing = false;
			//prevent player from jumping when leaving surface if they did so by holding jump button
			FPSWalkerComponent.landStartTime = Time.time + 0.25f;
			FPSWalkerComponent.jumpBtn = false;
			FPSWalkerComponent.noClimbingSfx = false;
		}
	}
	
}