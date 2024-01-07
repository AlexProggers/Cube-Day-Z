using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class USplashScreenUI : MonoBehaviour
{
	[Serializable]
	public class USS
	{
		public GameObject SplashUI;

		[Range(0.1f, 10f)]
		public float m_time = 2f;

		[Range(0.1f, 6f)]
		public float WaitForNext = 1f;
	}

	public string NextLevel = "MainMenu";

	public List<USS> m_uss = new List<USS>();

	[Space(5f)]
	public bool SkipWhenLoadLevel = true;

	public bool HideLoadingWhenLoad = true;

	[Range(1f, 15f)]
	public int TimeForSkip = 2;

	[Space(5f)]
	public GameObject SkipUI;

	public Image Black;

	public Slider ProgreesSlider;

	[Space(5f)]
	public USLoadingEffect Loading;

	private int current;

	private AsyncOperation async;

	private bool isDone;

	private bool _allSplashShown;

	private bool isPro
	{
		get
		{
			bool flag = false;
			return true;
		}
	}
	
	private bool _progress;

	private void Start()
	{
		if (isPro)
		{
			StartCoroutine(LevelProgress());
				
			if (!SkipWhenLoadLevel)
			{
				InvokeRepeating("CountSkip", 1f, 1f);
			}
		}
		else
		{
			ProgreesSlider.gameObject.SetActive(false);
			InvokeRepeating("CountSkip", 1f, 1f);
		}
		for (int i = 0; i < m_uss.Count; i++)
		{
			m_uss[i].SplashUI.SetActive(false);
		}
		StartCoroutine(SplashCorrutine());
		if (SkipUI != null && SkipUI.activeSelf)
		{
			SkipUI.SetActive(false);
		}
	}

	private IEnumerator SplashCorrutine()
	{
		for (int i = 0; i < m_uss.Count; i++)
		{
			m_uss[current].SplashUI.SetActive(true);
			yield return new WaitForSeconds(m_uss[i].m_time);
			if (i == m_uss.Count - 1)
			{
				yield return new WaitForSeconds(m_uss[current].WaitForNext);
				_allSplashShown = true;
				if (isPro)
				{
					if (async != null)
					{
						async.allowSceneActivation = true;
					}
				}
				else
				{
					StartCoroutine(LevelNext());
				}
			}
			else
			{
				m_uss[current].SplashUI.GetComponent<USplash>().Hide();
				yield return new WaitForSeconds(m_uss[current].WaitForNext);
				if (current < m_uss.Count - 1)
				{
					current++;
				}
			}
		}
	}

	private void Update()
	{
		ProgreesLoad();
	}

	private void ProgreesLoad()
	{
		if (!(ProgreesSlider != null) || async == null)
		{
			return;
		}

		float to = async.progress + 0.1f;
		ProgreesSlider.value = Mathf.Lerp(ProgreesSlider.value, to, Time.deltaTime * 2f);
		
		if ((async.isDone || ProgreesSlider.value >= 0.98f) && !isDone)
		{
			isDone = true;
			if (SkipWhenLoadLevel && SkipUI != null)
			{
				SkipUI.SetActive(true);
			}
			if (HideLoadingWhenLoad)
			{
				Loading.Loading = false;
			}
		}
	}

	public void Skip()
	{
	}

	private void CountSkip()
	{
		TimeForSkip--;
		if (TimeForSkip <= 0)
		{
			CancelInvoke("CountSkip");
			if (SkipUI != null)
			{
				SkipUI.SetActive(true);
			}
		}
	}

	private IEnumerator LevelProgress()
	{
		yield return new WaitForSeconds(5.0f);
		
		async = SceneManager.LoadSceneAsync(NextLevel);
		async.allowSceneActivation = _allSplashShown;
		_progress = true;
		
		yield return async;
	}
	
	private IEnumerator LevelNext()
	{
		yield return new WaitForSeconds(5.0f);
		_progress = true;
		SceneManager.LoadScene(NextLevel);
	}
}
