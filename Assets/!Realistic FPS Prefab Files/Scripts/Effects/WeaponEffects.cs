//WeaponEffects.cs by Azuline Studios© All Rights Reserved
//Emits particles and plays sound effects for impacts by surface type,
//instantiates weapon mark objects, and emits tracer effects. 
using UnityEngine;
using System.Collections;

public class WeaponEffects : MonoBehaviour {
	[HideInInspector]
	public GameObject weaponObj;
	
	//Particle Emitters
	//used for weapon fire and bullet impact effects

	public GameObject dirtImpact;
	public GameObject metalImpact;
	public GameObject woodImpact;
	public GameObject waterImpact;
	public GameObject glassImpact;
	public GameObject fleshImpact;
	public GameObject stoneImpact;
	
	public GameObject dirtImpactMelee;
	public GameObject metalImpactMelee;
	public GameObject woodImpactMelee;
	public GameObject stoneImpactMelee;
	
	private GameObject impactObj;
	
	public ParticleSystem tracerParticles;
	public ParticleSystem bubbleParticles;
	
	//impact marks to be placed on objects where projectiles hit
	
	public GameObject[] dirtMarks;
	public GameObject[] metalMarks;
	public GameObject[] woodMarks;
	public GameObject[] glassMarks;
	public GameObject[] stoneMarks;
	
	public GameObject[] dirtMarksMelee;
	public GameObject[] metalMarksMelee;
	public GameObject[] woodMarksMelee;
	
	private GameObject markObj;
	
	//impact sound effects for different materials
	
	public AudioClip[] defaultImpactSounds;
	public AudioClip[] metalImpactSounds;
	public AudioClip[] woodImpactSounds;
	public AudioClip[] waterImpactSounds;
	public AudioClip[] glassImpactSounds;
	public AudioClip[] fleshImpactSounds;
	public AudioClip[] stoneImpactSounds;
	
	public AudioClip[] defaultImpactSoundsMelee;
	public AudioClip[] metalImpactSoundsMelee;
	public AudioClip[] woodImpactSoundsMelee;
	public AudioClip[] fleshImpactSoundsMelee;
	public AudioClip[] stoneImpactSoundsMelee;
	
	private AudioClip hitSound;
	private float hitVolumeAmt = 1.0f;
	
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//Draw Impact Effects
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void ImpactEffects ( RaycastHit hit  ){
		//set up external script references
		PlayerWeapons PlayerWeaponsComponent = weaponObj.GetComponentInChildren<PlayerWeapons>();
		WeaponBehavior WeaponBehaviorComponent = PlayerWeaponsComponent.weaponOrder[PlayerWeaponsComponent.currentWeapon].GetComponent<WeaponBehavior>();
		
		if(WeaponBehaviorComponent.meleeSwingDelay == 0){//not a melee weapon
			//find the tag name of the hit game object and emit the particle effects for the surface type
			switch(hit.collider.gameObject.tag){
			case "Dirt":
				impactObj = dirtImpact;//set impactObj to the particle effect game object group for this surface type
				if(defaultImpactSounds[0]){
					hitSound = defaultImpactSounds[Random.Range(0, defaultImpactSounds.Length)];//select random impact sound for this surface type
				}
				break;
			case "Metal":
				impactObj = metalImpact;
				hitSound = metalImpactSounds[Random.Range(0, metalImpactSounds.Length)];
				break;
			case "Wood":
				impactObj = woodImpact;
				if(woodImpactSounds[0]){
					hitSound = woodImpactSounds[Random.Range(0, woodImpactSounds.Length)];
				}
				break;
			case "Water":
				impactObj = waterImpact;
				if(waterImpactSounds[0]){
					hitSound = waterImpactSounds[Random.Range(0, waterImpactSounds.Length)];
				}
				break;
			case "Glass":
				impactObj = glassImpact;
				if(waterImpactSounds[0]){
					hitSound = glassImpactSounds[Random.Range(0, glassImpactSounds.Length)];
				}
				break;
			case "Flesh":
				impactObj = fleshImpact;
				if(fleshImpactSounds[0]){
					hitSound = fleshImpactSounds[Random.Range(0, fleshImpactSounds.Length)];
				}
				break;
			case "Stone":
				impactObj = stoneImpact;
				if(stoneImpactSounds[0]){
					hitSound = stoneImpactSounds[Random.Range(0, stoneImpactSounds.Length)];
				}
				break;
			default:
				impactObj = dirtImpact;
				if(defaultImpactSounds[0]){
					hitSound = defaultImpactSounds[Random.Range(0, defaultImpactSounds.Length)];
				}
				break;
			}
		}else{//use different impact effects for melee weapons
			switch(hit.collider.gameObject.tag){
			case "Dirt":
				impactObj = dirtImpactMelee;
				hitSound = defaultImpactSoundsMelee[Random.Range(0, defaultImpactSoundsMelee.Length)];
				break;
			case "Metal":
				impactObj = metalImpactMelee;
				hitSound = metalImpactSoundsMelee[Random.Range(0, metalImpactSoundsMelee.Length)];
				break;
			case "Wood":
				impactObj = woodImpactMelee;
				hitSound = woodImpactSoundsMelee[Random.Range(0, woodImpactSoundsMelee.Length)];
				break;
			case "Water":
				impactObj = waterImpact;
				hitSound = waterImpactSounds[Random.Range(0, waterImpactSounds.Length)];
				break;
			case "Glass":
				impactObj = glassImpact;
				hitSound = glassImpactSounds[Random.Range(0, glassImpactSounds.Length)];
				break;
			case "Flesh":
				impactObj = fleshImpact;
				hitSound = fleshImpactSoundsMelee[Random.Range(0, fleshImpactSoundsMelee.Length)];
				break;
			case "Stone":
				impactObj = stoneImpactMelee;
				hitSound = stoneImpactSoundsMelee[Random.Range(0, stoneImpactSoundsMelee.Length)];
				break;
			default:
				impactObj = dirtImpactMelee;
				hitSound = defaultImpactSoundsMelee[Random.Range(0, defaultImpactSoundsMelee.Length)];
				break;
			}
		}
		
	    foreach (Transform child in impactObj.transform){//emit all particles in the particle effect game object group stored in impactObj var
			Transform childParticleTransform = child.GetComponent<ParticleSystem>().transform;//cache child particle transform ref for efficiency
			GameObject hitObject = hit.transform.gameObject;
			//align emitter position with contact position and move up from surface slightly if emitter is a HitSpark
			if(child.name == "HitSpark" || child.name == "HitSpark2"){
				childParticleTransform.position = hit.point + (hit.normal * 0.075f);
			}else if(child.name == "FastSplash"){//emit splash particles at lower y pos for better effect
				childParticleTransform.position = hit.point - (hit.normal * 0.15f);
			}else if(child.name == "BloodSplatter"){//emit blood splatters closer to camera to keep the effect in front of hit NPC's walking toward player
				childParticleTransform.position = hit.point - (Camera.main.transform.forward * 0.4f);
			}else{
				childParticleTransform.position = hit.point;	
			}
			if((hitObject.layer != 9 && hitObject.layer != 13)//emit smoke effects for all other objects
			//dont emit smoke effects for NPCs and ragdolls because they can get close to the screen and cause slowdown
			|| ((hitObject.layer == 9 || hitObject.layer == 13) && child.name != "SlowSmoke" && child.name != "FastSmoke" && child.name != "Debris")){ 
				childParticleTransform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);//rotate impact effects so they are perpendicular to surface hit
				child.GetComponent<ParticleSystem>().Play();//emit the particle(s)
			}
		}
		
		//modify the weapon impact sounds based on the weapon type, so the multiple shotgun impacts and automatic weapons aren't so loud
		if(WeaponBehaviorComponent.projectileCount > 1){
			hitVolumeAmt = 0.35f;	
		}else if(!WeaponBehaviorComponent.semiAuto){
			hitVolumeAmt = 0.8f;	
		}else{
			hitVolumeAmt = 1.0f;
		}
		
		//play sounds of bullets hitting surface/ricocheting
		AudioSource.PlayClipAtPoint(hitSound, hit.point, hitVolumeAmt);
		
	}
	
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//Draw Bullet Marks
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void BulletMarks ( RaycastHit hit  ){
		PlayerWeapons PlayerWeaponsComponent = weaponObj.GetComponentInChildren<PlayerWeapons>();
		WeaponBehavior WeaponBehaviorComponent = PlayerWeaponsComponent.weaponOrder[PlayerWeaponsComponent.currentWeapon].GetComponent<WeaponBehavior>();
		
		if(WeaponBehaviorComponent.meleeSwingDelay == 0){//not a melee weapon
			//find the tag name of the hit game object and select an impact mark for the surface type
			switch(hit.collider.gameObject.tag){
			case "Dirt":
				markObj = dirtMarks[Random.Range(0, dirtMarks.Length)];
				break;
			case "Metal":
				markObj = metalMarks[Random.Range(0, metalMarks.Length)];
				break;
			case "Wood":
				markObj = woodMarks[Random.Range(0, woodMarks.Length)];
				break;
			case "Glass":
				markObj = glassMarks[Random.Range(0, glassMarks.Length)];
				break;
			case "Stone":
				markObj = stoneMarks[Random.Range(0, stoneMarks.Length)];
				break;
			default:
				markObj = dirtMarks[Random.Range(0, dirtMarks.Length)];
				break;
			}
		}else{
			switch(hit.collider.gameObject.tag){//select a melee weapon impact mark
			case "Dirt":
				markObj = dirtMarksMelee[Random.Range(0, dirtMarksMelee.Length)];
				break;
			case "Metal":
				markObj = metalMarksMelee[Random.Range(0, metalMarksMelee.Length)];
				break;
			case "Wood":
				markObj = woodMarksMelee[Random.Range(0, woodMarksMelee.Length)];
				break;
			case "Glass":
				markObj = glassMarks[Random.Range(0, glassMarks.Length)];
				break;
			default:
				markObj = dirtMarksMelee[Random.Range(0, dirtMarksMelee.Length)];
				break;
			}
		}
		
		if(hit.collider//check only objects with colliders attatched to prevent null reference error
		&& hit.collider.gameObject.layer != 9//don't leave marks on ragdolls
		&& hit.collider.gameObject.tag != "NoHitMark"//don't leave marks on active NPCs or objects with NoHitMark or PickUp tag
		&& hit.collider.gameObject.tag != "PickUp"
		&& hit.collider.gameObject.tag != "Flesh"
		&& hit.collider.gameObject.tag != "Water"){
			//create an instance of the bullet mark and place it parallel and slightly above the hit surface to prevent z buffer fighting
			GameObject clone = Instantiate(markObj, hit.point + (hit.normal * 0.025f), Quaternion.FromToRotation(Vector3.up, hit.normal)) as GameObject; 	
			//create empty game object for parent of bullet mark to prevent bullet mark object from inheriting hit object's scale
			//we do this to create another layer between the bullet mark object and the hit object which may have been unevenly scaled in editor
			if(hit.collider.transform.GetComponent<BreakableObject>()){
				clone.transform.GetComponent<FadeOutDecals>().hitMesh = hit.collider.transform.GetComponent<MeshRenderer>();
			}
			var emptyObject = new GameObject();
			//define transforms for efficiency
			Transform tempObjTransform = emptyObject.transform;
			Transform cloneTransform = clone.transform;
			//save initial scaling of bullet mark prefab object
			Vector3 scale = cloneTransform.localScale;		
			//set parent of empty game object to hit object's transform
			tempObjTransform.parent = hit.transform;
			//set scale of empty game object to (1,1,1) to prepare it for applying the inverse scale of the object that was hit
			tempObjTransform.localScale = Vector3.one;
			//sync empty game object's rotation quaternion with hit object's quaternion for correct scaling of euler angles (use the same orientation of axes)
		    Quaternion tempQuat = hit.transform.rotation;
			tempObjTransform.rotation = tempQuat;
			//calculate inverse of hit object's scale to compensate for objects that have been unevenly scaled in editor
			Vector3 tempScale1 = new Vector3(1.0f / tempObjTransform.parent.transform.localScale.x, 
									  	     1.0f / tempObjTransform.parent.transform.localScale.y, 
				  						     1.0f / tempObjTransform.parent.transform.localScale.z);
			//apply inverse scale of the collider that was hit to empty game object's transform
			tempObjTransform.localScale = tempScale1;
			//set parent of bullet mark object to empy game object and set localScale to (1,1,1)
			cloneTransform.parent = null;
			cloneTransform.parent = tempObjTransform;
			//apply hit mark's initial scale to hit mark instance
			cloneTransform.localScale = scale;
			//randomly scale bullet marks slightly for more natural visual effect
			if(WeaponBehaviorComponent.meleeSwingDelay == 0){//not a melee weapon
				float tempScale = Random.Range (-0.25f, 0.25f);//find random scale amount
				cloneTransform.localScale = scale + new Vector3(tempScale, 0, tempScale);//apply random scale to bullet mark object's localScale
			}
			//rotate hit mark randomly for more variation
			cloneTransform.RotateAround(hit.point, hit.normal, Random.Range (-50, 50));
			//destroy bullet mark instance after a time
			Destroy(emptyObject.gameObject, 30); 
		}
		
	}
	
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//Draw Bullet Tracers
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public void BulletTracers ( Vector3 direction, Transform MuzzleFlashTransform, bool submerged ){
		//Draw Bullet Tracers
		if(!submerged){
			if (tracerParticles) {
				//Set tracer origin to a small amount forward of the end of gun barrel (muzzle flash position)
				tracerParticles.transform.position = MuzzleFlashTransform.position + MuzzleFlashTransform.forward * 0.5f;
				//add shotSpray/accuracy value to straight-forward rotation to make tracers follow raycast to hit position
				tracerParticles.transform.rotation = Quaternion.FromToRotation(Vector3.forward, direction);
				//emit tracer particle for every shot fired
				tracerParticles.Play();
			}
		}else{
			if (bubbleParticles) {
				bubbleParticles.transform.position = MuzzleFlashTransform.position - MuzzleFlashTransform.forward * 0.2f;
				bubbleParticles.transform.rotation = Quaternion.FromToRotation(Vector3.forward, direction);
				bubbleParticles.Play();
			}	
		}
		
	}
	
	
}
