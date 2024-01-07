using System.Collections.Generic;

public enum CraftingCategory
{
	None = 0,
	Supplies = 1,
	Components = 2,
	BarricadesTraps = 3,
	StorageUnits = 4,
	Structures = 5,
	Clothing = 6,
	MedicalSupplies = 7,
	FoodAndDrinks = 8,
	Farming = 9,
	Ammunition = 10,
	Attachments = 11,
	Gear = 12
}

public class CraftingRegistry
{
	private static CraftingRegistry _instance;

	public Dictionary<CraftingCategory, List<RecipeInfo>> Recipes = new Dictionary<CraftingCategory, List<RecipeInfo>>();

	public static CraftingRegistry Instance
	{
		get
		{
			return _instance ?? (_instance = new CraftingRegistry());
		}
	}

	public CraftingRegistry()
	{
		AddRecipe(CraftingCategory.Supplies, 1, "Log", 0, string.Empty, string.Empty, 4, "Board", 0);
		AddRecipe(CraftingCategory.Supplies, 1, "Board", 0, string.Empty, "saw", 4, "Stick", 0);
		AddRecipe(CraftingCategory.Supplies, 1, "Scrap Metal", 0, string.Empty, string.Empty, 2, "Nails", 0);
		AddRecipe(CraftingCategory.Supplies, 1, "Scrap Metal", 0, string.Empty, "Campfire", 2, "Bolts", 0);
		AddRecipe(CraftingCategory.Supplies, 2, "Stick", 0, string.Empty, "axe", 1, "Wooden Spike", 0);
		AddRecipe(CraftingCategory.Supplies, 2, "Stick", 0, string.Empty, "fireAxe", 1, "Wooden Spike", 0);
		AddRecipe(CraftingCategory.Supplies, 1, "Rocks", 0, string.Empty, string.Empty, 3, "Stone", 0);
		AddRecipe(CraftingCategory.Supplies, 2, "Stone", 0, string.Empty, "Campfire", 1, "Scrap Metal", 0);
		AddRecipe(CraftingCategory.Supplies, 1, "Animal Pelt", 1, "Can", string.Empty, 2, "Duct Tape", 0);
		AddRecipe(CraftingCategory.Supplies, 1, "Cloth", 0, string.Empty, "saw", 2, "Rope", 0);
		AddRecipe(CraftingCategory.Supplies, 1, "Scrap Metal", 0, string.Empty, "saw", 2, "Wire", 0);
		AddRecipe(CraftingCategory.Supplies, 2, "Scrap Metal", 0, string.Empty, "hammer", 1, "Can", 0);
		AddRecipe(CraftingCategory.Components, 2, "Board", 0, string.Empty, string.Empty, 1, "Wooden Plate", 0);
		AddRecipe(CraftingCategory.Components, 2, "Stick", 0, string.Empty, string.Empty, 1, "Wooden Support", 0);
		AddRecipe(CraftingCategory.Components, 2, "Wooden Plate", 0, string.Empty, string.Empty, 1, "Wooden Frame", 0);
		AddRecipe(CraftingCategory.Components, 2, "Wooden Support", 0, string.Empty, string.Empty, 1, "Wooden Cross", 0);
		AddRecipe(CraftingCategory.Components, 2, "Stone", 0, string.Empty, string.Empty, 1, "Stone Plate", 0);
		AddRecipe(CraftingCategory.Components, 1, "Stone", 1, "Board", string.Empty, 1, "Stone Support", 0);
		AddRecipe(CraftingCategory.Components, 2, "Stone Plate", 0, string.Empty, string.Empty, 1, "Stone Frame", 0);
		AddRecipe(CraftingCategory.BarricadesTraps, 3, "Board", 1, "Nails", string.Empty, 1, "Maple Barricade", 0);
		AddRecipe(CraftingCategory.BarricadesTraps, 1, "Wooden Frame", 1, "Bolts", string.Empty, 1, "Wooden Door", 0);
		AddRecipe(CraftingCategory.BarricadesTraps, 2, "Nails", 0, string.Empty, string.Empty, 1, "Caltrop", 0);
		AddRecipe(CraftingCategory.BarricadesTraps, 2, "Wire", 0, string.Empty, string.Empty, 1, "Barbed Wire", 0);
		AddRecipe(CraftingCategory.BarricadesTraps, 4, "Wooden Spike", 0, string.Empty, string.Empty, 1, "Wooden Spike Trap", 0);
		AddRecipe(CraftingCategory.BarricadesTraps, 1, "Can", 2, "Scrap Metal", string.Empty, 1, "Snare", 0);
		AddRecipe(CraftingCategory.BarricadesTraps, 3, "Scrap Metal", 2, "Wire", string.Empty, 1, "Electric Trap", 0);
		AddRecipe(CraftingCategory.BarricadesTraps, 8, "Cloth", 5, "Scrap Metal", string.Empty, 1, "Cot", 0);
		AddRecipe(CraftingCategory.BarricadesTraps, 4, "Stick", 4, "Stone", string.Empty, 1, "Campfire", 0);
		AddRecipe(CraftingCategory.BarricadesTraps, 1, "Wooden Door", 1, "Bolts", string.Empty, 1, "Wooden Shutter", 0);
		AddRecipe(CraftingCategory.BarricadesTraps, 1, "Wooden Shutter", 3, "Scrap Metal", string.Empty, 1, "Metal Shutter", 2);
		AddRecipe(CraftingCategory.BarricadesTraps, 15, "Raw Explosives", 6, "Duct Tape", string.Empty, 1, "MOAB", 2);
		AddRecipe(CraftingCategory.BarricadesTraps, 2, "Raw Explosives", 4, "Wire", string.Empty, 1, "Trip Mine", 2);
		AddRecipe(CraftingCategory.BarricadesTraps, 2, "Raw Explosives", 1, "Can", string.Empty, 1, "Landmine", 2);
		AddRecipe(CraftingCategory.BarricadesTraps, 1, "Wooden Door", 3, "Scrap Metal", string.Empty, 1, "Metal Door", 2);
		AddRecipe(CraftingCategory.BarricadesTraps, 2, "Stick", 2, "Bolts", string.Empty, 1, "Brazier", 0);
		AddRecipe(CraftingCategory.BarricadesTraps, 3, "Barbed Wire", 2, "Wooden Support", string.Empty, 1, "Barbed Fence", 0);
		AddRecipe(CraftingCategory.BarricadesTraps, 1, "Barbed Fence", 4, "Scrap Metal", string.Empty, 1, "Electric Fence", 0);
		AddRecipe(CraftingCategory.StorageUnits, 2, "Wooden Frame", 3, "Wooden Cross", string.Empty, 1, "Crate", 1);
		AddRecipe(CraftingCategory.StorageUnits, 1, "Crate", 3, "Wooden Cross", string.Empty, 1, "Chest", 2);
		AddRecipe(CraftingCategory.StorageUnits, 1, "Crate", 3, "Scrap Metal", string.Empty, 1, "Locker", 2);
		AddRecipe(CraftingCategory.Structures, 3, "Wooden Frame", 0, string.Empty, string.Empty, 1, "Wooden Foundation", 0);
		AddRecipe(CraftingCategory.Structures, 2, "Wooden Frame", 1, "Wooden Pillar", string.Empty, 1, "Wooden Wall", 0);
		AddRecipe(CraftingCategory.Structures, 1, "Wooden Wall", 1, "Wooden Support", string.Empty, 1, "Wooden Doorway", 0);
		AddRecipe(CraftingCategory.Structures, 2, "Wooden Support", 1, "Board", string.Empty, 1, "Wooden Pillar", 0);
		AddRecipe(CraftingCategory.Structures, 3, "Wooden Plate", 1, "Wooden Cross", string.Empty, 1, "Wooden Platform", 0);
		AddRecipe(CraftingCategory.Structures, 1, "Wooden Foundation", 4, "Fertilizer", string.Empty, 1, "Greenhouse Foundation", 0);
		AddRecipe(CraftingCategory.Structures, 1, "Wooden Platform", 4, "Fertilizer", string.Empty, 1, "Greenhouse Foundation", 0);
		AddRecipe(CraftingCategory.Structures, 1, "Wooden Platform", 1, "Wooden Frame", string.Empty, 1, "Wooden Hole", 0);
		AddRecipe(CraftingCategory.Structures, 9, "Stick", 1, "Duct Tape", string.Empty, 1, "Wooden Ladder", 0);
		AddRecipe(CraftingCategory.Structures, 1, "Wooden Doorway", 1, "Wooden Support", string.Empty, 1, "Wooden Window", 0);
		AddRecipe(CraftingCategory.Structures, 1, "Wooden Pillar", 0, string.Empty, string.Empty, 2, "Wooden Post", 0);
		AddRecipe(CraftingCategory.Structures, 2, "Stone Frame", 1, "Stone Pillar", string.Empty, 1, "Stone Wall", 1);
		AddRecipe(CraftingCategory.Structures, 1, "Stone Wall", 1, "Stone Support", string.Empty, 1, "Stone Doorway", 1);
		AddRecipe(CraftingCategory.Structures, 1, "Stone Doorway", 1, "Stone Support", string.Empty, 1, "Stone Window", 1);
		AddRecipe(CraftingCategory.Structures, 2, "Stone Support", 1, "Board", string.Empty, 1, "Stone Pillar", 1);
		AddRecipe(CraftingCategory.Structures, 1, "Wooden Window", 1, "Wooden Support", string.Empty, 1, "Garage Port", 0);
		AddRecipe(CraftingCategory.Structures, 2, "Wooden Shutter", 2, "Bolts", string.Empty, 1, "Wooden Gate", 0);
		AddRecipe(CraftingCategory.Structures, 2, "Metal Shutter", 2, "Bolts", string.Empty, 1, "Metal Gate", 0);
		AddRecipe(CraftingCategory.Clothing, 4, "Animal Pelt", 0, string.Empty, string.Empty, 1, "tatteredGreenJacket", 0);
		AddRecipe(CraftingCategory.Clothing, 4, "Animal Pelt", 0, string.Empty, string.Empty, 1, "tatteredBlackJacket", 0);
		AddRecipe(CraftingCategory.Clothing, 4, "Animal Pelt", 0, string.Empty, string.Empty, 1, "tatteredDarkBlueJacket", 0);
		AddRecipe(CraftingCategory.Clothing, 3, "Animal Pelt", 1, "Rope", string.Empty, 1, "tatteredGreenPants", 0);
		AddRecipe(CraftingCategory.Clothing, 3, "Animal Pelt", 1, "Rope", string.Empty, 1, "tatteredBluePants", 0);
		AddRecipe(CraftingCategory.Clothing, 3, "Animal Pelt", 1, "Rope", string.Empty, 1, "tatteredRedPants", 0);
		AddRecipe(CraftingCategory.Clothing, 4, "Animal Pelt", 1, "Duct Tape", string.Empty, 1, "Animalpack", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "tatteredGreenJacket", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "blueSuspenders", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "pinkShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "turquoiseBra", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "greenTieSuit", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "naziShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "rangerShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "denimJacket", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "specialForcesShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "postmanShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "blueTieSuit", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "doctorShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "firemanShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "tatteredBlackJacket", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "tatteredDarkBlueJacket", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "stripedPurpleSweater", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "purpleSuit", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "policeShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "turquoiseShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "leatherJacket", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "brownSuspenders", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "roadworkerShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "stripedGreyShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "sheriffShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "sportTShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "waiterShirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "tatteredGreenPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "greyPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "blueSkirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "turquoiseUnderpants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "brownPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "naziPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "rangerPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "grayBlueJeans", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "specialForcesPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "postmanPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "grayBluePants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "doctorPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "firemanPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "tatteredBluePants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "tatteredRedPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "crimsonPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "purplePants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "policePants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "purpleSkirt", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "jeans", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "blueJeans", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "bluePants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "darkBrownPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "sheriffPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "greenPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.Clothing, 1, "waiterPants", 0, string.Empty, string.Empty, 2, "Cloth", 0);
		AddRecipe(CraftingCategory.MedicalSupplies, 2, "Cloth", 0, string.Empty, string.Empty, 1, "Rag", 0);
		AddRecipe(CraftingCategory.MedicalSupplies, 2, "Rag", 0, string.Empty, string.Empty, 1, "Bandage", 0);
		AddRecipe(CraftingCategory.MedicalSupplies, 2, "Bandage", 0, string.Empty, string.Empty, 1, "Dressing", 0);
		AddRecipe(CraftingCategory.MedicalSupplies, 1, "Scrap Metal", 2, "Stick", string.Empty, 1, "Splint", 0);
		AddRecipe(CraftingCategory.MedicalSupplies, 1, "Fertilizer", 1, "Vitamins", string.Empty, 1, "Purification Tablets", 0);
		AddRecipe(CraftingCategory.MedicalSupplies, 2, "Red Mushroom", 0, string.Empty, "Stone", 1, "Crushed Red Mushroom", 0);
		AddRecipe(CraftingCategory.MedicalSupplies, 2, "Blue Mushroom", 0, string.Empty, "Stone", 1, "Crushed Blue Mushroom", 0);
		AddRecipe(CraftingCategory.MedicalSupplies, 2, "Pink Mushroom", 0, string.Empty, "Stone", 1, "Crushed Pink Mushroom", 0);
		AddRecipe(CraftingCategory.MedicalSupplies, 2, "Pale Mushroom", 0, string.Empty, "Stone", 1, "Crushed Pale Mushroom", 0);
		AddRecipe(CraftingCategory.MedicalSupplies, 2, "Green Mushroom", 0, string.Empty, "Stone", 1, "Crushed Green Mushroom", 0);
		AddRecipe(CraftingCategory.MedicalSupplies, 2, "Purple Mushroom", 0, string.Empty, "Stone", 1, "Crushed Purple Mushroom", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Raw Bacon", 0, string.Empty, "Campfire", 1, "Cooked Bacon", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Raw Mutton", 0, string.Empty, "Campfire", 1, "Cooked Mutton", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Raw Chicken", 0, string.Empty, "Campfire", 1, "Cooked Chicken", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Raw Rat", 0, string.Empty, "Campfire", 1, "Cooked Rat", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Moldy Bottled Water", 1, "Purification Tablets", string.Empty, 1, "Bottled Water", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Moldy Milk", 1, "Purification Tablets", string.Empty, 1, "Milk", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Moldy Orange Juice", 1, "Purification Tablets", string.Empty, 1, "Orange Juice", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Moldy Cabbage", 1, "Purification Tablets", string.Empty, 1, "Fresh Cabbage", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Moldy Tomato", 1, "Purification Tablets", string.Empty, 1, "Fresh Tomato", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Moldy Corn", 1, "Purification Tablets", string.Empty, 1, "Fresh Corn", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Moldy Potato", 1, "Purification Tablets", string.Empty, 1, "Fresh Potato", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Moldy Carrot", 1, "Purification Tablets", string.Empty, 1, "Fresh Carrot", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Moldy Bacon", 1, "Purification Tablets", string.Empty, 1, "Raw Bacon", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Moldy Mutton", 1, "Purification Tablets", string.Empty, 1, "Raw Mutton", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Moldy Chicken", 1, "Purification Tablets", string.Empty, 1, "Raw Chicken", 0);
		AddRecipe(CraftingCategory.FoodAndDrinks, 1, "Moldy Rat", 1, "Purification Tablets", string.Empty, 1, "Raw Rat", 0);
		AddRecipe(CraftingCategory.Farming, 1, "Rope", 1, "Cloth", string.Empty, 3, "Fertilizer", 0);
		AddRecipe(CraftingCategory.Farming, 1, "Fresh Carrot", 0, string.Empty, string.Empty, 2, "Carrot Seed", 0);
		AddRecipe(CraftingCategory.Farming, 1, "Moldy Carrot", 0, string.Empty, string.Empty, 1, "Carrot Seed", 0);
		AddRecipe(CraftingCategory.Farming, 1, "Fresh Tomato", 0, string.Empty, string.Empty, 2, "Tomato Seed", 0);
		AddRecipe(CraftingCategory.Farming, 1, "Moldy Tomato", 0, string.Empty, string.Empty, 1, "Tomato Seed", 0);
		AddRecipe(CraftingCategory.Farming, 1, "Fresh Cabbage", 0, string.Empty, string.Empty, 2, "Cabbage Seed", 0);
		AddRecipe(CraftingCategory.Farming, 1, "Moldy Cabbage", 0, string.Empty, string.Empty, 1, "Cabbage Seed", 0);
		AddRecipe(CraftingCategory.Farming, 1, "Fresh Corn", 0, string.Empty, string.Empty, 2, "Corn Seed", 0);
		AddRecipe(CraftingCategory.Farming, 1, "Moldy Corn", 0, string.Empty, string.Empty, 1, "Corn Seed", 0);
		AddRecipe(CraftingCategory.Farming, 1, "Fresh Potato", 0, string.Empty, string.Empty, 2, "Potato Seed", 0);
		AddRecipe(CraftingCategory.Farming, 1, "Moldy Potato", 0, string.Empty, string.Empty, 1, "Potato Seed", 0);
		AddRecipe(CraftingCategory.Ammunition, 1, "Stick", 1, "Nails", string.Empty, 3, "simpleBolts", 0);
		AddRecipe(CraftingCategory.Ammunition, 1, "Stick", 1, "Nails", string.Empty, 3, "simpleArrows", 0);
		AddRecipe(CraftingCategory.Gear, 1, "Batteries", 2, "Scrap Metal", string.Empty, 1, "Handlamp", 0);
		AddRecipe(CraftingCategory.Gear, 3, "Wooden Support", 2, "Rope", string.Empty, 1, "mapleBow", 0);
		AddRecipe(CraftingCategory.Gear, 3, "Wooden Support", 2, "Rope", string.Empty, 1, "birchBow", 0);
		AddRecipe(CraftingCategory.Gear, 3, "Wooden Support", 2, "Rope", string.Empty, 1, "pineBow", 0);
		AddRecipe(CraftingCategory.Gear, 1, "Can", 1, "Bottled Water", string.Empty, 1, "Canteen", 0);
	}

	private void AddRecipe(CraftingCategory category, int count1, string itemId1, int count2, string itemId2, string toolId, int resultItemCount, string resultItemId, int craftingLvl)
	{
		RecipeInfo recipeInfo = new RecipeInfo();
		recipeInfo.CraftingItems = new List<ItemRecipeInfo>();
		recipeInfo.ToolId = toolId;
		recipeInfo.ResultItemCount = resultItemCount;
		recipeInfo.ResultItemId = resultItemId;
		recipeInfo.CraftingLevel = craftingLvl;
		RecipeInfo recipeInfo2 = recipeInfo;
		if (count1 > 0)
		{
			recipeInfo2.CraftingItems.Add(new ItemRecipeInfo
			{
				Count = count1,
				ItemId = itemId1
			});
		}
		if (count2 > 0)
		{
			recipeInfo2.CraftingItems.Add(new ItemRecipeInfo
			{
				Count = count2,
				ItemId = itemId2
			});
		}
		if (Recipes.ContainsKey(category))
		{
			Recipes[category].Add(recipeInfo2);
			return;
		}

		Recipes.Add(category, new List<RecipeInfo> { recipeInfo2 });
	}
}
