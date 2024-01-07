//FPSRigidBodyWalker.cs by Azuline Studios© All Rights Reserved
//Manages player movement controls, sets player movement speed, plays certain sound effects 
//determines player movement state, and sets player's rigidbody velocity.
using UnityEngine;
using System.Collections;

public class FPSRigidBodyWalker : MonoBehaviour {
	
	//objects accessed by this script
	[HideInInspector]
	public GameObject mainObj;
	[HideInInspector]
	public GameObject weaponObj;
	[HideInInspector]
	public GameObject CameraObj;
	[HideInInspector]
	public Transform myTransform;
	private Transform mainCamTransform;
	
	//track player input
	[HideInInspector]
	public float inputXSmoothed = 0.0f;//binary inputs smoothed using lerps
	[HideInInspector]
	public float inputYSmoothed = 0.0f;
	[HideInInspector]
	public float inputX = 0;//1 = button pressed 0 = button released
	[HideInInspector]
	public float inputY = 0;
	private float InputYLerpSpeed;//to allow quick deceleration when running into walls
	private float InputXLerpSpeed;
	
	//player movement speed amounts
	public float walkSpeed = 4.0f;
	public float sprintSpeed = 9.0f;
	
	//sprinting
	public enum sprintType{//Sprint mode
		hold,
		toggle,
		both
	}
	public sprintType sprintMode = sprintType.both;
	private float sprintDelay = 0.4f;
	public bool limitedSprint = true;//true if player should run only while staminaForSprint > 0 
	public bool sprintRegenWait = true;//true if player should wait for stamina to fully regenerate before sprinting
	public float sprintRegenTime = 3.0f;//time it takes to fully regenerate stamina if sprintRegenWait is true
	private bool breathFxState;
	public float staminaForSprint = 5.0f;//duration allowed for sprinting when limitedSprint is true
	[HideInInspector]
	public float staminaForSprintAmt;//actual duration amt allowed for sprinting modified by scripts
	public bool catchBreathSound = true;//true if the catch breath sound effect should be played when staminaForSprint is depleted
	
	public float jumpSpeed = 3.0f;//vertical speed of player jump
	public float climbSpeed = 4.0f;//speed that player moves vertically when climbing
	public bool lowerGunForClimb = true;//if true, gun will be lowered when climbing surfaces
	public bool lowerGunForSwim = true;//if true, gun will be lowered when swimming
	public bool lowerGunForHold = true;//if true, gun will be lowered when holding object
	[HideInInspector]
	public bool holdingObject;
	[HideInInspector]
	public bool hideWeapon;//true when weapon should be hidden from view and deactivated
	
	//swimming customization
	public float swimSpeed = 4.0f;//speed that player moves vertically when swimming
	public float holdBreathDuration = 15.0f;//amount of time before player starts drowning
	public float drownDamage = 7.0f;//rate of damage to player while drowning
	
	//player speed limits
	private float limitStrafeSpeed = 0.0f;
	public float backwardSpeedPercentage = 0.6f;//percentage to decrease movement speed while moving backwards
	public float crouchSpeedPercentage = 0.55f;//percentage to decrease movement speed while crouching
	private float crouchSpeedAmt = 1.0f;
	public float strafeSpeedPercentage = 0.8f;//percentage to decrease movement speed while strafing directly left or right
	private float speedAmtY = 1.0f;//current player speed per axis which is applied to rigidbody velocity vector
	private float speedAmtX = 1.0f;
	[HideInInspector]
	public bool zoomSpeed;//to control speed of movement while zoomed, handled by Ironsights script and true when zooming
	public float zoomSpeedPercentage = 0.6f;//percentage to decrease movement speed while zooming
	private float zoomSpeedAmt = 1.0f;
	private float speed;//combined axis speed of player movement
		
	//rigidbody physics settings
	[HideInInspector]
	public float standingCamHeight = 0.9f;
	[HideInInspector]
	public float crouchingCamHeight = 0.45f;//y position of camera while crouching
	private float standingCapsuleheight = 2.0f;//height of capsule while standing
	private float crouchingCapsuleHeight = 1.25f;//height of capsule while crouching
	private float crouchingHeightChange = 5.0f;//rate to transition to crouching state
	private float standingHeightChange = 2.25f;//rate to transition back to standing state
	private float capsuleCastHeight = 0.75f;//height of capsule cast above player to check for obstacles before standing from crouch
	private float rayCastHeight = 2.6f;
	public float playerHeightMod = 0.0f;//amount to add to player height (proportionately increases player height, radius, and capsule cast/raycast heights)
	public float crouchHeightPercentage = 0.5f;//percent of standing height to move camera to when crouching
	public int gravity = 15;//additional gravity that is manually applied to the player rigidbody
	private int maxVelocityChange = 5;//maximum rate that player velocity can change
	private Vector3 moveDirection = Vector3.zero;//movement velocity vector, modified by other speed factors like walk, zoom, and crouch states
	
	//grounded and slopelimit checks
	public int slopeLimit = 50;//the maximum allowed ground surface/normal angle that the player is allowed to climb
	[HideInInspector]
	public bool grounded;//true when capsule cast hits ground surface
	private bool rayTooSteep;//true when ray from capsule origin hits surface/normal angle greater than slopeLimit, compared with capsuleTooSteep
	private bool capsuleTooSteep;//true when capsule cast hits surface/normal angle greater than slopeLimit, compared with rayTooSteep
	
	//player movement states
	[HideInInspector]
	public Vector3 velocity = Vector3.zero;//total movement velocity vector
	[HideInInspector]
	public CapsuleCollider capsule;
	private	Vector3 sweepPos;
	private	Vector3 sweepHeight;
	private bool parentState;//only set parent once to prevent rapid parenting and de-parenting that breaks functionality
	[HideInInspector]
	public bool inWater;//true when player is touching water collider/trigger
	[HideInInspector]
	public bool holdingBreath;//true when player view/camera is under the waterline
	[HideInInspector]
	public bool belowWater;//true when player is below water movement threshold and is not treading water (camera/view slightly above waterline)
	[HideInInspector]
	public bool swimming;//set by WaterZone.cs script, if true, player will start using swimming movement methods instead of ground/walking methods
	[HideInInspector]
	public bool canWaterJump = true;//to make player release and press jump button again to jump if surfacing from water by holding jump button
	private float swimmingVerticalSpeed;
	[HideInInspector]
	public float swimStartTime;
	[HideInInspector]
	public float diveStartTime;
	[HideInInspector]
	public bool drowning;//true when player has stayed under water for longer than holdBreathDuration
	private float drownStartTime = 0.0f;
		
	//falling
	[HideInInspector]
	public float airTime = 0.0f;//total time that player is airborn
	private bool airTimeState;
	public float fallingDamageThreshold = 5.5f;//Units that player can fall before taking damage
	private float fallStartLevel;//the y coordinate that the player lost grounding and started to fall
	[HideInInspector]
	public float fallingDistance;//total distance that player has fallen
	private bool falling;//true when player is losing altitude
		
	//climbing (ladders or other climbable surfaces)
	[HideInInspector]
	public bool climbing;//true when playing is in contact with ladder trigger or edge climbing trigger
	[HideInInspector]
	public bool noClimbingSfx;//true when playing is in contact with edge climbing trigger or ladder with false Play Climbing Audio value
	[HideInInspector]
	public float verticalSpeedAmt = 4.0f;//actual rate that player is climbing
	
	//jumping
	public float antiBunnyHopFactor = 0.35f;//to limit the time between player jumps
	[HideInInspector]
	public bool jumping;//true when player is jumping
	private float jumpTimer = 0.0f;//track the time player began jump
	private bool jumpfxstate = true;
	[HideInInspector]
	public bool jumpBtn = true;//to control jump button behavior
	[HideInInspector]
	public float landStartTime = 0.0f;//time that player landed from jump
		
	//sprinting
	[HideInInspector]
	public bool canRun = true;//true when player is allowed to sprint
	[HideInInspector]
	public bool sprintActive;//true when sprint button is ready
	private bool sprintBtnState;
	private float sprintStartTime;
	private float sprintStart = -2.0f;
	private float sprintEnd;
	private bool sprintEndState;
	private bool sprintStartState;
	[HideInInspector]
	public bool cancelSprint;//true when sprint is canceled by other player input
	[HideInInspector]
	public float sprintStopTime = 0.0f;//track when sprinting stopped for control of item pickup time in FPSPlayer script 
	private bool sprintStopState = true;
	
	//crouching	
	[HideInInspector]
	public float midPos = 0.9f;//camera vertical position which is passed to VerticalBob.cs and HorizontalBob.cs
	[HideInInspector]
	public bool crouched;//true when player is crouching
	[HideInInspector]
	public bool crouchState;
		
	//sound effects
	[HideInInspector]
	public AudioClip landfx;//audiosource attached to this game object with landing sound effect
	
	public LayerMask clipMask;//mask for reducing the amount of objects that ray and capsule casts have to check

	private FPSPlayer FPSPlayerComponent;

	private SmoothMouseLook SmoothMouseLookComponent;

	private Footsteps FootstepsComponent;

	private WorldRecenter WorldRecenterComponent;
	Animation mainCamAnim;

	void Start (){
		//Initialize rigidbody
		GetComponent<Rigidbody>().freezeRotation = true;
		GetComponent<Rigidbody>().useGravity = true;
		capsule = GetComponent<CapsuleCollider>();
		
		myTransform = transform;//cache transforms for efficiency
		mainCamTransform = Camera.main.transform;
		
		//clamp movement modifier percentages
		Mathf.Clamp01(backwardSpeedPercentage);
		Mathf.Clamp01(crouchSpeedPercentage);
		Mathf.Clamp01(strafeSpeedPercentage);
		Mathf.Clamp01(zoomSpeedPercentage);
		
		staminaForSprintAmt = staminaForSprint;//initialize sprint duration counter
		
		//Set capsule dimensions to default if they have been changed in the inspector
		//because the capsule cast for detecting obstacles in fron of player scales
		//its sweep distance based on these dimensions and the playerHeightMod var.
		capsule.height = 2.0f;
		capsule.radius = 0.5f;
		
		//initialize player height variables
		capsule.height = capsule.height + playerHeightMod;
		capsule.radius = capsule.height * 0.25f;
		//initilize capsule heights
		standingCapsuleheight = 2.2f + playerHeightMod;
		crouchingCapsuleHeight = (crouchHeightPercentage * 1.25f) * standingCapsuleheight;
		//initialize camera heights
		standingCamHeight = 0.42f * standingCapsuleheight;
		crouchingCamHeight = crouchHeightPercentage * standingCamHeight;
		//initialize height changing rates
		crouchingHeightChange = (5.0f + playerHeightMod);
		standingHeightChange = (2.25f + playerHeightMod);
		//initialize rayCast and capsule cast heights
		if(playerHeightMod > 2.0f){
			//adjust capsule cast height for playerHeightMod amount for better grounded detection and jump timing
			capsuleCastHeight = 0.25f + playerHeightMod / 2;
		}else{
			capsuleCastHeight = 0.75f + playerHeightMod / 2;
		}
		rayCastHeight = 2.6f + playerHeightMod;
		
		//scale up jump speed to height addition made by playerHeightMod
		jumpSpeed = jumpSpeed / (1 - (playerHeightMod / capsule.height));
		InitializeComponents();
	}

	private void InitializeComponents()
	{
		FPSPlayerComponent = GetComponent<FPSPlayer>();
		SmoothMouseLookComponent = CameraObj.GetComponent<SmoothMouseLook>();
		FootstepsComponent = GetComponent<Footsteps>();
		WorldRecenterComponent = base.transform.GetComponent<WorldRecenter>();
		mainCamAnim = mainCamTransform.GetComponent<Animation>();
	}
	
	void Update(){
		//set the vertical bounds of the capsule used to detect player collisions
		Vector3 p1 = myTransform.position;//bottom of player capsule
		Vector3 p2 = p1 + Vector3.up * capsule.height/2;//top of player capsule
    	//set crouched variable that other scripts will access to check for crouching
		//do this in Update() instead of FixedUpdate to prevent missed button presses between fixed updates
    	if((Input.GetKeyDown (FPSPlayerComponent.crouch) || MobileControl.I.HasCrouch) && !swimming && !climbing){
    		if(!crouchState){
    			if(!crouched){
    				crouched = true;
    				sprintActive = false;//cancel sprint if crouch button is pressed
    			}else{//only uncrouch if the player has room above them to stand up
					if(!Physics.CheckCapsule(p1, p2 + (Vector3.up * 0.3f), capsule.radius, clipMask.value)){
    					crouched = false;
					}
    			}
    			crouchState = true;
    		}
    	}else{
    		crouchState = false;
    		if((sprintActive || climbing || swimming) && !Physics.CheckCapsule(p1, p2 + (Vector3.up * 0.3f), capsule.radius, clipMask.value)){
    			crouched = false;//cancel crouch if sprint button is pressed and there is room above the player to stand up
    		}
    	}
		
	}

	
	public bool MyMoveForward()
	{
		return Input.GetKey(FPSPlayerComponent.moveForward) || MobileControl.I.MovementY > 0.25f;
	}

	public bool MyMoveBack()
	{
		return Input.GetKey(FPSPlayerComponent.moveBack) || MobileControl.I.MovementY < -0.25f;
	}

	public bool MyStrafeLeft()
	{
		return Input.GetKey(FPSPlayerComponent.strafeLeft) || MobileControl.I.MovementX < -0.25f;
	}

	public bool MyStrafeRight()
	{
		return Input.GetKey(FPSPlayerComponent.strafeRight) || MobileControl.I.MovementX > 0.25f;
	}

	public bool MyZoom()
	{
		return Input.GetKey(FPSPlayerComponent.zoom) || MobileControl.I.HasZoom;
	}

	public bool CanUseInput { get; set; }
	
	void FixedUpdate(){
		RaycastHit rayHit;
		RaycastHit capHit;
		RaycastHit hit2;
		
		if(Time.timeScale > 0){//allow pausing by setting timescale to 0
			
			//set the vertical bounds of the capsule used to detect player collisions
			Vector3 p1 = myTransform.position;//bottom of player capsule
			Vector3 p2 = p1 + Vector3.up * capsule.height/2;//top of player capsule
			
			//manage the CapsuleCast size for frontal collision check based on player grounded state
			if(grounded){
				//move bottom of frontal CapsuleCast higher than bottom of player capsule to allow climbing up stairs
				sweepPos = myTransform.position + Vector3.up * (0.1f + (playerHeightMod * 0.05f));//keep sweepPos in proportion with playerHeightMod (original capsule height must be 2.0f)
				sweepHeight = myTransform.position + Vector3.up * (0.75f + (playerHeightMod * 0.375f));
			}else{
				sweepPos = myTransform.position - Vector3.up * (0.2f + (playerHeightMod * 0.1f));
				sweepHeight = myTransform.position + Vector3.up * (capsule.height * 1.2f + (playerHeightMod * 0.6f));
			}
			
			//track rigidbody velocity
			velocity = GetComponent<Rigidbody>().velocity;
				
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Player Input
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		
			if(FPSPlayerComponent.hitPoints > 1.0f && CanUseInput){
				//track movement buttons and set input vars
				//Input.Axis is not used here to have have more control over button states and to avoid glitches (unless useAxisInput var of FPSPlayer.cs is true) 
				//such as pressing opposite movement buttons simultaneously causing player to move very slow in one direction
				
				//Sweep a capsule in front of player to detect walls or other obstacles and stop Y input if there is a detected
				//collision to prevent player capsule from overlapping into world collision geometry in between fixed updates. 
				//This allows smoother jumping over obstacles when the player is walking into them. 
				if(!Physics.CapsuleCast (sweepPos, sweepHeight, capsule.radius * 0.5f, myTransform.forward, out hit2, 0.4f + (playerHeightMod * 0.2f), clipMask.value) || climbing) {
					if(!FPSPlayerComponent.useAxisInput){
						//normal y input handling
						if(MyMoveForward()){inputY = 1;}
						//decrease y input lerp speed to allow the player to slowly come to rest when forward button is pressed
						if(!swimming){
							InputYLerpSpeed = 6.0f;
						}else{
							InputYLerpSpeed = 3.0f;//player accelerates and decelerates slower in water
						}
					}else{
						inputYSmoothed = Input.GetAxis("Vertical");
						inputY = inputYSmoothed;	
					}
				}else{
					if((!hit2.rigidbody && grounded)){//allow the player to run into rigidbodies
						//zero out player y input if an object is detected in front of player
						inputY = 0;	
						//increase the y input lerp speed to allow the player to stop quickly and prevent overlap of colliders
						InputYLerpSpeed = 128.0f;
						if(FPSPlayerComponent.useAxisInput){inputYSmoothed = 0.0f;}
					}
				}
				
				if(!FPSPlayerComponent.useAxisInput){//only check keyboard buttons if not using Unity's input axes
				
					if(!swimming){
						InputXLerpSpeed = 6.0f;
					}else{
						InputXLerpSpeed = 3.0f;//player accelerates and decelerates slower in water
					}
				
					//track input of other movement keys
					if (MyMoveBack()){inputY = -1;}
					if (!MyMoveBack() && !MyMoveForward()){inputY = 0;}
					if (MyMoveBack() && MyMoveForward()){inputY = 0;}
					if (MyStrafeLeft()){inputX = -1;}
					if (MyStrafeRight()){inputX = 1;}
					if (!MyStrafeLeft() && !MyStrafeRight()){inputX = 0;}
					if (MyStrafeLeft() && MyStrafeRight()){inputX = 0;}
					
					//Smooth our movement inputs using Mathf.Lerp
					inputXSmoothed = Mathf.Lerp(inputXSmoothed,inputX,Time.deltaTime * InputXLerpSpeed);
				    inputYSmoothed = Mathf.Lerp(inputYSmoothed,inputY,Time.deltaTime * InputYLerpSpeed);
						
				}else{//use Unity's input axes for movement instead
					inputXSmoothed = Input.GetAxis("Horizontal");
					inputX = inputXSmoothed;
				}
					
			}
			
			//set hideWeapon var in WeaponBehavior.cs to true if weapon should be hidden when climbing, swimming, or holding object
			if((holdingBreath && lowerGunForSwim) || (climbing && lowerGunForClimb) || (holdingObject && lowerGunForHold)){
				hideWeapon = true;	
			}else{
				hideWeapon = false;	
			}
			
			
			//This is the start of the large block that performs all movement actions while grounded	
			if (grounded) {
					
				/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				//Landing
				/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				
				//reset airTimeState var so that airTime will only be set once when player looses grounding
				airTimeState = true;
				
				if (falling){//reset falling state and perform actions if player has landed from a fall
					
					fallingDistance = 0;
					landStartTime = Time.time;//track the time when player landed
			       	falling = false;
			        
			        if((fallStartLevel - myTransform.position.y) > 2.0f){
			        	//play landing sound effect when falling and not landing from jump
			        	if(!jumping){
							
							if(!inWater){
								//play landing sound
								AudioSource.PlayClipAtPoint(landfx, mainCamTransform.position);
							}
								
							//make camera jump when landing for better feeling of player weight	
							if (mainCamAnim.IsPlaying("CameraLand")){
								//rewind animation if already playing to allow overlapping playback
								mainCamAnim.Rewind("CameraLand");
							}
							mainCamAnim.CrossFade("CameraLand", 0.35f,PlayMode.StopAll);
						}
			        }
			        
			        //track the distance of the fall and apply damage if over falling threshold
			        if (myTransform.position.y < fallStartLevel - fallingDamageThreshold && !inWater){
			        	CalculateFallingDamage(fallStartLevel - myTransform.position.y);
			        }
		    	}
					
				/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				//Crouch Mode Handling
				/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		    	//cancel crouch if jump button is pressed
		    	if(Input.GetKeyDown(FPSPlayerComponent.jump) 
				&& crouched 
				//only uncrouch if the player has room above them to stand up
				&& !Physics.CheckCapsule(p1, p2 + (Vector3.up * (0.3f + (playerHeightMod / 3.0f))), capsule.radius, clipMask.value)){
	    			crouched = false;
	    			landStartTime = Time.time;//set land time to time jump is pressed to prevent uncrouching and then also jumping
	    		}
					
				/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				//Sprinting
				/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				
				//set sprint mode to toggle, hold, or both, based on inspector setting
				switch (sprintMode){
					case sprintType.both:
						sprintDelay = 0.4f;
					break;
					case sprintType.hold:
						sprintDelay = 0.0f;
					break;
					case sprintType.toggle:
						sprintDelay = 999.0f;//time allowed between button down and release to activate toggle
					break;
				}

				//toggle or hold sprinting state by determining if sprint button is pressed or held
				if(Mathf.Abs(inputY) > 0.1f || MobileControl.I.MovementY > 0.1f){
					if(Input.GetKey(FPSPlayerComponent.sprint) || MobileControl.I.MovementY > 0.1f){
						if(!sprintStartState){
							sprintStart = Time.time;//track time that sprint button was pressed
							sprintStartState = true;//perform these actions only once
							sprintEndState = false;
							if(sprintEnd - sprintStart < sprintDelay * Time.timeScale){//if button is tapped, toggle sprint state
								if(!sprintActive){
									if(!sprintActive){//only allow sprint to start or cancel crouch if player is not under obstacle
										sprintActive = true;
									}else{
										sprintActive = false;//pressing sprint button again while sprinting stops sprint
									}
								}else{
									sprintActive = false;	
								}
							}
						}
					}else{
						if(!sprintEndState){
							sprintEnd = Time.time;//track time that sprint button was released
							sprintEndState = true;
							sprintStartState = false;
							if(sprintEnd - sprintStart > sprintDelay * Time.timeScale){//if releasing sprint button after holding it down, stop sprinting
								sprintActive = false;	
							}
						}
					}
				}else{
					if(!Input.GetKey(FPSPlayerComponent.sprint) || MobileControl.I.MovementY < 0.1f){
						sprintActive = false;
					}
				}
				
				//cancel a sprint in certain situations
				if((sprintActive && Input.GetAxis("Fire1") > 0.1f)//cancel sprint if fire button is pressed
				|| (sprintActive && Input.GetKey(FPSPlayerComponent.reload))//cancel sprint if reload button is pressed
				|| (sprintActive && MyZoom() && FPSPlayerComponent.CurrentWeaponBehavior.canZoom)//cancel sprint if zoom button is pressed
				|| (FPSPlayerComponent.zoomed && Input.GetKey (FPSPlayerComponent.fire))
				|| (Mathf.Abs(inputY) < 1.0f && Mathf.Abs(inputX) > 0.1f)//cancel sprint if player sprints into a wall and strafes left or right
				|| inputY < 0.0f //cancel sprint if player moves backwards
				|| jumping
				|| swimming
				|| climbing
				//cancel sprint if player runs out of breath
				|| (limitedSprint && staminaForSprintAmt <= 0.0f)){
					sprintActive = false;
				}
				
				//play catching breath sound if sprinting stamina is depleted
				if(limitedSprint){
					if(staminaForSprintAmt <= 0.0f){
						if(!breathFxState && FPSPlayerComponent.catchBreath && catchBreathSound){
							AudioSource.PlayClipAtPoint(FPSPlayerComponent.catchBreath, mainCamTransform.position);
							breathFxState = true;
						}
					}
				}
				
				//reset cancelSprint var so it has to pressed again to sprint
				if(!sprintActive && cancelSprint){
					if(!Input.GetKey (FPSPlayerComponent.zoom)){
						cancelSprint = false;
					}
				}

			//determine if player can run (check button input or Unity's input axes based on useAxisInput var of FPSPlayer.cs) for mobile
			if (MobileControl.I.MovementY > 0.25f)
			{
				FPSPlayerComponent.zoomed = false;//cancel zooming when sprinting
				sprintActive = true;
			}

			//determine if player can run (check button input or Unity's input axes based on useAxisInput var of FPSPlayer.cs) for pc
				if(((!FPSPlayerComponent.useAxisInput && MyMoveForward())
				||(FPSPlayerComponent.useAxisInput && Mathf.Abs(inputY) > 0.1f))
				&& sprintActive
				&& !crouched
				&& !cancelSprint
				&& grounded){
				 	canRun = true;
					FPSPlayerComponent.zoomed = false;//cancel zooming when sprinting
					sprintStopState = true;
					if(staminaForSprintAmt > 0.0f && limitedSprint){
						staminaForSprintAmt -= Time.deltaTime;//reduce stamina when sprinting
					}
				}else{
					if(sprintStopState){
						sprintStopTime = Time.time;
						sprintStopState = false;
					}
				 	canRun = false;
					if(limitedSprint){
						if(sprintRegenWait){//determine if player should not be allowed to run unless they have full stamina
							if(sprintStopTime + sprintRegenTime < Time.time){
								staminaForSprintAmt = staminaForSprint;//recover full stamina when not sprinting and sprintRegenTime has elapsed
							}
						}else{//option to allow player to run as soon as any stamina amount has regenerated 
							if(staminaForSprintAmt < staminaForSprint){
								staminaForSprintAmt += Time.deltaTime/* * 1.1f*/;//recover stamina when not sprinting (multiply this by a value to increase recover rate) 
							}
						}
						breathFxState = false;
					}
				}
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Player Movement Speeds
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//

            if(OrbitCameraController.I.ModelPers != null)
                    OrbitCameraController.I.ModelPers.GetComponentInChildren<Animator>().SetBool("Sprint_b", sprintActive);
			
				//check that player can run and set speed 
				if(canRun){
					if(speed < sprintSpeed - 0.1f){//offset speeds by 0.1f to prevent small oscillation of speed value
						speed += 12 * Time.deltaTime;//gradually accelerate to run speed
					}else if(speed > sprintSpeed + 0.1f){
						speed -= 12 * Time.deltaTime;//gradually decelerate to run speed
					}
				}else{
					if(!swimming){
						if(speed > walkSpeed + 0.1f){
							speed -= 16 * Time.deltaTime;//gradually decelerate to walk speed
						}else if(speed < walkSpeed - 0.1f){
							speed += 16 * Time.deltaTime;//gradually accelerate to walk speed
						}
					}else{
						if(speed > swimSpeed + 0.1f){
							speed -= 16 * Time.deltaTime;//gradually decelerate to swim speed
						}else if(speed < swimSpeed - 0.1f){
							speed += 16 * Time.deltaTime;//gradually accelerate to swim speed
						}		
					}
				}
						
				//check if player is zooming and set speed 
				if(zoomSpeed){
					if(zoomSpeedAmt > zoomSpeedPercentage){
						zoomSpeedAmt -= Time.deltaTime;//gradually decrease zoomSpeedAmt to zooming limit value
					}
				}else{
					if(zoomSpeedAmt < 1.0f){
						zoomSpeedAmt += Time.deltaTime;//gradually increase zoomSpeedAmt to neutral
					}
				}
				
				//check that player can crouch and set speed
				//also check midpos because player can still be under obstacle when crouch button is released 
				if(crouched || midPos < standingCamHeight){
					if(crouchSpeedAmt > crouchSpeedPercentage){
						crouchSpeedAmt -= Time.deltaTime;//gradually decrease crouchSpeedAmt to crouch limit value
					}
				}else{
					if(crouchSpeedAmt < 1.0f){
						crouchSpeedAmt += Time.deltaTime;//gradually increase crouchSpeedAmt to neutral
					}
				} 
				
				//limit move speed if backpedaling
				if (inputY >= 0){
					if(speedAmtY < 1.0f){
						speedAmtY += Time.deltaTime;//gradually increase speedAmtY to neutral
					}
				}else{
					if(speedAmtY > backwardSpeedPercentage){
						speedAmtY -= Time.deltaTime;//gradually decrease speedAmtY to backpedal limit value
					}
				}
				
				//allow limiting of move speed if strafing directly sideways and not diagonally
				if (inputX == 0 || inputY != 0){
					if(speedAmtX < 1.0f){
						speedAmtX += Time.deltaTime;//gradually increase speedAmtX to neutral
					}
				}else{
					if(speedAmtX > strafeSpeedPercentage){
						speedAmtX -= Time.deltaTime;//gradually decrease speedAmtX to strafe limit value
					}
				}
					
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Jumping
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
				if(jumping){
					//play landing sound effect after landing from jump and reset jumpfxstate
					if(jumpTimer + 0.05f < Time.time){
							//play landing sound
							if(!inWater){
								AudioSource.PlayClipAtPoint(landfx, mainCamTransform.position);
							}
							if (mainCamAnim.IsPlaying("CameraLand")){
								//rewind animation if already playing to allow overlapping playback
								mainCamAnim.Rewind("CameraLand");
							}
							mainCamAnim.CrossFade("CameraLand", 0.35f,PlayMode.StopAll);
							
							jumpfxstate = true;
					}

					//allow a small amount of time for capsule to become un-grounded before setting
					//jump button state to false to prevent continuous jumping if jump button is held.
					if(jumpTimer + 0.25f < Time.time){
						jumpBtn = false;
					}
					
					//reset jumping var (this check must be before jumping var is set to true below)
					jumping = false;
				}
				
				//determine if player is jumping and set jumping variable
				if ((Input.GetKey(FPSPlayerComponent.jump) || MobileControl.I.HasJump)
				&& !FPSPlayerComponent.zoomed
				&& jumpBtn//check that jump button is not being held
				&& !crouched
				&& !belowWater
				&& canWaterJump
				&& !climbing
				&& landStartTime + antiBunnyHopFactor < Time.time//check for bunnyhop delay before jumping
				&& (!rayTooSteep || inWater)){//do not jump if ground normal is greater than slopeLimit and not in water
					
					if(!jumping){
						jumping = true;
						//track the time we began to jump
						jumpTimer = Time.time;
					}
					//apply the jump velocity to the player rigidbody
					GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, Mathf.Sqrt(2.0f * jumpSpeed * gravity), velocity.z);
				}
				
				//reset jumpBtn to prevent continuous jumping while holding jump button.
				if (!Input.GetKey(FPSPlayerComponent.jump) && landStartTime + antiBunnyHopFactor < Time.time){
					jumpBtn = true;
				}
						
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Crouching
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				
				if(Time.timeSinceLevelLoad > 0.5f){			
					//crouch
					if(crouched || FPSPlayerComponent.hitPoints < 1.0f){//also lower to crouch position if player dies
				    		if(midPos > crouchingCamHeight){midPos -= crouchingHeightChange * Time.deltaTime;}//decrease camera height to crouch height
							if(capsule.height > crouchingCapsuleHeight){capsule.height -= crouchingHeightChange * Time.deltaTime;}//decrease capsule height to crouch height
					}else{
						if(!Input.GetKey (FPSPlayerComponent.jump)){
		            		if(midPos < standingCamHeight){midPos += standingHeightChange * Time.deltaTime;}//increase camera height to standing height
		         			if(capsule.height < standingCapsuleheight){capsule.height += standingHeightChange * Time.deltaTime;}//increase capsule height to standing height
						}
					}	
				}
				
			}else{//Player is airborn////////////////////////////////////////////////////////////////////////////////////////////////////////////
				
				//keep track of the time that player lost grounding for air manipulation and moving gun while jumping
				if(airTimeState){
					airTime = Time.time;
					airTimeState = false;
				}
					
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Falling
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				
				//subtract height we began falling from current position to get falling distance
				fallingDistance = fallStartLevel - myTransform.position.y;//this value referenced in other scripts
			
				if(!falling){
				    falling = true;			
				    //start tracking altitude (y position) for fall check
				    fallStartLevel = myTransform.position.y;
				    
				    //check jumpfxstate var to play jumping sound only once
				    if(jumping && jumpfxstate){
						//play jumping sound
						AudioSource.PlayClipAtPoint(FPSPlayerComponent.jumpfx, mainCamTransform.position);
						jumpfxstate = false;
					}	    
				}	
			}
				
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Climbing and Swimming
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			//make player climb up climbable surfaces, or swim up and down based on vertical mouslook angle
			if((climbing || swimming) && !jumping){//climbing var is managed by a ladder script attatched to a trigger that is placed near a ladder
				if(!FPSPlayerComponent.useAxisInput && MyMoveForward() || (FPSPlayerComponent.useAxisInput && inputY > 0)){//only climb if player is moving forward
					if(climbing){//make player climb up or down based on the pitch of the main camera (check mouselook script pitch)
						verticalSpeedAmt = 1 + (climbSpeed * (SmoothMouseLookComponent.inputY / 48));
						verticalSpeedAmt = Mathf.Clamp(verticalSpeedAmt, -climbSpeed, climbSpeed);//limit vertical speed to climb speed
					}else{
						if(swimming){
							verticalSpeedAmt = Mathf.Clamp(verticalSpeedAmt, -swimSpeed, swimSpeed);//limit vertical speed to swim speed
							if(!belowWater){
								if(SmoothMouseLookComponent.inputY < -55){//only swim downwards with look angle if player is treading water and looking down
									verticalSpeedAmt = 1 + (swimSpeed * -Mathf.Abs((SmoothMouseLookComponent.inputY / 48)));
								}else{
									verticalSpeedAmt = 0.0f;	
								}
							}else{
								if(SmoothMouseLookComponent.inputY > -15){//normal upwards swimming if not looking down too far
									verticalSpeedAmt = 1 + (swimSpeed * (SmoothMouseLookComponent.inputY / 48));
								}else{//prevent player from dragging against bottom if looking downwards and moving
									if(!Physics.CapsuleCast (p1, p2, capsule.radius * 0.9f, -myTransform.up, out capHit, capsuleCastHeight, clipMask.value)){//detect if player is close to bottom with a capsuleCast
										verticalSpeedAmt = 1 + (swimSpeed * (SmoothMouseLookComponent.inputY / 48));
									}else{
										verticalSpeedAmt = 0.0f;//prevent downward movement if player is at the bottom floor of water zone	
									}
								}
							}
						}
					}
					
					//apply climbing or swimming speed to the player's rigidbody velocity
					GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, verticalSpeedAmt, velocity.z);
					
				}else{
					//if not moving forward, do not add extra upward velocity, but allow the player to move off the ladder
					GetComponent<Rigidbody>().velocity = new Vector3(velocity.x, 0, velocity.z);
				}
			}
			
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Holding Breath
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
			if(holdingBreath){
				//determine if player will gasp for air when surfacing
				if(Time.time - diveStartTime > holdBreathDuration / 1.5f){
					drowning = true;	
				}
				//determine if player is drowning
				if(Time.time - diveStartTime > holdBreathDuration){
					if(drownStartTime < Time.time){
						FPSPlayerComponent.ApplyDamage(drownDamage); 
						drownStartTime = Time.time + 1.75f;
					}
				}
				
			}else{
				if(drowning){//play gasping sound if player needed air when surfacing
					AudioSource.PlayClipAtPoint(FPSPlayerComponent.gasp, mainCamTransform.position, 0.75f);
					drowning = false;
				}
			}
				
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Player Ground Check
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
			//cast capsule shape down to see if player is about to hit anything or is resting on the ground
		    if (Physics.CapsuleCast (p1, p2, capsule.radius * 0.9f, -myTransform.up, out capHit, capsuleCastHeight, clipMask.value) 
			|| climbing 
			|| (swimming)){
			
		        grounded = true;
				
				if(!climbing){
					if(!inWater && !swimming){//do not play landing sound if player is in water
						//determine what kind of surface the player is landing on and set landfx to that surface's landing sound effect
						switch(capHit.collider.gameObject.tag){
						case "Water":
							landfx = FootstepsComponent.waterLand;
							break;
						case "Dirt":
							landfx = FootstepsComponent.dirtLand;
							break;
						case "Wood":
							landfx = FootstepsComponent.woodLand;
							break;
						case "Metal":
							landfx = FootstepsComponent.metalLand;
							break;
						case "Stone":
							landfx = FootstepsComponent.stoneLand;
							break;
						default:
							landfx = FootstepsComponent.dirtLand;
							break;	
						}
					}
				}else{
					landfx = FootstepsComponent.dirtLand;	
				}
				
		    }else{
		    	grounded = false;
		    }
			
			//check that angle of the normal directly below the capsule center point is less than the movement slope limit 
			if (Physics.Raycast(myTransform.position, -myTransform.up, out rayHit, rayCastHeight, clipMask.value)) {
				if(Vector3.Angle ( rayHit.normal, Vector3.up ) > 60.0f && !inWater){
					rayTooSteep = true;	
				}else{
					rayTooSteep = false;	
				}
				//pass the material/surface type tag player is on to the Footsteps.cs script
				FootstepsComponent.materialType = rayHit.collider.gameObject.tag;
				
			}
				
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//Player Velocity
			/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		    
			//limit speed if strafing diagonally
			limitStrafeSpeed = (inputX != 0.0f && inputY != 0.0f)? .7071f : 1.0f;
			
			//align localEulerAngles and eulerAngles y values with cameras' to make player walk in the direction the camera is facing 
			Vector3 tempLocalEulerAngles = new Vector3(0.0f,CameraObj.transform.localEulerAngles.y,0.0f);//store angles in temporary vector
			myTransform.localEulerAngles = tempLocalEulerAngles;//apply angles from temporary vector to player object
			Vector3 tempEulerAngles = new Vector3(0.0f, CameraObj.transform.eulerAngles.y,0.0f);//store angles in temporary vector
			myTransform.eulerAngles = tempEulerAngles;//apply angles from temporary vector to player object
			
			//apply velocity to player rigidbody and allow a small amount of air manipulation
			//to allow jumping up on obstacles when jumping from stationary position with no forward velocity
			if((grounded || climbing || swimming || ((airTime + 0.3f) > Time.time)) && FPSPlayerComponent.hitPoints > 1.0f && !FPSPlayerComponent.restarting){
				//Check both capsule center point and capsule base slope angles to determine if the slope is too high to climb.
				//If so, bypass player control and apply some extra downward velocity to help capsule return to more level ground.
				if(!capsuleTooSteep || climbing || swimming || (capsuleTooSteep && !rayTooSteep)){
					
					// We are grounded, so recalculate movedirection directly from axes	
					moveDirection = new Vector3(inputXSmoothed * limitStrafeSpeed, 0.0f, inputYSmoothed * limitStrafeSpeed);
					//realign moveDirection vector to world space
					moveDirection = myTransform.TransformDirection(moveDirection);
					//apply speed limits to moveDirection vector
					moveDirection = moveDirection * speed * speedAmtX * speedAmtY * crouchSpeedAmt * zoomSpeedAmt;
			
					//apply a force that attempts to reach target velocity
					Vector3 velocityChange = moveDirection - velocity;
					//limit max speed
					velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
					velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
				
					//apply ladder climbing speed to velocityChange vector and set y velocity to zero if not climbing ladder
					if(climbing){
						if((!FPSPlayerComponent.useAxisInput && MyMoveForward()) || (FPSPlayerComponent.useAxisInput && Mathf.Abs(inputY) > 0.1f)){//move player up climbable surface if pressing forward button
							velocityChange.y = verticalSpeedAmt;
						}else if(Input.GetKey(FPSPlayerComponent.jump)){//move player up climbable surface if pressing jump button
							inputY = 1;//to cycle bobbing effects
							if(FPSPlayerComponent.useAxisInput){inputYSmoothed = 1.0f;}
							velocityChange.y = climbSpeed * 0.75f;
						}else if(Input.GetKey(FPSPlayerComponent.crouch)){//move player down climbable surface if pressing crouch button
							inputY = -1;//to cycle bobbing effects
							if(FPSPlayerComponent.useAxisInput){inputYSmoothed = 1.0f;}
							velocityChange.y = -climbSpeed * 0.75f;
						}else{
							velocityChange.y = 0;
						}
						
					}else{
						velocityChange.y = 0;
					}
					
					//finally, add movement velocity to player rigidbody velocity
					GetComponent<Rigidbody>().AddForce(velocityChange, ForceMode.VelocityChange);
					
				}else{
					//If slope is too high below both the center and base contact point of capsule, apply some downward velocity to help
					//the capsule fall to more level ground. Check the slope angle at two points on the collider to prevent it from 
					//getting stuck when player control is bypassed and to have more control over the slope angle limit.
					GetComponent<Rigidbody>().AddForce(new Vector3 (0, -2, 0), ForceMode.VelocityChange);
				}
			}else{
				if(FPSPlayerComponent.hitPoints < 1.0f || FPSPlayerComponent.restarting){		
					GetComponent<Rigidbody>().freezeRotation = false;//allow player's rigidbody to be pushed by forces and explosions after death
					SmoothMouseLookComponent.enabled = false;//disable mouselook on player death
				}
			}
			
			if(!climbing){
				if(!swimming){
					//apply gravity manually for more tuning control except when climbing a ladder to avoid unwanted downward movement
			    	GetComponent<Rigidbody>().AddForce(new Vector3 (0, -gravity * GetComponent<Rigidbody>().mass, 0));
			    	GetComponent<Rigidbody>().useGravity = true;
				}else{
					if(swimStartTime + 0.2f > Time.time){//make player sink under surface for a short time if they jumped in deep water 
						//dont make player sink if they are close to bottom
						if(landStartTime + 0.3f > Time.time){//make sure that player doesn't try to sink into the ground if wading into water
							if(!Physics.CapsuleCast (p1, p2, capsule.radius * 0.9f, -myTransform.up, out capHit, capsuleCastHeight, clipMask.value)){
								GetComponent<Rigidbody>().AddForce(new Vector3 (0, -6.0f, 0), ForceMode.VelocityChange);//make player sink into water after jump
							}
						}
					}else{	
						//make player rise to water surface if they hold the jump button
						if(Input.GetKey(FPSPlayerComponent.jump)){
							
							if(belowWater){
								swimmingVerticalSpeed = Mathf.MoveTowards(swimmingVerticalSpeed, 3.0f, Time.deltaTime * 4);
								if(holdingBreath){
									canWaterJump = false;//don't also jump if player just surfaced by holding jump button
								}
							}else{
								swimmingVerticalSpeed = 0.0f;	
							}
						//make player dive downwards if they hold the crouch button
						}else if(Input.GetKey(FPSPlayerComponent.crouch)){
							
							swimmingVerticalSpeed = Mathf.MoveTowards(swimmingVerticalSpeed, -3.0f, Time.deltaTime * 4);
							
						}else{
							//make player sink slowly when underwater due to the weight of their gear
							if(belowWater){
								swimmingVerticalSpeed = Mathf.MoveTowards(swimmingVerticalSpeed, -0.2f, Time.deltaTime * 4);
							}else{
								swimmingVerticalSpeed = 0.0f;	
							}
							
						}
						//allow jumping when treading water if player has released the jump button after surfacing 
						//by holding jump button down to prevent player from surfacing and immediately jumping
						if(!belowWater && !Input.GetKey(FPSPlayerComponent.jump)){
							canWaterJump = true;	
						}
						//apply the vertical swimming speed to the player rigidbody
						GetComponent<Rigidbody>().AddForce(new Vector3 (0, swimmingVerticalSpeed, 0), ForceMode.VelocityChange);
						
					}
					GetComponent<Rigidbody>().useGravity = false;//don't use gravity when swimming	
				}
			}else{
				GetComponent<Rigidbody>().useGravity = false;
			}
		}
		
	}
	
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//Rigidbody Collisions
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	void TrackCollision ( Collision col  ){
	    //define a height of about a fourth of the capsule height to check for collisions with platforms
		float maximumHeight = (capsule.bounds.min.y + capsule.radius);
		//check the collision points within our predefined height range  
		foreach(ContactPoint c in col.contacts){
			if (c.point.y < maximumHeight) {
				//check that we want to collide with this object (check for "Moving Platforms" layer) and that its surface is not too steep 
				if(!parentState && col.gameObject.layer == 15 && Vector3.Angle ( c.normal, Vector3.up ) < 70){
					//set player object parent to platform transform to inherit it's movement
					myTransform.parent = col.transform;
					parentState = true;//only set parent once to prevent rapid parenting and de-parenting that breaks functionality
				}
				//check that angle of the surface that the capsule base is touching is less than the movement slope limit  
				if(Vector3.Angle ( c.normal, Vector3.up ) > slopeLimit && !inWater){
					capsuleTooSteep = true;	
				}else{
					capsuleTooSteep = false;	
				}
			}
		}
		
	}
	
	void OnCollisionExit ( Collision col  ){
		WorldRecenter WorldRecenterComponent = transform.GetComponent<WorldRecenter>();
		if(col.gameObject.layer == 15){
			//unparent if we are no longer standing on a platform or elevator
			if(WorldRecenterComponent.removePrefabRoot){
				//prefab root was removed, so make player object's parent null when not on elevator or platform
				myTransform.parent = null;
			}else if(!WorldRecenterComponent.removePrefabRoot){
				//prefab root exists, so make player object's parent the mainObj, or !!!FPS Main object
				myTransform.parent = mainObj.transform;	
			}
		}
		//return parentState to false so we may check for collisions with elevators or platforms again
		parentState = false;
		capsuleTooSteep = false;	
		inWater = false;
	}
	
	void OnCollisionStay ( Collision col  ){
	   TrackCollision (col);
	}
	
	void OnCollisionEnter ( Collision col  ){
	   TrackCollision (col);
	}
	
	void CalculateFallingDamage ( float fallDistance  ){

	}
}