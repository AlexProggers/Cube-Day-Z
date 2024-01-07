//FadeOutDecals.cs by Azuline Studios© All Rights Reserved
using UnityEngine;
using System.Collections;
//script to fade out decals like bullet marks smoothly
public class FadeOutDecals : MonoBehaviour {

	float startTime;
	public int markDuration = 10;
	[HideInInspector]
	public MeshRenderer hitMesh;
		
	void Start (){
		startTime = Time.time;
	}
	
	void Update (){
		
		if(startTime + markDuration < Time.time){
			Color tempColorVec = GetComponent<Renderer>().material.color; 
	   		tempColorVec.a -= 1 * Time.deltaTime;//store the color's alpha amount
	    	GetComponent<Renderer>().material.color = tempColorVec;//set the guiTexture's color to the value(s) of our temporary color vector
		}
		
		//destroy this decal if the hit object's mesh component gets disabled 
		//such as when the player damages and destroys a breakable object
		if(hitMesh){
			if(!hitMesh.enabled){
				Destroy(transform.parent.transform.gameObject);
			}
		}
		
	}
}