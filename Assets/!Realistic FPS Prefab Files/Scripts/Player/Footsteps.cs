//Footsteps.cs by Azuline Studios© All Rights Reserved
//Plays footstep sounds by surface type.
using UnityEngine;
using System.Collections;

public class Footsteps : MonoBehaviour {
	[HideInInspector]
	public GameObject playerObj;
	[HideInInspector]
	private Transform cameraTransform;
	public string materialType;//tag of object that player is standing on (Metal, Wood, Dirt, Stone, Water)
	private float volumeAmt = 1.0f;//volume of audio clip to be played
	private AudioClip footStepClip;//audio clip to be played 
	//player movement sounds and volume amounts
	public float dirtStepVol = 1.0f;
	public AudioClip[] dirtSteps;
	public float woodStepVol = 1.0f;
	public AudioClip[] woodSteps;
	public float metalStepVol = 1.0f;
	public AudioClip[] metalSteps;
	public float waterSoundVol = 1.0f;
	public AudioClip[] waterSounds;
	public float climbSoundVol = 1.0f;
	public AudioClip[] climbSounds;
	public float stoneStepVol = 1.0f;
	public AudioClip[] stoneSteps;
		
	public AudioClip dirtLand;
	public AudioClip metalLand;
	public AudioClip woodLand;
	public AudioClip waterLand;
	public AudioClip stoneLand;
	
	void Start () {
		playerObj = transform.gameObject;
		cameraTransform = Camera.main.transform;
	}
	
	public void FootstepSfx (){
		FPSRigidBodyWalker FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		//play footstep sound effects
		if(!FPSWalkerComponent.climbing){
			if(FPSWalkerComponent.inWater){//play swimming/wading footstep effects
				footStepClip = waterSounds[Random.Range(0, waterSounds.Length)];//select random water step effect from waterSounds array
				volumeAmt = waterSoundVol;//set volume of audio clip to customized amount
				AudioSource.PlayClipAtPoint(footStepClip, cameraTransform.position, volumeAmt);
			}else{
				//Make a short delay before playing footstep sounds to allow landing sound to play
				if (FPSWalkerComponent.grounded && (FPSWalkerComponent.landStartTime + 0.4f) < Time.time){
					switch(materialType){//determine which material the player is standing on and select random footstep effect for surface type
					case "Wood":
						footStepClip = woodSteps[Random.Range(0, woodSteps.Length)];
						volumeAmt = woodStepVol;
						break;
					case "Metal":
						footStepClip = metalSteps[Random.Range(0, metalSteps.Length)];
						volumeAmt = metalStepVol;
						break;
					case "Dirt":
						footStepClip = dirtSteps[Random.Range(0, dirtSteps.Length)];
						volumeAmt = dirtStepVol;
						break;
					case "Stone":
						footStepClip = stoneSteps[Random.Range(0, stoneSteps.Length)];
						volumeAmt = stoneStepVol;
						break;
					default:
						footStepClip = dirtSteps[Random.Range(0, dirtSteps.Length)];
						volumeAmt = dirtStepVol;
						break;	
					}
					//play the sound effect
					AudioSource.PlayClipAtPoint(footStepClip, cameraTransform.position, volumeAmt);
				}
			}
		}else{//play climbing footstep effects
			footStepClip = climbSounds[Random.Range(0, climbSounds.Length)];
			volumeAmt = climbSoundVol;
			AudioSource.PlayClipAtPoint(footStepClip, cameraTransform.position, volumeAmt);
		}
	}
}
