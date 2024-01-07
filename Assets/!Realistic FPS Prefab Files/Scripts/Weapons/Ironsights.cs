//Ironsights.cs by Azuline Studios© All Rights Reserved
//Adjusts weapon position and bobbing speeds and magnitudes 
//for various player states like zooming, sprinting, and crouching.
using UnityEngine;
using System.Collections;

public class Ironsights : MonoBehaviour {
	//other objects accessed by this script
	[HideInInspector]
	public GameObject playerObj;
	[HideInInspector]
	public GameObject weaponObj;
	[HideInInspector]
	public GameObject CameraObj;
	[HideInInspector]
	public Camera WeapCameraObj;
	//weapon object (weapon object child) set by PlayerWeapons.cs script
	public GameObject gunObj;
	//Var set to sprint animation time of weapon
	[HideInInspector]
	public Transform gun;//this set by PlayerWeapons script to active weapon transform
	private Transform mainCamTransform;
		
	//weapon positioning	
	private float nextPosX = 0.0f;//weapon x position that is smoothed using smoothDamp function
	private float nextPosY = 0.0f;//weapon y position that is smoothed using smoothDamp function
	private float nextPosZ = 0.0f;//weapon z position that is smoothed using smoothDamp function
	private float zPosRecNext = 0.0f;//weapon recoil z position that is smoothed using smoothDamp function
	private float newPosX = 0.0f;//target weapon x position that is smoothed using smoothDamp function
	private float newPosY = 0.0f;//target weapon y position that is smoothed using smoothDamp function
	private float newPosZ = 0.0f;//target weapon z position that is smoothed using smoothDamp function
	private float zPosRec = 0.0f;//target weapon recoil z position that is smoothed using smoothDamp function
	private Vector3 dampVel = Vector3.zero;//velocities that are used by smoothDamp function
	private float recZDamp = 0.0f;//velocity that is used by smoothDamp function
	private Vector3 tempGunPos = Vector3.zero;

	//camera FOV handling
	public float defaultFov = 75.0f;//default camera field of view value
	public float sprintFov = 85.0f;//camera field of view value while sprinting
	public float weaponCamFovDiff = 20.0f;//amount to subtract from main camera FOV for weapon camera FOV
	private float nextFov = 75.0f;//camera field of view that is smoothed using smoothDamp
	private float newFov = 75.0f;//camera field of view that is smoothed using smoothDamp
	private float FovSmoothSpeed = 0.15f;//speed that camera FOV is smoothed
	private float dampFOV = 0.0f;//target weapon z position that is smoothed using smoothDamp function
		
	//zooming
	public enum zoomType{
		hold,
		toggle,
		both
	}
	public zoomType zoomMode = zoomType.both;
	public float zoomSensitivity = 0.5f;//percentage to reduce mouse sensitivity when zoomed
	public AudioClip sightsUpSnd;
	public AudioClip sightsDownSnd;
	[HideInInspector]
	public bool zoomSfxState = true;//var for only playing sights sound effects once
	[HideInInspector]
	public bool reloading = false;//this variable true when player is reloading
		
	//bobbing speeds and amounts for player movement states	
	public float WalkHorizontalBobAmount = 0.11f;
	public float WalkVerticalBobAmount = 0.075f;
	public float WalkBobPitchAmount = 0.0175f;
	public float WalkBobRollAmount = 0.01f;
	public float WalkBobYawAmount = 0.01f;
	public float WalkBobSpeed = 12f;
	
	public float CrouchHorizontalBobAmount = 0.11f;
	public float CrouchVerticalBobAmount = 0.075f;
	public float CrouchBobPitchAmount = 0.025f;
	public float CrouchBobRollAmount = 0.055f;
	public float CrouchBobYawAmount = 0.055f;
	public float CrouchBobSpeed = 8f;
	
	public float ZoomHorizontalBobAmount = 0.016f;
	public float ZoomVerticalBobAmount = 0.0075f;
	public float ZoomBobPitchAmount = 0.001f;
	public float ZoomBobRollAmount = 0.008f;
	public float ZoomBobYawAmount = 0.008f;
	public float ZoomBobSpeed = 8f;
		
	public float SprintHorizontalBobAmount = 0.135f;
	public float SprintVerticalBobAmount = 0.16f;
	public float SprintBobPitchAmount = 0.12f;
	public float SprintBobRollAmount = 0.075f;
	public float SprintBobYawAmount = 0.075f;
	public float SprintBobSpeed = 19f;
		
	//gun X position amount for tweaking ironsights position
	private float horizontalGunPosAmt = -0.02f;
	private float weaponSprintXPositionAmt = 0.0f;
	//vars to scale up bob speeds slowly to prevent jerky transitions
	private float HBamt = 0.075f;//dynamic head bobbing variable
	private float HRamt = 0.125f;//dynamic head rolling variable
	private float HYamt = 0.125f;//dynamic head yawing variable
	private float HPamt = 0.1f;//dynamic head pitching variable
	private float GBamt = 0.075f;//dynamic gun bobbing variable
	//weapon sprinting positioning
	private float gunup = 0.015f;//amount to move weapon up while sprinting
	private float gunRunSide = 1.0f;//to control horizontal bobbing of weapon during sprinting
	private float gunRunUp = 1.0f;//to control vertical bobbing of weapon during sprinting
	private float sprintBob = 0.0f;//to modify weapon bobbing speeds when sprinting
	private float sprintBobAmtX = 0.0f;//actual horizontal weapon bobbing speed when sprinting
	private float sprintBobAmtY = 0.0f;//actual vertical weapon bobbing speed when sprinting
	//weapon positioning
	private float yDampSpeed= 0.0f;//this value used to control speed that weapon Y position is smoothed
	private float zDampSpeed= 0.0f;//this value used to control speed that weapon Z position is smoothed
	private float bobDir = 0.0f;//positive or negative direction of bobbing
	private float bobMove = 0.0f;
	private float sideMove = 0.0f;
	[HideInInspector]
	public float switchMove = 0.0f;//for moving weapon down while switching weapons
	[HideInInspector]
	public float climbMove = 0.0f;//for moving weapon down while climbing
	private float jumpmove = 0.0f;//for moving weapon down while jumping
	[HideInInspector]
	public float jumpAmt = 0.0f;
	private float idleX = 0.0f;//amount of weapon movement when idle
	private float idleY = 0.0f;
	[HideInInspector]
	public float side = 0.0f;//amount to sway weapon position horizontally
	[HideInInspector]
	public float raise = 0.0f;//amount to sway weapon position vertically
	[HideInInspector]
	public float gunAnimTime = 0.0f;

	SmoothMouseLook SmoothMouseLook;
    PlayerWeapons PlayerWeaponsComponent;
    FPSRigidBodyWalker FPSWalker;
	WeaponBehavior WeaponBehaviorComponent;
	VerticalBob VerticalBob;
	HorizontalBob HorizontalBob;
	FPSPlayer FPSPlayerComponent;

	void Awake()
	{
		mainCamTransform = Camera.main.transform;

		//Set up external script references
		SmoothMouseLook = CameraObj.GetComponent<SmoothMouseLook>();
		PlayerWeaponsComponent = weaponObj.GetComponent<PlayerWeapons>();
		FPSWalker = playerObj.GetComponent<FPSRigidBodyWalker>();
		WeaponBehaviorComponent = gunObj.GetComponent<WeaponBehavior>();
		VerticalBob = playerObj.GetComponent<VerticalBob>();
		HorizontalBob = playerObj.GetComponent<HorizontalBob>();
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
	}

	public void UpdateWeaponBehavior()
	{
		WeaponBehaviorComponent = gunObj.GetComponent<WeaponBehavior>();
		FPSPlayerComponent.CurrentWeaponBehavior = WeaponBehaviorComponent;
	}
	
	void Update (){
		if(Time.timeScale > 0 && Time.deltaTime > 0){//allow pausing by setting timescale to 0
		
			//main weapon position smoothing happens here
			newPosX = Mathf.SmoothDamp(newPosX, nextPosX, ref dampVel.x, yDampSpeed, Mathf.Infinity, Time.deltaTime);
			newPosY = Mathf.SmoothDamp(newPosY, nextPosY, ref dampVel.y, yDampSpeed, Mathf.Infinity, Time.deltaTime);
			newPosZ = Mathf.SmoothDamp(newPosZ, nextPosZ, ref dampVel.z, zDampSpeed, Mathf.Infinity, Time.deltaTime);
			zPosRec = Mathf.SmoothDamp(zPosRec, zPosRecNext, ref recZDamp, 0.25f, Mathf.Infinity, Time.deltaTime);//smooth recoil kick back of weapon
			newFov = Mathf.SmoothDamp(Camera.main.fieldOfView, nextFov, ref dampFOV, FovSmoothSpeed, Mathf.Infinity, Time.deltaTime);//smooth camera FOV
		
			//Sync camera FOVs
			WeapCameraObj.fieldOfView = Camera.main.fieldOfView - weaponCamFovDiff;
			Camera.main.fieldOfView = newFov;
			//Get input from player movement script
			float horizontal = FPSWalker.inputX;
			float vertical = FPSWalker.inputY;
		
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Adjust weapon position and bobbing amounts dynamicly based on movement and player states
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			//move weapon back towards camera based on kickBack amount in WeaponBehavior.cs					
			if(WeaponBehaviorComponent.shootStartTime + 0.1f > Time.time){
				if(FPSPlayerComponent.zoomed){
					zPosRecNext = WeaponBehaviorComponent.kickBackAmtZoom;	
				}else{
					zPosRecNext = WeaponBehaviorComponent.kickBackAmtUnzoom;	
				}
			}else{
				zPosRecNext = 0.0f;
			}
				
			
			if (Mathf.Abs(horizontal) != 0 
			|| ((!FPSPlayerComponent.useAxisInput && (FPSWalker.MyMoveForward() || FPSWalker.MyMoveBack()))
			|| (FPSPlayerComponent.useAxisInput && Mathf.Abs(vertical) > 0.1f))){
				idleY = 0;
				idleX = 0;
				//check for sprinting
				if (FPSWalker.sprintActive
				&& !FPSPlayerComponent.zoomed
				&& !FPSWalker.crouched
				&& FPSWalker.midPos >= FPSWalker.standingCamHeight//player might not have completely stood up yet from crouch
				&& !((Mathf.Abs(horizontal) != 0.0f) && (Mathf.Abs(vertical) < 0.75f))
				&& !FPSWalker.cancelSprint){
					
					sprintBob = 128.0f;
					
					if (!FPSWalker.cancelSprint
					&& !reloading
					&& !FPSWalker.jumping
					&& FPSWalker.fallingDistance < 0.75f){//actually sprinting now
						//set the camera's fov back to normal if the player has sprinted into a wall, but the sprint is still active
						if(FPSWalker.inputY != 0){
							nextFov = sprintFov;
						}else{
							nextFov = defaultFov;	
						}
						//gradually move weapon more towards center while sprinting
						weaponSprintXPositionAmt = Mathf.MoveTowards(weaponSprintXPositionAmt, WeaponBehaviorComponent.weaponSprintXPosition, Time.deltaTime * 16);
						horizontalGunPosAmt = WeaponBehaviorComponent.weaponUnzoomXPosition + weaponSprintXPositionAmt;
						gunRunSide = 2.0f;
						if(gunRunUp < 1.4f){gunRunUp += Time.deltaTime / 4.0f;}//gradually increase for smoother transition
						bobMove = gunup + WeaponBehaviorComponent.weaponSprintYPosition;//raise weapon while sprinting
					}else{//not sprinting
						nextFov = defaultFov;
						horizontalGunPosAmt = WeaponBehaviorComponent.weaponUnzoomXPosition;
						gunRunSide = 1.0f;
						gunRunUp = 1.0f;
						bobMove = -0.01f;
						switchMove = 0.0f;
					}
				}else{//walking
					gunRunSide = 1.0f;
					gunRunUp = 1.0f;
					//reset horizontal weapon positioning var and make sure it returns to zero when not sprinting to prevent unwanted side movement
					weaponSprintXPositionAmt = Mathf.MoveTowards(weaponSprintXPositionAmt, 0, Time.deltaTime * 16);
					horizontalGunPosAmt = WeaponBehaviorComponent.weaponUnzoomXPosition + weaponSprintXPositionAmt;
					if(reloading){//move weapon position up when reloading and moving for full view of animation
						nextFov = defaultFov;
						sprintBob = 216;
						bobMove = 0.0F;
						sideMove = -0.0f;
					}else{
						nextFov = defaultFov;
						if(FPSPlayerComponent.zoomed && WeaponBehaviorComponent.meleeSwingDelay == 0) {//zoomed and not melee weapon
							sprintBob = 96.0f;
	//						if (Mathf.Abs(horizontal) != 0 || Mathf.Abs(vertical) > 0.75f){
	//							bobMove = -0.001f;//move weapon down
	//						}else{
								bobMove = 0.0F;//move weapon to idle
	//						}
						}else{//not zoomed
							sprintBob = 216.0f;
							if(Mathf.Abs(horizontal) != 0 || Mathf.Abs(vertical) > 0.75f){
								//move weapon down and left when crouching
								if (FPSWalker.crouched || FPSWalker.midPos < FPSWalker.standingCamHeight * 0.85f) {
									bobMove = -0.01f;
									sideMove = -0.0125f;
								}else{
									bobMove = -0.005f;
									sideMove = -0.00f;
								}
							}else{
								//move weapon to idle
								bobMove = 0.0F;
								sideMove = 0.0F;
							}
						}
					}
				}
			}else{//if not moving (no player movement input)
				nextFov = defaultFov;
				horizontalGunPosAmt = WeaponBehaviorComponent.weaponUnzoomXPosition;
				if(weaponSprintXPositionAmt > 0){weaponSprintXPositionAmt -= Time.deltaTime / 4;}
				sprintBob = 96.0f;
				if(reloading){
					nextFov = defaultFov;
					sprintBob = 96.0f;
					bobMove = 0.0F;
					sideMove = -0.0f;
				}else{
					//move weapon to idle
					if((FPSWalker.crouched || FPSWalker.midPos < FPSWalker.standingCamHeight * 0.85f) && !FPSPlayerComponent.zoomed) {
						bobMove = -0.005f;
						sideMove = -0.0125f;
					}else{
						bobMove = 0.0f;
						sideMove = 0.0f;
					}
				}
				//weapon idle motion
				if(FPSPlayerComponent.zoomed && WeaponBehaviorComponent.meleeSwingDelay == 0) {
					idleX = Mathf.Sin(Time.time * 1.25f) / 4800.0f;
					idleY = Mathf.Sin(Time.time * 1.5f) / 4800.0f;
				}else{
					if(!FPSWalker.swimming){
						idleX = Mathf.Sin(Time.time * 1.25f) / 800.0f;
						idleY = Mathf.Sin(Time.time * 1.5f) / 800.0f;
					}else{
						idleX = Mathf.Sin(Time.time * 1.25f) / 400.0f;
						idleY = Mathf.Sin(Time.time * 1.5f) / 400.0f;	
					}
				}
			}
			
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Weapon Swaying/Bobbing while moving
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			//track X axis while walking for side to side bobbing effect	
			if(HorizontalBob.waveslice != 0){bobDir = 1;}else{bobDir = -1;}
				
			//Reduce weapon bobbing by sprintBobAmount value defined in WeaponBehavior script.
			//This is for fine tuning of weapon bobbing. Pistols look better with less sprint bob 
			//because they use a different sprinting anim and have a different sprinting position 
			//than the animation used by rifle-type weapons.
			if(!FPSWalker.canRun){
				sprintBobAmtX = sprintBob / WeaponBehaviorComponent.walkBobAmountX;
				sprintBobAmtY = sprintBob / WeaponBehaviorComponent.walkBobAmountY;
			}else{
				sprintBobAmtX = sprintBob / WeaponBehaviorComponent.sprintBobAmountX;
				sprintBobAmtY = sprintBob / WeaponBehaviorComponent.sprintBobAmountY;
			}
				
			//set smoothed weapon position to actual gun position vector
			tempGunPos.x = newPosX;
			tempGunPos.y = newPosY;
			tempGunPos.z = newPosZ + zPosRec;//add weapon z position and recoil kick back
			//apply temporary vector to gun's transform position
			gun.localPosition = tempGunPos;
		
			//lower weapon when jumping, falling, or slipping off ledge
			if(FPSWalker.jumping || FPSWalker.fallingDistance > 1.25f){
				//lower weapon less when zoomed
				if (!FPSPlayerComponent.zoomed){
					//raise weapon when jump is ascending and lower when descending
					if((FPSWalker.airTime + 0.175f) > Time.time){
						jumpmove = 0.015f;
					}else{
						jumpmove = -0.025f;
					}
				}else{
					jumpmove = -0.01f;
				}
			}else{
				jumpmove = 0.0f;
			}
		   
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Adjust vars for zoom and other states
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				
			float deltaAmount = Time.deltaTime * 100;//define delta for framerate independence
			float bobDeltaAmount = 0.12f / Time.deltaTime;//define bobbing delta for framerate independence
		  	
			if(!WeaponBehaviorComponent.PistolSprintAnim || !FPSWalker.canRun){
				gunAnimTime = gunObj.GetComponent<Animation>()["RifleSprinting"].normalizedTime;//Track playback position of rifle sprinting animation
			}else{
				gunAnimTime = gunObj.GetComponent<Animation>()["PistolSprinting"].normalizedTime;//Track playback position of pistol sprinting animation	
			}
		   
			//if zoomed
			//check time of weapon sprinting anim to make weapon return to center, then zoom normally 
			if(FPSPlayerComponent.zoomed
			&& FPSPlayerComponent.hitPoints > 1.0f
			&& PlayerWeaponsComponent.switchTime + WeaponBehaviorComponent.readyTime < Time.time//don't raise sights when readying weapon 
			&& !reloading 
			&& gunAnimTime < 0.35f 
			&& WeaponBehaviorComponent.meleeSwingDelay == 0//not a melee weapon
			&& PlayerWeaponsComponent.currentWeapon != 0
			&& WeaponBehaviorComponent.reloadLastStartTime + WeaponBehaviorComponent.reloadLastTime < Time.time){
				//adjust FOV and weapon position for zoom
				nextFov = WeaponBehaviorComponent.zoomFOV;
				FovSmoothSpeed = 0.09f;//faster FOV zoom speed when zooming in
				yDampSpeed = 0.09f;
				zDampSpeed = 0.15f;
				//X pos with idle movement
				nextPosX = WeaponBehaviorComponent.weaponZoomXPosition + (side / 1.5f) + idleX 
					//also add X bobbing amounts here so they get smoothed to remove any small glitches in position
					+ (((HorizontalBob.translateChange / sprintBobAmtX) * bobDeltaAmount) * gunRunSide * bobDir);
				//Y pos with idle movement
				nextPosY = WeaponBehaviorComponent.weaponZoomYPosition + (raise / 1.5f) + idleY + (bobMove + switchMove + climbMove + jumpAmt + jumpmove) 
					//also add Y bobbing amounts here so they get smoothed to remove any small glitches in position
					+ (((VerticalBob.translateChange / sprintBobAmtY) * bobDeltaAmount) * gunRunUp * bobDir);
				//Z pos
				nextPosZ = WeaponBehaviorComponent.weaponZoomZPosition;
				//slow down turning and movement speed for zoom
				FPSWalker.zoomSpeed = true;
				//If not a melee weapon, play sound effect when raising sights
				if(zoomSfxState && WeaponBehaviorComponent.meleeSwingDelay == 0 && !WeaponBehaviorComponent.unarmed){
					AudioSource.PlayClipAtPoint(sightsUpSnd, mainCamTransform.position);
					zoomSfxState = false;
				}
				
				//Reduce mouse sensitivity when zoomed, but maintain sensitivity set in SmoothMouseLook script
				SmoothMouseLook.sensitivityAmt = SmoothMouseLook.sensitivity * zoomSensitivity;
				
				//Gradually increase or decrease bobbing amounts for smooth transitions between movement states
					
				////zoomed bobbing amounts////
				//horizontal bobbing 
				if(GBamt > ZoomHorizontalBobAmount){GBamt -= 0.005f * deltaAmount;}
				if(GBamt < ZoomHorizontalBobAmount){GBamt += 0.005f * deltaAmount;}
				//vertical bobbing	
				if(HBamt > ZoomVerticalBobAmount){HBamt -= 0.005f * deltaAmount;}
				if(HBamt < ZoomVerticalBobAmount){HBamt += 0.005f * deltaAmount;}
				//pitching	
				if(HPamt > ZoomBobPitchAmount){HPamt -= 0.0075f * deltaAmount;}
				if(HPamt < ZoomBobPitchAmount){HPamt += 0.0075f * deltaAmount;}
				//rolling	
				if(HRamt > ZoomBobRollAmount){HRamt -= 0.0075f * deltaAmount;}
				if(HRamt < ZoomBobRollAmount){HRamt += 0.0075f * deltaAmount;}
				//yawing	
				if(HYamt > ZoomBobYawAmount){HYamt -= 0.0075f * deltaAmount;}
				if(HYamt < ZoomBobYawAmount){HYamt += 0.0075f * deltaAmount;}
				
				if(!FPSWalker.swimming){
					//Set bobbing speeds and amounts in other scripts to these smoothed values
					VerticalBob.bobbingSpeed = ZoomBobSpeed;
					//make horizontal bob speed half as slow as vertical bob speed for synchronization of bobbing motions
					HorizontalBob.bobbingSpeed = ZoomBobSpeed / 2.0f;
				}else{
					//Set bobbing speeds and amounts in other scripts to these smoothed values
					VerticalBob.bobbingSpeed = ZoomBobSpeed / 2.0f;
					//make horizontal bob speed half as slow as vertical bob speed for synchronization of bobbing motions
					HorizontalBob.bobbingSpeed = ZoomBobSpeed / 4.0f;	
				}
				VerticalBob.bobbingAmount = HBamt * deltaAmount;//apply delta at this step for framerate independence
				VerticalBob.rollingAmount = HRamt * deltaAmount;
				VerticalBob.yawingAmount = HYamt * deltaAmount;
				VerticalBob.pitchingAmount = HPamt * deltaAmount;
				HorizontalBob.bobbingAmount = GBamt * deltaAmount;
				
			}else{//not zoomed
				
				FovSmoothSpeed = 0.18f;//slower FOV zoom speed when zooming out
				
				//adjust weapon Y position smoothing speed for unzoom and switching weapons
				if(!PlayerWeaponsComponent.switching){
					yDampSpeed = 0.18f;//weapon swaying speed
				}else{
					yDampSpeed = 0.2f;//weapon switch raising speed
				}
				zDampSpeed = 0.1f;
				//X pos with idle movement
				nextPosX = side + idleX + sideMove + horizontalGunPosAmt 
					//also add X bobbing amounts here so they get smoothed to remove any small glitches in position
					+ (((HorizontalBob.translateChange / sprintBobAmtX) * bobDeltaAmount) * gunRunSide * bobDir);
				//Y pos with idle movement
				nextPosY = raise + idleY + (bobMove + climbMove + switchMove + jumpAmt + jumpmove) + WeaponBehaviorComponent.weaponUnzoomYPosition
					//also add Y bobbing amounts here so they get smoothed to remove any small glitches in position
					+ (((VerticalBob.translateChange / sprintBobAmtY) * bobDeltaAmount) * gunRunUp * bobDir);
				//Z pos
				nextPosZ = WeaponBehaviorComponent.weaponUnzoomZPosition;
				//Set turning and movement speed for unzoom
				FPSWalker.zoomSpeed = false;	
				//If not a melee weapon, play sound effect when lowering sights	
				if(!zoomSfxState && WeaponBehaviorComponent.meleeSwingDelay == 0 && !WeaponBehaviorComponent.unarmed){
					AudioSource.PlayClipAtPoint(sightsDownSnd, mainCamTransform.position);
					zoomSfxState = true;
				}
				//Return mouse sensitivity to normal
				SmoothMouseLook.sensitivityAmt = SmoothMouseLook.sensitivity;
				
				//Set weapon and view bobbing amounts
				if (FPSWalker.sprintActive
				&& !((Mathf.Abs(horizontal) != 0.0f) && (Mathf.Abs(vertical) < 0.75f))
				&& Mathf.Abs(vertical) != 0.0f
				&& !FPSWalker.cancelSprint
				&& !FPSWalker.crouched
				&& FPSWalker.midPos >= FPSWalker.standingCamHeight
				&& !FPSPlayerComponent.zoomed
				&& !Input.GetKey (FPSPlayerComponent.fire)){
				
					//scale up bob speeds slowly to prevent jerky transition
					if (FPSWalker.grounded){
						////sprinting bobbing amounts////
						//horizontal bobbing 
						if(GBamt < SprintHorizontalBobAmount){GBamt += 0.005f * deltaAmount;}
						if(GBamt > SprintHorizontalBobAmount){GBamt -= 0.005f * deltaAmount;}
						//vertical bobbing
						if(HBamt < SprintVerticalBobAmount){HBamt += 0.005f * deltaAmount;}
						if(HBamt > SprintVerticalBobAmount){HBamt -= 0.005f * deltaAmount;}
						//pitching
						if(HPamt < SprintBobPitchAmount){HPamt += 0.0075f * deltaAmount;}
						if(HPamt > SprintBobPitchAmount){HPamt -= 0.0075f * deltaAmount;}
						//rolling
						if(HRamt < SprintBobRollAmount){HRamt += 0.0075f * deltaAmount;}
						if(HRamt > SprintBobRollAmount){HRamt -= 0.0075f * deltaAmount;}
						//yawing
						if(HYamt < SprintBobYawAmount){HYamt += 0.0075f * deltaAmount;}
						if(HYamt > SprintBobYawAmount){HYamt -= 0.0075f * deltaAmount;}
					}else{
						//reduce bobbing amounts for smooth jumping/landing transition
						if(HBamt > 0.0f){HBamt -= 0.01f * deltaAmount;}
						if(HRamt > 0.0f){HRamt -= 0.02f * deltaAmount;}
						if(HYamt > 0.0f){HYamt -= 0.02f * deltaAmount;}
						if(HPamt > 0.0f){HPamt -= 0.02f * deltaAmount;}
						if(GBamt > 0.0f){GBamt -= 0.01f * deltaAmount;}
					}
					//Set bobbing speeds and amounts in other scripts to these smoothed values
					VerticalBob.bobbingSpeed = SprintBobSpeed;
					//make horizontal bob speed half as slow as vertical bob speed for synchronization of bobbing motions
					HorizontalBob.bobbingSpeed = SprintBobSpeed / 2.0f;
					HorizontalBob.bobbingAmount = GBamt * deltaAmount;//apply delta at this step for framerate independence
					VerticalBob.rollingAmount = HRamt * deltaAmount;
					VerticalBob.yawingAmount = HYamt * deltaAmount;
					VerticalBob.pitchingAmount = HPamt * deltaAmount;
					VerticalBob.bobbingAmount = HBamt * deltaAmount;
					//move weapon toward or away from camera while sprinting
					nextPosZ = WeaponBehaviorComponent.weaponSprintZPosition;
		
				}else{
				
					//scale up bob speeds slowly to prevent jerky transition
					if (FPSWalker.grounded) {
						if (!FPSWalker.crouched && FPSWalker.midPos >= FPSWalker.standingCamHeight){
							////walking bob amounts///
							//horizontal bobbing 
							if(GBamt < WalkHorizontalBobAmount){GBamt += 0.005f * deltaAmount;}
							if(GBamt > WalkHorizontalBobAmount){GBamt -= 0.005f * deltaAmount;}
							//vertical bobbing
							if(HBamt < WalkVerticalBobAmount){HBamt += 0.005f * deltaAmount;}
							if(HBamt > WalkVerticalBobAmount){HBamt -= 0.005f * deltaAmount;}
							//pitching
							if(HPamt < WalkBobPitchAmount){HPamt += 0.0075f * deltaAmount;}
							if(HPamt > WalkBobPitchAmount){HPamt -= 0.0075f * deltaAmount;}
							//rolling
							if(!FPSWalker.swimming){
								if(HRamt < WalkBobRollAmount){HRamt += 0.0075f * deltaAmount;}
								if(HRamt > WalkBobRollAmount){HRamt -= 0.0075f * deltaAmount;}
							}else{
								if(HRamt < WalkBobRollAmount * 2.0f){HRamt += 0.0075f * deltaAmount;}
								if(HRamt > WalkBobRollAmount * 2.0f){HRamt -= 0.0075f * deltaAmount;}	
							}
							//yawing
							if(HYamt < WalkBobYawAmount){HYamt += 0.0075f * deltaAmount;}
							if(HYamt > WalkBobYawAmount){HYamt -= 0.0075f * deltaAmount;}
										
							if(!FPSWalker.swimming){
								VerticalBob.bobbingSpeed = WalkBobSpeed;
								//make horizontal bob speed half as slow as vertical bob speed for synchronization of bobbing motions
								HorizontalBob.bobbingSpeed = WalkBobSpeed / 2.0f;
							}else{//bobbing is slower while swimming
								VerticalBob.bobbingSpeed = WalkBobSpeed/2;
								HorizontalBob.bobbingSpeed = WalkBobSpeed / 4.0f;
							}
							
						}else{
							////crouching bob amounts////
							//horizontal bobbing 
							if(GBamt < CrouchHorizontalBobAmount){GBamt += 0.005f * deltaAmount;}
							if(GBamt > CrouchHorizontalBobAmount){GBamt -= 0.005f;}
							//vertical bobbing
							if(HBamt < CrouchVerticalBobAmount){HBamt += 0.005f * deltaAmount;}
							if(HBamt > CrouchVerticalBobAmount){HBamt -= 0.005f * deltaAmount;}
							//pitching
							if(HPamt < CrouchBobPitchAmount){HPamt += 0.0075f * deltaAmount;}
							if(HPamt > CrouchBobPitchAmount){HPamt -= 0.0075f * deltaAmount;}
							//rolling
							if(HRamt < CrouchBobRollAmount){HRamt += 0.0075f * deltaAmount;}
							if(HRamt > CrouchBobRollAmount){HRamt -= 0.0075f * deltaAmount;}
							//yawing
							if(HYamt < CrouchBobYawAmount){HYamt += 0.0075f * deltaAmount;}
							if(HYamt > CrouchBobYawAmount){HYamt -= 0.0075f * deltaAmount;}
				
							VerticalBob.bobbingSpeed = CrouchBobSpeed;
							HorizontalBob.bobbingSpeed = CrouchBobSpeed / 2.0f;
						}
					}else{
						//reduce bobbing amounts for smooth jumping/landing transition
						if(HBamt > 0.0f){HBamt -= 0.01f * deltaAmount;}
						if(HRamt > 0.0f){HRamt -= 0.02f * deltaAmount;}
						if(HYamt > 0.0f){HYamt -= 0.02f * deltaAmount;}
						if(HPamt > 0.0f){HPamt -= 0.02f * deltaAmount;}
						if(GBamt > 0.0f){GBamt -= 0.01f * deltaAmount;}	
					}
					VerticalBob.bobbingAmount = GBamt * deltaAmount;//apply delta at this step for framerate independence
					VerticalBob.rollingAmount = HRamt * deltaAmount;
					VerticalBob.yawingAmount = HYamt * deltaAmount;
					VerticalBob.pitchingAmount = HPamt * deltaAmount;
					HorizontalBob.bobbingAmount = HBamt * deltaAmount;
				}
			}
		}
	}
}