using System.Collections;
using BattleRoyaleFramework;
using UnityEngine;

namespace BattleRoyale
{
	public class BattleRoyaleConnectionController : MonoBehaviour
	{
		private int _recconnectAttempt = 5;

		public static BattleRoyaleConnectionController I;

		[HideInInspector]
		public int ReconnectCount;

		private void Awake()
		{
			if (I == null)
			{
				I = this;
			}
		}

		private void OnClickBattleRoyaleBtn()
		{
			if (!PhotonNetwork.connected)
			{
				Debug.Log("recconnect ");
				StartCoroutine("Reconnect");
			}
			else
			{
				ReconnectCount = 0;
				DataKeeper.IsBattleRoyaleClick = true;
				ConnectToBattleRoyaleServer();
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
			DataKeeper.IsBattleRoyaleClick = true;
			ConnectToBattleRoyaleServer();
		}

		public void ConnectToBattleRoyaleServer()
		{
			MainMenuController.I.HideMainMenu();
			MenuConnectionViewController.I.ShowWaitingScreen(true);
			DataKeeper.GameType = GameType.BattleRoyale;
			BattleRoyalJoin();
		}

		private void BattleRoyalJoin()
		{
			BRFConnection.JoinOrCreateForGameMode(GameType.BattleRoyale, BattleRoyaleSetupOptions.MaxPlayerOnServer);
		}
	}
}
