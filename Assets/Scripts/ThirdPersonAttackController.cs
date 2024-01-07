using System.Collections;
using UnityEngine;

public class ThirdPersonAttackController : MonoBehaviour
{
	private const float HandAttackWaitTime = 1f;

	private const float MuzzleFlashReduction = 6.5f;

	public const float MuzzleLightDelay = 0.1f;

	public const float MuzzleLightReduction = 100f;

	[SerializeField]
	private ModelClothingController _modelClothingController;

	[SerializeField]
	private Animation _itemHolderAnimation;

	[SerializeField]
	private Animator _characterAnimator;

	private float _startShootTime;

	private PositionInHandInfo ItemViewInfo
	{
		get
		{
			return _modelClothingController.ItemInHandViewInfo;
		}
	}

	public void ShowAttackAnimation(Weapon weapon)
	{
		if (!_characterAnimator.gameObject.activeSelf)
		{
			return;
		}
		if (weapon != null)
		{
			switch (weapon.WeaponType)
			{
			case WeaponType.Blunt:
			case WeaponType.Bladed:
				_itemHolderAnimation.Play("MeleeAttack");
				break;
			case WeaponType.Pistol:
			case WeaponType.Shotgun:
			case WeaponType.AssaultRifle:
			case WeaponType.SniperRifle:
			case WeaponType.Rifle:
			case WeaponType.SMG:
				ShowMuzzleFlash();
				_itemHolderAnimation.Play("RangeAttack");
				break;
			default:
				_itemHolderAnimation.Play("RangeAttack");
				break;
			}
		}
		else
		{
			StopCoroutine("HandPunchProcess");
			_characterAnimator.SetBool("HandPunch", true);
			StartCoroutine("HandPunchProcess");
		}
	}

	private void OnDisable()
	{
		StopCoroutine("HandPunchProcess");
	}

	private IEnumerator HandPunchProcess()
	{
		yield return new WaitForSeconds(1f);
		_characterAnimator.SetBool("HandPunch", false);
	}

	private void ShowMuzzleFlash()
	{
		if ((bool)ItemViewInfo && (bool)ItemViewInfo.MuzzleFlash)
		{
			if ((bool)ItemViewInfo.MuzzleFlashLight)
			{
				ItemViewInfo.MuzzleFlashLight.enabled = true;
				ItemViewInfo.MuzzleFlashLight.intensity = 8f;
			}
			ItemViewInfo.MuzzleFlash.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, UnityEngine.Random.Range(0.4f, 0.5f)));
			ItemViewInfo.MuzzleFlash.localRotation = Quaternion.AngleAxis(UnityEngine.Random.value * 360f, Vector3.forward);
			ItemViewInfo.MuzzleFlash.GetComponent<Renderer>().enabled = true;
			_startShootTime = Time.time;
		}
	}

	private void FadeMuzzleFlashProcess()
	{
		if (!ItemViewInfo)
		{
			return;
		}
		if ((bool)ItemViewInfo.MuzzleFlash && ItemViewInfo.MuzzleFlash.GetComponent<Renderer>().enabled)
		{
			Color color = ItemViewInfo.MuzzleFlash.GetComponent<Renderer>().material.GetColor("_TintColor");
			if (color.a > 0f)
			{
				color.a -= 6.5f * Time.deltaTime;
				ItemViewInfo.MuzzleFlash.GetComponent<Renderer>().material.SetColor("_TintColor", color);
			}
			else
			{
				ItemViewInfo.MuzzleFlash.GetComponent<Renderer>().enabled = false;
			}
		}
		if (!ItemViewInfo.MuzzleFlashLight || !ItemViewInfo.MuzzleFlashLight.enabled)
		{
			return;
		}
		if (ItemViewInfo.MuzzleFlashLight.intensity > 0f)
		{
			if (_startShootTime + 0.1f < Time.time)
			{
				ItemViewInfo.MuzzleFlashLight.intensity -= 100f * Time.deltaTime;
			}
		}
		else
		{
			ItemViewInfo.MuzzleFlashLight.enabled = false;
		}
	}

	private void Update()
	{
		FadeMuzzleFlashProcess();
	}
}
