//HealthText.cs by Azuline Studios© All Rights Reserved
using UnityEngine;
using System.Collections;

public class HealthText : MonoBehaviour {
	//draw health amount on screen
	public float healthGui = 0;
	public Color textColor;
	public float horizontalOffset = 0.0425f;
	public float verticalOffset = 0.075f;
	
	void Start(){
		GetComponent<GUIText>().material.color = textColor;
	}
	
	void Update (){
		GetComponent<GUIText>().text = "Health : "+ healthGui.ToString();
		GetComponent<GUIText>().pixelOffset = new Vector2 (Screen.width * horizontalOffset, Screen.height * verticalOffset);
	}
	
}