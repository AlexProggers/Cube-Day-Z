using UnityEngine;

public class RecipeItemView : MonoBehaviour
{
	[SerializeField]
	private tk2dSprite _icon;

	[SerializeField]
	private tk2dTextMesh _count;

	[SerializeField]
	private tk2dTextMesh _itemName;

	public void UpdateView(string id, int count)
	{
		Item itemInfo = DataKeeper.Info.GetItemInfo(id);
		if (itemInfo != null)
		{
			if (_icon.GetSpriteIdByName(itemInfo.Icon) == 0)
			{
				_icon.SetSprite(DataKeeper.NoIcon);
			}
			else
			{
				_icon.SetSprite(itemInfo.Icon);
			}
			if (DataKeeper.Language == Language.Russian)
			{
				_itemName.text = itemInfo.RussianName;
			}
			else
			{
				_itemName.text = itemInfo.Name;
			}
		}
		else
		{
			Debug.Log("RECIPE ITEM INFO IS NULL. ITEM ID " + id);
		}
		if (count > 0)
		{
			_count.text = count.ToString();
		}
		else
		{
			_count.text = string.Empty;
		}
	}
}
