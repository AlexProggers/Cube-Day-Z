using UnityEngine;


public enum CharacterMenuType
{
	Inventory = 0,
	Craft = 1,
	Skills = 2,
	Info = 3,
	Menu = 4
}

public class CharacterMenuController : MonoBehaviour
{
	[SerializeField]
	private tk2dUIToggleButtonGroup _viewTabs;

	[SerializeField]
	private InventoryViewController _inventory;

	[SerializeField]
	private MapViewController _mapView;

	public InventoryViewController Inventory
	{
		get
		{
			return _inventory;
		}
	}

	public MapViewController Map
	{
		get
		{
			return _mapView;
		}
	}

	public bool IsShown
	{
		get
		{
			return base.gameObject.activeSelf;
		}
	}

	public CharacterMenuType CurrentType { get; private set; }

	public void ShowView(bool show, CharacterMenuType type, bool consideringControl = true, InventoryViewType inventoryType = InventoryViewType.Default)
	{
		CurrentType = type;
		_inventory.ViewType = inventoryType;

		base.gameObject.SetActive(show);
		_viewTabs.SelectedIndex = (int)type;
		
		if (consideringControl)
		{
			GameControls.I.MenuControls(show);
		}
	}
}
