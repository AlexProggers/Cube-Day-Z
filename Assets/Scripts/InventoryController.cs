using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private const int DefaultSlotsCount = 4;

	public const int DefaultHotKeyCount = 5;

	public const string HandKey = "Hand";

	private static InventoryController _instance;

	private List<InventorySlot> _inventorySlots = new List<InventorySlot>();

	private Dictionary<KeyCode, int> _hotKeys = new Dictionary<KeyCode, int>();

	public Dictionary<string, string> Equipment = new Dictionary<string, string>();

	private KeyValueInt _takeOffBuffer;

	private byte _bufferAmmo;

	public static InventoryController Instance
	{
		get
		{
			return _instance ?? (_instance = new InventoryController());
		}
	}

	public List<InventorySlot> Slots
	{
		get
		{
			return _inventorySlots;
		}
	}

	public Dictionary<KeyCode, int> HotKeys
	{
		get
		{
			return _hotKeys;
		}
	}

	private InventoryController()
	{
		_takeOffBuffer = new KeyValueInt();
		for (int i = 0; i < 4; i++)
		{
			_inventorySlots.Add(new InventorySlot());
		}
		for (int j = 0; j < 5; j++)
		{
			if (j < 4)
			{
				_hotKeys.Add(GetKeyByNumber(j + 1), j);
			}
			else
			{
				_hotKeys.Add(GetKeyByNumber(j + 1), -1);
			}
		}
	}

	public static void Clear()
	{
		_instance = new InventoryController();
	}

	public bool ContainsAnyItems()
	{
		foreach (InventorySlot inventorySlot in _inventorySlots)
		{
			if (inventorySlot.Item != null)
			{
				return true;
			}
		}
		foreach (KeyValuePair<string, string> item in Equipment)
		{
			if (!string.IsNullOrEmpty(item.Value))
			{
				return true;
			}
		}
		return false;
	}

	public static KeyCode GetKeyByNumber(int number)
	{
		switch (number)
		{
		case 1:
			return KeyCode.Alpha1;
		case 2:
			return KeyCode.Alpha2;
		case 3:
			return KeyCode.Alpha3;
		case 4:
			return KeyCode.Alpha4;
		case 5:
			return KeyCode.Alpha5;
		default:
			return KeyCode.None;
		}
	}

	public static int GetNumberByKey(KeyCode key)
	{
		switch (key)
		{
		case KeyCode.Alpha1:
			return 1;
		case KeyCode.Alpha2:
			return 2;
		case KeyCode.Alpha3:
			return 3;
		case KeyCode.Alpha4:
			return 4;
		case KeyCode.Alpha5:
			return 5;
		default:
			return 0;
		}
	}

	public void EquipByHotKey(KeyCode key)
	{
		if (_hotKeys.ContainsKey(key))
		{
			int num = _hotKeys[key];
			if (_inventorySlots.Count > num && num >= 0 && _inventorySlots[num].Item != null && _inventorySlots[num].Item.IsEquippable && ((_inventorySlots[num].Item.Type != 0 && _inventorySlots[num].Item.Type != ItemType.Consumables) || !(GameControls.I != null) || !(GameControls.I.Walker != null) || !GameControls.I.Walker.climbing))
			{
				Equip(_inventorySlots[num]);
			}
		}
	}

	public int AddItems(Item item, int count, byte ammo = 0)
	{
		int num = count;
		while (num > 0)
		{
			InventorySlot inventorySlot = _inventorySlots.Find((InventorySlot s) => s.ContainsItem(item.Id) && (int)s.Count < (int)s.MaxItemCount);
			if (inventorySlot != null)
			{
				num = inventorySlot.AddSome(item, num, ammo);
				continue;
			}
			inventorySlot = _inventorySlots.Find((InventorySlot s) => s.Item == null);
			if (inventorySlot != null)
			{
				num = inventorySlot.AddSome(item, num, ammo);
				continue;
			}
			return num;
		}
		return 0;
	}

	public void RemoveItems(string itemId, int count)
	{
		int num = count;
		while (num > 0)
		{
			InventorySlot inventorySlot = _inventorySlots.Find((InventorySlot s) => s.ContainsItem(itemId));
			if (inventorySlot != null)
			{
				num = inventorySlot.Remove(num);
				continue;
			}
			break;
		}
	}

	public void AddSlots(int count)
	{
		if (count > 0)
		{
			for (int i = 0; i < count; i++)
			{
				_inventorySlots.Add(new InventorySlot());
			}
		}
	}

	public void RemoveSlots(int count)
	{
		int num = Mathf.Min(_inventorySlots.Count - 4, count);
		if (num <= 0)
		{
			return;
		}
		List<InventorySlot> list = _inventorySlots.FindAll((InventorySlot slot) => slot.Item == null);
		for (int i = 0; i < num; i++)
		{
			if (i < list.Count)
			{
				_inventorySlots.Remove(list[i]);
				continue;
			}
			int index = _inventorySlots.Count - 1;
			_inventorySlots[index].Drop(GameControls.I.Player.transform, _inventorySlots[index].Count, true);
			_inventorySlots.RemoveAt(index);
		}
		GameUIController.I.Inventory.UpdateCurrentPage();
	}

	public string GetEquipedId(string key)
	{
		return (!Equipment.ContainsKey(key)) ? string.Empty : Equipment[key];
	}

	public int GetItemsCount(string id)
	{
		int num = 0;
		List<InventorySlot> list = Slots.FindAll((InventorySlot slot) => slot.Item != null && slot.Item.Id == id);
		foreach (InventorySlot item in list)
		{
			num += (int)item.Count;
		}
		return num;
	}

	public void EquipFromPack(Item item, int additionalCount)
	{
		string text = string.Empty;
		byte ammo = 0;
		if ((item.Type == ItemType.Weapon || item.Type == ItemType.Consumables) && Equipment.ContainsKey("Hand") && !string.IsNullOrEmpty(Equipment["Hand"]))
		{
			int weaponIndex = DataKeeper.I.GetWeaponIndex(Equipment["Hand"]);
			if (weaponIndex < GameControls.I.PlayerWeapons.WeaponsBehavioursList.Count)
			{
				ammo = (byte)(int)GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex].bulletsLeft;
			}
		}
		switch (item.Type)
		{
		case ItemType.Clothing:
		{
			Clothing clothing = item as Clothing;
			if (clothing != null)
			{
				text = clothing.BodyPart.ToString();
				WorldController.I.Player.EuqipItem(clothing);
				AddSlots(clothing.AdditionalSlots);
			}
			break;
		}
		case ItemType.Weapon:
		{
			text = "Hand";
			int weaponIndex2 = DataKeeper.I.GetWeaponIndex(item.Id);
			Weapon weaponInfo = DataKeeper.Info.GetWeaponInfo(item.Id);
			if (weaponInfo != null && weaponIndex2 < GameControls.I.PlayerWeapons.WeaponsBehavioursList.Count)
			{
				GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex2].SetInfo(weaponInfo.Damage, weaponInfo.Range, weaponInfo.Speed);
				GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex2].haveWeapon = true;
				if (weaponInfo.Id == "binoculars")
				{
					GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex2].bulletsLeft = 0;
				}
				else
				{
					GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex2].bulletsLeft = ((weaponInfo.ActionType == WeaponActionType.Melee) ? 1 : additionalCount);
				}
				GameControls.I.PlayerWeapons.StartCoroutine(GameControls.I.PlayerWeapons.SelectWeapon(weaponIndex2));
				GameControls.I.PlayerWeapons.UpdateTotalWeapons();
			}
			break;
		}
		case ItemType.Consumables:
		{
			text = "Hand";
			Consumable item2 = item as Consumable;
			TakeOff(text, true, true, true, ammo);
			HandUseForRealistic.I.Equip(item2);
			break;
		}
		}
		if (!string.IsNullOrEmpty(text))
		{
			if (Equipment.ContainsKey(text))
			{
				if (!string.IsNullOrEmpty(Equipment[text]))
				{
					TakeOff(text, true, false, true, ammo);
				}
				Equipment[text] = item.Id;
			}
			else
			{
				Equipment.Add(text, item.Id);
			}
		}
		if (!string.IsNullOrEmpty(_takeOffBuffer.Key))
		{
			if (_takeOffBuffer.Value > 0)
			{
				Item itemInfo = DataKeeper.Info.GetItemInfo(_takeOffBuffer.Key);
				if (itemInfo != null)
				{
					int num = AddItems(itemInfo, _takeOffBuffer.Value, _bufferAmmo);
					if (num > 0)
					{
						DropItem(itemInfo, num, _bufferAmmo);
					}
				}
			}
			_takeOffBuffer.Key = null;
			_takeOffBuffer.Value = 0;
			_bufferAmmo = 0;
		}
		if (text == "Hand")
		{
			WorldController.I.Player.ChangeItemInHand(Equipment["Hand"]);
		}
	}

	public void Equip(InventorySlot slot, System.Action<string, string> equipCallback = null)
	{
		if (slot.Item == null)
		{
			return;
		}
		string text = string.Empty;
		bool flag = false;
		bool flag2 = false;
		byte ammo = 0;
		if (Equipment.ContainsKey("Hand") && !string.IsNullOrEmpty(Equipment["Hand"]))
		{
			Weapon weaponInfo = DataKeeper.Info.GetWeaponInfo(Equipment["Hand"]);
			if (weaponInfo != null)
			{
				int weaponIndex = DataKeeper.I.GetWeaponIndex(Equipment["Hand"]);
				if (weaponIndex < GameControls.I.PlayerWeapons.WeaponsBehavioursList.Count)
				{
					ammo = (byte)(int)GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex].bulletsLeft;
				}
			}
		}
		switch (slot.Item.Type)
		{
		case ItemType.Clothing:
		{
			Clothing clothing = slot.Item as Clothing;
			if (clothing != null)
			{
				text = clothing.BodyPart.ToString();
				WorldController.I.Player.EuqipItem(clothing);
				AddSlots(clothing.AdditionalSlots);
				flag = true;
			}
			break;
		}
		case ItemType.Weapon:
		{
			text = "Hand";
			int weaponIndex2 = DataKeeper.I.GetWeaponIndex(slot.Item.Id);
			Weapon weaponInfo2 = DataKeeper.Info.GetWeaponInfo(slot.Item.Id);
			if (weaponInfo2 != null && weaponIndex2 < GameControls.I.PlayerWeapons.WeaponsBehavioursList.Count)
			{
				GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex2].SetInfo(weaponInfo2.Damage, weaponInfo2.Range, weaponInfo2.Speed);
				GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex2].bulletsLeft = (byte)slot.Ammo;
				GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex2].haveWeapon = true;
				GameControls.I.PlayerWeapons.StartCoroutine(GameControls.I.PlayerWeapons.SelectWeapon(weaponIndex2));
				GameControls.I.PlayerWeapons.UpdateTotalWeapons();
			}
			flag2 = true;
			break;
		}
		case ItemType.Consumables:
		{
			text = "Hand";
			Consumable item = slot.Item as Consumable;
			TakeOff(text, true, true, true, ammo);
			HandUseForRealistic.I.Equip(item);
			flag2 = true;
			break;
		}
		}
		if (!string.IsNullOrEmpty(text))
		{
			if (Equipment.ContainsKey(text))
			{
				if (!string.IsNullOrEmpty(Equipment[text]))
				{
					TakeOff(text, true, false, true, ammo);
				}
				Equipment[text] = slot.Item.Id;
				slot.Remove(1);
			}
			else
			{
				Equipment.Add(text, slot.Item.Id);
				slot.Remove(1);
			}
			if (equipCallback != null)
			{
				equipCallback(text, Equipment[text]);
			}
		}
		if (!string.IsNullOrEmpty(_takeOffBuffer.Key))
		{
			if (_takeOffBuffer.Value > 0)
			{
				Item itemInfo = DataKeeper.Info.GetItemInfo(_takeOffBuffer.Key);
				if (itemInfo != null)
				{
					int num = AddItems(itemInfo, _takeOffBuffer.Value, _bufferAmmo);
					if (num > 0)
					{
						DropItem(itemInfo, num, _bufferAmmo);
					}
				}
			}
			_takeOffBuffer.Key = null;
			_takeOffBuffer.Value = 0;
			_bufferAmmo = 0;
		}
		if (text == "Hand")
		{
			WorldController.I.Player.ChangeItemInHand(Equipment["Hand"]);
		}
		if (flag)
		{
			GameUIController.I.Inventory.UpdateCurrentPage();
		}
		if (flag2)
		{
			GameUIController.I.ShowCharacterMenu(false, CharacterMenuType.Inventory);
		}
	}

	public void OnDie(bool killedByPlayer)
	{
		List<string> list = new List<string>();
		list.AddRange(Equipment.Keys);
		foreach (string item in list)
		{
			byte ammo = 0;
			if (item == "Hand" && DataKeeper.Info.GetWeaponInfo(Equipment["Hand"]) != null)
			{
				int weaponIndex = DataKeeper.I.GetWeaponIndex(Equipment["Hand"]);
				if (weaponIndex < GameControls.I.PlayerWeapons.WeaponsBehavioursList.Count)
				{
					ammo = (byte)(int)GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex].bulletsLeft;
				}
			}
			TakeOff(item, false, true, false, ammo);
		}
		DropAllItems(killedByPlayer);
	}

	private void DropAllItems(bool killedByPlayer)
	{
		foreach (InventorySlot inventorySlot in _inventorySlots)
		{
			inventorySlot.Drop(GameControls.I.Player.transform, inventorySlot.Count, killedByPlayer);
		}
	}

	public void TakeOff(string key, bool returnToInventory = true, bool emptyHand = true, bool addToBuffer = false, byte ammo = 0)
	{
		if (Equipment.ContainsKey(key))
		{
			string text = Equipment[key];
			Item itemInfo = DataKeeper.Info.GetItemInfo(text);
			Equipment.Remove(key);
			if (returnToInventory)
			{
				int num = AddItems(itemInfo, 1, ammo);
				if (addToBuffer)
				{
					_takeOffBuffer.Key = text;
					_takeOffBuffer.Value = num;
					_bufferAmmo = ammo;
				}
				else if (num > 0)
				{
					DropItem(itemInfo, num, ammo);
				}
			}
			GameUIController.I.Inventory.UpdateCurrentPage();
			switch (itemInfo.Type)
			{
			case ItemType.Weapon:
			{
				int weaponIndex = DataKeeper.I.GetWeaponIndex(itemInfo.Id);
				if (weaponIndex < GameControls.I.PlayerWeapons.WeaponsBehavioursList.Count)
				{
					GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex].haveWeapon = false;
				}
				break;
			}
			case ItemType.Consumables:
				HandUseForRealistic.I.TakeOff();
				break;
			case ItemType.Clothing:
			{
				Clothing clothing = itemInfo as Clothing;
				if (clothing != null)
				{
					WorldController.I.Player.TakeOffItem(clothing.BodyPart);
					RemoveSlots(clothing.AdditionalSlots);
				}
				break;
			}
			}
		}
		if (key == "Hand")
		{
			WorldController.I.Player.ChangeItemInHand(null);
		}
		if (emptyHand)
		{
			GameControls.I.PlayerWeapons.StartCoroutine(GameControls.I.PlayerWeapons.SelectWeapon(0));
			GameControls.I.PlayerWeapons.UpdateTotalWeapons();
		}
	}

	private void DropItem(Item item, int count, int additionalCount = 0)
	{
		Transform transform = GameControls.I.Player.transform;
		WorldController.I.Player.PlayerDropItem(transform.position, item.Id, count, (byte)additionalCount);
	}
}