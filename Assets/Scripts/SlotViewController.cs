using System.Collections.Generic;
using UnityEngine;


public class InventorySlot
{
	public Item Item { get; private set; }

	public int Count { get; private set; }

	public int MaxItemCount { get; private set; }

	public byte Ammo { get; private set; }

	public bool ContainsItem(string id)
	{
		return Item != null && Item.Id == id;
	}

	public int AddSome(Item item, int count, byte ammo = 0)
	{
		Ammo = ammo;
		if (Item == null)
		{
			Item = item;
			MaxItemCount = item.MaxInStack;
			if (Item.Id == "binoculars")
			{
				Ammo = (byte)0;
			}
			else if (item.Type == ItemType.Weapon)
			{
				Weapon weaponInfo = DataKeeper.Info.GetWeaponInfo(item.Id);
				if (weaponInfo != null && weaponInfo.ActionType == WeaponActionType.Melee)
				{
					Ammo = (byte)1;
				}
			}
		}
		int b = (int)Count + count - (int)MaxItemCount;
		Count = Mathf.Min(MaxItemCount, (int)Count + count);
		return Mathf.Max(0, b);
	}

	public int Remove(int count)
	{
		int b = count - (int)Count;
		Count = Mathf.Max(0, (int)Count - count);
		if ((int)Count <= 0)
		{
			Item = null;
			Count = 0;
			MaxItemCount = 0;
			Ammo = (byte)0;
		}
		return Mathf.Max(0, b);
	}

	public void Drop(Transform playerPos, int count, bool killedByPlayer)
	{
		if (Item == null)
		{
			return;
		}
		bool flag = false;
		if (!killedByPlayer)
		{
			bool flag2 = false;
			foreach (KeyValuePair<int, PhotonMan> worldPlayer in WorldController.I.WorldPlayers)
			{
				if (!(worldPlayer.Key != DataKeeper.backendInfo.playerId) || !(Vector3.Distance(playerPos.position, worldPlayer.Value.transform.position) < DataKeeper.DistanceToDropLocalLoot))
				{
					continue;
				}
				flag2 = true;
				break;
			}
			if (!flag2)
			{
				flag = true;
			}
		}
		if (flag)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(playerPos.position, -Vector3.up, out hitInfo, 100f, DataKeeper.I.ItemsCollisionsMask))
			{
				Quaternion rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, hitInfo.normal), Vector3.Cross(Vector3.up, hitInfo.normal));
				WorldController.I.InstantiateLocalItem(Item.Id, hitInfo.point, rotation, (byte)count, Ammo);
			}
		}
		else
		{
			WorldController.I.Player.PlayerDropItem(playerPos.position, Item.Id, count, Ammo);
		}
		Remove(count);
	}
}

public class SlotViewController : MonoBehaviour
{
	private const string DefaultStorageIcon = "storage";

	[SerializeField]
	private tk2dSprite _icon;

	[SerializeField]
	private tk2dTextMesh _count;

	[SerializeField]
	private tk2dTextMesh _buttonText;

	[SerializeField]
	private bool _isStorageSlot;

	private InventorySlot _slotInfo;

	private int _hotKeyNumber;

	public void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}

	public void UpdateView(InventorySlot slot, int slotIndex = -1)
	{
		_slotInfo = slot;
		if (slotIndex >= 0)
		{
			bool flag = false;
			foreach (KeyValuePair<KeyCode, int> hotKey in InventoryController.Instance.HotKeys)
			{
				if (hotKey.Value == slotIndex)
				{
					_hotKeyNumber = InventoryController.GetNumberByKey(hotKey.Key);
					_buttonText.text = _hotKeyNumber.ToString();
					flag = true;
				}
			}
			if (!flag)
			{
				_hotKeyNumber = 0;
				_buttonText.text = string.Empty;
			}
		}
		else
		{
			_hotKeyNumber = 0;
			_buttonText.text = string.Empty;
		}
		if (_slotInfo.Item != null)
		{
			if (_icon.GetSpriteIdByName(_slotInfo.Item.Icon) == 0)
			{
				_icon.SetSprite(DataKeeper.NoIcon);
			}
			else
			{
				_icon.SetSprite(_slotInfo.Item.Icon);
			}
			_icon.gameObject.SetActive(true);
			_count.text = "x" + _slotInfo.Count.ToString();
		}
		else
		{
			if (_isStorageSlot)
			{
				_icon.SetSprite("storage");
				_icon.gameObject.SetActive(true);
			}
			else
			{
				_icon.gameObject.SetActive(false);
			}
			_count.text = string.Empty;
		}
		Show(true);
	}

	private void OnClick()
	{
		if (_slotInfo.Item != null)
		{
			InventoryViewType viewType = GameUIController.I.Inventory.ViewType;
			if (viewType == InventoryViewType.Storage)
			{
				GameUIController.I.Inventory.StorageView.ShowHowManyWindow(_slotInfo, _slotInfo.Ammo, _isStorageSlot);
			}
			else
			{
				GameUIController.I.Inventory.SelectSlot(_slotInfo, _hotKeyNumber);
			}
		}
	}
}
