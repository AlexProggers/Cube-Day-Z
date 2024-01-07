using System;
using System.Collections.Generic;
using UnityEngine;

public class MobPullingSystem : MonoBehaviour
{
	private const float SendInfoWaitTime = 0.1f;

	private Dictionary<ZombieType, List<PhotonMob>> _activeMobs = new Dictionary<ZombieType, List<PhotonMob>>();

	private Dictionary<ZombieType, List<PhotonMob>> _inactiveMobs = new Dictionary<ZombieType, List<PhotonMob>>();

	private List<Action<int>> _pullingActivateActions = new List<Action<int>>();

	private float _lastSendTime;

	public static MobPullingSystem Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	public void ClearAllInfo()
	{
		_activeMobs.Clear();
		_inactiveMobs.Clear();
		_pullingActivateActions.Clear();
		_lastSendTime = 0f;
	}

	public int GetAllMobsCountByType(ZombieType type)
	{
		int num = 0;
		if (_activeMobs.ContainsKey(type))
		{
			num += _activeMobs[type].Count;
		}
		if (_inactiveMobs.ContainsKey(type))
		{
			num += _inactiveMobs[type].Count;
		}
		return num;
	}

	public void FirstAddMobToList(PhotonMob mob)
	{
		if (_inactiveMobs.ContainsKey(mob.MobType))
		{
			if (!_inactiveMobs[mob.MobType].Contains(mob))
			{
				_inactiveMobs[mob.MobType].Add(mob);
			}
		}
		else
		{
			_inactiveMobs.Add(mob.MobType, new List<PhotonMob> { mob });
		}
	}

	public void ReactivateMob(string mobId, Vector3 position)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		Mob mobInfo = DataKeeper.Info.GetMobInfo(mobId);
		if (mobInfo == null || !_inactiveMobs.ContainsKey(mobInfo.Type) || _inactiveMobs[mobInfo.Type].Count <= 0)
		{
			return;
		}
		_pullingActivateActions.Add(delegate(int i)
		{
			if (_inactiveMobs[mobInfo.Type].Count > i)
			{
				_inactiveMobs[mobInfo.Type][i].ActivateMobByMaster(true, position, mobInfo.Id);
			}
		});
	}

	public void ActivateMob(PhotonMob mob)
	{
		if (mob.MobIsActive)
		{
			return;
		}
		if (_inactiveMobs.ContainsKey(mob.MobType) && _inactiveMobs[mob.MobType].Contains(mob))
		{
			_inactiveMobs[mob.MobType].Remove(mob);
		}
		if (_activeMobs.ContainsKey(mob.MobType))
		{
			if (!_activeMobs[mob.MobType].Contains(mob))
			{
				_activeMobs[mob.MobType].Add(mob);
			}
		}
		else
		{
			_activeMobs.Add(mob.MobType, new List<PhotonMob> { mob });
		}
		WorldController.I.ActiveMobCount++;
	}

	public void DeactivateMob(PhotonMob mob)
	{
		if (!mob.MobIsActive)
		{
			return;
		}
		if (_activeMobs.ContainsKey(mob.MobType) && _activeMobs[mob.MobType].Contains(mob))
		{
			_activeMobs[mob.MobType].Remove(mob);
		}
		if (_inactiveMobs.ContainsKey(mob.MobType))
		{
			if (!_inactiveMobs[mob.MobType].Contains(mob))
			{
				_inactiveMobs[mob.MobType].Add(mob);
			}
		}
		else
		{
			_inactiveMobs.Add(mob.MobType, new List<PhotonMob> { mob });
		}
		WorldController.I.ActiveMobCount--;
	}

	private void Update()
	{
		if (_pullingActivateActions.Count > 0 && Time.time - _lastSendTime >= 0.1f)
		{
			Action<int> action = _pullingActivateActions[0];
			if (action != null)
			{
				action(0);
				_pullingActivateActions.Remove(action);
			}
			_lastSendTime = Time.time;
		}
	}
}
