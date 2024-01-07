using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public class PlayerPhotonSectorController : Photon.MonoBehaviour
{
	[SerializeField]
	private PhotonMan _player;

	[SerializeField]
	private float _checkInterval = 2f;

	[SerializeField]
	private float _changeSectorWaitTime = 3f;

	private PhotonSectorInfo _mySector;

	private void Start()
	{
		if (DataKeeper.GameType == GameType.Tutorial)
		{
			_checkInterval = 1f;
			_changeSectorWaitTime = 2f;
		}
		if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars)
		{
			_checkInterval = 1f;
			_changeSectorWaitTime = 2f;
		}
		if (base.photonView.isMine)
		{
			StartCoroutine(CheckSector());
		}
	}

	public PhotonPlayer[] GetNearPhotonPlayers()
	{
		PhotonPlayer[] array = null;
		if (_mySector != null)
		{
			List<PhotonMan> allPlayersInNeighborsArea = _mySector.GetAllPlayersInNeighborsArea();
			if (allPlayersInNeighborsArea.Count > 0)
			{
				array = new PhotonPlayer[allPlayersInNeighborsArea.Count];
				for (int i = 0; i < allPlayersInNeighborsArea.Count; i++)
				{
					array[i] = allPlayersInNeighborsArea[i].photonView.owner;
				}
			}
		}
		return array;
	}

	public PhotonPlayer[] GetNearPhotonPlayers(Vector3 positionToCheck, float distance)
	{
		PhotonPlayer[] result = null;
		if (_mySector != null)
		{
			List<PhotonMan> allPlayersInNeighborsArea = _mySector.GetAllPlayersInNeighborsArea();
			if (allPlayersInNeighborsArea.Count > 0)
			{
				List<PhotonPlayer> list = new List<PhotonPlayer>();
				for (int i = 0; i < allPlayersInNeighborsArea.Count; i++)
				{
					if (Vector3.Distance(positionToCheck, allPlayersInNeighborsArea[i].transform.position) < distance)
					{
						list.Add(allPlayersInNeighborsArea[i].photonView.owner);
					}
				}
				result = list.ToArray();
			}
		}
		return result;
	}

	public PhotonPlayer[] GetFarPhotonPlayers()
	{
		PhotonPlayer[] array = null;
		if (_mySector != null)
		{
			List<PhotonMan> nearPlayers = _mySector.GetAllPlayersInNeighborsArea();
			List<PhotonMan> list = new List<PhotonMan>(WorldController.I.WorldPlayers.Values);
			List<PhotonMan> list2 = list.FindAll((PhotonMan player) => !nearPlayers.Contains(player));
			if (list2.Count > 0)
			{
				array = new PhotonPlayer[nearPlayers.Count];
				for (int i = 0; i < nearPlayers.Count; i++)
				{
					array[i] = nearPlayers[i].photonView.owner;
				}
			}
		}
		return array;
	}

	private IEnumerator CheckSector()
	{
		PhotonSectorInfo newSector = PhotonSectorService.I.GetSector(GameControls.I.Player.transform.position);
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
			base.photonView.RPC("PlayerChangeSector", PhotonTargets.All, newSector.RowIndex, newSector.ColumnIndex);
		}
	}

	private void OnDestroy()
	{
		if (_mySector != null)
		{
			_mySector.RemovePlayer(_player);
			PhotonSectorService.I.OnPlayerChangeSector(_mySector, null, base.photonView.isMine);
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (base.photonView.isMine && _mySector != null)
		{
			base.photonView.RPC("PlayerChangeSector", player, _mySector.RowIndex, _mySector.ColumnIndex);
		}
	}

	[PunRPC]
	private void PlayerChangeSector(byte row, byte column)
	{
		if (_mySector != null)
		{
			_mySector.RemovePlayer(_player);
		}
		PhotonSectorInfo sector = PhotonSectorService.I.GetSector(row, column);
		if (PlayerChangeSectorListener.I != null)
		{
			PlayerChangeSectorListener.I.OnPlayerSectorChange(_player, _mySector, sector);
		}
		if (sector != null)
		{
			sector.AddPlayer(_player);
			PhotonSectorService.I.OnPlayerChangeSector(_mySector, sector, base.photonView.isMine);
			_mySector = sector;
		}
		if (base.photonView.isMine)
		{
			GameUIController.I.Map.OnPlayerSectorChanged(row, column);
		}
		CheckDistance(sector.RowIndex, sector.ColumnIndex);
	}

	public void CheckDistance(int checkPlayerRowIndex, int checkPlayerColumnIndex)
	{
		if (WorldController.I.Player != null && WorldController.I.Player.SectorController != null && WorldController.I.Player.SectorController._mySector != null)
		{
			byte b = 2;
			int num = Mathf.Abs(WorldController.I.Player.SectorController._mySector.RowIndex - checkPlayerRowIndex);
			int num2 = Mathf.Abs(WorldController.I.Player.SectorController._mySector.ColumnIndex - checkPlayerColumnIndex);
			if (num2 >= b || num >= b)
			{
				_player.ToggleAnimator(false);
			}
			else
			{
				_player.ToggleAnimator(true);
			}
		}
	}
}
