using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageViewController : MonoBehaviour
{
    	[SerializeField]
	private List<SlotViewController> _slotsViewController;

	[SerializeField]
	private GameObject _howManyWindow;

	[SerializeField]
	private tk2dUIProgressBar _howManyProgress;

	[SerializeField]
	private tk2dUIScrollbar _howManyScrollbar;

	[SerializeField]
	private tk2dTextMesh _currentCount;

	private InventorySlot _selectedSlot;

	private int _currentItemCount;

	private int _maxItemsCount;

	private bool _isStorageItem;

	private byte _currentAmmo;

	private void Awake()
	{
		SetToDefault();
	}

	private void OnEnable()
	{
		UpdateView();
	}

	public void UpdateView()
	{
		if (!(GameUIController.I.Inventory.StorageAction != null))
		{
			return;
		}
		List<InventorySlot> storageContent = GameUIController.I.Inventory.StorageAction.StorageContent;
		for (int i = 0; i < _slotsViewController.Count; i++)
		{
			if (i < GameUIController.I.Inventory.StorageAction.MaxStorageCount)
			{
				_slotsViewController[i].UpdateView(storageContent[i]);
			}
			else
			{
				_slotsViewController[i].Show(false);
			}
		}
	}

	public void ShowHowManyWindow(InventorySlot slot, byte ammo, bool isStorageSlot)
	{
		_selectedSlot = slot;
		_isStorageItem = isStorageSlot;
		_currentAmmo = ammo;
		SetMaxCount(slot.Count);
		if (_maxItemsCount > 1)
		{
			_howManyWindow.SetActive(true);
			return;
		}
		_currentItemCount = 1;
		OnClickOk();
	}

	private void SetMaxCount(int countInSlot)
	{
		List<InventorySlot> list = ((!_isStorageItem) ? GameUIController.I.Inventory.StorageAction.StorageContent : InventoryController.Instance.Slots);
		List<InventorySlot> list2 = list.FindAll((InventorySlot s) => s.Item == null || s.Item.Id == _selectedSlot.Item.Id);
		_maxItemsCount = 0;
		if (list2.Count <= 0)
		{
			return;
		}
		foreach (InventorySlot item in list2)
		{
			if (item.Item == null)
			{
				_maxItemsCount = countInSlot;
				break;
			}
			int num = (int)item.MaxItemCount - (int)item.Count;
			if (num > _maxItemsCount)
			{
				_maxItemsCount = num;
				if (_maxItemsCount > countInSlot)
				{
					_maxItemsCount = countInSlot;
				}
			}
		}
	}

	private bool CanChange()
	{
		List<InventorySlot> list = ((!_isStorageItem) ? GameUIController.I.Inventory.StorageAction.StorageContent : InventoryController.Instance.Slots);
		InventorySlot inventorySlot = list.Find((InventorySlot s) => s.Item == null || (s.Item.Id == _selectedSlot.Item.Id && (int)s.MaxItemCount - (int)s.Count >= _currentItemCount));
		return inventorySlot != null;
	}

	private void SetToDefault()
	{
		_currentItemCount = 0;
		_howManyProgress.Value = 0f;
		_howManyScrollbar.Value = 0f;
		_currentCount.text = "0";
		_currentAmmo = 0;
	}

	public void HideHowManyWindow()
	{
		_selectedSlot = null;
		SetToDefault();
		_howManyWindow.SetActive(false);
	}

	private void OnClickOk()
	{
		int num = ((!_isStorageItem) ? 1 : (-1));
		if (_currentItemCount > 0 && CanChange())
		{
			int slotIndex = ((!_isStorageItem) ? InventoryController.Instance.Slots.FindIndex((InventorySlot s) => s == _selectedSlot) : GameUIController.I.Inventory.StorageAction.StorageContent.FindIndex((InventorySlot s) => s == _selectedSlot));
			Debug.Log("Click storage and call ChangeStorageItem");
			GameUIController.I.Inventory.StorageAction.ChangeStorageItem(slotIndex, _selectedSlot.Item.Id, _currentItemCount * num, _currentAmmo, true, true);
		}
		HideHowManyWindow();
	}

	private void OnClickCancel()
	{
		HideHowManyWindow();
	}

	private void UpdateProgressValue()
	{
		float value = (float)_currentItemCount / (float)_maxItemsCount;
		_howManyProgress.Value = value;
		_howManyScrollbar.Value = value;
	}

	private void OnClickPlus()
	{
		_currentItemCount++;
		_currentCount.text = _currentItemCount.ToString();
		UpdateProgressValue();
	}

	private void OnClickMinus()
	{
		_currentItemCount--;
		_currentCount.text = _currentItemCount.ToString();
		UpdateProgressValue();
	}

	private void OnClickMax()
	{
		_howManyProgress.Value = 1f;
		_howManyScrollbar.Value = 1f;
		_currentItemCount = _maxItemsCount;
		_currentCount.text = _currentItemCount.ToString();
	}

	private void OnSliderChange()
	{
		_howManyProgress.Value = _howManyScrollbar.Value;
		_currentItemCount = Mathf.RoundToInt(_howManyScrollbar.Value * (float)_maxItemsCount);
		_currentCount.text = _currentItemCount.ToString();
	}
}
