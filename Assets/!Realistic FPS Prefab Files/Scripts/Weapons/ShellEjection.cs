//ShellEjection.cs by Azuline Studios© All Rights Reserved
//Rotates and moves instantiated rigidbody shell object and lerps mesh shell object.
using UnityEngine;

public class ShellEjection : MonoBehaviour
{
	//objects accessed by this script
	[HideInInspector]
	public GameObject playerObj;
	[HideInInspector]
	public GameObject gunObj;
	[HideInInspector]
	public Transform lerpShell;//the mesh shell object that lerps the rigidbody shell's position and rotation
	private Vector3 tempPos;
	private Vector3 tempRot;
	private bool rotated;
	private Transform myTransform;
	private Transform playerObjTransform;
	private Transform FPSMainTransform;
	public AudioClip[] shellSounds;//shell bounce sounds
	//shell states and settings
	private bool parentState = true;
	private bool soundState = true;
	//shell rotation
	private float rotateAmt = 0.0f;//amount that the shell rotates, scaled up after ejection
	[HideInInspector]
	public float shellRotateUp = 0.0f;//amount of vertical shell rotation
	[HideInInspector]
	public float shellRotateSide = 0.0f;//amount of horizontal shell rotation	
	//timers and shell lifetime duration
	private float shellRemovalTime = 0.0f;//time that this shell will be removed from the level
	[HideInInspector]
	public int shellDuration = 0;//time in seconds that shells persist in the world before being removed	
	private float startTime = 0.0f;//time that the shell instance was created in the world

	void Start(){
		//set up external script references
		WeaponBehavior WeaponBehaviorComponent = gunObj.GetComponent<WeaponBehavior>();
		myTransform = transform;//manually set transform for efficiency
		playerObjTransform = playerObj.transform;
		//initialize shell rotation amounts
		shellRotateUp = WeaponBehaviorComponent.shellRotateUp / (Time.fixedDeltaTime * 100.0f);
		shellRotateSide = WeaponBehaviorComponent.shellRotateSide / (Time.fixedDeltaTime * 100.0f);
		shellDuration = WeaponBehaviorComponent.shellDuration;
		//track the time that the shell was ejected
		startTime = Time.time;
		//set initial parent to gun object to inherit player velocity 
		myTransform.parent = gunObj.transform;	
		lerpShell.parent = gunObj.transform;
		shellRemovalTime = Time.time + shellDuration;//time that shell will be removed
		GetComponent<Rigidbody>().maxAngularVelocity = 100;//allow shells to spin faster than default
		//determine if shell rotates clockwise or counter-clockwise at random
		if(Random.value < 0.5f){shellRotateUp *= -1;} 
		tempPos = myTransform.position;
		lerpShell.position = tempPos;
		//rotate shell
		rotateAmt = 0.1f;
		//apply torque to rigidbody
		GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * (Random.Range (0.1f * 1.75f,rotateAmt) * shellRotateSide));
		GetComponent<Rigidbody>().AddRelativeTorque(Vector3.right * (Random.Range (0.1f * 4,rotateAmt * 6) * shellRotateUp));
	}
	
	void Update (){
		//smooth/lerp mesh shell object's position and rotation from rigidbody shell's position and rotation
		tempPos = Vector3.Lerp(tempPos, myTransform.position, Time.deltaTime * 64.0f);
		lerpShell.position = tempPos;
		tempRot.x = Mathf.LerpAngle(tempRot.x, myTransform.eulerAngles.x, Time.deltaTime * 64.0f);
		tempRot.y = Mathf.LerpAngle(tempRot.y, myTransform.eulerAngles.y, Time.deltaTime * 64.0f);
		tempRot.z = Mathf.LerpAngle(tempRot.z, myTransform.eulerAngles.z, Time.deltaTime * 64.0f);
		lerpShell.eulerAngles = tempRot;
	}
	
	void FixedUpdate(){
		//set up external script references
		FPSRigidBodyWalker FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		PlayerWeapons PlayerWeaponsComponent = gunObj.transform.parent.GetComponent<PlayerWeapons>();
		WorldRecenter WorldRecenterComponent = playerObj.transform.GetComponent<WorldRecenter>();
		
		if(Time.time > shellRemovalTime){
			Object.Destroy(lerpShell.gameObject);
			Object.Destroy(gameObject);
		}
		
		//Check if the player is on a moving platform to determine how to handle shell parenting and velocity
		//if(playerObjTransform.parent == FPSWalkerComponent.mainObj.transform){//if player is not on a moving platform
		if((WorldRecenterComponent.removePrefabRoot && playerObjTransform.parent == null)
		||(!WorldRecenterComponent.removePrefabRoot && playerObjTransform.parent == FPSWalkerComponent.mainObj.transform)){
			//Make the shell's parent the weapon object for a short time after ejection
			//to the link shell ejection position with weapon object for more consistent movement,
			if(((startTime + 0.35f < Time.time && parentState) 
			//don't parent shell if switching weapon
			|| (PlayerWeaponsComponent.switching && parentState)
			//don't parent shell if moving weapon to sprinting position
			|| (FPSWalkerComponent.sprintActive && !FPSWalkerComponent.cancelSprint && parentState))
			&& FPSWalkerComponent.grounded){
				Vector3 tempVelocity = playerObjTransform.GetComponent<Rigidbody>().velocity;
				tempVelocity.y = 0.0f;
				myTransform.parent = null;
				lerpShell.parent = null;
				//add player velocity to shell when unparenting from player object to prevent shell from suddenly changing direction
				if(!FPSWalkerComponent.sprintActive && !FPSWalkerComponent.canRun){//don't inherit parent velocity if sprinting to prevent visual glitches
					GetComponent<Rigidbody>().AddForce(tempVelocity, ForceMode.VelocityChange);
				}
				parentState = false;
			}
		}else{//if player is on elevator, keep gun object as parent for a longer time to prevent strange shell movements
			if(startTime + 0.5f < Time.time && parentState){
				myTransform.parent = null;
				lerpShell.parent = null;
				//add player velocity to shell when unparenting from player object to prevent shell from suddenly changing direction
				GetComponent<Rigidbody>().AddForce(playerObjTransform.GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);	
			}		
		}	
	}

	void OnCollisionEnter(Collision collision){
		//play a bounce sound when shell object collides with a surface
		if(soundState){
			if (shellSounds.Length > 0){
				AudioSource.PlayClipAtPoint(shellSounds[(int)Random.Range(0, (shellSounds.Length))], myTransform.position, 0.75f);
			}
			soundState = false;
		}
		//remove shells if they collide with a moving object like an elevator
		if(collision.gameObject.layer == 15){
			Object.Destroy(lerpShell.gameObject);
			Object.Destroy(gameObject);
		}
	}

}


	