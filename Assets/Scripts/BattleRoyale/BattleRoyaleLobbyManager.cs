using System.Collections;
using BattleRoyaleFramework;
using Photon;
using SkyWars;
using UnityEngine;

namespace BattleRoyale
{
	public class BattleRoyaleLobbyManager : Photon.MonoBehaviour
	{
		[SerializeField]
		private tk2dTextMesh _plrCount;

		[SerializeField]
		private GameObject _secondCountDown;

		[SerializeField]
		private GameObject _ui;

		[SerializeField]
		private GameObject _battleUi;

		[SerializeField]
		private GameObject _lobbyObj;

		[SerializeField]
		private tk2dTextMesh[] _playerConnectedLogs;

		public static BattleRoyaleLobbyManager I;

		private string _allPlayerPhrase = "Всего участников: ";

		private string _minText = " (минимум для старта:";

		private bool _enableFirst;

		private void Awake()
		{
			//Application.runInBackground = true;
			if (I == null)
			{
				I = this;
			}
		}

		private void OnDisable()
		{
			if (_ui != null)
			{
				_ui.SetActive(false);
			}
			if (_battleUi != null)
			{
				_battleUi.SetActive(true);
			}
			StopCoroutine("ShowCountDown");
		}

		private void Start()
		{
			if (DataKeeper.Language != 0)
			{
				_allPlayerPhrase = "Players in game: ";
				_minText = " (min players:";
			}
			UpdatePlayerCount();
			base.photonView.RPC("FillPlayerConnectedLogs", PhotonTargets.All, PhotonNetwork.playerName, true);
		}

		private void UpdatePlayerCount()
		{
			int maxPlayerOnServer = BattleRoyaleSetupOptions.MaxPlayerOnServer;
			int minimumPlayers    = BattleRoyaleSetupOptions.MinPlayersForStart;

			if (DataKeeper.GameType == GameType.SkyWars)
			{
				maxPlayerOnServer = SkyWarsSetupOptions.MaxPlayerOnServer;
				minimumPlayers    = SkyWarsSetupOptions.MinPlayersForStart;
			}

			if (BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.WaitForFillingRoom || BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.CountdownForStart)
			{
				_plrCount.text = "^CFFFFFFFF" + _allPlayerPhrase + "^C00CC00FF" + PhotonNetwork.room.PlayerCount + "^CFFFFFFFF / ^C00CC00FF" + maxPlayerOnServer + _minText + minimumPlayers + ")";
			}
			else
			{
				_plrCount.text = "^CFFFFFFFF" + _allPlayerPhrase + "^CFF0000FF" + PhotonNetwork.room.PlayerCount + "^CFFFFFFFF / ^CFF0000FF" + maxPlayerOnServer + _minText + minimumPlayers + ")";
			}
		}

		private void Update()
		{
			if (BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.WaitForFillingRoom || BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.CountdownForStart)
			{
				StartCoroutine("ShowCountDown");
			}
		}

		private IEnumerator ShowCountDown()
		{
			if (_enableFirst)
			{
				yield break;
			}

			_enableFirst = true;
			_secondCountDown.SetActive(true);

			tk2dTextMesh tm = _secondCountDown.GetComponent<tk2dTextMesh>();

			while (true)
			{
				string tmp = "Матч начнется через: " + GetTimeForLobby() + " cекунд.";

				if (DataKeeper.Language != 0)
				{
					tmp = "Match starts in: " + GetTimeForLobby() + " seconds.";
				}

				tm.text = tmp;
				yield return new WaitForSeconds(0.3f);
			}
		}

		public int GetTimeForLobby()
		{
			int result = 0;

			if (BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.WaitForFillingRoom)
			{
				result = (int)BattleRoyaleTimeManager.I.GetCurrentTime() + 10;
			}

			if (BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.CountdownForStart)
			{
				result = (int)BattleRoyaleTimeManager.I.GetCurrentTime();
			}

			return result;
		}

		private void OnPhotonPlayerConnected(PhotonPlayer player)
		{
			if (!BattleRoyaleGameManager.I.IsLobby())
			{
				return;
			}

			UpdatePlayerCount();

			if (PhotonNetwork.isMasterClient)
			{
				if (PhotonNetwork.room.PlayerCount == PhotonNetwork.room.MaxPlayers)
				{
					PhotonNetwork.room.IsOpen = false;
				}
				
				WaitForMinimumPlayers();
			}
		}

		private void OnPhotonPlayerDisconnected(PhotonPlayer player)
		{
			if (BattleRoyaleGameManager.I.IsLobby())
			{
				UpdatePlayerCount();
				Debug.Log("player Disconnected :(" + player);
				FillPlayerConnectedLogs(player.NickName, false);
			}
		}

		public void WaitForMinimumPlayers()
		{
			int playerCount = PhotonNetwork.room.PlayerCount;
			int minStart = BattleRoyaleSetupOptions.MinPlayersForStart;
			int waitInLobbyForFillRoomToMax = BattleRoyaleSetupOptions.WaitInLobbyForFillRoomToMax;

			if (DataKeeper.GameType == GameType.SkyWars)
			{
				minStart = SkyWarsSetupOptions.MinPlayersForStart;
				waitInLobbyForFillRoomToMax = SkyWarsSetupOptions.WaitInLobbyForFillRoomToMax;
			}

			if (playerCount >= minStart && BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.Waiting)
			{
				BattleRoyaleTimeManager.I.CallStartTimer(waitInLobbyForFillRoomToMax, TimerCurrentTask.WaitForFillingRoom);
			}
		}

		public void StartCountdownForStartGame()
		{
			if (PhotonNetwork.isMasterClient)
			{
				PhotonNetwork.room.IsOpen = false;
				PhotonNetwork.room.Name += "_RUNNING_";
				
				int startingCountdown = BattleRoyaleSetupOptions.StartingCountdown;
				
				if (DataKeeper.GameType == GameType.SkyWars)
				{
					startingCountdown = SkyWarsSetupOptions.StartingCountdown;
				}
				
				BattleRoyaleTimeManager.I.CallStartTimer(startingCountdown, TimerCurrentTask.CountdownForStart);
				CalculateSpawnPoints();
			}
		}

		public void DisableLobbyManager()
		{
			base.enabled = false;
			if (_lobbyObj != null)
			{
				_lobbyObj.SetActive(false);
			}
		}

		[PunRPC]
		private void FillPlayerConnectedLogs(string plrNm, bool isConnected = true)
		{
			string empty = string.Empty;
			if (isConnected)
			{
				empty = plrNm + " - подключился к игре";
				if (DataKeeper.Language != 0)
				{
					empty = plrNm + " - enter the game";
				}
			}
			else
			{
				empty = plrNm + " - испугался и вышел";
				if (DataKeeper.Language != 0)
				{
					empty = plrNm + " - disconnected";
				}
			}
			_playerConnectedLogs[3].text = _playerConnectedLogs[2].text;
			_playerConnectedLogs[2].text = _playerConnectedLogs[1].text;
			_playerConnectedLogs[1].text = _playerConnectedLogs[0].text;
			_playerConnectedLogs[0].text = empty;
		}


		public void CalculateSpawnPoints()
		{
			if (PhotonNetwork.isMasterClient)
			{
				PhotonPlayer[] playerList = PhotonNetwork.playerList;

				for (int i = 0; i < playerList.Length; i++)
				{
					base.photonView.RPC("SetupSpawnPoint", playerList[i], (byte)i);
				}
			}
		}
		
		[PunRPC]
		public void SetupSpawnPoint(byte point)
		{
			BattleRoyaleGameManager.I.MySkyWarsSpawnPoint = point;
		}

		[PunRPC]
		public void ExitRoom()
		{
			BRFConnection.ExitGames();
		}
	}
}
