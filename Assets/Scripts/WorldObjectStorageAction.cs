using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObjectStorageAction : BaseWorldObjectAction
{
	private PhotonWorldObject _photonObject;

	private List<InventorySlot> _storageSlots = new List<InventorySlot>();

	private int _storageCount = 6;

	private float _waitForSaveAfterBuild = 5f;

	private Coroutine _saveAfterChangeCoroutine;

	public List<InventorySlot> StorageContent
	{
		get
		{
			return _storageSlots;
		}
	}

	public int MaxStorageCount
	{
		get
		{
			return _storageCount;
		}
	}

	public override bool CanUse
	{
		get
		{
			return _photonObject.ObjInfo.OwnerUId == 0 || _photonObject.ObjInfo.OwnerUId == DataKeeper.backendInfo.playerId;
		}
	}

	private void Awake()
	{
		_photonObject = GetComponent<PhotonWorldObject>();
		for (int i = 0; i < _storageCount; i++)
		{
			_storageSlots.Add(new InventorySlot());
		}
	}

	public override void Use()
	{
		//GameUIController.I.ShowStorageInventory(this);
	}

	public override void OnObjectDestroy()
	{
		foreach (InventorySlot storageSlot in _storageSlots)
		{
			if (storageSlot.Item != null)
			{
				float x = UnityEngine.Random.Range(-1.5f, 1.5f);
				float z = UnityEngine.Random.Range(-1.5f, 1.5f);
				Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y + 1f, base.transform.position.z);
				GameObject gameObject = (GameObject)Object.Instantiate(Resources.Load("DropTest"), position, Quaternion.identity);
				gameObject.GetComponent<DropTestController>().Initialize(new DropTestInfo(storageSlot.Item.Id, storageSlot.Count, "PhotonItem", new Vector3(x, 1f, z) * 100f));
			}
		}
	}

	public int GetItemsCount(string id)
	{
		int num = 0;
		List<InventorySlot> list = StorageContent.FindAll((InventorySlot slot) => slot.ContainsItem(id));
		foreach (InventorySlot item in list)
		{
			num += (int)item.Count;
		}
		return num;
	}

	public void ChangeStorageItem(int slotIndex, string id, int count, byte ammo, bool removeFromInventory = true, bool save = false)
	{
		if (slotIndex < 0)
		{
			return;
		}
		if (removeFromInventory)
		{
			int num = count * -1;
			if (num < 0)
			{
				InventoryController.Instance.Slots[slotIndex].Remove(Mathf.Abs(num));
			}
			else
			{
				Item itemInfo = DataKeeper.Info.GetItemInfo(id);
				int num2 = InventoryController.Instance.AddItems(itemInfo, num, ammo);
				if (num2 > 0)
				{
					WorldController.I.Player.PlayerDropItem(base.transform.position, id, num2, ammo);
				}
			}
			GameUIController.I.Inventory.UpdateCurrentPage();
		}
		base.photonView.RPC("ChangeItemInStorage", PhotonTargets.All, (byte)slotIndex, id, count, ammo);
		if (save && DataKeeper.GameType == GameType.Single && _saveAfterChangeCoroutine == null)
		{
			_saveAfterChangeCoroutine = StartCoroutine(SaveAfterChange());
		}
	}

	private void AddStorageItems(string itemId, int count, byte ammo)
	{
		if (count != 0)
		{
			InventorySlot inventorySlot = StorageContent.Find((InventorySlot s) => s.Item == null || (s.Item.Id == itemId && (int)s.MaxItemCount - (int)s.Count >= count));
			if (inventorySlot != null)
			{
				Item itemInfo = DataKeeper.Info.GetItemInfo(itemId);
				inventorySlot.AddSome(itemInfo, count, ammo);
			}
		}
	}

	private void RemoveStorageItems(byte slotIndex, string itemId, int count)
	{
		if (count != 0 && slotIndex < StorageContent.Count)
		{
			StorageContent[slotIndex].Remove(Mathf.Abs(count));
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		for (byte b = 0; b < StorageContent.Count; b++)
		{
			if (StorageContent[b].Item != null && (int)StorageContent[b].Count > 0)
			{
				base.photonView.RPC("ChangeItemInStorage", player, b, StorageContent[b].Item.Id, (int)StorageContent[b].Count, (byte)StorageContent[b].Ammo);
			}
		}
	}

	private IEnumerator SaveAfterChange()
	{
		yield return new WaitForSeconds(_waitForSaveAfterBuild);
		_saveAfterChangeCoroutine = null;
	}

	[PunRPC]
	private void ChangeItemInStorage(byte slotIndex, string id, int count, byte ammo)
	{
		if (count < 0)
		{
			RemoveStorageItems(slotIndex, id, count);
		}
		else
		{
			AddStorageItems(id, count, ammo);
		}
		//GameUIController.I.Inventory.StorageView.UpdateView();
	}

	private void ChangeItemInStorage2(byte slotIndex, string id, int count, byte ammo)
	{
		if (count < 0)
		{
			RemoveStorageItems(slotIndex, id, count);
		}
		else
		{
			AddStorageItems(id, count, ammo);
		}
		//GameUIController.I.Inventory.StorageView.UpdateView();
	}
}
