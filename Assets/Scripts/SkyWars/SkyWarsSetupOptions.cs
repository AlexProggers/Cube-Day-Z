using System.Collections.Generic;
using UnityEngine;

namespace SkyWars
{
	public class SkyWarsSetupOptions
	{
		private static byte _maxPlayerOnServer = 8;

		public static int WaitInLobbyForFillRoomToMax = 20;

		private static float _minStart = 2f;

		public static int StartingCountdown = 10;

		public static float BridgeMovingSpeed = 0.1f;

		public static float BridgeMaxScale = 120f;

		public static float BridgeDelayBeforeStart = 30f;

		public static float HitForce = 2500f;

		public static int RoundDuration = 300;

		public static int RedBtnStartTime = 30;

		public static byte MaxPlayerOnServer
		{
			get
			{
				return _maxPlayerOnServer;
			}
		}

		public static int MinPlayersForStart
		{
			get
			{
				return (int)_minStart;
			}
		}

		public static void SetTestMinPercent(float newMin)
		{
			_minStart = newMin;
		}

		public static void AddWoodenBlocksToInventary()
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			dictionary.Add("Wooden Foundation SW", 500);
			dictionary.Add("Rucksack", 1);

			foreach (KeyValuePair<string, int> item in dictionary)
			{
				Item itemInfo = DataKeeper.Info.GetItemInfo(item.Key);
				if (itemInfo != null)
				{
					if (itemInfo.Type == ItemType.Consumables)
					{
						InventoryController.Instance.AddItems(itemInfo, item.Value);
					}
					else
					{
						InventoryController.Instance.EquipFromPack(itemInfo, 0);
					}
				}
			}
		}
	}
}
