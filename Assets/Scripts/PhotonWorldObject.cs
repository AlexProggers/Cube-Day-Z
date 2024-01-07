using System.Collections.Generic;
using System.Globalization;
using JsonFx.Json;
using Photon;
using UnityEngine;

public class PhotonWorldObjInfo
{
	public int OwnerUId;

	public string ObjectId;

	public int CurrentHealthPoint;
}

public class PhotonWorldObject : Photon.MonoBehaviour
{
	[SerializeField]
	private bool _localSpawn;

	[SerializeField]
	private string _localDestructedObjId;

	[SerializeField]
	private bool _canBeCompletelyDestroyed;

	[SerializeField]
	private GameObject _normalModel;

	[SerializeField]
	private GameObject _destructedModel;

	[SerializeField]
	private WorldObjectActionType _worldObjectActionType;

	[SerializeField]
	private WorldObjectTrap _trap;

	private DestructibleObject _info;

	private BaseWorldObjectAction _action;

	private short _indexInBase;

	public double insta_stamp;

	public WorldObjectActionType ActionType
	{
		get
		{
			return _worldObjectActionType;
		}
	}

	public PhotonWorldObjInfo ObjInfo { get; private set; }

	public DestructibleObjectType DestructibleType
	{
		get
		{
			return (_info != null) ? _info.Type : DestructibleObjectType.Other;
		}
	}

	public BaseWorldObjectAction Action
	{
		get
		{
			return _action;
		}
	}

	public bool IsLocal
	{
		get
		{
			return _localSpawn;
		}
	}

	public GameObject NormalModel
	{
		get
		{
			return _normalModel;
		}
	}

	private void Awake()
	{
		WorldController.I.WorldObjects.Add(this);
		if (_localSpawn)
		{
			_info = DataKeeper.Info.GetDestructibleObjectInfo(_localDestructedObjId);
			ObjInfo = new PhotonWorldObjInfo
			{
				ObjectId = _info.Id,
				CurrentHealthPoint = _info.HealthPoint
			};
			InitializeActions();
		}
	}

	private void OnDestroy()
	{
		if (WorldController.I.WorldObjects.Contains(this))
		{
			WorldController.I.WorldObjects.Remove(this);
		}
		string id = DataKeeper.Info.DestructibleObjects[_indexInBase].Id;
		if (!string.IsNullOrEmpty(id) && (id.ToLower().Contains("crate") || id.ToLower().Contains("chest") || id.ToLower().Contains("locker")) && HandUseForRealistic.I != null && base.photonView.viewID == HandUseForRealistic.I.cacheHoverViewId)
		{
			Debug.Log("выкллючаем UI ");
			if (GameUIController.I != null)
			{
				GameUIController.I.ShowCharacterMenu(false, CharacterMenuType.Menu);
			}
		}
	}

	private void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		insta_stamp = info.timestamp;
		if (_localSpawn)
		{
			return;
		}
		if (base.photonView.instantiationData[0] != null)
		{
			_indexInBase = (short)base.photonView.instantiationData[1];
			ObjInfo = new PhotonWorldObjInfo
			{
				OwnerUId = (int)base.photonView.instantiationData[0],
				ObjectId = DataKeeper.Info.DestructibleObjects[_indexInBase].Id
			};
			_info = DataKeeper.Info.GetDestructibleObjectInfo(ObjInfo.ObjectId);
			ObjInfo.CurrentHealthPoint = _info.HealthPoint;
			InitializeActions();
			string[] array = null;
			byte[] array2 = null;
			byte[] array3 = null;
			if (base.photonView.instantiationData[2] != null)
			{
				short[] itemsIndices = (short[])base.photonView.instantiationData[2];
				array = WorldController.I.GetItemsIdsByIndices(itemsIndices);
			}
			if (base.photonView.instantiationData[3] != null)
			{
				array2 = (byte[])base.photonView.instantiationData[3];
			}
			if (base.photonView.instantiationData[4] != null)
			{
				array3 = (byte[])base.photonView.instantiationData[4];
			}
			if (array != null)
			{
				WorldObjectStorageAction worldObjectStorageAction = Action as WorldObjectStorageAction;
				if (worldObjectStorageAction != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						if (array[i] != null && (float)Mathf.Abs((int)(PhotonNetwork.time - insta_stamp)) < 2f)
						{
							worldObjectStorageAction.ChangeStorageItem(i, array[i], array2[i], array3[i], false);
						}
					}
				}
			}
		}
		base.transform.parent = WorldController.I.DestructibleObjects;
		double time = ((!PhotonNetwork.offlineMode) ? info.timestamp : ((double)Time.time));
		if (_trap != null)
		{
			_trap.OnInstantiate(time);
		}
	}

	private void OnPhotonPlayerConnected()
	{
		if (PhotonNetwork.isMasterClient && ObjInfo.CurrentHealthPoint < _info.HealthPoint)
		{
			base.photonView.RPC("SyncInfo", PhotonTargets.All, ObjInfo.CurrentHealthPoint);
		}
	}

	public void Hit(int damage, List<DestructibleObjectType> type)
	{
		if (_info.Type == DestructibleObjectType.Other || type.Contains(_info.Type))
		{
			base.photonView.RPC("HitObject", PhotonTargets.All, damage);
		}
	}

	private void InitializeActions()
	{
		switch (ActionType)
		{
		case WorldObjectActionType.Storage:
			_action = base.gameObject.AddComponent<WorldObjectStorageAction>();
			break;
		case WorldObjectActionType.Farming:
			_action = base.gameObject.AddComponent<WorldObjectFarmingAction>();
			break;
		case WorldObjectActionType.Activate:
			_action = base.gameObject.AddComponent<WorldObjectActivateAction>();
			break;
		}
	}

	public void UseObject()
	{
		if (_action != null)
		{
			_action.Use();
		}
	}

	public void UseItemOn(UseOnObjectActionType actionType)
	{
		switch (actionType)
		{
		case UseOnObjectActionType.FastGrow:
		{
			PlantController component = GetComponent<PlantController>();
			if ((bool)component)
			{
				component.FastGrow();
			}
			break;
		}
		case UseOnObjectActionType.Repair:
			break;
		case UseOnObjectActionType.AddFuel:
			break;
		}
	}

	[PunRPC]
	private void DestroyByMaster()
	{
		if (PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("Destruct", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void HitObject(int damage)
	{
		ObjInfo.CurrentHealthPoint -= damage;
		if (_action != null)
		{
			_action.OnHit();
		}
		if (PhotonNetwork.isMasterClient && ObjInfo.CurrentHealthPoint <= 0)
		{
			base.photonView.RPC("Destruct", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void Destruct()
	{
		if (_normalModel != null)
		{
			Object.Destroy(_normalModel);
		}
		if (_canBeCompletelyDestroyed)
		{
			if ((bool)_destructedModel)
			{
				GameObject gameObject = (GameObject)Object.Instantiate(_destructedModel, _destructedModel.transform.position, _destructedModel.transform.rotation);
				gameObject.SetActive(true);
			}
			if (PhotonNetwork.isMasterClient)
			{
				PhotonNetwork.Destroy(base.gameObject);
			}
		}
		else if ((bool)_destructedModel)
		{
			_destructedModel.SetActive(true);
		}
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		if (_info.DropItems != null)
		{
			foreach (DropItem dropItem in _info.DropItems)
			{
				Item itemInfo = DataKeeper.Info.GetItemInfo(dropItem.ItemId);
				int valueInt = dropItem.Count.GetValueInt();
				Vector3 vector = ((!_destructedModel) ? Vector3.zero : _destructedModel.GetComponent<Renderer>().bounds.size);
				float x = UnityEngine.Random.Range(-0.5f, 0.5f);
				float z = UnityEngine.Random.Range(-0.5f, 0.5f);
				Vector3 position = new Vector3(base.transform.position.x, base.transform.position.y + vector.y + 1f, base.transform.position.z);
				GameObject gameObject2 = (GameObject)Object.Instantiate(Resources.Load("DropTest"), position, Quaternion.identity);
				gameObject2.GetComponent<DropTestController>().Initialize(new DropTestInfo(itemInfo.Id, valueInt, "PhotonItem", new Vector3(x, 1f, z) * 100f));
			}
		}
		if (_action != null)
		{
			_action.OnObjectDestroy();
		}
	}

	[PunRPC]
	private void SyncInfo(int hp)
	{
		ObjInfo.CurrentHealthPoint = hp;
		if (ObjInfo.CurrentHealthPoint <= 0)
		{
			Destruct();
		}
	}
}
