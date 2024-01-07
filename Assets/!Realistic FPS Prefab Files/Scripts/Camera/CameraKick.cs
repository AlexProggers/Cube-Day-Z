//CameraKick.cs by Azuline Studios© All Rights Reserved
//Camera positioning and angle management for smooth camera kicks and animations.
using UnityEngine;
using System.Collections;

public class CameraKick : MonoBehaviour {
	//other objects accessed by this script
	[HideInInspector]
	public GameObject gun;//this variable updated by PlayerWeapons script
	[HideInInspector]
	public GameObject playerObj;
	[HideInInspector]
	public GameObject weaponObj;
	private Transform myTransform;
	private FPSRigidBodyWalker FPSWalkerComponent;
	private Ironsights IronsightsComponent;
	private FPSPlayer FPSPlayerComponent;
	private WorldRecenter WorldRecenterComponent;
	//camera angles
	[HideInInspector]
	public float CameraYawAmt = 0.0f;//this value is modified by animations and added to camera angles
	[HideInInspector]
	public float CameraPitchAmt = 0.0f;//this value is modified by animations and added to camera angles
	[HideInInspector]
	public float CameraRollAmt = 0.0f;//this value is modified by animations and added to camera angles
//	private float timer;
//	private float waveslice;
	[HideInInspector]
	public Vector3 bobAngles = new Vector3(0,0,0);//view bobbing angles are sent here from the HeadBob script
	private float returnSpeed = 4.0f;//speed that camera angles return to neutral
	//to move gun and view down slightly on contact with ground
	private bool  landState = false;
	private float landStartTime = 0.0f;
	private float landElapsedTime = 0.0f;
	private float landTime = 0.35f;
	private float landAmt = 20.0f;
	private float landValue = 0.0f;
	//weapon position
	private float gunDown = 0.0f;
	[HideInInspector]
	public float dampOriginX = 0.0f;//Player X position is passed from the GunBob script
	[HideInInspector]
	public float dampOriginY = 0.0f;//Player Y position is passed from the HeadBob script
	//camera position vars
	private Vector3 tempLerpPos;
	private Vector3 dampVel;
	private float lerpSpeed;
	private Transform playerObjTransform;
	private Transform mainCameraTransform;

	void Start (){
		myTransform = transform;//store this object's transform for optimization
		playerObjTransform = playerObj.transform;
		mainCameraTransform = Camera.main.transform;
		//define external script references
		FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		IronsightsComponent = playerObj.GetComponent<Ironsights>();
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		WorldRecenterComponent = playerObj.GetComponent<WorldRecenter>();
	}

	
	public void OnRespawn()
	{
		tempLerpPos = playerObjTransform.position;
	}
	
	void LateUpdate (){

		if(Time.timeScale > 0 && Time.deltaTime > 0){//allow pausing by setting timescale to 0
		
			//make sure that animated camera angles zero-out when not playing an animation
			//this is necessary because sometimes the angle amounts did not return to zero
			//which resulted in the gun and camera angles becoming misaligned
			if(!GetComponent<Animation>().isPlaying){
				CameraPitchAmt = 0.0f;
				CameraYawAmt = 0.0f;
				CameraRollAmt = 0.0f;
			}
			
	//		//make player view roll slightly when in water
	//		if(FPSWalkerComponent.swimming){
	//			
	//			waveslice = Mathf.Sin(timer); 
	//			
	//			if (timer > Mathf.PI * 2) { 
	//				timer = timer - (Mathf.PI * 2); 
	//			}
	//			
	//			timer = timer + 0.015f; 	
	//			CameraRollAmt = waveslice / 16.0f * Time.deltaTime * 60.0f;	
	//		}
			
			//if world has just been recentered, don't lerp camera position to prevent lagging behind FPS Player object position
			if(WorldRecenterComponent.worldRecenterTime + (0.1f * Time.timeScale) > Time.time){
				tempLerpPos = playerObjTransform.position;
			}else{//lerp camera normally if not on elevator or moving platform
				if((WorldRecenterComponent.removePrefabRoot && playerObj.transform.parent == null)
				||(!WorldRecenterComponent.removePrefabRoot && playerObj.transform.parent == FPSWalkerComponent.mainObj.transform)){
					lerpSpeed = Mathf.MoveTowards(lerpSpeed, 0.075f, Time.deltaTime);//gradually change lerpSpeed for smoother lerp transition
				}else{//faster camera lerp speed on platforms or elevators to minimize camera position lag
					lerpSpeed = Mathf.MoveTowards(lerpSpeed, 0.025f, Time.deltaTime);
				}
				//smooth player position before applying bob effects
				tempLerpPos.x = Mathf.SmoothDamp(tempLerpPos.x, playerObjTransform.position.x, ref dampVel.x, lerpSpeed, Mathf.Infinity, Time.deltaTime);
				tempLerpPos.y = Mathf.SmoothDamp(tempLerpPos.y, playerObjTransform.position.y, ref dampVel.y, lerpSpeed, Mathf.Infinity, Time.deltaTime);
				tempLerpPos.z = Mathf.SmoothDamp(tempLerpPos.z, playerObjTransform.position.z, ref dampVel.z, lerpSpeed, Mathf.Infinity, Time.deltaTime);
			}
			
			//side to side bobbing/moving of camera (stored in the dampOriginX) needs to added to the right vector
			//of the transform so that the bobbing amount is correctly applied along the X and Z axis.
			//If added just to the x axis, like done with the vertical Y axis, the bobbing does not rotate
			//with camera/mouselook and only moves on the world axis.  
			Vector3 tempPosition = tempLerpPos + (playerObjTransform.right * dampOriginX) + new Vector3(0.0f, dampOriginY, 0.0f);
			mainCameraTransform.parent.transform.position = tempPosition;
			mainCameraTransform.position = tempPosition;
			
			//initialize camera position/angles quickly before fade out on level load
			if(Time.timeSinceLevelLoad < 1){returnSpeed = 32.0f;}else{returnSpeed = 4.0f;};
			//apply a force to the camera that returns it to neutral angles (Quaternion.identity) over time after being changed by code or by animations
			myTransform.localRotation = Quaternion.Slerp(myTransform.localRotation, Quaternion.identity, Time.deltaTime * returnSpeed);
			
			//store camera angles in temporary vector and add yaw and pitch from animations 
			Vector3 tempCamAngles = new Vector3(mainCameraTransform.localEulerAngles.x - bobAngles.x + (CameraPitchAmt * Time.deltaTime * 75.0f) ,
												mainCameraTransform.localEulerAngles.y + bobAngles.y + (CameraYawAmt * Time.deltaTime * 75.0f),
												mainCameraTransform.localEulerAngles.z - bobAngles.z + (CameraRollAmt * Time.deltaTime * 75.0f)); 
			
			//apply tempCamAngles to camera angles
			mainCameraTransform.localEulerAngles = tempCamAngles;
			
			//Track time that player has landed from jump or fall for gun kicks
			landElapsedTime = Time.time - landStartTime;
			
			if(FPSWalkerComponent.fallingDistance < 1.25f && !FPSWalkerComponent.jumping){
				if(!landState){
					//init timer amount
					landStartTime = Time.time;
					//set landState only once
					landState = true;
				}
			}else{
				if(landState){
					//if recoil time has elapsed
					if(landElapsedTime >= landTime){ 
						//reset shootState
						landState = false;
					}
				}
			}
		
			//perform jump of gun when landing
			if(landElapsedTime < landTime){
				//only rise for half of landing time for quick rising and slower lowering
				if(landElapsedTime > landTime / 2.0f){//move up view and gun
					landValue += landAmt * Time.deltaTime;
				}else{//for remaining half of landing time, move down view and gun
					landValue -= landAmt* Time.deltaTime;
				}
			}else{
				//reset vars
				landValue = 0.0F;
			}
		
			//make landing kick less when zoomed
			if (!FPSPlayerComponent.zoomed){gunDown = landValue / 96.0f;}else{gunDown = landValue / 192.0f;}
			//pass value of gun kick to IronSights script where it will be added to gun position
			IronsightsComponent.jumpAmt = gunDown;
		}
	}
	
}