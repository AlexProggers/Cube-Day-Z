using ExitGames.Client.Photon;
using Photon;
using SkyWars;
using UnityEngine;

namespace BattleRoyale
{
        public enum TimerCurrentTask
	{
		Waiting = 0,
		WaitForFillingRoom = 1,
		CountdownForStart = 2,
		WaitForGasRelease = 3,
		SkyWarsRedBtnTime = 4,
		SkyWarsEndRound = 5
	}

	public class BattleRoyaleTimeManager : Photon.MonoBehaviour
	{
		private const string StartTimeKey = "RoomTime";

		private float _currentTime;

		private int m_countdown = 10;

		private bool _timerWork;

		private float _photonTime;

		private int _timerDuration;

		private TimerCurrentTask _currentTask;

		public static BattleRoyaleTimeManager I;

		private bool GetTimeServed
		{
			get
			{
				bool result = false;
				if (Time.timeSinceLevelLoad > 7f)
				{
					result = true;
				}
				return result;
			}
		}

		public TimerCurrentTask GetCurrentTask()
		{
			return _currentTask;
		}

		public void SetCurrentTask(TimerCurrentTask task)
		{
			_currentTask = task;
		}

		public float GetCurrentTime()
		{
			return _currentTime;
		}

		public string GetCurrentTimeFormat()
		{
			int num = 60;
			float num2 = Mathf.CeilToInt(_currentTime);
			int num3 = Mathf.FloorToInt(num2 % (float)num);
			int num4 = Mathf.FloorToInt(num2 / (float)num % (float)num);
			return GetTimeFormat(num4, num3);
		}

		private void Awake()
		{
			if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
			{
				base.gameObject.SetActive(false);
				return;
			}
			if (I == null)
			{
				I = this;
			}
			if (PhotonNetwork.connected && PhotonNetwork.room != null && PhotonNetwork.isMasterClient)
			{
				PhotonNetwork.room.SetCustomProperties(new Hashtable { { "TimeRoom", 0 } });
			}
		}

		private void Start()
		{
			GetTime();
		}

		private void GetTime()
		{
			if (PhotonNetwork.room != null && PhotonNetwork.room.CustomProperties.ContainsKey("TimeRoom"))
			{
				_timerDuration = (int)PhotonNetwork.room.CustomProperties["TimeRoom"];
				if (PhotonNetwork.isMasterClient)
				{
					_photonTime = (float)PhotonNetwork.time;
					Hashtable hashtable = new Hashtable();
					hashtable.Add("RoomTime", _photonTime);
					PhotonNetwork.room.SetCustomProperties(hashtable);
					base.photonView.RPC("GetTimeRpc", PhotonTargets.Others);
				}
				else if (PhotonNetwork.room.CustomProperties.ContainsKey("RoomTime"))
				{
					_photonTime = (float)PhotonNetwork.room.CustomProperties["RoomTime"];
				}
			}
		}

		private void FixedUpdate()
		{
			if (!_timerWork)
			{
				return;
			}
			float num = (float)_timerDuration - ((float)PhotonNetwork.time - _photonTime);
			if (num > 0f)
			{
				_currentTime = num;
				if (_currentTask == TimerCurrentTask.WaitForFillingRoom && _currentTime < 1f)
				{
					BattleRoyaleSoundManager.I.StartCountDown();
				}
			}
			else if ((double)num <= 0.001 && GetTimeServed)
			{
				_currentTime = 0f;
				if (_timerWork)
				{
					_timerWork = false;
					OnTimerStop(_currentTask);
				}
			}
			else
			{
				Refresh();
			}
		}

		private void OnTimerStop(TimerCurrentTask currentTask)
		{
			switch (currentTask)
			{
			case TimerCurrentTask.WaitForFillingRoom:
				if (BattleRoyaleLobbyManager.I != null)
				{
					BattleRoyaleLobbyManager.I.StartCountdownForStartGame();
				}
				break;
			case TimerCurrentTask.CountdownForStart:
				if (BattleRoyaleGameManager.I != null)
				{
					BattleRoyaleGameManager.I.CallStartGame();
				}
				break;
			case TimerCurrentTask.WaitForGasRelease:
				BattleRoyaleFogManager.I.CallStartMoveFogArea();
				break;
			case TimerCurrentTask.SkyWarsRedBtnTime:
				SkyWarsGameLogic.I.CallShowRedBtn();
				break;
			case TimerCurrentTask.SkyWarsEndRound:
				SkyWarsGameLogic.I.OnRoundEnd();
				break;
			}
		}

		public void CallStartTimer(int duration, TimerCurrentTask currentTask)
		{
			if (PhotonNetwork.isMasterClient)
			{
				Debug.Log("start timer " + currentTask);
				base.photonView.RPC("StartTimer", PhotonTargets.All, duration, (byte)currentTask);
			}
		}

		[PunRPC]
		private void StartTimer(int duration, byte currentTask)
		{
			_timerWork = true;
			if (PhotonNetwork.room != null)
			{
				if (PhotonNetwork.isMasterClient)
				{
					PhotonNetwork.room.SetCustomProperties(new Hashtable { { "TimeRoom", duration } });
					GetTime();
				}
				_currentTask = (TimerCurrentTask)currentTask;
				if (_currentTask == TimerCurrentTask.CountdownForStart)
				{
					BattleRoyaleSoundManager.I.StartCountDown();
				}
			}
		}

		[PunRPC]
		private void GetTimeRpc()
		{
			GetTime();
		}

		private void Refresh()
		{
			_timerWork = true;
			if (PhotonNetwork.isMasterClient)
			{
				_photonTime = (float)PhotonNetwork.time;
				Hashtable hashtable = new Hashtable();
				hashtable.Add("RoomTime", _photonTime);
				PhotonNetwork.room.SetCustomProperties(hashtable);
			}
			else if (PhotonNetwork.room.CustomProperties.ContainsKey("RoomTime"))
			{
				_photonTime = (float)PhotonNetwork.room.CustomProperties["RoomTime"];
			}
		}

		private void countdown()
		{
			m_countdown--;
			if (m_countdown <= 0)
			{
				CancelInvoke("countdown");
				m_countdown = 10;
			}
		}

		public static string GetTimeFormat(float m, float s)
		{
			return string.Format("{0:00}:{1:00}", m, s);
		}
	}
}
