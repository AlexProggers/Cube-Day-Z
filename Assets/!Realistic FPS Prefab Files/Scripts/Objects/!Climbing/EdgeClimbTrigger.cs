//EdgeClimbTrigger.cs by Azuline Studios© All Rights Reserved
//Detects when player enters trigger and pulls them up a ledge
using UnityEngine;
using System.Collections;
//allows the player to pull themselves up over ledges
public class EdgeClimbTrigger : MonoBehaviour {
	public float upwardPullForce = 0.3f;//force that pulls the player upwards when they enter the vault trigger when jumping
	private GameObject playerObj;
	
	void Start (){
		playerObj = Camera.main.transform.GetComponent<CameraKick>().playerObj;
	}

	void OnTriggerStay ( Collider other ){
		FPSRigidBodyWalker FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		if(other.gameObject.tag == "Player"){
			//apply upward velocity to player rigidbody to vault ledge
			playerObj.GetComponent<Rigidbody>().AddForce(new Vector3 (0, upwardPullForce, 0), ForceMode.VelocityChange);
			//set grounded in FPSRigidBodyWalker to true to allow the player
			//full air manipulation to move forward over ledge
			FPSWalkerComponent.climbing = true;
			FPSWalkerComponent.noClimbingSfx = true;
			FPSWalkerComponent.inputY = 1;//make player play bob cycle when climbing ledge
			FPSWalkerComponent.grounded = true;
			FPSWalkerComponent.jumpBtn = false;//prevent player from jumping once they reach top of ledge
		}
	}
	
	void OnTriggerExit( Collider other  ){
		FPSRigidBodyWalker FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		//on exit of vault trigger, deactivate trigger to prevent player from falling on it from above and hovering
		if(other.gameObject.tag == "Player"){
			FPSWalkerComponent.climbing = false;
			FPSWalkerComponent.noClimbingSfx = false;
			transform.gameObject.GetComponent<BoxCollider>().enabled = false;	
		}
	} 
}

