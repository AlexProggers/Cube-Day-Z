using System.Collections;
using System.Collections.Generic;
using Photon;
using SkyWars;
using UnityEngine;

namespace BattleRoyale
{
	public class BattleRoyaleGameManager : Photon.MonoBehaviour
	{
		[SerializeField]
		private tk2dTextMesh _plrCount;

		[SerializeField]
		private tk2dTextMesh _gasInfoMsg;

		public static BattleRoyaleGameManager I;

		private bool _isLobby = true;

		[SerializeField]
		private AudioSource _killSound;

		public GameObject Buttons;

		public GameObject BattleRoyalFinalScreen;

		[SerializeField]
		private tk2dTextMesh[] _killLogs;

		[SerializeField]
		private GameObject[] _fogsRenderers;

		[SerializeField]
		private BattleRoyaleDisableAfter _remainPlayerFade;

		public int AllPlayersOnStart;

		public int MySkyWarsSpawnPoint = -1;

		private bool _lockShowMsg;

		public int _curMsgNumber = 60;

		public int MyFinalPlace = -1;

		private bool _isPlaying;

		private void Awake()
		{
			if (I == null)
			{
				I = this;
			}
		}

		public bool IsLobby()
		{
			return _isLobby;
		}

		public void SetIsLobbyFalse()
		{
			_isLobby = false;
		}

		private void Start()
		{
			ToggleFogRenderes(false);
		}

		private void OnDisable()
		{
			StopCoroutine("CheckPlayerRemaining");
		}

		public void StartShowGasInfoMessage(int time)
		{
			StartCoroutine("ShowGasInfoMessage", time);
		}

		private IEnumerator ShowGasInfoMessage(int time)
		{
			_gasInfoMsg.gameObject.SetActive(true);
			if (time == 0)
			{
				string str2 = "Отравляющий газ запущен!";
				if (DataKeeper.Language != 0)
				{
					str2 = "Releasing the toxic gas!";
				}
				_gasInfoMsg.text = str2;
			}
			else
			{
				string str = "Безопасная зона отмечена на вашей карте. Отравляющий газ будет запущен через " + time + " cекунд.";
				if (DataKeeper.Language != 0)
				{
					str = "The safe zone has been marked on your map! Toxic gas will be released in " + time + " seconds";
				}
				_gasInfoMsg.text = str;
			}
			yield return new WaitForSeconds(6f);
			_gasInfoMsg.gameObject.SetActive(false);
		}

		private void Update()
		{
			if (BattleRoyaleTimeManager.I.GetCurrentTime() <= (float)_curMsgNumber && !IsLobby() && BattleRoyaleTimeManager.I.GetCurrentTime() > 4f && BattleRoyaleTimeManager.I.GetCurrentTask() == TimerCurrentTask.WaitForGasRelease)
			{
				StartShowGasInfoMessage(_curMsgNumber);
				if (_curMsgNumber == 60)
				{
					BattleRoyaleFogManager.I.CanShowAreaOnMap = true;
					_curMsgNumber = 30;
				}
				else if (_curMsgNumber == 30)
				{
					_curMsgNumber = 10;
				}
				else if (_curMsgNumber == 10)
				{
					_curMsgNumber = 0;
				}
			}
			if (BattleRoyaleTimeManager.I.GetCurrentTime() <= 2f && !IsLobby())
			{
				_curMsgNumber = 60;
			}
		}

		private IEnumerator CheckPlayerRemaining()
		{
			int prevCount = 0;
			while (true)
			{
				int count = GetPlayerCount();
				string tmp = "Участников осталось в живых : " + count + " ";
				if (DataKeeper.Language != 0)
				{
					tmp = "Only " + count + " players remain.";
				}
				if (count != prevCount && count > 0)
				{
					_plrCount.text = tmp;
					_remainPlayerFade.ShowPlayerRemain(tmp);
					if (!_isPlaying)
					{
						StartCoroutine("PlayKillSound");
					}
				}
				prevCount = count;
				yield return new WaitForSeconds(2f);
			}
		}

		private void OnPhotonPlayerDisconnected(PhotonPlayer player)
		{
			Debug.Log("player Disconnected :(" + player);
		}

		public void CallStartGame()
		{
			if (PhotonNetwork.isMasterClient)
			{
				base.photonView.RPC("StartGame", PhotonTargets.All);
			}
		}

		[PunRPC]
		public void StartGame()
		{
			BattleRoyaleTimeManager.I.SetCurrentTask(TimerCurrentTask.WaitForGasRelease);

			if (DataKeeper.GameType == GameType.BattleRoyale)
			{
				GameControls.I.Player.EnableParachute(true);
			}

			if (DataKeeper.GameType == GameType.SkyWars)
			{
                GameControls.I.Player.SpawnSkyWars();
                if (PhotonNetwork.isMasterClient)
				{
					BattleRoyaleTimeManager.I.CallStartTimer(SkyWarsSetupOptions.RedBtnStartTime, TimerCurrentTask.SkyWarsRedBtnTime);
				}
			}

			BattleRoyaleLobbyManager.I.DisableLobbyManager();
			if (PhotonNetwork.isMasterClient && DataKeeper.GameType == GameType.BattleRoyale)
			{
				BattleRoyaleTimeManager.I.CallStartTimer(BattleRoyaleFogManager.I.GetTimeForNexFogWave(), TimerCurrentTask.WaitForGasRelease);
			}

			WorldController.I.Player.StartStateEffects();
			StartCoroutine("CheckPlayerRemaining");
			StartCoroutine("IsLobbyFalse");
			AllPlayersOnStart = PhotonNetwork.room.PlayerCount;
			ClearKillLog();
		}

		public int GetPlayerCount()
		{
			int num = 0;
			
			foreach (KeyValuePair<int, PhotonMan> worldPlayer in WorldController.I.WorldPlayers)
			{
				if (!worldPlayer.Value.IsDead)
				{
					num++;
				}
			}
			
			if (num == 1)
			{
				WorldController.I.Player.Suicide();
			}

			return num;
		}

		public void CalculateMyFinalePlace()
		{
			Debug.Log("CalculateMyFinalePlace");
			int playerCount = GetPlayerCount();
			MyFinalPlace = playerCount + 1;
			Buttons.SetActive(false);
			BattleRoyalFinalScreen.SetActive(true);
		}

		public void StartShowKillLog(string killerName, string killedName, string weaponid, int atakerid)
		{
			base.photonView.RPC("ShowKillLog", PhotonTargets.All, killerName, killedName, weaponid, atakerid);
		}

		[PunRPC]
		public void ShowKillLog(string killerName, string killedName, string weaponId, int atakerid)
		{
			string empty = string.Empty;
			if (string.IsNullOrEmpty(killerName) && string.IsNullOrEmpty(weaponId))
			{
				empty = "^CFF0000FF" + killedName + "^CFFFFFFFF - дезертир (покинул бой) ";
			}
			else
			{
				Weapon weapon = (string.IsNullOrEmpty(weaponId) ? null : DataKeeper.Info.GetWeaponInfo(weaponId));
				string text = ((weapon == null) ? " руками" : weapon.RussianName);
				if (DataKeeper.Language != 0)
				{
					text = ((weapon == null) ? "Fists" : weapon.Name);
				}
				if (atakerid == -1)
				{
					empty = "^CFF0000FF" + killedName + "^CFFFFFFFF был убит ядовитым газом";
					if (DataKeeper.Language != 0)
					{
						empty = "^CFF0000FF" + killedName + "^CFFFFFFFF was killed by gas";
					}
					if (DataKeeper.GameType == GameType.SkyWars)
					{
						empty = "^CFF0000FF" + killedName + "^CFFFFFFFF упал в пропасть";
						if (DataKeeper.Language != 0)
						{
							empty = "^CFF0000FF" + killedName + "^CFFFFFFFF fell into the abyss";
						}
					}
				}
				else
				{
					empty = "^C00CC00FF" + killerName + "^CFFFFFFFF - завалил ^CFF0000FF" + killedName + "^CFFFFFFFF: " + text;
					if (DataKeeper.Language != 0)
					{
						empty = "^C00CC00FF" + killerName + "^CFFFFFFFF killed ^CFF0000FF" + killedName + "^CFFFFFFFF: " + text;
					}
				}
			}
			FillKillLogs(empty);
		}

		private void FillKillLogs(string newKillLog)
		{
			Debug.Log(_killLogs.Length + newKillLog);
			_killLogs[3].text = _killLogs[2].text;
			_killLogs[2].text = _killLogs[1].text;
			_killLogs[1].text = _killLogs[0].text;
			_killLogs[0].text = newKillLog;
		}

		private void ClearKillLog()
		{
			_killLogs[3].text = string.Empty;
			_killLogs[2].text = string.Empty;
			_killLogs[1].text = string.Empty;
			_killLogs[0].text = string.Empty;
		}

		private IEnumerator PlayKillSound()
		{
			_isPlaying = true;
			_killSound.enabled = true;
			yield return new WaitForSeconds(6.5f);
			_killSound.enabled = false;
			_isPlaying = false;
		}

		public void ToggleFogRenderes(bool flag)
		{
			for (int i = 0; i < _fogsRenderers.Length; i++)
			{
				_fogsRenderers[i].SetActive(flag);
			}
		}

		private IEnumerator IsLobbyFalse()
		{
			yield return new WaitForSeconds(60f);
			_isLobby = false;
		}
	}
}
