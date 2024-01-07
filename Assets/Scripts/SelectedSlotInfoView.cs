using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedSlotInfoView : MonoBehaviour
{
    [SerializeField]
	private tk2dSprite _icon;

	[SerializeField]
	private tk2dTextMesh _count;

	[SerializeField]
	private ShadowText _title;

	[SerializeField]
	private ShadowText _description;

	[SerializeField]
	private tk2dTextMesh _equipText;

	[SerializeField]
	private tk2dTextMesh _currentHotKey;

	[SerializeField]
	private EquipmentViewController _equipmentViewController;
	
	[SerializeField]
	private HowManyWindowController _howManyWindow;

	[SerializeField]
	private GameObject _equipButton;

	[SerializeField]
	private GameObject _hotKeyPopup;

	[SerializeField]
	private tk2dUIToggleButtonGroup _hotKeys;

	private InventorySlot _selectedSlot;

	private int _slotIndex;

	private int _hotKeyNumber;

    public void Show(bool show)
	{
		base.gameObject.SetActive(show);
	}

	private void OnSelectedNumberChanged()
	{
		Dictionary<KeyCode, int> hotKeys = InventoryController.Instance.HotKeys;
		KeyCode keyByNumber = InventoryController.GetKeyByNumber(_hotKeyNumber);
		KeyCode keyByNumber2 = InventoryController.GetKeyByNumber(_hotKeys.SelectedIndex);
		if (hotKeys.ContainsKey(keyByNumber))
		{
			hotKeys[keyByNumber] = -1;
		}
		if (hotKeys.ContainsKey(keyByNumber2))
		{
			hotKeys[keyByNumber2] = _slotIndex;
		}
		_hotKeyPopup.SetActive(false);
		_hotKeyNumber = _hotKeys.SelectedIndex;
		GameUIController.I.Inventory.UpdateCurrentPage();
		UpdateCurrent();
	}

	private void OnClickArrow()
	{
		_hotKeyPopup.SetActive(!_hotKeyPopup.activeSelf);
	}

	public void UpdateView(InventorySlot slot, int slotIndex, int hotKeyNumber)
	{
		_slotIndex = slotIndex;
		_hotKeyNumber = hotKeyNumber;
		_selectedSlot = slot;
		UpdateCurrent();
		_hotKeys.SelectedIndex = hotKeyNumber;
	}

	public void UpdateCurrent()
	{
		bool flag = _selectedSlot != null && _selectedSlot.Item != null;
		if (flag)
		{
			if (_hotKeyNumber > 0)
			{
				_currentHotKey.text = _hotKeyNumber.ToString();
			}
			else
			{
				_currentHotKey.text = string.Empty;
			}
			if (_icon.GetSpriteIdByName(_selectedSlot.Item.Icon) == 0)
			{
				_icon.SetSprite(DataKeeper.NoIcon);
			}
			else
			{
				_icon.SetSprite(_selectedSlot.Item.Icon);
			}
			_count.text = "x" + _selectedSlot.Count;
			if (DataKeeper.Language == Language.Russian)
			{
				_title.SetText(_selectedSlot.Item.RussianName);
				_description.SetText(_selectedSlot.Item.RussianDescription);
			}
			else
			{
				_title.SetText(_selectedSlot.Item.Name);
				_description.SetText(_selectedSlot.Item.Description);
			}
		}
		SetEquipButton();
		Show(flag);
	}

	private void SetEquipButton()
	{
		if (_selectedSlot != null && _selectedSlot.Item != null)
		{
			switch (_selectedSlot.Item.Type)
			{
			case ItemType.Weapon:
			case ItemType.Clothing:
				_equipButton.SetActive(true);
				break;
			case ItemType.Consumables:
				_equipButton.SetActive(_selectedSlot.Item.IsEquippable);
				break;
			}
		}
	}

	private void OnClickEquip()
	{
		if (_selectedSlot.Item == null || (_selectedSlot.Item.Type != 0 && _selectedSlot.Item.Type != ItemType.Consumables) || !(GameControls.I != null) || !(GameControls.I.Walker != null) || !GameControls.I.Walker.climbing)
		{
			InventoryController.Instance.Equip(_selectedSlot, _equipmentViewController.EquipItem);
		}
	}

	private void OnClickDrop()
	{
		if ((int)_selectedSlot.Count > 1)
		{
			_howManyWindow.ShowHowManyWindow(_selectedSlot, UpdateCurrent);
			return;
		}

		string id = _selectedSlot.Item.Id;
		byte ammo = _selectedSlot.Ammo;

		_selectedSlot.Remove(1);
		Transform transform = GameControls.I.Player.transform;

		WorldController.I.Player.PlayerDropItem(transform.position, id, 1, ammo);
		GameUIController.I.Inventory.UpdateCurrentPage();

		UpdateCurrent();
	}
}
