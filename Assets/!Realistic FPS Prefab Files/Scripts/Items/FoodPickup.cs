//FoodPickup.cs by Azuline Studios© All Rights Reserved
//script for food pickups
using UnityEngine;
using System.Collections;

public class FoodPickup : MonoBehaviour {
	
	private GameObject playerObj;//the GameObject that is a child of FPS Weapons which has the WeaponBehavior script attatched
	private Transform myTransform;
	
	public bool removeOnUse = true;//Does this pickup disappear when used/activated by player?
	
	public AudioClip pickupSound;//sound to play when picking up food
	public AudioClip fullSound;//sound to play when player is full
	
	public int hungerToRemove = 15;//amount of hunger to remove when picking up this food item
	
	private FPSPlayer FPSPlayerComponent;

	void Start () {
		myTransform = transform;//manually set transform for efficiency
		playerObj = Camera.main.transform.GetComponent<CameraKick>().playerObj;
		FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
	}
	
	public void PickUpItem(){
		//if player is hungry, apply hungerToRemove to hungerPoints
		if (FPSPlayerComponent.hungerPoints > 0.0f && FPSPlayerComponent.usePlayerHunger) {
			
			if(FPSPlayerComponent.hungerPoints - hungerToRemove > 0.0){
				FPSPlayerComponent.UpdateHunger(-hungerToRemove);
			}else{
				FPSPlayerComponent.UpdateHunger(-FPSPlayerComponent.hungerPoints);	
			}
			
			//play pickup sound
			if(pickupSound){AudioSource.PlayClipAtPoint(pickupSound, myTransform.position, 0.75f);}
			
			if(removeOnUse){
				//remove this food pickup
				Object.Destroy(gameObject);
			}
		}else{
			//if player is not hungry, just play beep sound
			if(fullSound){AudioSource.PlayClipAtPoint(fullSound, myTransform.position, 0.75f);}	
		}
	}
}