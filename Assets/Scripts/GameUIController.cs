using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    public static GameUIController I;

	[SerializeField]
	private KeyCode _inventoryMenuKey = KeyCode.I;

	[SerializeField]
	private KeyCode _craftMenuKey = KeyCode.C;

	[SerializeField]
	private KeyCode _mapMenuKey = KeyCode.M;

	[SerializeField]
	private KeyCode _menuKey = KeyCode.Tab;

	
	[SerializeField]
	public RespawnMenuController _respawnMenu;

	[SerializeField]
	public CharacterMenuController characterMenuController;

    [SerializeField]
    private Transform _tckCanvasContent;

    [SerializeField]
    private bool _tckDisableOnNonMobile;

	public InventoryViewController Inventory
	{
		get
		{
			return characterMenuController.Inventory;
		}
	}

	public MapViewController Map
	{
		get
		{
			return characterMenuController.Map;
		}
	}

    private void Awake()
	{
		I = this;
        
        if(!Application.isMobilePlatform && _tckDisableOnNonMobile)
        {
            _tckCanvasContent.position = new Vector3(0, -1000, 0);
        }
    }

    private void Update()
	{
		if (WorldController.I == null || WorldController.I.Player == null)
		{
			return;
		}

		if (Input.GetKeyDown(_inventoryMenuKey))
		{
			characterMenuController.ShowView(ShowCharacterMenu(CharacterMenuType.Inventory), CharacterMenuType.Inventory);
		}

		if (Input.GetKeyDown(_craftMenuKey))
		{
			characterMenuController.ShowView(ShowCharacterMenu(CharacterMenuType.Craft), CharacterMenuType.Craft);
		}
		if (Input.GetKeyDown(_mapMenuKey))
		{
			characterMenuController.ShowView(ShowCharacterMenu(CharacterMenuType.Info), CharacterMenuType.Info);
		}
		if (Input.GetKeyDown(_menuKey))
		{
			characterMenuController.ShowView(ShowCharacterMenu(CharacterMenuType.Menu), CharacterMenuType.Menu);
		}
    }

	private bool ShowCharacterMenu(CharacterMenuType type)
	{
		return !characterMenuController.IsShown || characterMenuController.CurrentType != type;
	}

	public void ShowRespawnMenu(bool show, string killerName = null, string weaponId = null)
	{
		_respawnMenu.gameObject.SetActive(show);
		_respawnMenu.SetKillerName(killerName, weaponId);
		
		if (show)
		{
			GameControls.I.MenuControls(true, false);
			return;
		}
		
		GameControls.I.MenuControls(false);
		GameControls.I.Player.GetComponent<Rigidbody>().freezeRotation = true;
	}

	public void ShowCharacterMenu(bool show, CharacterMenuType type, bool consideringControl = true)
	{
		characterMenuController.ShowView(show, type, consideringControl);

		if (consideringControl)
		{
			GameControls.I.MenuControls(show);
		}
	}
}
