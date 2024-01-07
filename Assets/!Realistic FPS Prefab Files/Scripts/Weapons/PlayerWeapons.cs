//PlayerWeapons.cs by Azuline Studios© All Rights Reserved
//Switches and drops weapons and sets weapon parent object position. 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerWeapons : MonoBehaviour {
	public int firstWeapon = 0;//the weaponOrder index of the first weapon that will be selected when the map loads
	//backupWeapon is the weaponOrder index number of a weapon like fists, a knife, or sidearm that player will select when all other weapons are dropped
	//this weapon should also have its "droppable" and "addsToTotalWeaps" values in its WeaponBehavior.cs component set to false
	public int backupWeapon = 1;
	public int maxWeapons = 10;
	[HideInInspector]
	public int totalWeapons;
	public int currentWeapon;//index of weaponOrder array that corresponds to current weapon 
	//Define array for storing order of weapons. This array is created in the inspector by dragging and dropping 
	//weapons from under the FPSWeapons branch in the FPS Prefab. Weapon 0 should always be the unarmed/null weapon.
	public GameObject[] weaponOrder;
	
	//objects accessed by this script
	[HideInInspector]
	public GameObject playerObj;
	[HideInInspector]
	public GameObject cameraObj;
	[HideInInspector]
	public GameObject weaponObj;
	[HideInInspector]
	public GameObject ammoGuiObj;//this GUI object will be instantiated on level load to display player ammo
	[HideInInspector]
	public GameObject ammoGuiObjInstance;
	private Transform myTransform;
	private Transform mainCamTransform;
	[HideInInspector]
	public Color waterMuzzleFlashColor;
	
	//weapon switching
	[HideInInspector]
	public float switchTime = 0.0f;//time that weapon switch started
	[HideInInspector]
	public float sprintSwitchTime = 0.0f;//time that weapon sprinting animation started, set in WeaponBehavior script
	[HideInInspector]
	public bool switching = false;//true when switching weapons
	[HideInInspector]
	public bool sprintSwitching = false;//true when weapon sprinting animation is playing

	private bool dropWeapon;
	private bool deadDropped;
	//sound effects
	public AudioClip changesnd;
	private bool audioPaused;// used to pause and resume reloading sound based on timescale/game pausing state

	private FPSRigidBodyWalker _fpsWalkerComponent;

	private FPSPlayer _fpsPlayerComponent;

	private AudioSource[] _aSources;

	private List<WeaponBehavior> _weaponsBehavioursList = new List<WeaponBehavior>();

	private Ironsights _ironsightsComponent;

	private CameraKick _cameraKickComponent;

	private bool _isInitialized;
	
	public bool CanSelectWeaponByKeys;

	public bool CanSelectWeaponByScroll;

	public List<WeaponBehavior> WeaponsBehavioursList
	{
		get
		{
			return _weaponsBehavioursList;
		}
	}

	public bool CanShoot { get; set; }

	private System.Action _switchWeaponCallback;

	public void Initialize(bool selectDefaultWeapon = true)
	{
		InitializeComponents();
		myTransform = base.transform;
		mainCamTransform = Camera.main.transform;
		ammoGuiObjInstance = UnityEngine.Object.Instantiate(ammoGuiObj, Vector3.zero, myTransform.rotation) as GameObject;

		for (int i = 0; i < weaponOrder.Length; i++)
		{
			WeaponBehavior component = weaponOrder[i].GetComponent<WeaponBehavior>();
			component.weaponNumber = i;
			_weaponsBehavioursList.Add(component);
		}

		if (selectDefaultWeapon)
		{
			GameControls.I.Player.CurrentWeaponBehavior = GameControls.I.PlayerWeapons.WeaponsBehavioursList[2];

			if (_weaponsBehavioursList[firstWeapon].haveWeapon)
			{
				StartCoroutine(SelectWeapon(firstWeapon));
			}
			else
			{
				StartCoroutine(SelectWeapon(0));
			}
		}

		_weaponsBehavioursList[backupWeapon].droppable = false;
		_weaponsBehavioursList[backupWeapon].addsToTotalWeaps = false;

		UpdateTotalWeapons();
		CanShoot = true;
		
		_isInitialized = true;
	}

	private void InitializeComponents()
	{
		_fpsWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		_fpsPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		_aSources = base.transform.GetComponents<AudioSource>();
		_ironsightsComponent = playerObj.GetComponent<Ironsights>();
		_cameraKickComponent = Camera.main.GetComponent<CameraKick>();
	}
	
	void Update (){

		if (!_isInitialized)
		{
			return;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//Switch Weapons
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			
		if(Time.timeSinceLevelLoad > 2.0f//don't allow weapon switching when level is still loading/fading out
		&& !(!_fpsWalkerComponent.grounded && _fpsWalkerComponent.sprintActive)//don't allow switching if player is sprinting and airborn
		&& !switching//only allow one weapon switch at once
		&& !weaponOrder[currentWeapon].GetComponent<WeaponBehavior>().shooting//don't switch weapons if shooting
		&& !_fpsWalkerComponent.holdingObject//don't allow switching if player is holding an object
		&& !sprintSwitching){//don't allow weapon switching while sprint anim is active/transitioning
			
			//drop weapons
			if(Input.GetKeyDown(_fpsPlayerComponent.dropWeapon) 
			&& currentWeapon != 0
			&& !_fpsWalkerComponent.sprintActive
			&& weaponOrder[currentWeapon].GetComponent<WeaponBehavior>().droppable
			&& !weaponOrder[currentWeapon].GetComponent<WeaponBehavior>().dropWillDupe){//if drop button is pressed and weapon isn't holstered (null weap 0 selected)
				DropWeapon(currentWeapon);		
			}
			
			//drop current weapon if player dies
			if(_fpsPlayerComponent.hitPoints < 1.0f && !deadDropped){
				weaponOrder[currentWeapon].GetComponent<WeaponBehavior>().droppable = true;
				deadDropped = true;
				DropWeapon(currentWeapon);	
			}
		  	
		  	//Cycle weapons using the mousewheel (cycle through FPS Weapon children) and skip weapons that are not in player inventory.
			//weaponOrder.Length - 1 is the last weapon because the built in array starts counting at zero and weaponOrder.Length starts counting at one. 
			if(CanSelectWeaponByScroll)
			{
			if (Input.GetAxis("Mouse ScrollWheel") < 0 || Input.GetKeyDown(_fpsPlayerComponent.selectPreviousWeapon)){//mouse wheel down or previous weapon button pressed
				if(currentWeapon != 0){//not starting at zero
					for(int i = currentWeapon; i > -1; i--){
						if(weaponOrder[i].GetComponent<WeaponBehavior>().haveWeapon && i != currentWeapon){//check that player has weapon and it is not currently selected weapon
							StartCoroutine(SelectWeapon(i));//run the SelectWeapon function with the next weapon index that was found
							break;
						}else if(i == 0){//reached zero, count backwards from end of list to find next weapon
							for(int n = weaponOrder.Length - 1; n > -1; n--){
								if(weaponOrder[n].GetComponent<WeaponBehavior>().haveWeapon && n != currentWeapon){
									StartCoroutine(SelectWeapon(n));
									break;
								}
							}
						}
					}
				}else{//starting at 0
					for (int i = weaponOrder.Length - 1; i > -1; i--){
						if(weaponOrder[i].GetComponent<WeaponBehavior>().haveWeapon && i != currentWeapon){
							StartCoroutine(SelectWeapon(i));
							break;
						}
					}
				}
			}else if(Input.GetAxis("Mouse ScrollWheel") > 0 //mouse wheel up
			|| Input.GetKeyDown(_fpsPlayerComponent.selectNextWeapon)//select next weapon button pressed
			|| (dropWeapon && totalWeapons != 0)){//drop weapon button pressed and player has weapons in their inventory
				if(currentWeapon < weaponOrder.Length -1){//not starting at last weapon
					for(int i = currentWeapon; i < weaponOrder.Length; i++){
						//cycle weapon selection manually
						if((weaponOrder[i].GetComponent<WeaponBehavior>().haveWeapon 
						&& i != currentWeapon && !dropWeapon) 
						//do not select backupWeapon if dropping a weapon and automatically selecting the next weapon
						//but allow backupWeapon to be selected when cycling weapon selection manually
						|| (weaponOrder[i].GetComponent<WeaponBehavior>().haveWeapon 
						&& i != currentWeapon && i != backupWeapon && dropWeapon)){
							StartCoroutine(SelectWeapon(i));
							break;
						}else if(i == weaponOrder.Length - 1){//reached end of list, count forwards from zero to find next weapon
							for(int n = 0; n < weaponOrder.Length - 1; n++){
								//cycle weapon selection manually
								if((weaponOrder[n].GetComponent<WeaponBehavior>().haveWeapon 
								&& n != currentWeapon && !dropWeapon) 
								//do not select backupWeapon if dropping a weapon and automatically selecting the next weapon
								//but allow backupWeapon to be selected when cycling weapon selection manually
								|| (weaponOrder[n].GetComponent<WeaponBehavior>().haveWeapon 
								&& n != currentWeapon && n != backupWeapon && dropWeapon)){
									StartCoroutine(SelectWeapon(n));
									break;
								}
							}
						}
					}
				}else{//starting at last weapon
					for(int i = 0; i < weaponOrder.Length - 1; i++){
						//cycle weapon selection manually
						if((weaponOrder[i].GetComponent<WeaponBehavior>().haveWeapon 
						&& i != currentWeapon && !dropWeapon) 
						//do not select backupWeapon if dropping a weapon and automatically selecting the next weapon
						//but allow backupWeapon to be selected when cycling weapon selection manually
						|| (weaponOrder[i].GetComponent<WeaponBehavior>().haveWeapon 
						&& i != currentWeapon && i != backupWeapon && dropWeapon)){
							StartCoroutine(SelectWeapon(i));
							break;
						}
					}
				}	
			}
			}
			
			//select weapons with number keys
			if(CanSelectWeaponByKeys)
			{
			if (Input.GetKeyDown(_fpsPlayerComponent.holsterWeapon)) {
				if(currentWeapon != 0){StartCoroutine(SelectWeapon(0));}
			}else if (Input.GetKeyDown(_fpsPlayerComponent.selectWeapon1)) {
				if(currentWeapon != 1){StartCoroutine(SelectWeapon(1));}
			}else if (Input.GetKeyDown(_fpsPlayerComponent.selectWeapon2)) {
				if(currentWeapon != 2){StartCoroutine(SelectWeapon(2));}
			}else if (Input.GetKeyDown(_fpsPlayerComponent.selectWeapon3)) {
				if(currentWeapon != 3){StartCoroutine(SelectWeapon(3));}
			}else if (Input.GetKeyDown(_fpsPlayerComponent.selectWeapon4)) {
				if(currentWeapon != 4){StartCoroutine(SelectWeapon(4));}
			}else if (Input.GetKeyDown(_fpsPlayerComponent.selectWeapon5)) {
				if(currentWeapon != 5){StartCoroutine(SelectWeapon(5));}
			}else if (Input.GetKeyDown(_fpsPlayerComponent.selectWeapon6)) {
				if(currentWeapon != 6){StartCoroutine(SelectWeapon(6));}
			}else if (Input.GetKeyDown(_fpsPlayerComponent.selectWeapon7)) {
				if(currentWeapon != 7){StartCoroutine(SelectWeapon(7));}
			}else if (Input.GetKeyDown(_fpsPlayerComponent.selectWeapon8)) {
				if(currentWeapon != 8){StartCoroutine(SelectWeapon(8));}
			}else if (Input.GetKeyDown(_fpsPlayerComponent.selectWeapon9)) {
				if(currentWeapon != 9){StartCoroutine(SelectWeapon(9));}
			}else if (Input.GetKeyDown(_fpsPlayerComponent.selectWeapon10)) {
				if(currentWeapon != 9){StartCoroutine(SelectWeapon(10));}
			}
			}
			
		}
		
		//check timer for switch to prevent shooting
		//this var checked in "WeaponBehavior" script in the Fire() function 
		if(switchTime + 0.87f > Time.time){
			switching = true;
		}else{
			switching = false;
		}
		
		//define time that sprinting anim is active/transitioning to disable weapon switching
		if(sprintSwitchTime + 0.44f > Time.time){
			sprintSwitching = true;
		}else{
			sprintSwitching = false;
		}
		
		//pause and resume reloading sound based on timescale/game pausing state
		if(Time.timeScale > 0){
			if(audioPaused){
				_aSources[1].Play();	
				audioPaused = false;
			}
		}else{
			if(!audioPaused && _aSources[1].isPlaying){
				_aSources[1].Pause();
				audioPaused = true;
			}
		}
		
	}
	
	//check if the player has a weapon in their inventory (can be used in other scripts)
//	private bool CheckWeapon(string weaponName){
//		//find the PlayerWeapons script in the FPS Prefab to access weaponOrder array
//		PlayerWeapons PlayerWeaponsComponent = Camera.main.transform.GetComponent<CameraKick>().weaponObj.GetComponent<PlayerWeapons>();
//		//iterate through the children of the FPS Weapons object (PlayerWeapon's weaponOrder array) 
//		//and check if player has a certain weapon by gameObject string name like "AK47" or "MP5"
//		for (int i = 0; i < PlayerWeaponsComponent.weaponOrder.Length; i++)	{
//			if(PlayerWeaponsComponent.weaponOrder[i].name == weaponName && PlayerWeaponsComponent.weaponOrder[i].GetComponent<WeaponBehavior>().haveWeapon){
//				return true;//player has the weapon with the string name "weaponName", return true
//			}
//		}
//		return false;//player does not have the weapon with the string name "weaponName", return false
//	}
	
	//set weapon parent position in LateUpdate to sync with CameraKick.cs LateUpdate actions
	void LateUpdate (){
		//align weapon parent origin with player camera origin
		Vector3 tempGunPosition = new Vector3(mainCamTransform.position.x, mainCamTransform.position.y,mainCamTransform.position.z);
		myTransform.position = tempGunPosition;
	}

	public void SetSelectWeaponCallback(System.Action callback)
	{
		_switchWeaponCallback = callback;
	}
	
	public void DropWeapon ( int weapon ){
		FPSRigidBodyWalker FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		FPSPlayer FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		float dropVel;//var to allow velocity to be added to weapon if dropped while moving
		
		//set haveWeapon value to false for this weapon to remove it from player's inventory
		weaponOrder[weapon].GetComponent<WeaponBehavior>().haveWeapon = false;
		
		//modify drop velocity based on player speed so the weapon doesn't fly far away 
		//if player is moving backwards or drop behind player when moving
		if(!deadDropped){
			if(FPSWalkerComponent.inputY > 0){
				dropVel = 8.0f;	
			}else if(FPSWalkerComponent.inputY < 0){
				dropVel = 2.0f;	
			}else{
				dropVel = 4.0f;
			}
		}else{
			if(FPSWalkerComponent.inputY > 0){
				dropVel = 4.0f;
			}else{
				dropVel = 2.0f;
			}
		}
		
		if((currentWeapon != backupWeapon || deadDropped) && currentWeapon != 0){//only drop backup weapon if player dies
			//set dropWeapon value to true for weapon switch code to check below
			dropWeapon = true;
		}
		
		UpdateTotalWeapons();
		
		//instantiate weaponDropObj from WeaponBehavios.cs at camera position
		if(weaponOrder[weapon].GetComponent<WeaponBehavior>().weaponDropObj){
			GameObject weaponObjDrop = Instantiate(weaponOrder[weapon].GetComponent<WeaponBehavior>().weaponDropObj, mainCamTransform.position + playerObj.transform.forward * 0.25f + Vector3.up * -0.25f, mainCamTransform.rotation) as GameObject;
			//add forward velocity to dropped weapon in the direction that the player currently faces 
			weaponObjDrop.GetComponent<Rigidbody>().AddForce(playerObj.transform.forward * dropVel, ForceMode.Impulse);
			//add random rotation to the dropped weapon
			float rotateAmt;
			if(Random.value > 0.5f){rotateAmt = 7.0f;}else{rotateAmt = -7.0f;}
			weaponObjDrop.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * rotateAmt,ForceMode.Impulse);
			if(Random.value > 0.5f){rotateAmt = 7.0f;}else{rotateAmt = -7.0f;}
			weaponObjDrop.GetComponent<Rigidbody>().AddRelativeTorque(Vector3.right * rotateAmt,ForceMode.Impulse);
		}

		//if player dropped their last weapon, just select the null/holstered weapon 
		if(!deadDropped){
			if(totalWeapons == 0 && currentWeapon != 0 && currentWeapon != backupWeapon){
				if(weaponOrder[backupWeapon].GetComponent<WeaponBehavior>().haveWeapon && FPSPlayerComponent.hitPoints > 1.0f){
					StartCoroutine(SelectWeapon(backupWeapon));
				}else{
					StartCoroutine(SelectWeapon(0));	
				}
			}
		}else{
			StartCoroutine(SelectWeapon(0));//don't select another weapon if player died
		}
		
	}
	
	public void UpdateTotalWeapons (){
		totalWeapons = 0;//initialize totalWeapons value at zero because a weapon could have been picked up since we checked last
		for(int i = 1; i < weaponOrder.Length; i++){//iterate through weaponOrder array and count total weapons in player's inventory
			if(weaponOrder[i].GetComponent<WeaponBehavior>().haveWeapon && weaponOrder[i].GetComponent<WeaponBehavior>().addsToTotalWeaps){
				totalWeapons ++;//increment totalWeapons by one if player has this weapon	
			}
		}
	}
	
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//Select Weapons
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
	public IEnumerator SelectWeapon ( int index  ) {
				dropWeapon = false;
		if ((!_weaponsBehavioursList[index].haveWeapon && index != 0) || (bool)_fpsWalkerComponent.hideWeapon)
		{
			yield break;
		}
		if (index != 0)
		{
			_weaponsBehavioursList[0].haveWeapon = false;
		}
		_fpsPlayerComponent.zoomed = false;
		if (_fpsPlayerComponent.CurrentWeaponBehavior.bulletsToReload != (int)_fpsPlayerComponent.CurrentWeaponBehavior.bulletsPerClip && (bool)_fpsPlayerComponent.CurrentWeaponBehavior.weaponMesh.GetComponent<Animation>()["Neutral"] && _ironsightsComponent.reloading)
		{
			_fpsPlayerComponent.CurrentWeaponBehavior.weaponMesh.GetComponent<Animation>()["Neutral"].speed = 1.5f;
			_fpsPlayerComponent.CurrentWeaponBehavior.weaponMesh.GetComponent<Animation>().Play("Neutral", PlayMode.StopAll);
			_fpsPlayerComponent.CurrentWeaponBehavior.bulletsReloaded = 0;
		}
		_ironsightsComponent.reloading = false;
		_fpsPlayerComponent.CurrentWeaponBehavior.StopCoroutine("Reload");
		switchTime = Time.time - Time.deltaTime;
		if (Time.timeSinceLevelLoad > 2f)
		{
			AudioSource.PlayClipAtPoint(changesnd, mainCamTransform.position);
			Camera.main.GetComponent<Animation>().Rewind("CameraSwitch");
			Camera.main.GetComponent<Animation>().CrossFade("CameraSwitch", 0.35f, PlayMode.StopAll);
		}
		if (!_fpsPlayerComponent.CurrentWeaponBehavior.PistolSprintAnim)
		{
			if (!_fpsWalkerComponent.sprintActive)
			{
				weaponOrder[currentWeapon].GetComponent<Animation>()["RifleSprinting"].normalizedTime = 0f;
				weaponOrder[currentWeapon].GetComponent<Animation>()["RifleSprinting"].speed = 1.5f;
				weaponOrder[currentWeapon].GetComponent<Animation>().CrossFade("RifleSprinting", 0.00025f, PlayMode.StopAll);
			}
			else
			{
				weaponOrder[currentWeapon].GetComponent<Animation>()["RifleSprinting"].normalizedTime = 1f;
			}
		}
		else if (!_fpsWalkerComponent.sprintActive)
		{
			weaponOrder[currentWeapon].GetComponent<Animation>()["PistolSprinting"].normalizedTime = 0f;
			weaponOrder[currentWeapon].GetComponent<Animation>()["PistolSprinting"].speed = 1.5f;
			weaponOrder[currentWeapon].GetComponent<Animation>().CrossFade("PistolSprinting", 0.00025f, PlayMode.StopAll);
		}
		else
		{
			weaponOrder[currentWeapon].GetComponent<Animation>()["PistolSprinting"].normalizedTime = 1f;
		}
		if (Time.timeSinceLevelLoad > 2f)
		{
			if (_fpsPlayerComponent.CurrentWeaponBehavior.meleeSwingDelay == 0f)
			{
				_ironsightsComponent.switchMove = -0.4f;
			}
			else
			{
				_ironsightsComponent.switchMove = -1.2f;
			}
			yield return new WaitForSeconds(0.2f);
		}
		for (int i = 0; i < weaponOrder.Length; i++)
		{
			if (i == index)
			{
				weaponOrder[i].SetActive(true);
				currentWeapon = index;
				_fpsPlayerComponent.CurrentWeaponBehavior = _weaponsBehavioursList[currentWeapon];
				weaponOrder[i].transform.localPosition = weaponOrder[i].transform.localPosition + new Vector3(0f, weaponOrder[i].transform.localPosition.y - 0.3f, 0f);
				if (Time.timeSinceLevelLoad > 2f)
				{
					_ironsightsComponent.switchMove = 0f;
				}
				if (!_weaponsBehavioursList[i].PistolSprintAnim)
				{
					if (!_fpsWalkerComponent.sprintActive)
					{
						weaponOrder[i].GetComponent<Animation>()["RifleSprinting"].normalizedTime = 1f;
						weaponOrder[i].GetComponent<Animation>()["RifleSprinting"].speed = -1.5f;
						weaponOrder[i].GetComponent<Animation>().CrossFade("RifleSprinting", 0.00025f, PlayMode.StopAll);
					}
					else
					{
						weaponOrder[i].GetComponent<Animation>()["RifleSprinting"].normalizedTime = 1f;
						weaponOrder[i].GetComponent<Animation>()["RifleSprinting"].speed = 1.5f;
						weaponOrder[i].GetComponent<Animation>().CrossFade("RifleSprinting", 0.00025f, PlayMode.StopAll);
					}
				}
				else if (!_fpsWalkerComponent.sprintActive)
				{
					weaponOrder[i].GetComponent<Animation>()["PistolSprinting"].normalizedTime = 1f;
					weaponOrder[i].GetComponent<Animation>()["PistolSprinting"].speed = -1.5f;
					weaponOrder[i].GetComponent<Animation>().CrossFade("PistolSprinting", 0.00025f, PlayMode.StopAll);
				}
				else
				{
					weaponOrder[i].GetComponent<Animation>()["PistolSprinting"].normalizedTime = 1f;
					weaponOrder[i].GetComponent<Animation>()["PistolSprinting"].speed = 1.5f;
					weaponOrder[i].GetComponent<Animation>().CrossFade("PistolSprinting", 0.00025f, PlayMode.StopAll);
				}
				_ironsightsComponent.gun = weaponOrder[i].transform;
				_ironsightsComponent.gunObj = weaponOrder[i];
				_cameraKickComponent.gun = weaponOrder[i];
			}
			else
			{
				weaponOrder[i].transform.position = myTransform.position;
				if (!_weaponsBehavioursList[i].PistolSprintAnim)
				{
					weaponOrder[i].GetComponent<Animation>()["RifleSprinting"].normalizedTime = 1f;
				}
				else
				{
					weaponOrder[i].GetComponent<Animation>()["PistolSprinting"].normalizedTime = 1f;
				}
				_weaponsBehavioursList[i].sprintState = true;
				weaponOrder[i].SetActive(false);
			}
		}
		if (_switchWeaponCallback != null)
		{
			_switchWeaponCallback();
		}
		_ironsightsComponent.UpdateWeaponBehavior();
	}
}