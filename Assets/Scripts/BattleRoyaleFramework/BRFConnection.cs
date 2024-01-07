using System;
using System.Collections.Generic;
using System.Linq;
using SkyWars;
using UnityEngine;

namespace BattleRoyaleFramework
{
	public class BRFConnection
	{
		public static void JoinOrCreateForGameMode(GameType gameMode, int maxPlr)
		{
			List<RoomInfo> availibleRoomsForGameMode = GetAvailibleRoomsForGameMode(gameMode);
			if (availibleRoomsForGameMode != null)
			{
				if (availibleRoomsForGameMode.Count > 0)
				{
					JoinRandomInGameMode(gameMode, availibleRoomsForGameMode);
				}
				else
				{
					CreateRoomForGameMode(gameMode, maxPlr);
				}

				Debug.Log(availibleRoomsForGameMode.Count);
			}
			else
			{
				CreateRoomForGameMode(gameMode, maxPlr);
			}
		}

		public static void CreateRoomForGameMode(GameType gameMode, int maxPlayers)
		{
			System.Random random = new System.Random();
			int num = random.Next(0, 99999);
			MenuConnectionController.I.CreateRoom(GetRoomPrefix(gameMode) + num, false, false, false, (byte)maxPlayers);
		}

		public static List<RoomInfo> GetAvailibleRoomsForGameMode(GameType gameMode)
		{
			List<RoomInfo> infos = new List<RoomInfo>();
			RoomInfo[] roomList = PhotonNetwork.GetRoomList();
			
			if (roomList != null)
			{
				foreach (RoomInfo roomInfo in roomList)
				{
					if (roomInfo.Name.Contains(GetRoomPrefix(gameMode)))
					{
						if (roomInfo.IsOpen)
						{
							infos.Add(roomInfo);
						}
					}
				}
			}

			return infos;
		}

		public static void JoinRandomInGameMode(GameType gameMode, List<RoomInfo> rooms)
		{
			Debug.Log("Try to joing random prefix = " + GetRoomPrefix(gameMode));
			int num = UnityEngine.Random.Range(0, rooms.Count);
			num = 0;
			num = SkyWarsConnectionController.I.ReconnectCount;
			if (num >= rooms.Count)
			{
				num = 0;
			}
			if (rooms[num] != null && rooms[num].open)
			{
				Debug.Log("Random tjoin to " + GetRoomPrefix(gameMode) + "--" + rooms[num].name + "-number=" + num);
				MenuConnectionController.I.JoinRoom(rooms[num].name, true);
			}
		}

		public static int GetPlayersCountInGameMode(GameType gameMode)
		{
			int num = 0;
			RoomInfo[] roomList = PhotonNetwork.GetRoomList();
			if (roomList != null)
			{
				RoomInfo[] array = roomList;
				foreach (RoomInfo roomInfo in array)
				{
					if (roomInfo.name.Contains(GetRoomPrefix(gameMode)))
					{
						num += roomInfo.playerCount;
					}
				}
			}
			return num;
		}

		private static string GetRoomPrefix(GameType gamemode)
		{
			string empty = string.Empty;
			switch (gamemode)
			{
			case GameType.BattleRoyale:
				return "hungrygame";
			case GameType.SkyWars:
				return "skywars";
			default:
				return string.Empty;
			}
		}

		public static void ExitGames()
		{
			RespawnMenuController.SetDieFlagFalse();

			WorldController.I.StopCoroutine("AddMobsForPulling");
			WorldController.I.StopCoroutine("AutoSaveMultiplayerWorld");

			if (PhotonNetwork.room != null)
			{
				PhotonNetwork.LeaveRoom();
			}
		}
	}
}
