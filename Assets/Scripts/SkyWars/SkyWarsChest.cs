using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;

namespace SkyWars
{
	public class SkyWarsChest : Photon.MonoBehaviour
	{
		[SerializeField]
		private bool _isOpen;

		[SerializeField]
		private Transform _topCover;

		private void OnTriggerEnter(Collider collider)
		{
			if (collider.name.Contains("Player"))
			{
				base.photonView.RPC("CheckChest", PhotonTargets.All);
			}
		}

		[PunRPC]
		public void CheckChest(PhotonMessageInfo info)
		{
			if (PhotonNetwork.isMasterClient && !_isOpen)
			{
				base.photonView.RPC("OpenChest", info.sender);
			}
		}

		[PunRPC]
		public void OpenChest()
		{
			StartCoroutine("SpawnLoot");
		}

		private IEnumerator SpawnLoot()
		{
			_isOpen = true;
			_topCover.localPosition = new Vector3(_topCover.localPosition.x, _topCover.localPosition.y, _topCover.localPosition.z - 0.8f);
			yield return new WaitForSeconds(2.5f);
			List<Item> items = DataKeeper.Info.GetAllItems();
			for (int i = 0; i < 4; i++)
			{
				int itemIndex = UnityEngine.Random.Range(0, items.Count);
				Item itemInfo = items[itemIndex];
				int additionalCount = ((itemInfo.Type == ItemType.Weapon) ? UnityEngine.Random.Range(0, 10) : 0);
				string id = itemInfo.Id;
				if (DataKeeper.Info.GetWeaponInfo(itemInfo.Id) == null && i > 2)
				{
					int j = DataKeeper.Info.Weapons.Count;
					int rand = UnityEngine.Random.Range(0, j);
					id = DataKeeper.Info.Weapons[rand].Id;
				}
				WorldController.I.InstantiateLocalItem(id, base.transform.position, Quaternion.identity, (byte)UnityEngine.Random.Range(1, itemInfo.MaxInStack), (byte)additionalCount);
				base.photonView.RPC("DestroyChest", PhotonTargets.All);
			}
		}

		[PunRPC]
		public void DestroyChest()
		{
			if (PhotonNetwork.isMasterClient && !_isOpen)
			{
				PhotonNetwork.Destroy(base.photonView);
			}
		}
	}
}
