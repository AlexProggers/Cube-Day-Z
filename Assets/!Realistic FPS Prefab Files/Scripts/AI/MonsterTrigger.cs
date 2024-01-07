//MonsterTrigger.cs by Azuline Studios© All Rights Reserved
//used to spawn NPCs after player enters trigger to set up traps and ambushes
//this script should be excecuted before the AI.js script (set in script excecution order window)
using UnityEngine;
using System.Collections;

public class MonsterTrigger : MonoBehaviour {
	//NPC objects to deactivate on level load and activate when player walks into trigger (set in inspector)
	public GameObject[] npcsToTrigger;
	
	void Start () {
		//deactivate the npcs in the npcsToTrigger array on start up
		for (int i = 0; i < npcsToTrigger.Length; i++){
			if(npcsToTrigger[i]){
				#if UNITY_3_5
					npcsToTrigger[i].SetActiveRecursively(false);
				#else
					npcsToTrigger[i].SetActive(false);
				#endif
			}
		}
	}

	void OnTriggerEnter ( Collider col  ){
		if(col.gameObject.tag == "Player"){
			//activate the npcs in the npcsToTrigger array when object is picked up/used by player
			for (int i = 0; i < npcsToTrigger.Length; i++){
				if(npcsToTrigger[i]){
					#if UNITY_3_5
						npcsToTrigger[i].SetActiveRecursively(true);
					#else
						npcsToTrigger[i].SetActive(true);
					#endif	
				}
			}
			
			Destroy(transform.gameObject);
		}
	}
}
