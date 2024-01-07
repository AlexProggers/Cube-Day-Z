//VerticalBob.cs by Azuline Studios© All Rights Reserved 
using UnityEngine;
using System.Collections;

public class VerticalBob : MonoBehaviour {
	[HideInInspector]
	public GameObject playerObj;
	//Variables for vertical aspect of sine bob of camera and weapons
	//This script also makes camera view roll and pitch with bobbing
	private float timer = 0.0f;
	private float timerRoll = 0.0f;
	[HideInInspector]
	public float bobbingSpeed = 0.0f;
	//Vars for smoothing view position
	private float dampOrg = 0.0f;//Smoothed view postion to be passed to CameraKick script
	private float dampTo = 0.0f;
	private Vector3 tempLocalEulerAngles = new Vector3(0,0,0);
	//These two vars controlled from ironsights script
	//to allow different values for sprinting/walking ect.
	[HideInInspector]
	public float bobbingAmount = 0.0f;
	[HideInInspector]
	public float rollingAmount = 0.0f;
	[HideInInspector]
	public float yawingAmount = 0.0f;
	[HideInInspector]
	public float pitchingAmount = 0.0f;
	private float midpoint = 0.0f;//Vertical position of camera during sine bobbing
	private float idleYBob = 0.0f;
	[HideInInspector]
	public float translateChange = 0.0f;
	private float translateChangeRoll = 0.0f;
	private float translateChangePitch = 0.0f;
	private float translateChangeYaw = 0.0f;
	private float waveslice = 0.0f;
	private float wavesliceRoll = 0.0f;
	private float dampVelocity = 0.0f;
	private Vector3 dampVelocity2;
	private float totalAxes;
	private float horizontal;
	private float vertical;
	private float inputSpeed;

	//Set up external script references
	FPSRigidBodyWalker FPSWalkerComponent;
	FPSPlayer FPSPlayerComponent;
	CameraKick CameraKickComponent;
	Footsteps FootstepsComponent;

	void Awake()
	{
		//Set up external script references
		FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		CameraKickComponent = Camera.main.GetComponent<CameraKick>();
		FootstepsComponent = playerObj.GetComponent<Footsteps>();
	}

	void Update (){
		if(Time.timeScale > 0 && Time.deltaTime > 0){//allow pausing by setting timescale to 0
	
			waveslice = 0.0f;
			if(!FPSPlayerComponent.useAxisInput){
				horizontal = FPSWalkerComponent.inputX;//get input from player movement script
				vertical = FPSWalkerComponent.inputY;
			}else{
				horizontal = FPSWalkerComponent.inputXSmoothed;//get input from player movement script
				vertical = FPSWalkerComponent.inputYSmoothed;	
			}
			midpoint = FPSWalkerComponent.midPos;//Start bob from view position set in player movement script
		
			if (Mathf.Abs(horizontal) != 0 || Mathf.Abs(vertical) != 0 && FPSWalkerComponent.grounded){//Perform bob only when moving and grounded
		
				waveslice = Mathf.Sin(timer);
				wavesliceRoll = Mathf.Sin(timerRoll);
				if(Mathf.Abs(FPSWalkerComponent.inputY) > 0.1f){
					inputSpeed = Mathf.Abs(FPSWalkerComponent.inputY);
				}else{
					inputSpeed = Mathf.Abs(FPSWalkerComponent.inputX);
				}	   
				timer = timer + bobbingSpeed * inputSpeed * Time.deltaTime;
				timerRoll = timerRoll + (bobbingSpeed / 2.0f) * Time.deltaTime;//Make view roll bob half the speed of view pitch bob
				
				if (timer > Mathf.PI * 2.0f){
					timer = timer - (Mathf.PI * 2.0f);
					if(!FPSWalkerComponent.noClimbingSfx){//dont play climbing footsteps if noClimbingSfx is true
						FootstepsComponent.FootstepSfx();//play footstep sound effect by calling FootstepSfx() function in Footsteps.cs
					}
				}
				
				//Perform bobbing of camera roll
				if (timerRoll > Mathf.PI * 2.0f){
					timerRoll = (timerRoll - (Mathf.PI * 2.0f));
					if (!FPSWalkerComponent.grounded){
						timerRoll = 0;//reset timer when airborne to allow soonest resume of footstep sfx
					}
				}
			   
			}else{
				//reset variables to prevent view twitching when falling
				timer = 0.0f;
				timerRoll = 0.0f;
				tempLocalEulerAngles = new Vector3(0,0,0);//reset camera angles to 0 when stationary
			}
		
			if (waveslice != 0){
				
				translateChange = waveslice * bobbingAmount;
				translateChangePitch = waveslice * pitchingAmount;
				translateChangeRoll = wavesliceRoll * rollingAmount;
				translateChangeYaw = wavesliceRoll * yawingAmount;
				totalAxes = Mathf.Abs(horizontal) + Mathf.Abs(vertical);
				totalAxes = Mathf.Clamp (totalAxes, 0.0f, 1.0f);
				//needed for smooth return to neutral view pos
				translateChange = totalAxes * translateChange;
				translateChangePitch = totalAxes * translateChangePitch;
				translateChangeRoll = totalAxes * translateChangeRoll;
				//Set position for smoothing function and add jump value
				//divide position by deltaTime for framerate independence
				dampTo = midpoint + (translateChange / Time.deltaTime * 0.01f);
				//camera roll and pitch bob
				tempLocalEulerAngles = new Vector3(translateChangePitch, translateChangeYaw,translateChangeRoll);
				
			}else{
				
				if(!FPSWalkerComponent.swimming){
					idleYBob = Mathf.Sin(Time.time * 1.25f) * 0.015f;
				}else{
					idleYBob = Mathf.Sin(Time.time * 1.25f) * 0.05f;//increase vertical bob when swimming
				}
				
				//reset variables to prevent view twitching when falling
				dampTo = midpoint + idleYBob;//add small sine bob for camera idle movement
				totalAxes = 0;
				translateChange = 0;
			}
			//use SmoothDamp to smooth position and remove any small glitches in bob amount 
			dampOrg = Mathf.SmoothDamp(dampOrg, dampTo, ref dampVelocity, 0.1f, Mathf.Infinity, Time.deltaTime);
			//Pass bobbing amount and angles to the camera kick script in the camera object after smoothing
			CameraKickComponent.dampOriginY = dampOrg;
			CameraKickComponent.bobAngles = Vector3.SmoothDamp(CameraKickComponent.bobAngles, tempLocalEulerAngles, ref dampVelocity2, 0.1f, Mathf.Infinity, Time.deltaTime);
		}
	}
}