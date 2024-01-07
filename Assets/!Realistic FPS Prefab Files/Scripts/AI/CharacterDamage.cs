//CharacterDamage.cs by Azuline Studios© All Rights Reserved
//Applies damage to NPCs 
using UnityEngine;
using System.Collections;

public class CharacterDamage : MonoBehaviour {
	public float hitPoints = 100.0f;
	public Transform deadReplacement;
	public AudioClip dieSound;
	public bool  removeBody;
	public float bodyStayTime = 15.0f;
	private Vector3 attackerPos2;
	private Vector3 attackDir2;
	private Transform myTransform;
	//set ray mask to layer 9, the ragdoll layer
	//for applying force to specific ragdoll limbs
	private LayerMask raymask = 1 << 9;
	
	void Start (){
		myTransform = transform;
	}
	//damage NPC
	public void ApplyDamage ( float damage, Vector3 attackDir, Vector3 attackerPos ){

		if (hitPoints <= 0.0f){
			return;
		}
		
		hitPoints -= damage;
		
		attackDir2 = attackDir;
		attackerPos2 = attackerPos;
		
		//expand enemy search radius if attacked outside default search radius to defend against sniping
		if(transform.GetComponent<AI>()){
			transform.GetComponent<AI>().attackRangeAmt = transform.GetComponent<AI>().attackRange * 3;
		}
		
		if (hitPoints <= 0.0f){
			Die();
		}
	}
	
	void Die (){
		
		RaycastHit rayHit;
		// Play a dying audio clip
		if (dieSound){
			AudioSource.PlayClipAtPoint(dieSound, transform.position);
		}
	
		// Replace ourselves with the dead body
		if (deadReplacement) {
			Transform dead = Instantiate(deadReplacement, transform.position, transform.rotation) as Transform;
	
			// Copy position & rotation from the old hierarchy into the dead replacement
			CopyTransformsRecurse(transform, dead);
			
			//apply damage force to NPC ragdoll if being damaged by player
			if(Physics.SphereCast(attackerPos2, 0.2f, attackDir2, out rayHit, 750.0f, raymask)
			&& rayHit.rigidbody 
			&& attackDir2.x !=0){
				//apply damage force to the ragdoll rigidbody hit by the sphere cast (can be any body part)
				rayHit.rigidbody.AddForce(attackDir2 * 50.0f, ForceMode.Impulse);
			
			}else{//apply damage force to NPC ragdoll if being damaged by an explosive object or other damage source without a specified attack direction
			
				Component[] bodies;
				bodies = dead.GetComponentsInChildren<Rigidbody>();
				foreach(Rigidbody body in bodies) {
					if(body.transform.name == "Chest"){//only apply damage force to the chest of the ragdoll if damage is from non-player source 
						//calculate direction to apply damage force to ragdoll
						body.AddForce((myTransform.position - attackerPos2).normalized * 30.0f, ForceMode.Impulse);
					}
				}
				
			}
			
			//initialize the RemoveBody.cs script attached to the NPC ragdoll
			if(dead.GetComponent<RemoveBody>()){
				if(removeBody){
					dead.GetComponent<RemoveBody>().enabled = true;
					dead.GetComponent<RemoveBody>().bodyStayTime = bodyStayTime;//pass bodyStayTime to RemoveBody.js script
				}else{
					dead.GetComponent<RemoveBody>().enabled = false;
				}
			}
			
			Destroy(transform.gameObject);
			
		}
	
	}
	
	static void CopyTransformsRecurse ( Transform src , Transform dst ){
		dst.position = src.position;
		dst.rotation = src.rotation;
		
		foreach(Transform child in dst) {
			// Match the transform with the same name
			Transform curSrc = src.Find(child.name);
			if (curSrc)
				CopyTransformsRecurse(curSrc, child);
		}
	}
}