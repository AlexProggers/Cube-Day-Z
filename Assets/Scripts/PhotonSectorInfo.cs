using System.Collections.Generic;
using UnityEngine;

public class PhotonSectorInfo
{
	public Vector3 Position;

	public byte RowIndex;

	public byte ColumnIndex;

	private List<PhotonMan> _players;

	private List<PhotonMob> _mobs;

	private List<PhotonSpawnMobInfo> _spawnInfo;

	public List<PhotonSectorInfo> Neighbors;

	public bool HasPlayers
	{
		get
		{
			return _players.Count > 0;
		}
	}

	public bool Enabled { get; private set; }

	public int MobCount
	{
		get
		{
			return _spawnInfo.Count;
		}
	}

	public bool UseMobInterpolateSync { get; set; }

	public PhotonSectorInfo(Vector3 position, byte rowIndex, byte columnIndex)
	{
		_players = new List<PhotonMan>();
		_mobs = new List<PhotonMob>();
		_spawnInfo = new List<PhotonSpawnMobInfo>();
		RowIndex = rowIndex;
		ColumnIndex = columnIndex;
	}

	public void SetNeighbors(List<PhotonSectorInfo> neighbors)
	{
		Neighbors = neighbors;
	}

	public List<PhotonMan> GetAllPlayersInNeighborsArea()
	{
		List<PhotonMan> list = new List<PhotonMan>();
		foreach (PhotonSectorInfo neighbor in Neighbors)
		{
			list.AddRange(neighbor._players);
		}
		list.AddRange(_players);
		return list;
	}

	public void AddSpawnInfo(PhotonSpawnMobInfo spawnInfo)
	{
		_spawnInfo.Add(spawnInfo);
	}

	public void AddMob(PhotonMob mob)
	{
		if (!_mobs.Contains(mob))
		{
			mob.UseInterpolateSync(UseMobInterpolateSync);
			_mobs.Add(mob);
		}
		if (PhotonNetwork.isMasterClient && !Enabled && mob != null)
		{
			mob.ActivateMobByMaster(false, mob.transform.position);
		}
	}

	public void RemoveMob(PhotonMob mob)
	{
		if (_mobs.Contains(mob))
		{
			_mobs.Remove(mob);
		}
	}

	public void RemovePlayer(PhotonMan player)
	{
		if (_players.Contains(player))
		{
			_players.Remove(player);
		}
	}

	public void AddPlayer(PhotonMan player)
	{
		if (!_players.Contains(player))
		{
			_players.Add(player);
		}
	}

	private void CheckMobInterpolateSync()
	{
		foreach (PhotonMob mob in _mobs)
		{
			mob.UseInterpolateSync(UseMobInterpolateSync);
		}
	}

	public void Check(bool playerIsMine)
	{
		bool flag = HasPlayers;
		foreach (PhotonSectorInfo neighbor in Neighbors)
		{
			if (neighbor.HasPlayers)
			{
				flag = true;
				break;
			}
		}
		if (playerIsMine)
		{
			UseMobInterpolateSync = flag;
			CheckMobInterpolateSync();
		}
		if (flag)
		{
			EnableSector();
		}
		else
		{
			DisableSector();
		}
	}

	private void EnableSector()
	{
		if (Enabled)
		{
			return;
		}
		Enabled = true;
		if (PhotonNetwork.isMasterClient)
		{
			int playersInRoom = ((PhotonNetwork.room != null) ? (PhotonNetwork.room.PlayerCount - 1) : 0);
			int num = ZombieBalanceService.CallculateSectorZombiesLimit(WorldController.I.IsDay, WorldController.I.ActiveMobCount, _spawnInfo.Count, playersInRoom);
			for (int i = 0; i < num; i++)
			{
				MobPullingSystem.Instance.ReactivateMob(_spawnInfo[i].Id, _spawnInfo[i].Position);
			}
		}
	}

	private void DisableSector()
	{
		if (!Enabled)
		{
			return;
		}
		Enabled = false;
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		foreach (PhotonMob mob in _mobs)
		{
			if (mob != null)
			{
				mob.ActivateMobByMaster(false, mob.transform.position);
			}
		}
		_mobs.Clear();
	}
}
