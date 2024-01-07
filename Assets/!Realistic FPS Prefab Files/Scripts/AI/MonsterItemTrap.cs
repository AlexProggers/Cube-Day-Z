//MonsterItemTrap.cs by Azuline Studios© All Rights Reserved
//used to spawn NPCs after player picks up an item to set up traps and ambushes
//this script should be excecuted before the AI.js script (set in script excecution order window)
using UnityEngine;
using System.Collections;

public class MonsterItemTrap : MonoBehaviour {
	//NPC objects to deactivate on level load and activate when player picks up item (set in inspector)
	public GameObject[] npcsToTrigger;
	
	void Start () {
		//deactivate the npcs in the npcsToTrigger array on start up
		for(int i = 0; i < npcsToTrigger.Length; i++){
			if(npcsToTrigger[i]){
				#if UNITY_3_5
					npcsToTrigger[i].SetActiveRecursively(false);
				#else
					npcsToTrigger[i].SetActive(false);
				#endif
			}
		}
	}
	
	//ActivateObject is called by every object with the "Usable" tag that the player activates/picks up by pressing the use key
	void ActivateObject () {
		//activate the npcs in the npcsToTrigger array when object is picked up/used by player
		for(int i = 0; i < npcsToTrigger.Length; i++){
			if(npcsToTrigger[i]){
				#if UNITY_3_5
					npcsToTrigger[i].SetActiveRecursively(true);
				#else
					npcsToTrigger[i].SetActive(true);
				#endif	
			}
		}
	}
}
