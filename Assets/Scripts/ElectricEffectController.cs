using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ElectricEffectController : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem _particleSystem;

	[SerializeField]
	private List<AudioClip> _sounds;

	public void Play()
	{
		_particleSystem.Play();
		if (_sounds.Count > 0)
		{
			AudioClip clip = _sounds[UnityEngine.Random.Range(0, _sounds.Count)];
			GetComponent<AudioSource>().PlayOneShot(clip);
		}
	}
}
