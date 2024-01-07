//BreakableObject.cs by Azuline Studios© All Rights Reserved
//Attach to object with a particle emitter and box collider to create breakable objects.
using UnityEngine;
using System.Collections;
//this script used to create breakable glass objects
public class BreakableObject : MonoBehaviour {
	public float hitPoints = 150;
	private ParticleSystem breakParticles;
	private bool broken;
	private Transform myTransform;
	
	void Start () {
		myTransform = transform;
		breakParticles = myTransform.GetComponent<ParticleSystem>();
	}
	
	void Update () {
		if(broken){//remove breakable object if it is broken and particles have faded
			if(breakParticles.particleCount == 0.0f){
				Destroy(myTransform.gameObject);
			}
		}
	}
	
	public void ApplyDamage (float damage){
		hitPoints -= damage;
		if(hitPoints <= 0 && !broken){
			breakParticles.Play();//emit broken object particles
			GetComponent<AudioSource>().pitch = Random.Range(0.95f * Time.timeScale, 1.0f * Time.timeScale);//add slight random value to breaking sound pitch for variety
			GetComponent<AudioSource>().Play();//play break sound
			//disable mesh and collider of glass object untill object is deleted after sound effect finishes playing
			myTransform.GetComponent<MeshRenderer>().enabled = false;
			myTransform.GetComponent<BoxCollider>().enabled = false;//can use other collider types if needed
			broken = true;
		}
	}
}
