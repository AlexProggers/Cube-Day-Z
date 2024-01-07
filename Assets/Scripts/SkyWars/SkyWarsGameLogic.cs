using System.Collections;
using BattleRoyale;
using Photon;
using UnityEngine;

namespace SkyWars
{
	public class SkyWarsGameLogic : Photon.MonoBehaviour
	{
		public static SkyWarsGameLogic I;

		[SerializeField]
		private AudioSource _sirenaSound;

		[SerializeField]
		private GameObject _redButton;

		[SerializeField]
		private Transform[] _crateSpawnPoints;

		[SerializeField]
		private SkyWarsBreageMoving[] _bridges;

		[SerializeField]
		private SkyWarsIsland[] _islands;

		public void Awake()
		{
			if (I == null)
			{
				I = this;
			}
		}

		public void CallStartBridgesMoving()
		{
			StartCoroutine("StartBridgesMoving");
		}

		private IEnumerator StartBridgesMoving()
		{
			yield return new WaitForSeconds(SkyWarsSetupOptions.BridgeDelayBeforeStart);
			for (int i = 0; i < _bridges.Length; i++)
			{
				_bridges[i].enabled = true;
			}
		}

		public void OnRoundEnd()
		{
			WorldController.I.Player.Suicide();
			Debug.Log("Skywars Round END");
			BattleRoyaleGameManager.I.CalculateMyFinalePlace();
		}

		public void StartDestroyIslands()
		{
			base.photonView.RPC("DestroyIslands", PhotonTargets.All);
		}

		public void CallShowRedBtn()
		{
			if (PhotonNetwork.isMasterClient)
			{
				base.photonView.RPC("ShowRedBtn", PhotonTargets.All);
			}
		}

		[PunRPC]
		public void ShowRedBtn()
		{
			_redButton.SetActive(true);
			if (PhotonNetwork.isMasterClient)
			{
				BattleRoyaleTimeManager.I.CallStartTimer(SkyWarsSetupOptions.RoundDuration, TimerCurrentTask.SkyWarsEndRound);
			}
		}

		[PunRPC]
		public void DestroyIslands()
		{
			StartCoroutine("WaitAndDestroy");
		}

		private IEnumerator WaitAndDestroy()
		{
			StartCoroutine("PlaySirenaSound");
			yield return new WaitForSeconds(2f);
			for (int i = 0; i < _islands.Length; i++)
			{
				_islands[i].DestroyIsland();
			}
			Debug.Log("DestroyIslands!");
		}

		private IEnumerator PlaySirenaSound()
		{
			_sirenaSound.enabled = true;
			yield return new WaitForSeconds(15f);
			_sirenaSound.enabled = false;
		}
	}
}
