using System.Collections.Generic;
using UnityEngine;

public class CraftingViewController : MonoBehaviour
{
	[HideInInspector]
	public List<CraftingCategory> TabsCategory;

	[SerializeField]
	private List<RecipeView> _recipeViews;

	[SerializeField]
	private tk2dUIToggleButtonGroup _tabs;

	[SerializeField]
	private tk2dUIScrollbar _scrollbar;

	[SerializeField]
	private GameObject _content;

	private int _currentPosition;

	private CraftingCategory _currentCategory;

	public tk2dUIToggleButtonGroup Tabs
	{
		get
		{
			return _tabs;
		}
	}

	private int MaxPosition
	{
		get
		{
			if (!CraftingRegistry.Instance.Recipes.ContainsKey(_currentCategory))
			{
				return 0;
			}
			return Mathf.Max(0, CraftingRegistry.Instance.Recipes[_currentCategory].Count - 1 - (_recipeViews.Count - 1));
		}
	}

	private void OnEnable()
	{
		if (_tabs.SelectedIndex < 0)
		{
			_tabs.SelectedIndex = 0;
		}
		UpdateCurrentPage();
	}

	private void OnTabChange()
	{
		if (TabsCategory.Count > _tabs.SelectedIndex)
		{
			_scrollbar.Value = 0f;
			_currentPosition = 0;
			_currentCategory = TabsCategory[_tabs.SelectedIndex];
		}
		UpdateCurrentPage();
	}

	private void OnScroll()
	{
		_currentPosition = (int)((float)MaxPosition * _scrollbar.Value);
		UpdateCurrentPage();
	}

	public void UpdateCurrentPage()
	{
		if (_currentPosition > MaxPosition)
		{
			_currentPosition = Mathf.Max(0, _currentPosition - 1);
		}
		UpdatePage(_currentPosition);
	}

	private void UpdatePage(int position)
	{
		bool flag = CraftingRegistry.Instance.Recipes.ContainsKey(_currentCategory);
		if (flag)
		{
			for (int i = position; i < position + _recipeViews.Count; i++)
			{
				int index = i - position;
				bool show = false;
				if (CraftingRegistry.Instance.Recipes[_currentCategory].Count - 1 >= i)
				{
					_recipeViews[index].UpdateView(CraftingRegistry.Instance.Recipes[_currentCategory][i]);
					show = true;
				}
				_recipeViews[index].ShowView(show);
			}
		}
		_content.SetActive(flag);
	}
}
