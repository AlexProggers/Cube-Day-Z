using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrapType
{
	Hit = 0,
	Explosion = 1
}

public class WorldObjectTrap : MonoBehaviour
{
	[SerializeField]
	private PhotonWorldObject _photonWorldObject;

	[SerializeField]
	private TrapType _trapType;

	[SerializeField]
	private bool _corruptible;

	[SerializeField]
	private float _explosionArea;

	[SerializeField]
	private LayerMask _explosionLayerMask;

	[SerializeField]
	private List<DestructibleObjectType> _destructibleObjTypes;

	[SerializeField]
	private int _destructibleObjDamage;

	[SerializeField]
	private float _hitInterval;

	[SerializeField]
	private int _damage;

	[SerializeField]
	private int _damageToTrap;

	[SerializeField]
	private bool _pushMob;

	[SerializeField]
	private float _pushForce;

	[SerializeField]
	private bool _activateByTime;

	[SerializeField]
	private float _activationTime;

	[SerializeField]
	private GameObject _explosionEffect;

	[SerializeField]
	private ElectricEffectController _electricEffect;

	private float _lastHitTime;

	private bool _canActivate;

	private bool _showEffect;

	public void OnInstantiate(double time)
	{
		_canActivate = true;
		_showEffect = true;
		if (_activateByTime)
		{
			StartCoroutine(ActivateByTime(time));
		}
	}

	private void OnDestroy()
	{
		if (_showEffect && _trapType == TrapType.Explosion && (bool)_explosionEffect)
		{
			ExplosionController component = GameObject.Instantiate(_explosionEffect, base.transform.position, base.transform.rotation).GetComponent<ExplosionController>();
			component.Initialize(_explosionArea);
		}
	}

	private IEnumerator ActivateByTime(double landTime)
	{
		float lifetime = (float)(PhotonNetwork.time - landTime);
		if (lifetime < _activationTime)
		{
			yield return new WaitForSeconds(_activationTime - lifetime);
		}
		if (_canActivate)
		{
			if (PhotonNetwork.isMasterClient)
			{
				ApplyAreaDamage();
				DestroyByMaster();
			}
			_canActivate = false;
		}
	}

	private void PushMob(Collider target)
	{
		switch (target.tag)
		{
		case "Zombie":
			break;
		}
	}

	public void ApplyDamage(Collider target)
	{
		switch (target.tag)
		{
		case "Player":
			if (DataKeeper.backendInfo.playerId != _photonWorldObject.ObjInfo.OwnerUId)
			{
				WorldController.I.Player.HitPlayer((short)_damage, (byte)0, _photonWorldObject.ObjInfo.OwnerUId);
				if (_corruptible)
				{
					_photonWorldObject.Hit(_damageToTrap, new List<DestructibleObjectType> { _photonWorldObject.DestructibleType });
				}
			}
			break;
		}
	}

	private void ApplyAreaDamage()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, _explosionArea, _explosionLayerMask);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			switch (collider.tag)
			{
			case "Player":
				WorldController.I.Player.HitPlayer((short)_damage, (byte)0, _photonWorldObject.ObjInfo.OwnerUId);
				continue;
			}
			if (collider.gameObject.layer == 25)
			{
				PhotonMan component2 = collider.GetComponent<PhotonMan>();
				if ((bool)component2)
				{
					component2.HitPlayer((short)_damage, (byte)0, 0);
				}
			}
			else
			{
				HitWorldObject(collider);
			}
		}
	}

	private void HitWorldObject(Collider target)
	{
		if (target.gameObject.layer != 28 && target.gameObject.layer != 21)
		{
			return;
		}
		PhotonWorldObject component = target.transform.parent.transform.parent.GetComponent<PhotonWorldObject>();
		if ((bool)component && component != _photonWorldObject)
		{
			component.Hit(_destructibleObjDamage, _destructibleObjTypes);
			return;
		}
		PhotonObject component2 = target.transform.parent.transform.parent.GetComponent<PhotonObject>();
		if ((bool)component2)
		{
			component2.Hit((short)_destructibleObjDamage, _destructibleObjTypes);
		}
	}

	private void OnTriggerStay(Collider target)
	{
		if (!_canActivate)
		{
			return;
		}
		switch (_trapType)
		{
		case TrapType.Hit:
			if (Time.time - _lastHitTime >= _hitInterval)
			{
				if (_electricEffect != null)
				{
					_electricEffect.Play();
				}
				ApplyDamage(target);
				_lastHitTime = Time.time;
			}
			PushMob(target);
			break;
		case TrapType.Explosion:
			switch (target.tag)
			{
			case "Player":
				if (DataKeeper.backendInfo.playerId != _photonWorldObject.ObjInfo.OwnerUId)
				{
					ApplyAreaDamage();
					_canActivate = false;
					DestroyByMaster();
				}
				break;
			case "Zombie":
				if (PhotonNetwork.isMasterClient)
				{
					ApplyAreaDamage();
					_canActivate = false;
					DestroyByMaster();
				}
				break;
			}
			break;
		}
	}

	private void DestroyByMaster()
	{
		_photonWorldObject.photonView.RPC("DestroyByMaster", PhotonTargets.All);
	}
}
