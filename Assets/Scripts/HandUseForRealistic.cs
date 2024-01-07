using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TextureForUseAction
{
	public WorldObjectActionType ObjType;

	public Texture2D Texture;
}


public class HandUseForRealistic : MonoBehaviour
{
	public static HandUseForRealistic I;

	[SerializeField]
	private LayerMask _useableObjLayer;

	[SerializeField]
	private float _useableRayDistance;

	[SerializeField]
	private List<TextureForUseAction> _cursorTextures;

	[SerializeField]
	private GameObject _hand;

	[SerializeField]
	private Renderer _handRenderer;

	[SerializeField]
	private Transform _itemHolder;

	[SerializeField]
	private PlaceItemViewController _placeItemView;

	[SerializeField]
	private GameObject _myInfoCursor;

	private RaycastHit _cursorHitInfo;

	private GameObject _handItem;

	private Consumable _info;

	private bool _canSwitchWeapon = true;

	private float _waitForSaveAfterBuild = 5f;

	private Coroutine _saveAfterBuildCoroutine;

	public int cacheHoverViewId;

	private float _catchTime = 0.25f;

	private float _lastClickTime;

	public Consumable ItemInHand
	{
		get
		{
			return _info;
		}
	}

	private void Awake()
	{
		I = this;
	}

	private void EnableInfoCursor(bool enable)
	{
		_myInfoCursor.SetActive(enable);
		GameControls.I.Player.CrosshairGuiObjInstance.SetActive(!enable);
		if (!enable)
		{
			HelpMessageController.I.Hide();
		}
	}

	private void SetCursor(WorldObjectActionType type)
	{
		if (type == WorldObjectActionType.None)
		{
			EnableInfoCursor(false);
			return;
		}
		EnableInfoCursor(true);
		TextureForUseAction textureForUseAction = _cursorTextures.Find((TextureForUseAction tex) => tex.ObjType == type);
		if (textureForUseAction != null)
		{
			_myInfoCursor.GetComponent<GUITexture>().texture = textureForUseAction.Texture;
		}
		HelpMessageController.I.ShowMessage(GetHelpMessageType(type));
	}

	private HelpMessageType GetHelpMessageType(WorldObjectActionType actionType)
	{
		switch (actionType)
		{
		case WorldObjectActionType.Storage:
			return HelpMessageType.Storage;
		case WorldObjectActionType.Activate:
			return HelpMessageType.Activate;
		case WorldObjectActionType.Farming:
			return HelpMessageType.Plant;
		default:
			return HelpMessageType.None;
		}
	}

	private void SetHandItemLayer(GameObject obj)
	{
		foreach (Transform item in obj.transform)
		{
			item.gameObject.layer = _itemHolder.gameObject.layer;
			SetHandItemLayer(item.gameObject);
		}
	}

	public void Equip(Consumable item)
	{
		if (item == null)
		{
			return;
		}
		if (_info != null)
		{
			TakeOff();
		}
		_info = item;
		if (_info.ActionType == ConsumableActionType.Placeable)
		{
			_placeItemView.SetItem(_info.PlacebleId, item.PlaceOrientedOnPlayer);
			HelpMessageController.I.ShowMessage(HelpMessageType.PlaceItem);
		}
		else
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(Resources.Load(WorldController.I.GetResourceLoadPath(item.Type) + item.Prefab), _itemHolder.position, _itemHolder.rotation);
			gameObject.transform.parent = _itemHolder;
			gameObject.transform.localScale = Vector3.one;
			SetHandItemLayer(gameObject);
			_handItem = gameObject;
			if (_info.ActionType == ConsumableActionType.Useable)
			{
				HelpMessageController.I.ShowMessage(HelpMessageType.Use);
			}
		}
		_hand.SetActive(true);
	}

	public void TakeOff()
	{
		if (_info != null && (_info.ActionType == ConsumableActionType.Useable || _info.ActionType == ConsumableActionType.Placeable))
		{
			HelpMessageController.I.Hide();
		}
		if (_handItem != null)
		{
			UnityEngine.Object.Destroy(_handItem);
		}
		_info = null;
		_placeItemView.Reset();
		_hand.SetActive(false);
	}

	private void UseHallucinogen(HallucinogenType type)
	{
		WorldController.I.Player.UseHallucinogen(type);
	}

	private void Use()
	{
		switch (_info.ActionType)
		{
		case ConsumableActionType.Useable:
			foreach (KeyValuePair<string, int> item in _info.ChangeStatesOnUse)
			{
				ManState state = (ManState)(byte)Enum.Parse(typeof(ManState), item.Key);
				WorldController.I.Player.ChangeState(state, (short)item.Value);
			}
			UseHallucinogen(_info.HallucinogenType);
			InventoryController.Instance.TakeOff("Hand", false);
			break;
		case ConsumableActionType.Placeable:
			WorldController.I.Player.PlayerPlaceItem(_placeItemView.PlaceTransform.position, _placeItemView.PlaceTransform.rotation, _info.PlacebleId);
			_placeItemView.Reset();
			InventoryController.Instance.TakeOff("Hand", false);
			if (_saveAfterBuildCoroutine == null)
			{
				_saveAfterBuildCoroutine = StartCoroutine(SaveAfterBuild());
			}
			break;
		}
	}

	private IEnumerator SaveAfterBuild()
	{
		yield return new WaitForSeconds(_waitForSaveAfterBuild);
		_saveAfterBuildCoroutine = null;
	}

	private void SwitchWeaponCallback()
	{
		_canSwitchWeapon = true;
	}

	private void ShowHand()
	{
		if (_canSwitchWeapon && !InventoryController.Instance.Equipment.ContainsKey("Hand") && GameControls.I.PlayerWeapons.currentWeapon != 1)
		{
			_canSwitchWeapon = false;
			GameControls.I.Player.CurrentWeaponBehavior = GameControls.I.PlayerWeapons.WeaponsBehavioursList[2];
			GameControls.I.PlayerWeapons.SetSelectWeaponCallback(SwitchWeaponCallback);
			GameControls.I.PlayerWeapons.StartCoroutine(GameControls.I.PlayerWeapons.SelectWeapon(1));
			GameControls.I.PlayerWeapons.UpdateTotalWeapons();
		}
	}

	private void HotKeyProcess()
	{
		for (int i = 0; i < 5; i++)
		{
			KeyCode keyByNumber = InventoryController.GetKeyByNumber(i + 1);
			if (!Input.GetKeyDown(keyByNumber))
			{
				continue;
			}
			else
			{
				InventoryController.Instance.EquipByHotKey(keyByNumber);
			}
		}
	}

	public static bool MyUseButtonDown()
	{
		return Input.GetKeyDown(GameControls.I.Player.use) && GameControls.I.Player.IsHoverTargetUsableNow;
	}

	public static bool MyUseButton()
	{
		return Input.GetKey(GameControls.I.Player.use) && GameControls.I.Player.IsHoverTargetUsableNow;
	}

	private void Update()
	{
		if (WorldController.I.Player == null || WorldController.I.Player.IsDead)
		{
			return;
		}
		if (Input.GetButtonDown("Fire1"))
		{
			if (_info != null)
			{
				if (_info.PlacebleId == "PlaceWoodenFoundation")
				{
					Use();
				}
			}
		}

		HotKeyProcess();
		ShowHand();
		bool flag = true;
		if (Physics.Raycast(GameControls.I.Player.mainCamTransform.position, GameControls.I.Player.mainCamTransform.TransformDirection(Vector3.forward), out _cursorHitInfo, _useableRayDistance, _useableObjLayer))
		{
			if (_cursorHitInfo.collider.tag == "ForAction")
			{
				GameControls.I.Player.IsHoverTargetUsableNow = true;
				PhotonWorldObject component = _cursorHitInfo.collider.transform.parent.transform.parent.GetComponent<PhotonWorldObject>();
				if (component != null && component.Action.CanUse)
				{
					SetCursor(component.ActionType);
					if (MyUseButtonDown())
					{
						PhotonView component2 = component.GetComponent<PhotonView>();
						if (component2 != null)
						{
							cacheHoverViewId = component2.viewID;
						}
						component.UseObject();
					}
					flag = false;
				}
				else
				{
					PhotonObject component3 = _cursorHitInfo.collider.transform.parent.transform.parent.GetComponent<PhotonObject>();
					if (component3 != null && component3.ActionType != 0)
					{
						SetCursor(component3.ActionType);
						if (MyUseButtonDown())
						{
							component3.Use();
						}
						flag = false;
					}
				}
			}
			else if (_info != null && _info.ActionType == ConsumableActionType.UsableOnObject)
			{
				PhotonWorldObject component4 = _cursorHitInfo.collider.transform.parent.transform.parent.GetComponent<PhotonWorldObject>();
				if ((bool)component4 && _info.CanBeUsedOn.Contains(component4.ObjInfo.ObjectId))
				{
					SetCursor(WorldObjectActionType.Activate);
					if (MyUseButtonDown())
					{
						component4.UseItemOn(_info.UseOnObjectActionType);
					}
					flag = false;
				}
			}
		}
		else if (!GameControls.I.Player.IsHoverTargetUsableNow)
		{
			GameControls.I.Player.StartSetUsableFalse();
		}
		if (flag && _myInfoCursor.activeSelf)
		{
			EnableInfoCursor(false);
		}
		if (_info != null && _info.PlacebleId != "PlaceWoodenFoundation")
		{
			if(Input.GetButtonDown("Fire1"))
			{
				Use();
			}
		}
	}
}
