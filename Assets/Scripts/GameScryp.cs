using SkyWars;
using UnityEngine;

public class GameScryp : MonoBehaviour
{
	private void OnTriggerEnter(Collider collider)
	{
		if (collider.tag == "Player")
		{
			MenuConnectionViewController.I.HideMainMenu(false);
			SkyWarsConnectionController.I.ConnectToSkyWarsServer();
		}
	}
}
