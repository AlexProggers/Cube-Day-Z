using System.Collections.Generic;
using System.Linq;

public enum ItemType
{
	Weapon = 0,
	Clothing = 1,
	Consumables = 2
}

public class Item
{
	public string Id { get; set; }

	public string Name { get; set; }

	public string Description { get; set; }

	public string RussianName { get; set; }

	public string RussianDescription { get; set; }

	public string Icon { get; set; }

	public string Prefab { get; set; }

	public virtual ItemType Type { get; private set; }

	public bool IsEquippable { get; set; }

	public int MaxInStack { get; set; }
}


public enum WeaponType
{
	Blunt = 0,
	Bladed = 1,
	Fire = 2,
	Pistol = 3,
	Bow = 4,
	Shotgun = 5,
	AssaultRifle = 6,
	SniperRifle = 7,
	Rifle = 8,
	SMG = 9
}

public enum AmmoType
{
	None = 0,
	Arrow = 1,
	Bolt = 2,
	Shells = 3,
	Mm9 = 4,
	Mm7_62 = 5,
	Mm5_56 = 6,
	Mm12_7 = 7,
	ACP_45 = 8
}

public enum WeaponActionType
{
	Melee = 0,
	Ranged = 1
}

public class Weapon : Item
{
	public WeaponType WeaponType;

	public WeaponActionType ActionType;

	public List<DestructibleObjectType> DestructObjOfypes;

	public int Damage;

	public float Range;

	public float Speed;

	public AmmoType AmmoType;

	public override ItemType Type
	{
		get
		{
			return ItemType.Weapon;
		}
	}
}

public enum ConsumableActionType
{
	None = 0,
	Placeable = 1,
	Useable = 2,
	UsableOnObject = 3,
	UsableForFarming = 4
}

public enum ZombieType
{
	Simple = 0,
	Infective = 1,
	Mega = 2
}

public class Mob
{
	public string Id;

	public string Prefab;

	public ZombieType Type;

	public float HealthPoint;

	public float Damage;

	public byte Sickness;

	public List<DropItem> DropItems;

	public int MaxItems;
}

public enum UseOnObjectActionType
{
	Repair = 0,
	FastGrow = 1,
	AddFuel = 2
}

public class Consumable : Item
{
	public ConsumableActionType ActionType;

	public string PlacebleId;

	public bool PlaceOrientedOnPlayer;

	public List<string> CanBeUsedOn;

	public UseOnObjectActionType UseOnObjectActionType;

	public Dictionary<string, int> ChangeStatesOnUse;

	public HallucinogenType HallucinogenType;

	public bool IsTwohanded;

	public override ItemType Type
	{
		get
		{
			return ItemType.Consumables;
		}
	}
}

public enum ClothingBodyPart : byte
{
	Backpack = 1,
	Headwear = 2,
	Bodywear = 3,
	Legwear = 4,
	Vest = 5
}

public class Clothing : Item
{
	public ClothingBodyPart BodyPart;

	public float ArmorFactor;

	public int AdditionalSlots;

	public override ItemType Type
	{
		get
		{
			return ItemType.Clothing;
		}
	}
}

public class DropItem
{
	public string ItemId;

	public VariableValue Count;
}

public enum DestructibleObjectType
{
	Other = 0,
	Tree = 1,
	Rock = 2,
	Metal = 3,
	Wood = 4,
	Stone = 5
}

public class DestructibleObject
{
	public string Id;

	public string Prefab;

	public string HitEffect;

	public DestructibleObjectType Type;

	public int HealthPoint;

	public List<DropItem> DropItems;
}

public class ContentInfo
{
	public List<DestructibleObject> DestructibleObjects;

	public List<Mob> Mobs;

	public List<Weapon> Weapons;

	public List<Clothing> Clothings;

	public List<Consumable> Consumables;

	public Dictionary<string, Item> Items;

	public List<Item> GetAllItems()
	{
		List<Item> list = Weapons.Cast<Item>().ToList();
		list.AddRange(Clothings.Cast<Item>());
		list.AddRange(Consumables.Cast<Item>());
		return list;
	}

	public void Initialize()
	{
		Items = new Dictionary<string, Item>();
		if (Weapons != null)
		{
			foreach (Weapon weapon in Weapons)
			{
				Items.Add(weapon.Id, weapon);
			}
		}
		if (Clothings != null)
		{
			foreach (Clothing clothing in Clothings)
			{
				Items.Add(clothing.Id, clothing);
			}
		}
		if (Consumables == null)
		{
			return;
		}
		foreach (Consumable consumable in Consumables)
		{
			Items.Add(consumable.Id, consumable);
		}
	}

	public Mob GetMobInfo(string id)
	{
		return Mobs.Find((Mob mob) => mob.Id == id);
	}

	public DestructibleObject GetDestructibleObjectInfo(string id)
	{
		return DestructibleObjects.Find((DestructibleObject o) => o.Id == id);
	}

	public Item GetItemInfo(string id)
	{
		if (Items.ContainsKey(id))
		{
			return Items[id];
		}
		return null;
	}

	public Weapon GetWeaponInfo(string id)
	{
		return GetItemInfo(id) as Weapon;
	}

	public Consumable GetConsumableInfo(string id)
	{
		return GetItemInfo(id) as Consumable;
	}

	public Clothing GetClothingInfo(string id)
	{
		return GetItemInfo(id) as Clothing;
	}
}
