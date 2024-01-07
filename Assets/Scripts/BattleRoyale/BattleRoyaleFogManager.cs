using System.Collections;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

namespace BattleRoyale
{
        public enum FogSide
	{
		Left = 0,
		Right = 1,
		Top = 2,
		Bottom = 3,
		Center = 4
	}

	public class BattleRoyaleFogManager : Photon.MonoBehaviour
	{
		[SerializeField]
		private Transform _topSide;

		[SerializeField]
		private Transform _bottomSide;

		[SerializeField]
		private Transform _rightSide;

		[SerializeField]
		private Transform _leftSide;

		[SerializeField]
		private Transform _fogArea;

		[SerializeField]
		private Transform _fogAreaCenter;

		[SerializeField]
		private Transform[] _safetyZones;

		private Transform _currentSafityZone;

		public float СurrentFogWaveStep;

		private int _сurrentDamage;

		private int _fogWaveNumber;

		public Transform _nextTopSide;

		public Transform _nextBottomSide;

		public Transform _nextRightSide;

		public Transform _nextLeftSide;

		public Transform _nextFogArea;

		public Transform _nextFogAreaCenter;

		[SerializeField]
		private AudioSource _sirenaSound;

		public bool CanShowAreaOnMap;

		public static BattleRoyaleFogManager I;

		public int CurrentSafetyZoneNumber;

		public Transform GetCurrentSafityZone()
		{
			return _currentSafityZone;
		}

		public int GetCurrentDamage()
		{
			return _сurrentDamage;
		}

		public int GetFogWaveNumber()
		{
			return _fogWaveNumber;
		}

		private void Awake()
		{
			if (I == null)
			{
				I = this;
			}
			SetStartWaveStep();
		}

		public void Start()
		{
			StartCoroutine("SetupCurrentSafetyZone");
		}

		public void CallStartMoveFogArea()
		{
			base.photonView.RPC("StartMoveFogArea", PhotonTargets.All, _nextFogArea.position);
		}

		[PunRPC]
		public void StartMoveFogArea(Vector3 pos)
		{
			StartCoroutine("MoveFogArea", pos);
		}

		public IEnumerator MoveFogArea(Vector3 newPos)
		{
			BattleRoyaleGameManager.I.StartShowGasInfoMessage(0);
			StartCoroutine("PlaySirenaSound");
			if (GetFogWaveNumber() > 3)
			{
				BattleRoyaleSoundManager.I.PlayFinalGastTrack();
			}
			else
			{
				BattleRoyaleSoundManager.I.PlayAfterGastTrack();
			}
			while (Vector3.Distance(newPos, _fogArea.position) > 0.05f)
			{
				_fogArea.position = Vector3.MoveTowards(_fogArea.position, newPos, BattleRoyaleSetupOptions.FogAreaMovingSpeed * Time.deltaTime);
				yield return null;
			}
			Debug.Log("stop moving");
			StartCoroutine("FogWaveRelease");
			BattleRoyaleGameManager.I.ToggleFogRenderes(true);
		}

		public IEnumerator FogWaveRelease()
		{
			Debug.Log("Fog wave release! " + _leftSide.name + " --- " + _topSide.position);
			while ((double)Vector3.Distance(_topSide.position, _nextTopSide.position) >= 0.005)
			{
				_topSide.position = Vector3.MoveTowards(_topSide.position, _nextTopSide.position, BattleRoyaleSetupOptions.FogSpeed * Time.deltaTime);
				_bottomSide.position = Vector3.MoveTowards(_bottomSide.position, _nextBottomSide.position, BattleRoyaleSetupOptions.FogSpeed * Time.deltaTime);
				_leftSide.position = Vector3.MoveTowards(_leftSide.position, _nextLeftSide.position, BattleRoyaleSetupOptions.FogSpeed * Time.deltaTime);
				_rightSide.position = Vector3.MoveTowards(_rightSide.position, _nextRightSide.position, BattleRoyaleSetupOptions.FogSpeed * Time.deltaTime);
				yield return null;
			}
			СurrentFogWaveStep = BattleRoyaleSetupOptions.DefaultFogWaveStep;
			_fogWaveNumber++;
			_сurrentDamage = BattleRoyaleSetupOptions.GetCurrentFogDamage(_fogWaveNumber);
			Debug.Log("stop  Fog Release  step = " + СurrentFogWaveStep);
			if (BattleRoyaleSoundManager.I != null)
			{
				BattleRoyaleSoundManager.I.PlayStartTrack();
			}
			yield return new WaitForSeconds(20f);
			if (PhotonNetwork.isMasterClient)
			{
				base.photonView.RPC("CalculateNextFogSidePosition", PhotonTargets.All, _currentSafityZone.localPosition, false);
			}
		}

		[PunRPC]
		private void CalculateNextFogSidePosition(Vector3 curSafityZonePosbool, bool isFirst = false)
		{
			_nextFogArea.localPosition = curSafityZonePosbool;
			_nextTopSide.localPosition = _topSide.localPosition;
			Vector3 localPosition = _nextTopSide.localPosition;
			localPosition.z -= СurrentFogWaveStep;
			_nextTopSide.localPosition = localPosition;
			_nextBottomSide.localPosition = _bottomSide.localPosition;
			Vector3 localPosition2 = _nextBottomSide.localPosition;
			localPosition2.z += СurrentFogWaveStep;
			_nextBottomSide.localPosition = localPosition2;
			_nextLeftSide.localPosition = _leftSide.localPosition;
			Vector3 localPosition3 = _nextLeftSide.localPosition;
			localPosition3.x += СurrentFogWaveStep;
			_nextLeftSide.localPosition = localPosition3;
			_nextRightSide.localPosition = _rightSide.localPosition;
			Vector3 localPosition4 = _nextRightSide.localPosition;
			localPosition4.x -= СurrentFogWaveStep;
			_nextRightSide.localPosition = localPosition4;
			if (PhotonNetwork.isMasterClient && !isFirst)
			{
				StartOffsetDebugArea();
			}
		}

		private void StartOffsetDebugArea()
		{
			if (PhotonNetwork.isMasterClient)
			{
				byte b = (byte)UnityEngine.Random.Range(0, 2);
				byte b2 = (byte)UnityEngine.Random.Range(0, 2);
				base.photonView.RPC("OffsetDebugArea", PhotonTargets.All, b, b2);
			}
		}

		[PunRPC]
		public void OffsetDebugArea(byte moveSide, byte plusminus)
		{
			Debug.Log("Offesrt debug area " + moveSide + "--" + plusminus);
			if (_fogArea != null)
			{
				Vector3 localPosition = _fogArea.localPosition;
				if (moveSide == 0)
				{
					if (plusminus == 0)
					{
						localPosition.x += BattleRoyaleSetupOptions.DefaultFogWaveStep;
					}
					else
					{
						localPosition.x -= BattleRoyaleSetupOptions.DefaultFogWaveStep;
					}
				}
				else if (plusminus == 0)
				{
					localPosition.z += BattleRoyaleSetupOptions.DefaultFogWaveStep;
				}
				else
				{
					localPosition.z -= BattleRoyaleSetupOptions.DefaultFogWaveStep;
				}
				_nextFogArea.localPosition = localPosition;
				_currentSafityZone.localPosition = localPosition;
			}
			if (!BattleRoyaleGameManager.I.IsLobby())
			{
				BattleRoyaleGameManager.I.StartShowGasInfoMessage(GetTimeForNexFogWave());
			}
			if (PhotonNetwork.isMasterClient)
			{
				BattleRoyaleTimeManager.I.CallStartTimer(GetTimeForNexFogWave(), TimerCurrentTask.WaitForGasRelease);
			}
		}

		public IEnumerator SetupCurrentSafetyZone()
		{
			Debug.Log("SetupCurrentSafetyZone");
			if (PhotonNetwork.room == null)
			{
				yield break;
			}
			if (!PhotonNetwork.room.CustomProperties.ContainsKey("sz"))
			{
				if (PhotonNetwork.isMasterClient)
				{
					CurrentSafetyZoneNumber = UnityEngine.Random.Range(0, _safetyZones.Length);
					_currentSafityZone = _safetyZones[CurrentSafetyZoneNumber];
					ExitGames.Client.Photon.Hashtable sz = new ExitGames.Client.Photon.Hashtable { { "sz", CurrentSafetyZoneNumber } };
					PhotonNetwork.room.SetCustomProperties(sz);
				}
				else
				{
					yield return new WaitForSeconds(2f);
					StartCoroutine("SetupCurrentSafetyZone");
					Debug.Log("Error room key null and im not master");
				}
			}
			else
			{
				CurrentSafetyZoneNumber = (int)PhotonNetwork.room.CustomProperties["sz"];
				_currentSafityZone = _safetyZones[CurrentSafetyZoneNumber];
			}
			if (_currentSafityZone != null)
			{
				CalculateNextFogSidePosition(_currentSafityZone.localPosition, true);
			}
		}

		private void SetStartWaveStep()
		{
			СurrentFogWaveStep = DistanceBetweenTwoFogSides() / 4f;
		}

		private float DistanceBetweenTwoFogSides()
		{
			return Vector3.Distance(_leftSide.position, _rightSide.position);
		}

		public bool IfObjectInsideTheGasArea(Transform objectForCheck)
		{
			bool result = true;
			if (objectForCheck.position.z < _topSide.position.z && objectForCheck.position.z > _bottomSide.position.z && objectForCheck.position.x > _leftSide.position.x && objectForCheck.position.x < _rightSide.position.x)
			{
				result = false;
			}
			return result;
		}

		public Vector3 GetFogSidePos(FogSide side)
		{
			Vector3 result = Vector3.zero;
			switch (side)
			{
			case FogSide.Left:
				result = _leftSide.position;
				break;
			case FogSide.Right:
				result = _rightSide.position;
				break;
			case FogSide.Top:
				result = _topSide.position;
				break;
			case FogSide.Bottom:
				result = _bottomSide.position;
				break;
			case FogSide.Center:
				result = _fogAreaCenter.position;
				break;
			}
			return result;
		}

		public int GetTimeForNexFogWave()
		{
			return BattleRoyaleSetupOptions.FogConfigArray[_fogWaveNumber];
		}

		private IEnumerator PlaySirenaSound()
		{
			_sirenaSound.enabled = true;
			yield return new WaitForSeconds(15f);
			_sirenaSound.enabled = false;
		}
	}
}
