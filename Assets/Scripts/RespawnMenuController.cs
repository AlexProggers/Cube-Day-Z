using System.Collections.Generic;
using UnityEngine;

public class RespawnMenuController : MonoBehaviour
{
	[SerializeField]
	private tk2dTextMesh _dieMessage;

	[SerializeField]
	private GameObject _respawnAtBedBtn;

	[SerializeField]
	private GameObject _respawnBtn;

	private bool _canRespawn = true;

	private bool _respawnAtBed;

	public void SetKillerName(string killerName, string weaponId)
	{
		if (!string.IsNullOrEmpty(killerName))
		{
			Weapon weapon = (string.IsNullOrEmpty(weaponId) ? null : DataKeeper.Info.GetWeaponInfo(weaponId));
			if (DataKeeper.Language == Language.Russian)
			{
				_dieMessage.text = "Вас убил(а): " + killerName + "\nОружие: " + ((weapon == null) ? "Кулаки" : weapon.RussianName);
			}
			else
			{
				_dieMessage.text = "Killed by: " + killerName + "\nWeapon: " + ((weapon == null) ? "Fists" : weapon.Name);
			}
		}
		else
		{
			_dieMessage.text = string.Empty;
		}
	}

	private void OnEnable()
	{
		_respawnAtBedBtn.SetActive(PlayerSpawnsController.I.CanRespawnAtBed);
	}

	private void Respawn()
	{
		if (_respawnAtBed)
		{
			PlayerSpawnsController.I.RespawnAtBed();
		}
		else
		{
			PlayerSpawnsController.I.Respawn(false);
		}
	}

	public static void SetDieFlagFalse()
	{
		if (PlayerPrefs.HasKey("Die"))
		{
			PlayerPrefs.SetString("Die", "false");
		}
	}

	private void OnClickRespawn()
	{
		if (_canRespawn)
		{
			SetDieFlagFalse();
			_canRespawn = false;
			_respawnAtBed = false;
		}
	}

	private void OnClickRespawnAtBed()
	{
		if (_canRespawn)
		{
			SetDieFlagFalse();
			_canRespawn = false;
			_respawnAtBed = true;
		}
	}

	private void OnDisable()
	{
		_canRespawn = true;
	}

    private void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MainMenu");
    }

    public void OnClickExitBattleRoyale()
	{
		if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars)
		{
			SetDieFlagFalse();

			if (PhotonNetwork.room != null)
			{
				PhotonNetwork.LeaveRoom();
			}
		}
	}
}
