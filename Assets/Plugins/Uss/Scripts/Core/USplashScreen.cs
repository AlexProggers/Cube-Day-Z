using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USplashScreen : MonoBehaviour
{
	[Serializable]
	public class USS
	{
		public Texture2D SplashLogo;

		public Rect CustomRect = new Rect(Screen.width / 2 - 200, Screen.height / 2 - 200, 400f, 400f);

		[Header("Time to Show")]
		[Range(1f, 10f)]
		public float m_time = 1f;

		[Range(1f, 15f)]
		public float m_fadeinspeed = 2f;

		[Range(1f, 15f)]
		public float m_fadeoutspeed = 2f;

		[Range(0.1f, 5f)]
		public float m_alpha = 2f;

		[Range(1f, 5f)]
		public float m_TimeForNext = 1f;

		[Header("Audio")]
		public AudioClip m_splashIn;

		public AudioClip m_splashOut;

		[Range(0f, 1f)]
		public float m_volume;

		[Range(0f, 2f)]
		public float m_pitch;

		public bool isFullScreen = true;

		[HideInInspector]
		public bool m_show;
	}

	public string m_NextLevel = "MainMenu";

	public float StartWait = 0.5f;

	public List<USS> m_uss = new List<USS>();

	private int t_current;

	private void Start()
	{
		StartCoroutine(SplashCorrutine());
	}

	private void OnGUI()
	{
		for (int i = 0; i < m_uss.Count; i++)
		{
			if (m_uss[i].m_show)
			{
				if (m_uss[i].isFullScreen)
				{
					GUI.color = new Color(1f, 1f, 1f, m_uss[i].m_alpha);
					GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), m_uss[i].SplashLogo);
					GUI.color = Color.white;
				}
				else
				{
					GUI.color = new Color(1f, 1f, 1f, m_uss[i].m_alpha);
					GUI.DrawTexture(new Rect((float)(Screen.width / 2) + m_uss[i].CustomRect.x, (float)(Screen.height / 2) + m_uss[i].CustomRect.y, m_uss[i].CustomRect.width, m_uss[i].CustomRect.height), m_uss[i].SplashLogo);
					GUI.color = Color.white;
				}
			}
		}
	}

	private IEnumerator SplashCorrutine()
	{
		yield return new WaitForSeconds(StartWait);
		for (int i = 0; i < m_uss.Count; i++)
		{
			float t_alpha = m_uss[t_current].m_alpha;
			m_uss[t_current].m_alpha = 0f;
			m_uss[t_current].m_show = true;
			if (m_uss[t_current].m_splashIn != null)
			{
				PlayAudioClip(m_uss[t_current].m_splashIn, m_uss[t_current].m_volume, m_uss[t_current].m_pitch);
			}
			while (m_uss[t_current].m_alpha < t_alpha)
			{
				m_uss[t_current].m_alpha += Time.deltaTime * m_uss[t_current].m_fadeinspeed;
				yield return 0;
			}
			yield return new WaitForSeconds(m_uss[i].m_time);
			if (m_uss[t_current].m_splashOut != null)
			{
				PlayAudioClip(m_uss[t_current].m_splashOut, m_uss[t_current].m_volume, m_uss[t_current].m_pitch);
			}
			while (m_uss[t_current].m_alpha > 0f)
			{
				m_uss[t_current].m_alpha -= Time.deltaTime * m_uss[t_current].m_fadeoutspeed;
				yield return 0;
			}
			yield return new WaitForSeconds(m_uss[t_current].m_TimeForNext);
			if (t_current >= m_uss.Count - 1)
			{
				yield return new WaitForSeconds(1f);
				Application.LoadLevel(m_NextLevel);
			}
			else
			{
				t_current++;
			}
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
		UnityEngine.Object.Destroy(gameObject, clip.length);
		return audioSource;
	}
}
