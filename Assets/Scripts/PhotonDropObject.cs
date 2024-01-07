using System.Collections;
using Photon;
using UnityEngine;

public class PhotonDropObjInfo
{
	public string ObjectId;

	public byte Amount;

	public bool EnableLifeTime;

	public byte AdditionalCount;
}

public class PhotonDropObject : Photon.MonoBehaviour
{
	private float _currentLifeTime;

	private double _dropTime;

	private GameObject _model;

	private bool _canPickUp = true;

	public PhotonDropObjInfo ObjInfo { get; private set; }

	private void OnDestroy()
	{
		if (!(WorldController.I == null))
		{
			if (WorldController.I.WorldItems.Contains(this))
			{
				WorldController.I.WorldItems.Remove(this);
			}
			if (PhotonNetwork.isMasterClient)
			{
				StopCoroutine("ResetItem");
			}
		}
	}

	private void SetViewModel()
	{
		Item itemInfo = DataKeeper.Info.GetItemInfo(ObjInfo.ObjectId);
		_model = GameObject.Instantiate(Resources.Load<GameObject>(WorldController.I.GetResourceLoadPath(itemInfo.Type) + itemInfo.Prefab));
		_model.transform.parent = base.transform;
		_model.transform.localPosition = Vector3.zero;
		_model.transform.localRotation = Quaternion.identity;
		if (_model.GetComponent<Collider>() != null)
		{
			_model.GetComponent<Collider>().enabled = true;
		}
	}

	private void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		if ((string)base.photonView.instantiationData[0] != null)
		{
			_dropTime = ((!PhotonNetwork.offlineMode) ? info.timestamp : ((double)Time.time));
			ObjInfo = new PhotonDropObjInfo
			{
				ObjectId = (string)base.photonView.instantiationData[0],
				Amount = (byte)base.photonView.instantiationData[1],
				EnableLifeTime = (bool)base.photonView.instantiationData[2]
			};
			if (base.photonView.instantiationData[3] != null)
			{
				ObjInfo.AdditionalCount = (byte)base.photonView.instantiationData[3];
			}
			_currentLifeTime = DataKeeper.DefaultPhotonItemLifetime;
			base.transform.parent = WorldController.I.Items;
			WorldController.I.WorldItems.Add(this);
			SetViewModel();
		}
	}

	public void PickUpItem()
	{
		if (WorldController.I != null && WorldController.I.Player != null)
		{
			GetItemLocal();
		}
		else
		{
			base.photonView.RPC("CheckIfCanGetItem", PhotonTargets.MasterClient);
		}
	}

	[PunRPC]
	public void GetItem()
	{
		GetItemLocal();
	}

	private void GetItemLocal()
	{
		if (ObjInfo == null)
		{
			return;
		}
		Item itemInfo = DataKeeper.Info.GetItemInfo(ObjInfo.ObjectId);

		int num = InventoryController.Instance.AddItems(itemInfo, ObjInfo.Amount, ObjInfo.AdditionalCount);
		int num2 = Mathf.Max(0, ObjInfo.Amount - num);
		if (num > 0)
		{
			if (num2 > 0)
			{
				base.photonView.RPC("SyncAmount", PhotonTargets.All, (byte)num2);
			}
		}
		else
		{
			GameControls.I.Player.PlayPickSnd();
			_model.SetActive(false);
			base.photonView.RPC("DestroyByMaster", PhotonTargets.MasterClient);
			return;
		}

		GameControls.I.Player.PlayNoPickSnd();
	}

	private void Update()
	{
		if (ObjInfo != null && PhotonNetwork.isMasterClient && ObjInfo.EnableLifeTime)
		{
			_currentLifeTime = DataKeeper.DefaultPhotonItemLifetime - (float)(PhotonNetwork.time - _dropTime);
			if (_currentLifeTime <= 0f)
			{
				PhotonNetwork.Destroy(base.gameObject);
			}
		}
	}

	[PunRPC]
	private void CheckIfCanGetItem(PhotonMessageInfo info)
	{
		if (PhotonNetwork.isMasterClient && _canPickUp)
		{
			_canPickUp = false;
			base.photonView.RPC("GetItem", info.sender);
			StartCoroutine("ResetItem");
		}
	}

	[PunRPC]
	private void DestroyByMaster()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PhotonNetwork.Destroy(base.gameObject);
		}
	}

	[PunRPC]
	private void SyncAmount(byte removeValue)
	{
		ObjInfo.Amount = (byte)Mathf.Max(0, ObjInfo.Amount - removeValue);
	}

	private IEnumerator ResetItem()
	{
		yield return new WaitForSeconds(5f);
		if (PhotonNetwork.isMasterClient)
		{
			_canPickUp = true;
		}
	}
}
