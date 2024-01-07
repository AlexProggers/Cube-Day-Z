using UnityEngine;

public class LocalDropObject : MonoBehaviour
{
	private GameObject _model;

	public PhotonDropObjInfo ObjInfo { get; private set; }

	public void Initialize(string itemId, byte count, byte additionalCount)
	{
		ObjInfo = new PhotonDropObjInfo
		{
			ObjectId = itemId,
			Amount = count,
			AdditionalCount = additionalCount
		};
		base.transform.parent = WorldController.I.Items;
		WorldController.I.LocalDropObjects.Add(this);
		SetViewModel();
	}

	private void OnDestroy()
	{
		if (!(WorldController.I == null) && WorldController.I.LocalDropObjects.Contains(this))
		{
			WorldController.I.LocalDropObjects.Remove(this);
		}
	}

	private void SetViewModel()
	{
		Item itemInfo = DataKeeper.Info.GetItemInfo(ObjInfo.ObjectId);
		if (itemInfo != null)
		{
			_model = GameObject.Instantiate(Resources.Load<GameObject>(WorldController.I.GetResourceLoadPath(itemInfo.Type) + itemInfo.Prefab));
			_model.transform.parent = base.transform;
			_model.transform.localPosition = Vector3.zero;
			_model.transform.localRotation = Quaternion.identity;
			if (_model.GetComponent<Collider>() != null)
			{
				_model.GetComponent<Collider>().enabled = true;
			}
		}
		else
		{
			Debug.Log("BUG IN ITEM " + ObjInfo.ObjectId);
		}
	}

	public void PickUpItem()
	{
		if (ObjInfo == null)
		{
			return;
		}

		Item itemInfo = DataKeeper.Info.GetItemInfo(ObjInfo.ObjectId);

		int num = InventoryController.Instance.AddItems(itemInfo, ObjInfo.Amount, (byte)UnityEngine.Random.Range(0, 12));
		int num2 = Mathf.Max(0, ObjInfo.Amount - num);

		if (num > 0)
		{
			if (num2 > 0)
			{
				ObjInfo.Amount = (byte)Mathf.Max(0, ObjInfo.Amount - num2);
				GameControls.I.Player.PlayPickSnd();
			}
		}
		else
		{
			Object.Destroy(base.gameObject);
			GameControls.I.Player.PlayPickSnd();
			return;
		}

		GameControls.I.Player.PlayNoPickSnd();
	}
}
