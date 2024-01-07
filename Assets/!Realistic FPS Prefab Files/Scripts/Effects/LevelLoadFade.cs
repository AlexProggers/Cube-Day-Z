//LevelLoadFade.cs by Azuline Studios© All Rights Reserved
using UnityEngine;
using System.Collections;
//script to fade in from black and fade out to black
public class LevelLoadFade : MonoBehaviour {

	[HideInInspector]
	public GameObject LevelLoadFadeobj;
	
	public void FadeAndLoadLevel ( Color color, float fadeLength, bool fadeIn ){
		Texture2D fadeTexture = new Texture2D (1, 1);//Create texture for screen fade
		fadeTexture.SetPixel(0, 0, color);
		fadeTexture.Apply();
		
		LevelLoadFadeobj.layer = 14;//set fade object's layer to one not ignored by weapon camera
		LevelLoadFadeobj.AddComponent<GUITexture>();
		LevelLoadFadeobj.transform.position = new Vector3 (0.5f, 0.5f, 1000);
		LevelLoadFadeobj.GetComponent<GUITexture>().texture = fadeTexture;
	
		DontDestroyOnLoad(fadeTexture);
	
		if(fadeIn){//Call DoFadeIn or DoFadeout functions based on which argument is called
			StartCoroutine(DoFadeIn(fadeLength, true));
		}else{
			StartCoroutine(DoFadeout(fadeLength, true));	
		}
	}

	IEnumerator DoFadeIn ( float fadeLength ,   bool destroyTexture  ){
		 // Dont destroy the fade game object during level load
		DontDestroyOnLoad(LevelLoadFadeobj);
	
		//make alpha of color = 0 (transparent for starting fade out)
		//Create a temporary Vector4 (C# does not allow modifying guiTexture color directly, but JS will)
		Vector4 tempColorVec = GetComponent<GUITexture>().color; 
   		tempColorVec.w = 0.0f;//store the color's alpha amount as the fourth value of the Vector4
    	GetComponent<GUITexture>().color = tempColorVec;//set the guiTexture's color to the value(s) of our temporary color vector
		
		// Fade texture in
		float time = 0.0f;
		while (time < fadeLength){
			time += Time.deltaTime;
			tempColorVec.w = Mathf.InverseLerp(fadeLength, 0.0f, time);//smoothly fade alpha in
			GetComponent<GUITexture>().color = tempColorVec;
			yield return 0;
		}
	
		Destroy (LevelLoadFadeobj);//destroy temporary texture 
	
		// If we created the texture from code we used DontDestroyOnLoad,
		// which means we have to clean it up manually to avoid leaks
		if (destroyTexture){
			Destroy (GetComponent<GUITexture>().texture);
		}
	}
	
	IEnumerator DoFadeout (float fadeLength, bool destroyTexture){
		
		//FPSPlayer FPSPlayerComponent = Camera.main.transform.parent.transform.parent.GetComponentInChildren<FPSPlayer>();
		//FPSRigidBodyWalker FPSWalkerComponent = Camera.main.transform.parent.transform.parent.GetComponentInChildren<FPSRigidBodyWalker>();
		
		Vector4 tempColorVec = GetComponent<GUITexture>().color; 
   		tempColorVec.w = 0.0f;//store the color's alpha amount as the fourth value of the Vector4
    	GetComponent<GUITexture>().color = tempColorVec;//set the guiTexture's color to the value(s) of our temporary color vector
		
		//Fade texture in
		float time = 0.0f;
		while (time < fadeLength){
			time += Time.deltaTime;
			tempColorVec.w = Mathf.InverseLerp(0.0f, fadeLength, time);//smoothly fade alpha out
			GetComponent<GUITexture>().color = tempColorVec;
			yield return 0;
		}
	
		//Complete the fade out (Load a level or reset player position, not needed if using checkpoint spawning)
		Application.LoadLevel(Application.loadedLevel);
		
		//GameObject llf = Instantiate(FPSPlayerComponent.levelLoadFadeObj) as GameObject;
		//llf.GetComponent<LevelLoadFade>().FadeAndLoadLevel(Color.black, 2.0f, true);
		
		//respawn the player at the checkpoint position with full health
		//FPSPlayerComponent.HealPlayer(100.0f - FPSPlayerComponent.hitPoints);
		//FPSWalkerComponent.myTransform.position = FPSPlayerComponent.checkpointPos.position;
		
		yield return new WaitForSeconds(1.0f);
		
		Destroy (LevelLoadFadeobj);//destroy temporary texture 
	
		// If we created the texture from code we used DontDestroyOnLoad,
		// which means we have to clean it up manually to avoid leaks
		if (destroyTexture){
			Destroy (GetComponent<GUITexture>().texture);
		}

	}
}