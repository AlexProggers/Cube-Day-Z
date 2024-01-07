//HelpText.cs by Azuline Studios© All Rights Reserved
using UnityEngine;
using System.Collections;

public class HelpText : MonoBehaviour {
	//draw ammo amount on screen
	public Color textColor;
	public float horizontalOffset = 0.0425f;
	public float verticalOffset = 0.075f;
	private bool helpTextState = true;
	private bool helpTextEnabled = false;
	private float startTime = 0.0f;
	private bool initialHide = true;
	private bool moveState = true;
	private bool F1pressed = false;
	private bool fadeState = false;
	private float moveTime = 0.0f;
	private float fadeTime = 5.0f;
	[HideInInspector]
	public GameObject playerObj;
	
	void Start(){
		playerObj = GameObject.FindWithTag ("Player");
		GetComponent<GUIText>().pixelOffset = new Vector2 (Screen.width * horizontalOffset, Screen.height * verticalOffset);
		GetComponent<GUIText>().text = "Press F1 for controls";
		GetComponent<GUIText>().material.color = textColor;
		this.GetComponent<GUIText>().enabled = true;
		startTime = Time.time;
	}
	
	void Update (){
		//Initialize script references
		FPSRigidBodyWalker FPSWalkerComponent = playerObj.GetComponent<FPSRigidBodyWalker>();
		FPSPlayer FPSPlayerComponent = playerObj.GetComponent<FPSPlayer>();
		float horizontal = FPSWalkerComponent.inputX;//Get input from player movement script
		float vertical = FPSWalkerComponent.inputY;
		
		Vector4 tempColorVec = GetComponent<GUIText>().material.color; 
		
		if((Mathf.Abs(horizontal) > 0.75f || Mathf.Abs(vertical) > 0.75f) && moveState){
			moveState = false;	
			if(startTime + fadeTime < Time.time){
				moveTime = Time.time;//fade F1 message if moved after fadeTime
			}else{
				moveTime = startTime + fadeTime;//if moved before fade started, start fade at fadeTime	
			}
		}
		
		//if F1 is pressed before fade, bypass fading of F1 message and show help text
		if(Input.GetKeyDown(FPSPlayerComponent.showHelp) && (moveState || moveTime > Time.time)){
			moveState = false;	
			F1pressed = true;
			moveTime = Time.time;
		}
		
		if(!fadeState && !F1pressed){
			if(!moveState && (startTime + fadeTime < Time.time)){
				if(moveTime + 1.0f > Time.time){
					tempColorVec.w -= Time.deltaTime;//fade out color alpha element for one second
					GetComponent<GUIText>().material.color = tempColorVec;
				}else{
					fadeState = true;//F1 message has faded, allow controls to be shown with F1 press
				}
			}
		}else{
			
			if(initialHide){
				GetComponent<GUIText>().text = "Mouse 1 : fire weapon\nMouse 2 : raise sights\nW : forward\nS : backward\nA : strafe left\nD : strafe right\nLeft Shift : sprint\nLeft Ctrl : crouch\nSpace : jump\nE : use item\nR : reload\nF : toggle semi auto\nH : holster weapon\nG : drop weapon\nZ : pick up physics object\nX : throw held physics object\nQ : enter bullet time\nTab : pause game\nV : restart level\nEsc : exit game";
				this.GetComponent<GUIText>().enabled = false;
				tempColorVec.w = 1.0f;//reset alpha to opaque
				GetComponent<GUIText>().material.color = tempColorVec;
				initialHide = false;//do these actions only once after F1 help notice has faded out
			}
			
			if(Input.GetKeyDown(FPSPlayerComponent.showHelp)){
				if(helpTextState){
					if(!helpTextEnabled){
						this.GetComponent<GUIText>().enabled = true;
						helpTextEnabled = true;
					}else{
						this.GetComponent<GUIText>().enabled = false;
						helpTextEnabled = false;
					}
					helpTextState = false;
				}
			}else{
				helpTextState = true;		
			}
		}
		
	}
	
}