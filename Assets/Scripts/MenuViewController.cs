using System.Text;
using BattleRoyale;
using CodeStage.AdvancedFPSCounter;
using UnityEngine;

public class MenuViewController : MonoBehaviour
{
	[SerializeField]
	private ShadowText _staticticsText;

	[SerializeField]
	private ShadowText _staticticsValues;

	[SerializeField]
	private GameObject _statisticsView;

	[SerializeField]
	private GameObject _settingsView;

	[SerializeField]
	private GameObject _buttons;

	[SerializeField]
	private tk2dUIToggleButtonGroup _qualityLvl;

	[SerializeField]
	private tk2dUIToggleButton _profileInfoToggle;

	public GameObject SuicideBtn;

	public GameObject BattleRoayleExitMenu;

	public static MenuViewController I;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
		_qualityLvl.SelectedIndex = QualitySettings.GetQualityLevel();
	}

	private void OnEnable()
	{
		_statisticsView.SetActive(true);
		_settingsView.SetActive(false);
		UpdateStatistics();
		if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars)
		{
			SuicideBtn.SetActive(false);
		}
	}

	private void OnDisable()
	{
		BattleRoayleExitMenu.SetActive(false);
		_statisticsView.SetActive(true);
		_buttons.SetActive(true);
	}

	private void UpdateStatistics()
	{
		StringBuilder stringBuilder = new StringBuilder();

		if (DataKeeper.Language == Language.English) {
			stringBuilder.Append ("Zombie kills").Append ("\n").Append ("Creature kills")
				.Append ("\n")
				.Append ("Player kills")
				.Append ("\n")
				.Append ("\n")
				.Append ("Craft items")
				.Append ("\n")
				.Append ("Plans planted")
				.Append ("\n")
				.Append ("\n")
				.Append ("Die")
				.Append ("\n")
				.Append ("Suicide");
		}
		else if (DataKeeper.Language == Language.Russian) {
			stringBuilder.Append ("Убито зомби").Append ("\n").Append ("Убито существ")
				.Append ("\n")
				.Append ("Убито игроков")
				.Append ("\n")
				.Append ("\n")
				.Append ("Создано вещей")
				.Append ("\n")
				.Append ("Посажено растений")
				.Append ("\n")
				.Append ("\n")
				.Append ("Смерти")
				.Append ("\n")
				.Append ("Самоубийства");
		}

		_staticticsText.SetText(stringBuilder.ToString());

		StatisticsInfo playerStatistic = WorldController.I.Statistics;
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.Append(playerStatistic.ZombieKills).Append("\n").Append(playerStatistic.CreatureKills)
			.Append("\n")
			.Append(playerStatistic.PlayerKills)
			.Append("\n")
			.Append("\n")
			.Append(playerStatistic.CraftItems)
			.Append("\n")
			.Append(playerStatistic.PlantsPlanted)
			.Append("\n")
			.Append("\n")
			.Append(playerStatistic.Die)
			.Append("\n")
			.Append(playerStatistic.Suicide);
		_staticticsValues.SetText(stringBuilder2.ToString());
	}

	private void OnClickConsume()
	{
		GameUIController.I.ShowCharacterMenu(false, CharacterMenuType.Menu);
	}

	private void OnClickSettings()
	{
		_statisticsView.SetActive(false);
		_settingsView.SetActive(true);
	}

	private void OnQuailityChanged()
	{
		if (_qualityLvl.SelectedIndex >= 0 && QualitySettings.names.Length > _qualityLvl.SelectedIndex)
		{
			QualitySettings.SetQualityLevel(_qualityLvl.SelectedIndex);
		}

		PlayerPrefs.SetInt("QuailityLvl", QualitySettings.GetQualityLevel());
		PlayerPrefs.Save();
	}

	private void OnProfileToggleChanged()
	{
		if (_profileInfoToggle.IsOn)
		{
			AFPSCounter.Instance.OperationMode = OperationMode.Normal;
			PlayerPrefs.SetInt("AFPS", 1);
		}
		else
		{
			AFPSCounter.Instance.OperationMode = OperationMode.Disabled;
			PlayerPrefs.SetInt("AFPS", 0);
		}
		PlayerPrefs.Save();
	}

	private void OnClickSuicide()
	{
		if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
		{
			GameUIController.I.ShowCharacterMenu(false, CharacterMenuType.Menu);
			WorldController.I.Player.Suicide();
		}
	}

	private void OnClickExitBattleRoyaleMenu()
	{
		if (!BattleRoyaleGameManager.I.IsLobby())
		{
			BattleRoyaleGameManager.I.StartShowKillLog(string.Empty, PhotonNetwork.playerName, string.Empty, 0);
		}
		ExitFromGame();
	}

	private void OnClickExit()
	{
		if (DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
		{
			ExitFromGame();
		}
		else
		{
			ShowExitDialog();
		}
	}

	public void ShowExitDialog()
	{
		_statisticsView.SetActive(false);
		_buttons.SetActive(false);
		BattleRoayleExitMenu.SetActive(true);
	}

	public void ExitFromGame()
	{
		RespawnMenuController.SetDieFlagFalse();
		if (PhotonNetwork.room != null)
		{
			PhotonNetwork.LeaveRoom();
		}
	}

	private void OnLeftRoom()
	{
		PhotonNetwork.LoadLevel("MainMenu");
	}

	public void ExitFromServer()
	{
		if (PhotonNetwork.room != null)
		{
			PhotonNetwork.LeaveRoom();
		}
	}
}
