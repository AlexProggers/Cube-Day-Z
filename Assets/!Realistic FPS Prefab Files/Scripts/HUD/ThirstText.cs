//ThirstText.cs by Azuline Studios© All Rights Reserved
using UnityEngine;
using System.Collections;

public class ThirstText : MonoBehaviour {
	//draw Thirst amount on screen
	[HideInInspector]
	public float thirstGui = 0.0f;
	public Color textColor;
	public float horizontalOffset = 0.0425f;
	public float verticalOffset = 0.075f;
	
	void Start(){
		GetComponent<GUIText>().material.color = textColor;
	}
	
	void Update (){
		GetComponent<GUIText>().text = "Thirst : "+ thirstGui.ToString();
		GetComponent<GUIText>().pixelOffset = new Vector2 (Screen.width * horizontalOffset, Screen.height * verticalOffset);
	}
	
}