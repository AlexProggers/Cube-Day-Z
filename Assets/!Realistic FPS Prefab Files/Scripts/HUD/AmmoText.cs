//AmmoText.cs by Azuline Studios© All Rights Reserved
using UnityEngine;
using System.Collections;

public class AmmoText : MonoBehaviour {
	//draw ammo amount on screen
	public int ammoGui = 0;//bullets remaining in clip
	public int ammoGui2 = 0;//total ammo in inventory
	public Color textColor;
	public float horizontalOffset = 0.95f;
	public float verticalOffset = 0.075f;
	[HideInInspector]
	public float horizontalOffsetAmt = 0.78f;
	[HideInInspector]
	public float verticalOffsetAmt = 0.1f;
	
	void start (){
		horizontalOffsetAmt = horizontalOffset;
		verticalOffsetAmt = verticalOffset;	
	}
	
	void Update (){
		if(DataKeeper.Language == Language.Russian)
		{
			GetComponent<GUIText>().text = "Патронов: "+ ammoGui.ToString();
		}
		else if(DataKeeper.Language == Language.English)
		{
			GetComponent<GUIText>().text = "Ammo: "+ ammoGui.ToString();
		}

		GetComponent<GUIText>().pixelOffset = new Vector2 (Screen.width * horizontalOffsetAmt, Screen.height * verticalOffsetAmt);
		GetComponent<GUIText>().material.color = textColor;
	
	}
}