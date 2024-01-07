using System.Collections;
using Photon;
using UnityEngine;

public class MobPhotonSectorController : Photon.MonoBehaviour
{
	[SerializeField]
	private PhotonMob _mob;

	[SerializeField]
	private float _checkInterval = 2f;

	[SerializeField]
	private float _changeSectorWaitTime = 3f;

	private PhotonSectorInfo _mySector;

	private void Start()
	{
		_mob.ReenableAI();
		if (PhotonNetwork.isMasterClient)
		{
			StartCoroutine(CheckSector());
		}
	}

	private void OnMasterClientSwitched()
	{
		StopAllCoroutines();
		_mob.ReenableAI();
		if (PhotonNetwork.isMasterClient)
		{
			StartCoroutine(CheckSector());
		}
	}

	private IEnumerator CheckSector()
	{
		PhotonSectorInfo newSector = ((!_mob.MobIsActive) ? null : PhotonSectorService.I.GetSector(base.transform.position));
		if (newSector != _mySector)
		{
			StartCoroutine(ChangeSector(newSector));
		}
		yield return new WaitForSeconds(_checkInterval);
		StartCoroutine(CheckSector());
	}

	private IEnumerator ChangeSector(PhotonSectorInfo newSector)
	{
		yield return new WaitForSeconds(_changeSectorWaitTime);
		if (newSector != _mySector)
		{
			if (newSector != null)
			{
				base.photonView.RPC("MobChangeSector", PhotonTargets.All, newSector.RowIndex, newSector.ColumnIndex);
			}
			else
			{
				base.photonView.RPC("MobResetSector", PhotonTargets.All);
			}
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient && _mySector != null && _mob.MobIsActive)
		{
			base.photonView.RPC("MobChangeSector", player, _mySector.RowIndex, _mySector.ColumnIndex);
		}
	}

	[PunRPC]
	private void MobResetSector()
	{
		if (_mySector != null)
		{
			_mySector.RemoveMob(_mob);
		}
		_mySector = null;
	}

	[PunRPC]
	private void MobChangeSector(byte row, byte column)
	{
		if (_mySector != null)
		{
			_mySector.RemoveMob(_mob);
		}
		PhotonSectorInfo photonSectorInfo = (_mySector = PhotonSectorService.I.GetSector(row, column));
		if (photonSectorInfo != null)
		{
			photonSectorInfo.AddMob(_mob);
		}
	}
}
