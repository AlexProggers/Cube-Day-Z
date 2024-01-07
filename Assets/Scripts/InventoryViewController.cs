using UnityEngine;

public enum InventoryViewType
{
	Default = 0,
	Storage = 1
}

public class InventoryViewController : MonoBehaviour
{
    [SerializeField]
	private int _slotsOnRow;

	[SerializeField]
	private System.Collections.Generic.List<SlotViewController> _slotsViewController;

	[SerializeField]
	private tk2dUIScrollbar _scrollbar;

	[SerializeField]
	private StorageViewController _storageView;

	[SerializeField]
	private GameObject _equipmentView;

	[SerializeField]
	private SelectedSlotInfoView _selectedSlotView;

	private int _currentPosition;

	public InventoryViewType ViewType { get; set; }
	public WorldObjectStorageAction StorageAction { get; set; }

	public StorageViewController StorageView
	{
		get
		{
			return _storageView;
		}
	}

	private int MaxPosition
	{
		get
		{
			return Mathf.Max(0, (InventoryController.Instance.Slots.Count - 1) / _slotsOnRow - (_slotsViewController.Count / _slotsOnRow - 1));
		}
	}

	private void OnEnable()
	{
		UpdateCurrentPage();
		ShowAdditionlInfo();
	}

	private void OnDisable()
	{
		_currentPosition = 0;
		_selectedSlotView.Show(false);
		_equipmentView.SetActive(false);
		_storageView.gameObject.SetActive(false);
	}

	private void ShowAdditionlInfo()
	{
		InventoryViewType viewType = ViewType;
		if (viewType == InventoryViewType.Storage)
		{
			_storageView.gameObject.SetActive(true);
		}
		else
		{
			_equipmentView.SetActive(true);
		}
	}

	private void OnScroll()
	{
		_currentPosition = (int)((float)MaxPosition * _scrollbar.Value);
		UpdateCurrentPage();
	}

	public void UpdateCurrentPage()
	{
		if (_currentPosition > MaxPosition)
		{
			_currentPosition = Mathf.Max(0, _currentPosition - 1);
		}
		UpdatePage(_currentPosition);
	}

	public void SelectSlot(InventorySlot slot, int hotKeyNumber)
	{
		int slotIndex = InventoryController.Instance.Slots.FindIndex((InventorySlot s) => s == slot);
		_selectedSlotView.UpdateView(slot, slotIndex, hotKeyNumber);
	}

	private void UpdatePage(int position)
	{
		int num = position * _slotsOnRow;
		for (int i = num; i < num + _slotsViewController.Count; i++)
		{
			int index = i % _slotsOnRow + (i - num) / _slotsOnRow * _slotsOnRow;
			if (InventoryController.Instance.Slots.Count - 1 >= i)
			{
				_slotsViewController[index].UpdateView(InventoryController.Instance.Slots[i], i);
			}
			else
			{
				_slotsViewController[index].Show(false);
			}
		}
		_selectedSlotView.UpdateCurrent();
	}
}
