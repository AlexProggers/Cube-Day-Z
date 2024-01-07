using System.Collections.Generic;
using UnityEngine;

public class ModelClothingController : MonoBehaviour
{
    	private const int IgnoreRaycastLayer = 2;

	[SerializeField]
	private Transform _headwearHolder;

	[SerializeField]
	private Transform _backpackHolder;

	[SerializeField]
	private Transform _vestHolder;

	[SerializeField]
	private Transform _bodyHolder;

	[SerializeField]
	private Renderer _characterRenderer;

	[SerializeField]
	private ThirdPlayerModelController _modelController;

	[SerializeField]
	private GameObject _rainCoatManager;

	private Dictionary<ClothingBodyPart, GameObject> _currentEquipedModels = new Dictionary<ClothingBodyPart, GameObject>();

	private GameObject _itemInHand;

	private string _itemInHandId;

	private CharacterViewInfo _viewInfo;

	public PositionInHandInfo ItemInHandViewInfo { get; private set; }

	public string ItemInHandId
	{
		get
		{
			return _itemInHandId;
		}
	}

    public bool IsEquiped(ClothingBodyPart bodyPart)
	{
		if (_currentEquipedModels.ContainsKey(bodyPart) && _currentEquipedModels[bodyPart] != null)
		{
			return true;
		}
		return false;
	}

    	public void SetRagdollView(GameObject ragdoll)
	{
		RagdollViewController component = ragdoll.GetComponent<RagdollViewController>();
		if ((bool)component)
		{
			component.SetView(_characterRenderer.material);
		}
	}

	public void Equip(ClothingBodyPart bodyPart, string info, bool isPlayer = false)
	{
		if (!string.IsNullOrEmpty(info))
		{
			switch (bodyPart)
			{
			case ClothingBodyPart.Bodywear:
			case ClothingBodyPart.Legwear:
				EquipTexture(bodyPart, info);
				break;
			case ClothingBodyPart.Backpack:
			case ClothingBodyPart.Headwear:
			case ClothingBodyPart.Vest:
				EquipModel(bodyPart, info, isPlayer);
				break;
			}
		}
	}

	public void AddInHand(string info)
	{
		if (!string.IsNullOrEmpty(info))
		{
			if (_itemInHand != null)
			{
				Object.Destroy(_itemInHand);
			}
			Item itemInfo = DataKeeper.Info.GetItemInfo(info);
			_itemInHandId = info;
			if (itemInfo != null)
			{
				_itemInHand = GameObject.Instantiate(Resources.Load<GameObject>(WorldController.I.GetResourceLoadPath(itemInfo.Type) + itemInfo.Prefab));
				ItemInHandViewInfo = _itemInHand.GetComponent<PositionInHandInfo>();
				_itemInHand.transform.parent = _bodyHolder;
				_itemInHand.transform.localPosition = ItemInHandViewInfo.ItemPositionOffset;
				_itemInHand.transform.localRotation = Quaternion.Euler(ItemInHandViewInfo.ItemRotationOffset);
				_itemInHand.transform.localScale = ItemInHandViewInfo.ItemScaleOffset;
				ChangeLayer(_itemInHand);
				_modelController.EnableIk(ItemInHandViewInfo.LeftHandPoint, ItemInHandViewInfo.RightHandPoint);
			}
		}
	}

    public void AddInHand(Item item)
	{
		if (item != null)
		{
			if (_itemInHand != null)
			{
				Object.Destroy(_itemInHand);
			}
			_itemInHandId = item.Id;
			_itemInHand = GameObject.Instantiate(Resources.Load<GameObject>("Weapon/" + item.Prefab));
			ItemInHandViewInfo = _itemInHand.GetComponent<PositionInHandInfo>();
			_itemInHand.transform.parent = _bodyHolder;
			_itemInHand.transform.localPosition = ItemInHandViewInfo.ItemPositionOffset;
			_itemInHand.transform.localRotation = Quaternion.Euler(ItemInHandViewInfo.ItemRotationOffset);
			_itemInHand.transform.localScale = ItemInHandViewInfo.ItemScaleOffset;
			ChangeLayer(_itemInHand);
			_modelController.EnableIk(ItemInHandViewInfo.LeftHandPoint, ItemInHandViewInfo.RightHandPoint);
		}
	}

	private void ChangeLayer(GameObject obj)
	{
		obj.layer = 2;
		foreach (Transform item in obj.transform)
		{
			ChangeLayer(item.gameObject);
		}
	}

    public void RemoveFromHand()
	{
		if (_itemInHand != null)
		{
			Object.Destroy(_itemInHand);
		}
		_itemInHandId = null;
		ItemInHandViewInfo = null;
		_modelController.DisableIk();
	}

	public void TakeOff(ClothingBodyPart bodyPart)
	{
		switch (bodyPart)
		{
		case ClothingBodyPart.Bodywear:
		case ClothingBodyPart.Legwear:
			TakeOffTexture(bodyPart);
			break;
		case ClothingBodyPart.Backpack:
		case ClothingBodyPart.Headwear:
		case ClothingBodyPart.Vest:
			TakeOffModel(bodyPart);
			break;
		}
	}

    	private void EquipTexture(ClothingBodyPart bodyPart, string id)
	{
		SetTexture(bodyPart, id);
	}

	private void SetTexture(ClothingBodyPart bodyPart, string textureId)
	{
		Texture clothingTexture = DataKeeper.I.GetClothingTexture(textureId);
		string propertyName = string.Empty;

		switch (bodyPart)
		{
		case ClothingBodyPart.Bodywear:
			propertyName = "_BodyTex";
			//_viewInfo.BodyTexId = textureId;
			break;
		case ClothingBodyPart.Legwear:
			propertyName = "_LegsTex";
			//_viewInfo.LegsTexId = textureId;
			break;
		}
		_characterRenderer.material.SetTexture(propertyName, clothingTexture);
	}

    private void TakeOffTexture(ClothingBodyPart bodyPart)
	{
		SetTexture(bodyPart, null);
	}

    private void EquipModel(ClothingBodyPart bodyPart, string model, bool isPlayer = false)
	{
		TakeOffModel(bodyPart);
		Transform parent = GetParent(bodyPart);
		if (!(parent != null))
		{
			return;
		}
		GameObject gameObject = GameObject.Instantiate(Resources.Load<GameObject>("Clothing/" + model));
		gameObject.transform.parent = parent;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		if (isPlayer)
		{
			ItemEffectInitializer component = gameObject.GetComponent<ItemEffectInitializer>();
			if ((bool)component)
			{
				component.Initialize();
			}
		}
		_currentEquipedModels.Add(bodyPart, gameObject);
	}

	private void TakeOffModel(ClothingBodyPart bodyPart)
	{
		if (_currentEquipedModels.ContainsKey(bodyPart))
		{
			Object.Destroy(_currentEquipedModels[bodyPart]);
			_currentEquipedModels.Remove(bodyPart);
		}
	}

	private Transform GetParent(ClothingBodyPart bodyPart)
	{
		switch (bodyPart)
		{
		case ClothingBodyPart.Headwear:
			return _headwearHolder;
		case ClothingBodyPart.Vest:
			return _vestHolder;
		case ClothingBodyPart.Backpack:
			return _backpackHolder;
		default:
			return null;
		}
	}
}