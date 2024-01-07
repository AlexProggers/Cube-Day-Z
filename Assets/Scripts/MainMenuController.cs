using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController I;
    [SerializeField] tk2dTextMesh _nickName;
	[SerializeField] tk2dTextMesh _id;
    [SerializeField] GameObject _shopMenu;
    [SerializeField] tk2dSlicedSprite RusBtn;
	[SerializeField] tk2dSlicedSprite EngBtn;
    void Awake()
    {
        I = this;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
    }
    
    void Update()
    {
        _nickName.text = PhotonNetwork.playerName;
		_id.text = "ID: " + DataKeeper.backendInfo.playerId;
    }

    public void ToggleMainMenu(bool flag)
	{
		MenuConnectionViewController.I.MainMenuContent.SetActive(flag);
		MenuConnectionViewController.I.MainMenuZombiez.SetActive(flag);
	}

    private void OnCkickStart()
	{
		DataKeeper.IsBattleRoyaleClick = false;
		DataKeeper.IsSkyWarsClick = false;
		base.gameObject.SetActive(false);
		MenuConnectionViewController.I.ChooseGameTypeMenu.SetActive(true);
	}

    private void OnCkickShop()
	{
		base.gameObject.SetActive(false);
		_shopMenu.SetActive(true);
	}

    private void OnCkickQuit()
	{
        Application.Quit();
	}

    public void HideMainMenu()
	{
		if (base.gameObject != null)
		{
			base.gameObject.SetActive(false);
		}
	}
	public void ChangeLanguageRus(){
		RusBtn.SetSprite("button_blue");
		EngBtn.SetSprite("button_brown");
        DataKeeper.Language = Language.Russian;
		LocalizationController.Instance.UpdateLabels();
	}
	public void ChangeLanguageEng(){
		RusBtn.SetSprite("button_brown");
		EngBtn.SetSprite("button_blue");
		 DataKeeper.Language = Language.English;
		 LocalizationController.Instance.UpdateLabels();
	}
}
