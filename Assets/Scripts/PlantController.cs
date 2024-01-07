using System.Collections;
using Photon;
using UnityEngine;

public class PlantController : Photon.MonoBehaviour
{
	[SerializeField]
	private float _growTimeInSec = 60f;

	[SerializeField]
	private GameObject _plantModel;

	private int _plantSection;

	private string _plantId;

	private float _plantTime;

	private WorldObjectFarmingAction _farming;

	private void OnPhotonInstantiate()
	{
		if (base.photonView.instantiationData.Length <= 0)
		{
			return;
		}
		int greenhousePhotonId = (int)base.photonView.instantiationData[0];
		_plantSection = (int)base.photonView.instantiationData[1];
		_plantId = (string)base.photonView.instantiationData[2];
		_plantTime = (float)base.photonView.instantiationData[3];
		PhotonWorldObject photonWorldObject = WorldController.I.WorldObjects.Find((PhotonWorldObject obj) => obj.photonView.viewID == greenhousePhotonId);
		if (photonWorldObject != null)
		{
			_farming = photonWorldObject.Action as WorldObjectFarmingAction;
			if (_farming != null)
			{
				_farming.AddPlant(_plantSection, this);
				Vector3? section = _farming.Sections.GetSection(_plantSection);
				base.transform.parent = _farming.transform;
				base.transform.localRotation = Quaternion.identity;
				if (section.HasValue)
				{
					base.transform.localPosition = section.Value;
				}
			}
		}
		StartCoroutine("Grow");
	}

	private IEnumerator Grow()
	{
		float timeForGrow = _growTimeInSec - ((float)PhotonNetwork.time - _plantTime);
		if (timeForGrow > 0f)
		{
			yield return new WaitForSeconds(timeForGrow);
		}
		Ripe();
	}

	public void FastGrow()
	{
		StopAllCoroutines();
		Ripe();
	}

	private void Ripe()
	{
		if (PhotonNetwork.isMasterClient)
		{
			Item itemInfo = DataKeeper.Info.GetItemInfo(_plantId);
			_plantModel.SetActive(false);
			RaycastHit hitInfo;
			if (itemInfo != null && Physics.Raycast(base.transform.position + Vector3.up, -Vector3.up, out hitInfo, 10f, DataKeeper.I.ItemsCollisionsMask))
			{
				Quaternion rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, hitInfo.normal), Vector3.Cross(Vector3.up, hitInfo.normal));
				PhotonNetwork.InstantiateSceneObject("PhotonItem", hitInfo.point, rotation, 0, new object[4]
				{
					itemInfo.Id,
					(byte)1,
					false,
					(byte)0
				});
			}
			PhotonNetwork.Destroy(base.gameObject);
		}
	}
}
