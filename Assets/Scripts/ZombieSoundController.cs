using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSoundController : MonoBehaviour
{
	[SerializeField]
	private AudioSource _audio;

	[SerializeField]
	private float _zombieVoiceVolume;

	[SerializeField]
	private float _calmSoundMinTimeout = 3f;

	[SerializeField]
	private float _calmSoundMaxTimeout = 6f;

	[SerializeField]
	private List<AudioClip> _calmSounds;

	[SerializeField]
	private List<AudioClip> _attackSounds;

	[SerializeField]
	private List<AudioClip> _getHurtSounds;

	[SerializeField]
	private List<AudioClip> _findTargetSounds;

	[SerializeField]
	private List<AudioClip> _dieSounds;

	public bool Enabled { get; private set; }

	public void EnableZombieSounds(bool enable)
	{
		if (enable)
		{
			StartCoroutine(OnCalmState());
		}
		else
		{
			StopAllCoroutines();
		}
		Enabled = enable;
	}

	private IEnumerator OnCalmState()
	{
		while (true)
		{
			float waitTime = UnityEngine.Random.Range(_calmSoundMinTimeout, _calmSoundMaxTimeout);
			yield return new WaitForSeconds(waitTime);
			if (WorldSoundQueue.I.CanPlaySound(WorldSoundType.ZombieCalm))
			{
				int soundIndex = UnityEngine.Random.Range(0, _calmSounds.Count);
				_audio.PlayOneShot(_calmSounds[soundIndex]);
				WorldSoundQueue.I.AddSoundPlayedTime(WorldSoundType.ZombieCalm);
			}
		}
	}

	public void OnDeath()
	{
		if (Enabled && WorldSoundQueue.I.CanPlaySound(WorldSoundType.ZombieDeath))
		{
			int index = UnityEngine.Random.Range(0, _dieSounds.Count);
			AudioSource.PlayClipAtPoint(_dieSounds[index], base.transform.position);
			WorldSoundQueue.I.AddSoundPlayedTime(WorldSoundType.ZombieDeath);
		}
	}

	public void OnFindTarget()
	{
		if (Enabled && WorldSoundQueue.I.CanPlaySound(WorldSoundType.ZombieGrowl))
		{
			int index = UnityEngine.Random.Range(0, _findTargetSounds.Count);
			_audio.PlayOneShot(_findTargetSounds[index]);
			WorldSoundQueue.I.AddSoundPlayedTime(WorldSoundType.ZombieGrowl);
		}
	}

	public void OnAttack()
	{
		if (Enabled && WorldSoundQueue.I.CanPlaySound(WorldSoundType.ZombieAttack))
		{
			int index = UnityEngine.Random.Range(0, _attackSounds.Count);
			_audio.PlayOneShot(_attackSounds[index]);
			WorldSoundQueue.I.AddSoundPlayedTime(WorldSoundType.ZombieAttack);
		}
	}

	public void OnGetHurt()
	{
		if (Enabled && WorldSoundQueue.I.CanPlaySound(WorldSoundType.ZombieHurt))
		{
			int index = UnityEngine.Random.Range(0, _getHurtSounds.Count);
			_audio.PlayOneShot(_getHurtSounds[index]);
			WorldSoundQueue.I.AddSoundPlayedTime(WorldSoundType.ZombieHurt);
		}
	}
}
