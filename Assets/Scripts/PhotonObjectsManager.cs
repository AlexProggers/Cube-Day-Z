using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PhotonObjectsManager : Photon.MonoBehaviour
{
	[SerializeField]
	private float _syncInterval;

	[HideInInspector]
	public List<PhotonObject> Objects = new List<PhotonObject>();

	public void Hit(short index, short damage)
	{
		base.photonView.RPC("HitObject", PhotonTargets.All, index, damage);
	}

	public void Activate(short index, bool activate)
	{
		base.photonView.RPC("ActivateObject", PhotonTargets.All, index, activate);
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient)
		{
			StartCoroutine(SyncInfo(player));
		}
	}

	private IEnumerator SyncInfo(PhotonPlayer player)
	{
		for (short i = 0; i < Objects.Count; i++)
		{
			if (Objects[i].CurrentHp < Objects[i].Info.HealthPoint)
			{
				base.photonView.RPC("HitObject", player, i, (short)(Objects[i].Info.HealthPoint - Objects[i].CurrentHp));
				yield return new WaitForSeconds(_syncInterval);
			}
			if (Objects[i].ActionType == WorldObjectActionType.Activate && Objects[i].IsActivated)
			{
				base.photonView.RPC("ActivateObject", player, i, Objects[i].IsActivated);
				yield return new WaitForSeconds(_syncInterval);
			}
		}
	}

	[PunRPC]
	private void ActivateObject(short index, bool activate)
	{
		if (index < Objects.Count && Objects[index].CurrentHp > 0)
		{
			Objects[index].IsActivated = activate;
			if (Objects[index].Animation != null)
			{
				string animation = ((!activate) ? "Disable" : "Enable");
				Objects[index].Animation.Play(animation);
			}
		}
	}

	[PunRPC]
	private void HitObject(short index, short damage)
	{
		if (index >= Objects.Count || Objects[index].CurrentHp <= 0)
		{
			return;
		}
		Objects[index].CurrentHp -= damage;
		if (!PhotonNetwork.isMasterClient || Objects[index].CurrentHp > 0 || Objects[index].Info.DropItems == null)
		{
			return;
		}
		foreach (DropItem dropItem in Objects[index].Info.DropItems)
		{
			Item itemInfo = DataKeeper.Info.GetItemInfo(dropItem.ItemId);
			int valueInt = dropItem.Count.GetValueInt();
			Vector3 origin = Objects[index].transform.position + Objects[index].transform.up * 3f;
			RaycastHit hitInfo;
			if (Physics.Raycast(origin, -Objects[index].transform.up, out hitInfo, 100f, DataKeeper.I.ItemsCollisionsMask))
			{
				Quaternion rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, hitInfo.normal), Vector3.Cross(Vector3.up, hitInfo.normal));
				PhotonNetwork.InstantiateSceneObject("PhotonItem", hitInfo.point, rotation, 0, new object[4]
				{
					itemInfo.Id,
					(byte)valueInt,
					true,
					(byte)0
				});
			}
		}
	}
}
