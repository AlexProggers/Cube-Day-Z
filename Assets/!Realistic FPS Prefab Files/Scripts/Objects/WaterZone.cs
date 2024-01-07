//WaterZone.cs by Azuline Studios© All Rights Reserved
//Attach to trigger to create a swimmable zone. 
//This script manages swimming/diving behaviors and water related effects. 
using UnityEngine;
using System.Collections;

public class WaterZone : MonoBehaviour {
	private GameObject playerObj;
	private GameObject weaponObj;
	private bool swimTimeState = false;
	private bool holdBreathState;
	private AudioSource audioSource;
	public AudioClip underwaterSound;//sound effect to play underwater
	public AudioSource[] aboveWaterAudioSources;//above-water ambient audio sources to pause when submerged
	//if true, the waterPlane object will be flipped upside down when player is submerged so player can see the surface
	public bool flipWaterPlane = true;
	public Transform waterPlane;
	private Vector3 waterPlaneRot;
	public ParticleSystem rippleEffect;//particles emitted around player treading water
	public ParticleSystem particlesEffect;//particles emitted underwater for ambient bubbles/particles
	private float particlesYPos;//to limit y position of underwater particle effect
	private float particleForwardPos;//to limit distance forward of camera for underwater particle effect
	public Transform waterSplash;//splash particles effect
	private Vector3 splashPos;
	public Transform splashTrail;//particles to emit when player is swimming on water surface
	private Vector3 trailPos;
	private float splashTrailTime;
	//sun/directional light that  should be changed to underwaterSunightColor when player is submerged 
	public Light SunlightObj;
	public Color underwaterSunightColor = new Color(0.15f, 0.32f, 0.4f, 1.0f);
	private Color origSunightColor = new Color(0.15f, 0.32f, 0.4f, 1.0f);
	//vars to apply underwater fog settings if submerged 
	public bool underwaterFogEnabled;//if true, underwater fog settings will be applied when submerged 
	public FogMode underwaterFogMode = FogMode.Linear;//changing fog mode from linear to exponential at runtime might cause small hiccup when first diving
	public Color underwaterFogColor = new Color(0.15f, 0.32f, 0.4f, 1.0f);
	public Color underwaterLightColor = new Color(0.15f, 0.32f, 0.4f, 1.0f);
	public float underwaterFogDensity = 0.15f;
	public float underwaterLinearFogStart = 0.0f;
	public float underwaterLinearFogEnd = 15.0f;
	//vars to set effects for above water
	private bool fogEnabled;
	private FogMode origFogMode = FogMode.Linear;
	private Color origFogColor = new Color(0.15f, 0.32f, 0.4f, 1.0f);
	private Color origLightColor = new Color(0.15f, 0.32f, 0.4f, 1.0f);
	private float origFogDensity = 0.15f;
	private float origLinearFogStart = 15.0f;
	private float origLinearFogEnd = 30.0f;
	//cache transform for efficiency
	private Transform myTransform;
	private Transform mainCamTransform;
	
	void Start () {
		myTransform = transform;
		mainCamTransform = Camera.main.transform;
		//assign this item's playerObj and weaponObj value
		playerObj = mainCamTransform.GetComponent<CameraKick>().playerObj;
		weaponObj = mainCamTransform.GetComponent<CameraKick>().weaponObj;
		//set up underwater sound effects
		audioSource = gameObject.AddComponent<AudioSource>();
	    audioSource.clip = underwaterSound;
		audioSource.loop = true;
		audioSource.volume = 0.8f;
		//store original fog values to apply to render settings when player surfaces
		fogEnabled = RenderSettings.fog;
		origFogColor = RenderSettings.fogColor;
		origFogDensity = RenderSettings.fogDensity;
		origFogMode = RenderSettings.fogMode;
		origLinearFogStart = RenderSettings.fogStartDistance;
		origLinearFogEnd = RenderSettings.fogEndDistance;
		origLightColor = RenderSettings.ambientLight;
		//store original sun/directional light color
		if(SunlightObj){origSunightColor = SunlightObj.color;}
	}
	
	void OnTriggerStay(Collider col){
		EnterWater(col);
	}
	
	void OnTriggerEnter(Collider col){
		FPSRigidBodyWalker FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		Footsteps FootstepsComponent = playerObj.GetComponent<Footsteps>();
		string colTag = col.gameObject.tag;
		Transform colTransform = col.gameObject.transform;

		//play splash effects for player and objects thrown into water
		if((colTag == "Player" && (FPSWalkerComponent.jumping || !FPSWalkerComponent.grounded))//play splash effects for player if they jumped into water
		|| ((colTag == "Usable"//play splash effects for objects if they hit water
		|| colTag == "Metal" 
		|| colTag == "Wood" 
		|| colTag == "Glass" 
		|| col.gameObject.name == "Chest") 
		&& colTransform.position.y > myTransform.GetComponent<Collider>().bounds.max.y - 0.3f)){
		
			if(colTag == "Player"){EnterWater(col);}
			
			AudioSource.PlayClipAtPoint(FootstepsComponent.waterLand, colTransform.position);//play splash sound
			
			if(waterSplash){
			    foreach (Transform child in waterSplash.transform){//emit all particles in the particle effect game object group stored in impactObj var
					
					if(child.name == "FastSplash"){
						splashPos = new Vector3(colTransform.position.x, myTransform.GetComponent<Collider>().bounds.max.y - 0.15f, colTransform.position.z);
						child.GetComponent<ParticleSystem>().transform.position = splashPos;
					}else{
						splashPos = new Vector3(colTransform.position.x, myTransform.GetComponent<Collider>().bounds.max.y + 0.01f, colTransform.position.z);
						child.GetComponent<ParticleSystem>().transform.position = splashPos;	
					}
					child.GetComponent<ParticleSystem>().transform.rotation = Quaternion.FromToRotation(Vector3.up, waterSplash.transform.up);//rotate impact effects so they are perpendicular to surface hit
					child.GetComponent<ParticleSystem>().Play();//emit the particle(s)
				}
			}
		}
	}
	
	void EnterWater(Collider col){
		//set up external script references
		FPSRigidBodyWalker FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		PlayerWeapons PlayerWeaponsComponent = weaponObj.GetComponent<PlayerWeapons>();
		
		if(col.gameObject.tag == "Player"){
			
			FPSWalkerComponent.inWater = true;
			//check if player is at wading depth in water (water line at chest) after wading into water
			if(col.gameObject.GetComponent<Collider>().bounds.max.y - 0.6f <= myTransform.GetComponent<Collider>().bounds.max.y){
				
				FPSWalkerComponent.swimming = true;
				
				if(!swimTimeState){
					FPSWalkerComponent.swimStartTime = Time.time;//track time that swimming started
					swimTimeState = true;
				}
				//check if player is at treading water depth (water line at shoulders/neck) after surfacing from dive
				if(col.gameObject.GetComponent<Collider>().bounds.max.y - 0.5f <= myTransform.GetComponent<Collider>().bounds.max.y){
					FPSWalkerComponent.belowWater = true;
				}else{
					FPSWalkerComponent.belowWater = false;
				}
				
			}else{
				FPSWalkerComponent.swimming = false;
			}
			//check if view height is under water line
			if(mainCamTransform.position.y + 0.02f <= myTransform.GetComponent<Collider>().bounds.max.y){
				
				if(!holdBreathState){
					
					FPSWalkerComponent.holdingBreath = true;
					FPSWalkerComponent.diveStartTime = Time.time;
					//play underwater sound effect and pause above-water ambient audio sources
					audioSource.Play();
					foreach (AudioSource aSource in aboveWaterAudioSources){
						aSource.Pause();	
					}

					//emit ambient underwater particles/bubbles
					particlesEffect.Play();
					
					//set color of underwater muzzle flash to underwaterFogColor
					PlayerWeaponsComponent.waterMuzzleFlashColor.r = underwaterFogColor.r;
					PlayerWeaponsComponent.waterMuzzleFlashColor.g = underwaterFogColor.g;
					PlayerWeaponsComponent.waterMuzzleFlashColor.b = underwaterFogColor.b;
					//settings fot underwater fog
					RenderSettings.fog = underwaterFogEnabled;
					RenderSettings.fogColor = underwaterFogColor;
					RenderSettings.fogDensity = underwaterFogDensity;
					RenderSettings.fogMode = underwaterFogMode;
					RenderSettings.fogStartDistance = underwaterLinearFogStart;
					RenderSettings.fogEndDistance = underwaterLinearFogEnd;
					RenderSettings.ambientLight = underwaterLightColor;
					//change original sun/directional light color to underwaterSunightColor
					if(SunlightObj){SunlightObj.color = underwaterSunightColor;}
					
					if(waterPlane && flipWaterPlane){//flip the water plane so we can see the surface from underwater
						waterPlaneRot.z = 180.0f;
						waterPlane.localEulerAngles = waterPlaneRot;
					}
					//perform above actions only once at start of dive
					holdBreathState = true;
				}
				
				//Make sure that water particles don't rise past the surface of the water by subtracting the 
				//particle emitter ellipsoid y amount from the top of the water zone collilder/trigger.
				//Otherwise, just emit at the same height/water depth as the camera
				if(myTransform.GetComponent<Collider>().bounds.max.y - 2.04f > mainCamTransform.position.y){
					particlesYPos = mainCamTransform.position.y;	
					particleForwardPos = 3.25f;
				}else{
					particlesYPos = myTransform.GetComponent<Collider>().bounds.max.y - 2.04f;	
					particleForwardPos = 0.0f;
				}
				//make underwater particles/bubbles follow player position
				Vector3 tempParticlePos = new Vector3(mainCamTransform.position.x, particlesYPos, mainCamTransform.position.z) + (mainCamTransform.forward * particleForwardPos);
				particlesEffect.transform.position = tempParticlePos;
				
			}else{
				
				if(holdBreathState){
					
					FPSWalkerComponent.holdingBreath = false;
					//pause underwater sound effect and resume playing above-water ambient audio sources
					audioSource.Pause();
					foreach (AudioSource aSource in aboveWaterAudioSources){
						aSource.Play();	
					}
					//stop emitting underwater particles/bubbles
					particlesEffect.Stop();
					//apply original fog settings when above water
					RenderSettings.fog = fogEnabled;
					RenderSettings.fogColor = origFogColor;
					RenderSettings.fogDensity = origFogDensity;
					RenderSettings.fogMode = origFogMode;
					RenderSettings.fogStartDistance = origLinearFogStart;
					RenderSettings.fogEndDistance = origLinearFogEnd;
					RenderSettings.ambientLight = origLightColor;
					//change original sun/directional light color to origSunightColor
					if(SunlightObj){SunlightObj.color = origSunightColor;}
					
					if(waterPlane && flipWaterPlane){
						waterPlaneRot.z = 0.0f;
						waterPlane.localEulerAngles = waterPlaneRot;
					}
					//perform above actions only once when surfacing
					holdBreathState = false;
				}
				//check if treading water ripples or player wake/ripple trail should be emitted
				if(FPSWalkerComponent.inputY == 0 && FPSWalkerComponent.inputX == 0){//player is treading water
					//play idle treading water ripples around player
					rippleEffect.Play();
					//emit the particles slightly above the water surface so they are not hidden by the visual water effect plane
					Vector3 tempRipplePos = new Vector3(playerObj.transform.position.x, myTransform.GetComponent<Collider>().bounds.max.y + 0.0005f, playerObj.transform.position.z);
					rippleEffect.transform.position = tempRipplePos;
					
				}else{//playing is swimming on surface
					//emit player wake particle group at a set interval
					if(splashTrailTime + 0.075f < Time.time){
						if(splashTrail){
						    foreach (Transform child in splashTrail.transform){//emit all particles in the particle effect game object group stored in impactObj var
								//emit the particles slightly above the water surface so they are not hidden by the visual water effect plane
								trailPos = new Vector3(col.gameObject.transform.position.x, myTransform.GetComponent<Collider>().bounds.max.y + 0.0005f, col.gameObject.transform.position.z);
								child.GetComponent<ParticleSystem>().transform.position = trailPos;	
								//rotate impact effects so they are perpendicular to surface hit
								child.GetComponent<ParticleSystem>().transform.rotation = Quaternion.FromToRotation(Vector3.up, splashTrail.transform.up);
								child.GetComponent<ParticleSystem>().Play();//emit the particle(s)
							}
						}
						splashTrailTime = Time.time;//store last emitted time to set particle emission interval
					}	
				}		
			}
		}	
	}
	
	void OnTriggerExit(Collider col){
		//set up external script references
		FPSRigidBodyWalker FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		//player has exited water, so reset swimming related variables
		if(col.gameObject.tag == "Player"){
			swimTimeState = false;
			FPSWalkerComponent.inWater = false;
			FPSWalkerComponent.swimming = false;
			FPSWalkerComponent.belowWater = false;
			FPSWalkerComponent.canWaterJump = true;
			FPSWalkerComponent.holdingBreath = false;
		}
	}
}
