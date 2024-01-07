using System.Collections.Generic;
using UnityEngine;

public class EquipmentViewController : MonoBehaviour
{
    [SerializeField]
	private List<EquipmentSlot> _equipmentSlots;

	private void OnEnable()
	{
		UpdateAllView();
	}

	private void UpdateAllView()
	{
		foreach (EquipmentSlot equipmentSlot in _equipmentSlots)
		{
			if (InventoryController.Instance.Equipment.ContainsKey(equipmentSlot.Key))
			{
				equipmentSlot.UpdateView(InventoryController.Instance.Equipment[equipmentSlot.Key]);
			}
			else
			{
				equipmentSlot.UpdateView();
			}
		}
	}

	public void EquipItem(string slotType, string id)
	{
		EquipmentSlot equipmentSlot = _equipmentSlots.Find((EquipmentSlot s) => s.Key == slotType);
		if (equipmentSlot != null)
		{
			equipmentSlot.UpdateView(id);
		}
	}
}
