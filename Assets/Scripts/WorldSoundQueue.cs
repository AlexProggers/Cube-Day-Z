using System;
using System.Collections.Generic;
using UnityEngine;

public enum WorldSoundType
{
	ZombieAttack = 0,
	ZombieCalm = 1,
	ZombieDeath = 2,
	ZombieGrowl = 3,
	ZombieHurt = 4
}

public class WorldSoundQueue : MonoBehaviour
{
	public static WorldSoundQueue I;

	[SerializeField]
	private float _playSoundsTimeout = 1f;

	private Dictionary<WorldSoundType, float> _worldSounds = new Dictionary<WorldSoundType, float>();

	private void Awake()
	{
		I = this;
		Array values = Enum.GetValues(typeof(WorldSoundType));
		foreach (object item in values)
		{
			_worldSounds.Add((WorldSoundType)(int)item, 0f);
		}
	}

	public bool CanPlaySound(WorldSoundType soundType)
	{
		if (_worldSounds.ContainsKey(soundType))
		{
			return Time.time - _worldSounds[soundType] > 0f;
		}
		return true;
	}

	public void AddSoundPlayedTime(WorldSoundType soundType)
	{
		if (_worldSounds.ContainsKey(soundType))
		{
			_worldSounds[soundType] = Time.time + _playSoundsTimeout;
		}
	}
}
