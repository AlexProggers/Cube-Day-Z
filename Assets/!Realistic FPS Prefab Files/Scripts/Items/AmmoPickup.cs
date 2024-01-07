//AmmoPickup.cs by Azuline Studios© All Rights Reserved
//Script for ammo pickups.
using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour {
	
	private GameObject weaponObj;//the GameObject that is a child of FPS Weapons which has the WeaponBehavior script attatched
	private Transform myTransform;
	
	public int weaponNumber = 0;//this number corresponds with the weapon's index in the PlayerWeapons script weaponOrder array
	public bool removeOnUse = true;//Does this pickup disappear when used/activated by player?
	
	public AudioClip pickupSound;//sound to play when picking up weapon
	public AudioClip fullSound;//sound to play when ammo is full
	
	public int ammoToAdd = 1;//amount of ammo to add when picking up this ammo item
	public Texture2D ammoPickupReticle;//the texture used for the pick up crosshair
	
	void Start () {
		myTransform = transform;//manually set transform for efficiency
		weaponObj = Camera.main.transform.GetComponent<CameraKick>().weaponObj;
		//find the PlayerWeapons script in the FPS Prefab to access weaponOrder array
		PlayerWeapons PlayerWeaponsComponent = weaponObj.GetComponentInChildren<PlayerWeapons>();
		
		//scan the children of the FPS Weapons object (PlayerWeapon's weaponOrder array) and assign this item's weaponObj to the
		//weapon object whose weaponNumber in its WeaponBehavior script matches this item's weapon number
		for (int i = 0; i < PlayerWeaponsComponent.weaponOrder.Length; i++)	{
			if(PlayerWeaponsComponent.weaponOrder[i].GetComponent<WeaponBehavior>().weaponNumber == weaponNumber){
				weaponObj = PlayerWeaponsComponent.weaponOrder[i];
				break;
			}
		}
	}
	
	public void PickUpItem(){
		//if player has less than max ammo for this weapon, give player ammoToAdd amount
		if (weaponObj.GetComponent<WeaponBehavior>().ammo < weaponObj.GetComponent<WeaponBehavior>().maxAmmo) {
			
			if(weaponObj.GetComponent<WeaponBehavior>().ammo + ammoToAdd > weaponObj.GetComponent<WeaponBehavior>().maxAmmo){
				//just give player max ammo if they only are a few bullets away from having max ammo
				weaponObj.GetComponent<WeaponBehavior>().ammo = weaponObj.GetComponent<WeaponBehavior>().maxAmmo;	
			}else{
				//give player the ammoToAdd amount
				weaponObj.GetComponent<WeaponBehavior>().ammo += ammoToAdd;	
			}
			
			//play pickup sound
			if(pickupSound){AudioSource.PlayClipAtPoint(pickupSound, myTransform.position, 0.75f);}
			
			if(removeOnUse){
				//remove this weapon pickup
				Object.Destroy(gameObject);
			}
		}else{
			//if player is at max ammo, just play beep sound
			if(fullSound){AudioSource.PlayClipAtPoint(fullSound, myTransform.position, 0.75f);}	
		}
	}
}