using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuConnectionViewController : MonoBehaviour
{
    public static MenuConnectionViewController I;

	public MenuConnectionController _connectionController;

	[SerializeField]
	private GameObject _mainMenu;

	public GameObject MainMenuContent;

	public GameObject MainMenuZombiez;

	[SerializeField]
	private GameObject _content;

	[SerializeField]
	private GameObject _waitingScreen;

	[SerializeField]
	public GameObject _failToConnectWindow;

	[SerializeField]
	private GameObject _failToCreateWindow;

	[SerializeField]
	private GameObject _chooseGameTypeMenu;

	[SerializeField]
	private GameObject _singleplayer;

    
	[SerializeField]
	private GameObject _multiplayer;

    [HideInInspector]
	public string _joinRoomName;

	[HideInInspector]
	public bool _isRandomRoom;

	public GameObject ChooseGameTypeMenu
	{
		get
		{
			return _chooseGameTypeMenu;
		}
	}

    private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	public void HideMainMenu(bool flag)
	{
		if (_mainMenu != null)
		{
			_mainMenu.SetActive(!flag);
		}
	}

    public void ShowWaitingScreen(bool show)
	{
		_content.SetActive(!show);
		_waitingScreen.SetActive(show);
	}

	private void OnClickSingleplayer()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		_chooseGameTypeMenu.SetActive(false);
		_singleplayer.SetActive(true);
	}

	private void OnClickMultiplayer()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		_chooseGameTypeMenu.SetActive(false);
		_multiplayer.SetActive(true);
	}

	private void BackToChooseMenu()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		_singleplayer.SetActive(false);
		_multiplayer.SetActive(false);
		_chooseGameTypeMenu.SetActive(true);
	}

    private void BackToMainMenu()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		_chooseGameTypeMenu.SetActive(false);
		_mainMenu.SetActive(true);
	}

	private void StartTutorial()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
        ShowWaitingScreen(true);
		DataKeeper.GameType = GameType.Tutorial;
		_connectionController.StartOfflinePlay();
	}

	private void ContinueSingleplayer()
	{
		DataKeeper.IsSkyWarsClick = false;
		DataKeeper.IsBattleRoyaleClick = false;
		ShowWaitingScreen(true);
		DataKeeper.GameType = GameType.Single;
		_connectionController.StartSinglePlayer();
	}

    private void StartNewSingleplayer()
	{
		DataKeeper.IsSkyWarsClick = false;
		DataKeeper.IsBattleRoyaleClick = false;
		_isRandomRoom = false;
		ShowWaitingScreen(true);
		DataKeeper.GameType = GameType.Single;
		_connectionController.StartSinglePlayer();
	}

    public void ConnectToRoom(string roomName)
	{
		_joinRoomName = roomName;
		_isRandomRoom = false;
		ShowWaitingScreen(true);
		DataKeeper.GameType = GameType.Multiplayer;
		_connectionController.JoinRoom(roomName, false);
	}

	private void ConnectToRandomRoom()
	{
		DataKeeper.IsSkyWarsClick = false;
		DataKeeper.IsBattleRoyaleClick = false;
		_isRandomRoom = true;
		ShowWaitingScreen(true);
		DataKeeper.GameType = GameType.Multiplayer;
		_connectionController.JoinRandomRoom();
	}

	private void OnPhotonCreateRoomFailed()
	{
		ShowWaitingScreen(false);
		Debug.Log("FAIL TO CREATE ROOM!");
		_failToCreateWindow.SetActive(true);
	}
}