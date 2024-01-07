//HealthPickup.cs by Azuline Studios© All Rights Reserved
//script for health pickup items
using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour {
	private GameObject playerObj;
	private Transform myTransform;
	
	public float healthToAdd = 25.0f;
	public bool removeOnUse = true;//Does this pickup disappear when used/activated by player?
	
	public AudioClip pickupSound;//sound to playe when picking up this item
	public AudioClip fullSound;//sound to play when health is full
	
	public Texture2D healthPickupReticle;//the texture used for the pick up crosshair
	
	void Start () {
		myTransform = transform;//manually set transform for efficiency
		//assign this item's playerObj value
		playerObj = Camera.main.transform.GetComponent<CameraKick>().playerObj;
	}
	
	void PickUpItem (){
	FPSPlayer FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
	
		if (FPSPlayerComponent.hitPoints < FPSPlayerComponent.maximumHitPoints){
			//heal player
			FPSPlayerComponent.HealPlayer(healthToAdd);
			
			if(pickupSound){AudioSource.PlayClipAtPoint(pickupSound, myTransform.position, 0.75f);}
			
			if(removeOnUse){
				//remove this pickup
				Object.Destroy(gameObject);
			}
			
		}else{
			//player is already at max health, just play beep sound effect
			if(fullSound){AudioSource.PlayClipAtPoint(fullSound, myTransform.position, 0.75f);}		
		}
	}
}