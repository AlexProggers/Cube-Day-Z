using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControls : MonoBehaviour
{
    public static GameControls I;

	[SerializeField]
	private GameObject _audioDummy;

	[SerializeField]
	private List<GameObject> _water;

	[SerializeField]
	private GameObject _weather;

	private FPSPlayer _fpsPlayer;

	private FPSRigidBodyWalker _fpsRigidBodyWalker;

	private SmoothMouseLook _mouseLook;

	public Transform PlayerTransform;

	public FPSPlayer Player
	{
		get
		{
			return _fpsPlayer;
		}
	}

	public PlayerWeapons PlayerWeapons
	{
		get
		{
			return _fpsPlayer.PlayerWeapons;
		}
	}

	public FPSRigidBodyWalker Walker
	{
		get
		{
			return _fpsRigidBodyWalker;
		}
	}

	private void Awake()
	{
		I = this;
	}

    public void Initialize()
	{
		_fpsPlayer = Object.FindObjectOfType<FPSPlayer>();
		_fpsRigidBodyWalker = Object.FindObjectOfType<FPSRigidBodyWalker>();
		_mouseLook = Object.FindObjectOfType<SmoothMouseLook>();

		foreach (GameObject item in _water)
		{
			if (item != null)
			{
				item.SetActive(true);
			}
		}

		_audioDummy.SetActive(false);

		_fpsPlayer.Initialize();
		_fpsPlayer.PlayerWeapons = _fpsPlayer.weaponObj.GetComponent<PlayerWeapons>();
		_fpsPlayer.PlayerWeapons.Initialize();

		_fpsPlayer.CanUseInput = true;
		_fpsRigidBodyWalker.CanUseInput = true;
		_weather.SetActive(true);
	}

    public void FPSGameControls(bool enable)
	{
		_fpsPlayer.PlayerWeapons.CanShoot = enable;
		_fpsPlayer.CrosshairGuiObjInstance.SetActive(enable);
		_fpsPlayer.CanUseInput = enable;
		_fpsRigidBodyWalker.CanUseInput = enable;
		_mouseLook.enabled = enable;
	}

	public void MenuControls(bool enable, bool enablePlayerCollision = true)
	{
		if (Player.FpsCamera.activeSelf || Player.FpsWeapons.activeSelf)
		{
			FPSGameControls(!enable);
			if (!enablePlayerCollision)
			{
				Player.GetComponent<Rigidbody>().velocity = Vector3.zero;
				Player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				Player.GetComponent<Rigidbody>().Sleep();
				Player.GetComponent<Rigidbody>().sleepThreshold = 0f;
				Player.GetComponent<Rigidbody>().isKinematic = true;
			}
			else
			{
				Player.GetComponent<Rigidbody>().WakeUp();
				Player.GetComponent<Rigidbody>().isKinematic = false;
			}
			if (enable)
			{
				Cursor.lockState = CursorLockMode.None;
			}
			else
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
			Cursor.visible = enable;
		}
	}
}
