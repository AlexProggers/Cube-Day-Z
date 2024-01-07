using System.Collections.Generic;
using UnityEngine;

public class WorldObjectFarmingAction : BaseWorldObjectAction
{
	private Dictionary<int, PlantController> _plants = new Dictionary<int, PlantController>();

	private WorldObjectSection _worldObjectSection;

	public WorldObjectSection Sections
	{
		get
		{
			return _worldObjectSection;
		}
	}

	public override bool CanUse
	{
		get
		{
			Consumable itemInHand = HandUseForRealistic.I.ItemInHand;
			return itemInHand != null && itemInHand.ActionType == ConsumableActionType.UsableForFarming;
		}
	}

	private void Awake()
	{
		_worldObjectSection = GetComponent<WorldObjectSection>();
		for (int i = 0; i < _worldObjectSection.SectionsCount; i++)
		{
			_plants.Add(i, null);
		}
	}

	public override void Use()
	{
		Debug.LogError("Hover 7777777777777777  ");
		Consumable itemInHand = HandUseForRealistic.I.ItemInHand;
		if (Plant(itemInHand.PlacebleId))
		{
			//WorldController.I.PlayerStatistic.PlantsPlanted++;
			if (InventoryController.Instance.GetItemsCount(itemInHand.Id) > 0)
			{
				InventoryController.Instance.RemoveItems(itemInHand.Id, 1);
			}
			else
			{
				InventoryController.Instance.TakeOff("Hand", false);
			}
		}
	}

	private bool Plant(string plantId)
	{
		if (GetKeyForPlant() >= 0)
		{
			base.photonView.RPC("PlantItemByMaster", PhotonTargets.MasterClient, plantId);
			return true;
		}
		return false;
	}

	public void AddPlant(int section, PlantController plant)
	{
		if (_plants.ContainsKey(section))
		{
			_plants[section] = plant;
		}
	}

	private int GetKeyForPlant()
	{
		int result = -1;
		foreach (KeyValuePair<int, PlantController> plant in _plants)
		{
			if (plant.Value == null)
			{
				result = plant.Key;
				break;
			}
		}
		return result;
	}

	[PunRPC]
	private void PlantItemByMaster(string plantId)
	{
		int keyForPlant = GetKeyForPlant();
		if (keyForPlant >= 0)
		{
			PhotonNetwork.InstantiateSceneObject("WorldObjects/" + _worldObjectSection.PlantPrefabName, base.transform.position, Quaternion.identity, 0, new object[4]
			{
				base.photonView.viewID,
				keyForPlant,
				plantId,
				(float)PhotonNetwork.time
			});
		}
	}
}
