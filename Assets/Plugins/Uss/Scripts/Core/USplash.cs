using System.Collections;
using UnityEngine;

public class USplash : MonoBehaviour
{
	public float WaitStart = 0.5f;

	public Animation m_animation;

	public string ShowAnimation = "Show";

	public string HideAnimation = "Hide";

	[Range(0.1f, 10f)]
	public float ShowAnimSpeed = 1f;

	[Range(0.1f, 10f)]
	public float HideAnimSpeed = 1f;

	public AudioClip ShowSound;

	public AudioClip HideSound;

	[Range(0f, 1f)]
	public float m_volume;

	[Range(0f, 2f)]
	public float m_pitch = 1f;

	private void OnEnable()
	{
		StartCoroutine(ShowCorrutine());
	}

	public void Hide()
	{
		StartCoroutine(HideCorrutine());
		if ((bool)HideSound)
		{
			PlayAudioClip(HideSound, m_volume, m_pitch);
		}
	}

	private IEnumerator ShowCorrutine()
	{
		if (WaitStart > 0f)
		{
			yield return new WaitForSeconds(WaitStart);
		}
		if (m_animation != null)
		{
			m_animation[ShowAnimation].speed = ShowAnimSpeed;
			m_animation.Play(ShowAnimation);
		}
		if ((bool)ShowSound)
		{
			PlayAudioClip(ShowSound, m_volume, m_pitch);
		}
	}

	private IEnumerator HideCorrutine()
	{
		if (m_animation != null)
		{
			m_animation[HideAnimation].speed = HideAnimSpeed;
			m_animation.Play(HideAnimation);
			yield return new WaitForSeconds(m_animation[HideAnimation].length);
			base.gameObject.SetActive(false);
		}
	}

	private AudioSource PlayAudioClip(AudioClip clip, float volume, float pitch)
	{
		GameObject gameObject = new GameObject("One shot audio");
		if (Camera.main != null)
		{
			gameObject.transform.position = Camera.main.transform.position;
		}
		else
		{
			gameObject.transform.position = Camera.current.transform.position;
		}
		AudioSource audioSource = gameObject.AddComponent<AudioSource>();
		audioSource.clip = clip;
		audioSource.volume = volume;
		audioSource.pitch = pitch;
		audioSource.Play();
		Object.Destroy(gameObject, clip.length);
		return audioSource;
	}
}
