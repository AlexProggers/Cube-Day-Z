//RemoveBody.cs by Azuline Studios© All Rights Reserved
//Removes NPC ragdolls after bodyStayTime.
using UnityEngine;
using System.Collections;

public class RemoveBody : MonoBehaviour {

	private float startTime = 0;
	[HideInInspector]
	public float bodyStayTime = 15.0f;
	
	void Start (){
		startTime = Time.time;
	}
	
	void FixedUpdate (){
		if(startTime + bodyStayTime < Time.time){
			Destroy(gameObject);
		}
	}

}