using System;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesCustomizer : MonoBehaviour
{
	private const int BloodMaskIndexMax = 3;

	[SerializeField]
	private bool _oneClothingTexture;

	[SerializeField]
	private Renderer _zombie;

	[SerializeField]
	private List<Texture> _bloodMasks;

	[SerializeField]
	private Color _bloodColor;

	[SerializeField]
	private List<Color> _skinsColors;

	[SerializeField]
	private Texture _maleDefaultSkin;

	[SerializeField]
	private Texture _femaleDefaultSkin;

	public List<string> ClothingsIds;

	public List<string> MansHeadsIds;

	public List<string> WomansHeadsIds;

	private CharacterViewInfo _currentViewInfo;

	private void Start()
	{
		SetRandomViewInfo();
		AddRandomBlood();
	}

	public void SetRagdollView(GameObject ragdoll)
	{
		RagdollViewController component = ragdoll.GetComponent<RagdollViewController>();
		if ((bool)component)
		{
			component.SetView(_zombie.material);
		}
	}

	private void SetRandomViewInfo()
	{
		Sex sex = (Sex)UnityEngine.Random.Range(0, Enum.GetNames(typeof(Sex)).Length);
		List<string> list = ((sex != 0) ? WomansHeadsIds : MansHeadsIds);
		byte skinColorIndex = (byte)UnityEngine.Random.Range(0, _skinsColors.Count);
		string headTexId = list[UnityEngine.Random.Range(0, list.Count)];
		string text = ClothingsIds[UnityEngine.Random.Range(0, ClothingsIds.Count)];
		string legsTexId = ((!_oneClothingTexture) ? ClothingsIds[UnityEngine.Random.Range(0, ClothingsIds.Count)] : text);
		_currentViewInfo = new CharacterViewInfo(sex, skinColorIndex, headTexId, text, legsTexId);
		CustomizeZombie();
	}

	private void CustomizeZombie()
	{
		Texture texture = ((_currentViewInfo.Sex != 0) ? _femaleDefaultSkin : _maleDefaultSkin);
		_zombie.material.SetColor("_SkinColor", _skinsColors[_currentViewInfo.SkinColorIndex]);
		_zombie.material.SetTexture("_MainTex", texture);
		_zombie.material.SetTexture("_HeadTex", DataKeeper.I.GetClothingTexture(_currentViewInfo.HeadTexId));
		_zombie.material.SetTexture("_BodyTex", DataKeeper.I.GetClothingTexture(_currentViewInfo.BodyTexId));
		_zombie.material.SetTexture("_LegsTex", DataKeeper.I.GetClothingTexture(_currentViewInfo.LegsTexId));
	}

	public void OnTextureLoad(List<string> ids)
	{
		if (ids.Contains(_currentViewInfo.HeadTexId))
		{
			_zombie.material.SetTexture("_HeadTex", DataKeeper.I.GetClothingTexture(_currentViewInfo.HeadTexId));
		}
		if (ids.Contains(_currentViewInfo.BodyTexId))
		{
			_zombie.material.SetTexture("_BodyTex", DataKeeper.I.GetClothingTexture(_currentViewInfo.BodyTexId));
		}
		if (ids.Contains(_currentViewInfo.LegsTexId))
		{
			_zombie.material.SetTexture("_LegsTex", DataKeeper.I.GetClothingTexture(_currentViewInfo.LegsTexId));
		}
	}

	private void AddRandomBlood()
	{
		int value = UnityEngine.Random.Range(0, 3);
		Texture texture = _bloodMasks[UnityEngine.Random.Range(0, _bloodMasks.Count)];
		_zombie.material.SetInt("_MaskIndex", value);
		_zombie.material.SetTexture("_BloodMask", texture);
		_zombie.material.SetColor("_BloodColor", _bloodColor);
	}
}
