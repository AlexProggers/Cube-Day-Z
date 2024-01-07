using System.Collections;
using UnityEngine;

namespace BattleRoyale
{
	public class BattleRoyaleSoundManager : MonoBehaviour
	{
		public AudioClip Parachute;

		public AudioClip StartTrack;

		public AudioClip AfterGas;

		public AudioClip Final;

		public AudioSource _audioSource;

		public AudioClip[] CountDownPrase;

		public static BattleRoyaleSoundManager I;

		private bool isplay;

		private void Awake()
		{
			if (I == null)
			{
				I = this;
			}
		}

		public void PlayParachuteTrack()
		{
			_audioSource.clip = Parachute;
			_audioSource.Play();
		}

		public void PlayStartTrack()
		{
			_audioSource.clip = StartTrack;
			_audioSource.Play();
		}

		public void PlayAfterGastTrack()
		{
			_audioSource.clip = AfterGas;
			_audioSource.Play();
		}

		public void PlayFinalGastTrack()
		{
			_audioSource.clip = Final;
			_audioSource.Play();
		}

		public void StartCountDown()
		{
			if (!isplay)
			{
				isplay = true;
				StartCoroutine("PlayCountDown");
			}
		}

		private IEnumerator PlayCountDown()
		{
			_audioSource.loop = false;
			for (int i = 10; i > 0; i--)
			{
				_audioSource.clip = CountDownPrase[i - 1];
				_audioSource.Play();
				yield return new WaitForSeconds(1f);
			}
			_audioSource.loop = true;
		}
	}
}
