using UnityEngine;

public class EquipmentSlot : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite _icon;

	[SerializeField]
	private string _key;

	[SerializeField]
	private string _defaultSprite;

	private string _id;

	public string Key
	{
		get
		{
			return _key;
		}
	}

	public void UpdateView(string id = null)
	{
		_id = id;
		if (_defaultSprite == null)
		{
			_defaultSprite = _icon.CurrentSprite.name;
		}
		string text = null;
		if (!string.IsNullOrEmpty(id))
		{
			text = DataKeeper.Info.GetItemInfo(id).Icon;
			if (_icon.GetSpriteIdByName(text) == 0)
			{
				text = DataKeeper.NoIcon;
			}
		}
		_icon.SetSprite(text ?? _defaultSprite);
	}

	private void OnClick()
	{
		byte ammo = 0;
		if (DataKeeper.Info.GetWeaponInfo(_id) != null)
		{
			int weaponIndex = DataKeeper.I.GetWeaponIndex(_id);
			if (weaponIndex < GameControls.I.PlayerWeapons.WeaponsBehavioursList.Count)
			{
				ammo = (byte)GameControls.I.PlayerWeapons.WeaponsBehavioursList[weaponIndex].bulletsLeft;
			}
		}
		Item itemInfo = DataKeeper.Info.GetItemInfo(_id);
		if ((itemInfo.Type != 0 && itemInfo.Type != ItemType.Consumables) || !(GameControls.I != null) || !(GameControls.I.Walker != null) || !GameControls.I.Walker.climbing)
		{
			InventoryController.Instance.TakeOff(_key, true, false, false, ammo);
			UpdateView();
		}
	}
}
