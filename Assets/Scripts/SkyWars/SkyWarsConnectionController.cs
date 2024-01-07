using System.Collections;
using BattleRoyaleFramework;
using UnityEngine;

namespace SkyWars
{
	public class SkyWarsConnectionController : MonoBehaviour
	{
		private int _recconnectAttempt = 5;

		[HideInInspector]
		public int ReconnectCount;

		public static SkyWarsConnectionController I;

		private void Awake()
		{
			if (I == null)
			{
				I = this;
			}
		}

		private IEnumerator Reconnect()
		{
			while (!PhotonNetwork.connected)
			{
				MenuConnectionController.I.ConnectToPhoton();
				yield return new WaitForSeconds(2f);
			}
			ReconnectCount = 0;
			DataKeeper.IsSkyWarsClick = true;
			ConnectToSkyWarsServer();
			MenuConnectionViewController.I.HideMainMenu(true);
		}

		private void OnClickSkyWarsBtn()
		{
			if (!PhotonNetwork.connected)
			{
				Debug.Log("recconnect ");
				StartCoroutine("Reconnect");
			}

			else
			{
				ReconnectCount = 0;
				DataKeeper.IsSkyWarsClick = true;
				ConnectToSkyWarsServer();
				MenuConnectionViewController.I.HideMainMenu(true);
			}
		}

		public void ConnectToSkyWarsServer()
		{
			MainMenuController.I.HideMainMenu();
			MenuConnectionViewController.I.ShowWaitingScreen(true);
			DataKeeper.GameType = GameType.SkyWars;
			Join();
		}

		private void Join()
		{
			BRFConnection.JoinOrCreateForGameMode(GameType.SkyWars, SkyWarsSetupOptions.MaxPlayerOnServer);
		}
	}
}
