using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuConnectionController : MonoBehaviour
{
    private const float TryReconnectCooldown = 3f;

	public static MenuConnectionController I;

	private bool _withZombies;

	private bool _withFriends;

	private bool _isRandomRoom;

	private bool _disconnectBySelf;

    public bool CanPlayNetworkGame
	{
		get
		{
			return PhotonNetwork.connected;
		}
	}

	private void Awake()
	{
		PhotonNetwork.playerName = "Quest" + UnityEngine.Random.Range(1, 9999);
		I = this;
		ConnectToPhoton();
	}

    private void OnJoinedLobby()
	{
		Debug.Log("Join to Lobby");
	}

    public void ConnectToPhoton()
	{
        DataKeeper.OfflineMode = false;

        if (!PhotonNetwork.connected || PhotonNetwork.offlineMode)
        {
            PhotonNetwork.offlineMode = false;

            PhotonNetwork.ConnectUsingSettings("UNT" + DataKeeper.BuildVersion);
        }
	}

    private void OnConnectedToMaster()
    {
        Debug.Log("Connect from Photon.");
    }

    private IEnumerator Reconnect()
	{
		yield return new WaitForSeconds(TryReconnectCooldown);
		PhotonNetwork.ConnectUsingSettings("UNT" + DataKeeper.BuildVersion);


	}

	private void OnDisconnectedFromPhoton()
	{
		DataKeeper.OfflineMode = true;
		Debug.LogError("Disconnect from Photon.");
		if (!_disconnectBySelf)
		{
			StartCoroutine("Reconnect");
		}
	}

    private void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		Debug.LogError("Fail to connect to Photon. Reason: " + cause);
		StartCoroutine("Reconnect");
	}

    private void OnJoinedRoom()
	{
		if (DataKeeper.GameType == GameType.Tutorial)
		{
			PhotonNetwork.LoadLevel("TutorialScene");
			return;
		}
		else if (DataKeeper.GameType == GameType.SkyWars)
		{
			PhotonNetwork.LoadLevel("UnturnedSkyWars");
		}
		else if (DataKeeper.GameType == GameType.Single || DataKeeper.GameType == GameType.Multiplayer || DataKeeper.GameType == GameType.BattleRoyale)
		{
			PhotonNetwork.LoadLevel("UnturnedGameMap");
		}
	}

    public void StartOfflinePlay()
	{
		_disconnectBySelf = true;
		_withZombies = true;
		CreateRoom(null, true, true, false);
	}

    public void StartSinglePlayer()
	{
		Debug.Log("Offline mode " + DataKeeper.OfflineMode);
		if (!DataKeeper.OfflineMode)
		{
			_withZombies = true;
			CreateRoom("single", true, true, false);
		}
		else
		{
			StartOfflinePlay();
		}
	}

    public void CreateRoom(string roomName, bool withZombies, bool single, bool isPrivate, byte maxPlayers = 1)
	{
		_withZombies = withZombies;
		RoomOptions roomOptions = new RoomOptions();
		roomOptions.MaxPlayers = maxPlayers;
		roomOptions.IsVisible = !single;
		roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
		roomOptions.CustomRoomProperties.Add("single", single);
		roomOptions.CustomRoomProperties.Add("zombies", _withZombies);
		roomOptions.CustomRoomProperties.Add("startTime", PhotonNetwork.time);
		roomOptions.CustomRoomProperties.Add("private", isPrivate);
		roomOptions.CustomRoomProperties.Add("creatorId", PhotonNetwork.playerName);
		roomOptions.CustomRoomPropertiesForLobby = new string[6] { "zombies", "single", "private", "creatorId", "softcap", "id" };
		PhotonNetwork.CreateRoom(roomName, roomOptions, PhotonNetwork.lobby);
	}

    public void JoinRandomRoom()
	{
		if (DataKeeper.GameType == GameType.BattleRoyale)
		{
			Debug.Log("Battle royale random room");
			return;
		}
		if (DataKeeper.GameType == GameType.SkyWars)
		{
			Debug.Log("Sky wars random room");
			return;
		}

		Debug.Log("Join random room! ");
		PhotonNetwork.JoinRandomRoom();
	}

    public void JoinRoom(string roomName, bool joinRandomIfFail)
	{
		_isRandomRoom = joinRandomIfFail;
		PhotonNetwork.JoinRoom(roomName);
	}
}
