using System.Runtime.InteropServices;
using Photon;
using UnityEngine;

public class PhotonMobInfo
{
	public string Id;

	public float HealthPoint;
}


public class PhotonMob : Photon.MonoBehaviour
{
	private const float _destroyTime = 30f;

	[SerializeField]
	private ZombieType _mobType;

	[SerializeField]
	private MobAI _ai;

	[SerializeField]
	private GameObject deadBody;

	[SerializeField]
	private ZombieSoundController _soundController;

	[SerializeField]
	private PhotonTransformView m_TransformView;

	private Mob _info;

	private bool _lastAttackerIsPlayer;

	private Vector3 _lastAttackerPosition;

	private Vector3 _lastAttackDirection;

	private LayerMask raymask = 512;

	private bool _isDead;

	private string _currentAnim;

	private Vector3 _lastPosition;

	private Animator _animator;

	private GameObject _viewModel;

	public PhotonMobInfo MobInfo { get; private set; }

	public ZombieType MobType
	{
		get
		{
			return _mobType;
		}
	}

	public bool MobIsActive { get; private set; }

	public ZombiesCustomizer Customizer
	{
		get
		{
			return (!_viewModel) ? null : _viewModel.GetComponent<ZombiesCustomizer>();
		}
	}

	private void Update()
	{
		if (_animator != null)
		{
			if (base.photonView.isMine)
			{
				Vector3 speed = _ai.Speed;
				m_TransformView.SetSynchronizedValues(speed, _ai.TurnSpeed);
				_animator.SetFloat("Speed", speed.magnitude);
				_animator.speed = GetAnimatorSpeed(speed.magnitude);
			}
			else if (Time.deltaTime > 0f)
			{
				float num = Vector3.Distance(base.transform.position, _lastPosition);
				float num2 = num / Time.deltaTime;
				_animator.SetFloat("Speed", num2);
				_animator.speed = GetAnimatorSpeed(num2);
				_lastPosition = base.transform.position;
			}
		}
	}

	public void UseInterpolateSync(bool use)
	{
		m_TransformView.UseInterpolateSync = use;
	}

	private float GetAnimatorSpeed(float speed)
	{
		return 1f + speed / 3f;
	}

	private void OnDestroy()
	{
		if (WorldController.I.WorldMobs.Contains(this))
		{
			WorldController.I.WorldMobs.Remove(this);
		}
	}

	public void ReenableAI()
	{
		if (PhotonNetwork.isMasterClient && MobIsActive && !_isDead)
		{
			_ai.EnableAI();
		}
		else
		{
			_ai.DisableAI();
		}
	}

	public void Push(Vector3 direction, float force)
	{
		if (!_isDead)
		{
			_ai.Push(direction, force);
		}
	}

	private void OnPhotonInstantiate()
	{
		base.transform.parent = WorldController.I.Mobs;
		MobInfo = new PhotonMobInfo();
		m_TransformView.SyncTransform = false;
		MobIsActive = false;
		MobPullingSystem.Instance.FirstAddMobToList(this);
		WorldController.I.WorldMobs.Add(this);
		ReenableAI();
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient && MobIsActive && MobInfo.HealthPoint > 0f)
		{
			base.photonView.RPC("SyncInfo", player, MobInfo.Id, (short)MobInfo.HealthPoint);
		}
	}

	public void Hit(short damage, Vector3 attackerPos, Vector3 attackerDir, bool isPlayer, int attackerSocialId)
	{
		base.photonView.RPC("HitMob", PhotonTargets.All, damage, attackerPos, attackerDir, isPlayer, attackerSocialId);
	}

	private void CopyTransformsRecurse(Transform src, Transform dst)
	{
		dst.position = src.position;
		dst.rotation = src.rotation;
		foreach (Transform item in dst)
		{
			Transform transform2 = src.Find(item.name);
			if ((bool)transform2)
			{
				CopyTransformsRecurse(transform2, item);
			}
		}
	}

	public void ActivateMobByMaster(bool activate, Vector3 position, string mobId = null)
	{
		if (activate)
		{
			if (!MobIsActive)
			{
				base.transform.position = position;
				base.photonView.RPC("ActivateMob", PhotonTargets.All, mobId);
			}
		}
		else if (MobIsActive)
		{
			base.photonView.RPC("DeactivateMob", PhotonTargets.All);
		}
	}

	private void DestroyViewModel()
	{
		if (_viewModel != null)
		{
			_animator = null;
			Object.Destroy(_viewModel);
		}
	}

	private void ActivateMobLogic(Mob mob, short hp)
	{
		_info = mob;
		MobInfo.HealthPoint = hp;
		MobInfo.Id = mob.Id;
		_ai.SetInfo(base.transform.position, _info.Damage, _info.Sickness);
		GameObject zombieObj = ZombieViewManager.I.GetZombieObj(mob.Prefab);
		if ((bool)zombieObj)
		{
			_viewModel = Object.Instantiate(zombieObj);
			_viewModel.transform.parent = base.transform;
			_viewModel.transform.localPosition = Vector3.zero;
			_viewModel.transform.localRotation = Quaternion.identity;
			_viewModel.transform.localScale = Vector3.one;
			_animator = _viewModel.GetComponentInChildren<Animator>();
			AISensor componentInChildren = _viewModel.GetComponentInChildren<AISensor>();
			componentInChildren.Initialize(_ai);
			_isDead = false;
			_lastPosition = base.transform.position;
			MobPullingSystem.Instance.ActivateMob(this);
			GetComponent<Collider>().enabled = true;
			m_TransformView.SyncTransform = true;
			MobIsActive = true;
			_soundController.EnableZombieSounds(true);
			ReenableAI();
		}
		else
		{
			Debug.Log("Mob prefab model is Null! MobId = " + mob.Id);
		}
	}

	[PunRPC]
	private void ActivateMob(string mobId)
	{
		if (!string.IsNullOrEmpty(mobId))
		{
			Mob mobInfo = DataKeeper.Info.GetMobInfo(mobId);
			if (mobInfo != null)
			{
				ActivateMobLogic(mobInfo, (short)mobInfo.HealthPoint);
			}
		}
		else
		{
			Debug.Log("On RPC ActivateMob mobId is Null!");
		}
	}

	[PunRPC]
	private void DeactivateMob()
	{
		DestroyViewModel();
		MobPullingSystem.Instance.DeactivateMob(this);
		GetComponent<Collider>().enabled = false;
		m_TransformView.SyncTransform = false;
		MobIsActive = false;
		_soundController.EnableZombieSounds(false);
		ReenableAI();
	}

	[PunRPC]
	private void SyncInfo(string id, short hp)
	{
		MobInfo.Id = id;
		MobInfo.HealthPoint = hp;
		if (!string.IsNullOrEmpty(id))
		{
			Mob mobInfo = DataKeeper.Info.GetMobInfo(id);
			if (mobInfo != null)
			{
				ActivateMobLogic(mobInfo, hp);
			}
		}
		else
		{
			Debug.Log("On RPC ActivateMob mobId is Null!");
		}
	}

	private void AddFragToKiller(int attackerSocialId, PhotonPlayer killer)
	{
		if (attackerSocialId != 0)
		{
			PhotonMan photonMan = ((!WorldController.I.WorldPlayers.ContainsKey(attackerSocialId)) ? null : WorldController.I.WorldPlayers[attackerSocialId]);
			if ((bool)photonMan)
			{
				WorldController.I.WorldPlayers[attackerSocialId].photonView.RPC("AddZombieFrag", killer);
			}
		}
	}

	[PunRPC]
	private void HitMob(short damage, Vector3 attackerPos, Vector3 attackerDir, bool isPlayer, int attackerSocialId, PhotonMessageInfo info)
	{
		if (MobInfo.HealthPoint <= 0f)
		{
			return;
		}
		_lastAttackerPosition = attackerPos;
		_lastAttackDirection = attackerDir;
		_lastAttackerIsPlayer = isPlayer;
		MobInfo.HealthPoint -= damage;
		_soundController.OnGetHurt();
		if (!PhotonNetwork.isMasterClient || !(MobInfo.HealthPoint <= 0f))
		{
			return;
		}
		AddFragToKiller(attackerSocialId, info.sender);
		base.photonView.RPC("CreateRagdoll", PhotonTargets.All);

		ActivateMobByMaster(false, this.transform.position);

		if (_info.DropItems == null)
		{
			return;
		}
		int num = 0;
		foreach (DropItem dropItem in _info.DropItems)
		{
			if ((float)num >= (float)_info.MaxItems * DataKeeper.ZombieLootFactor)
			{
				break;
			}
			Item itemInfo = DataKeeper.Info.GetItemInfo(dropItem.ItemId);
			if (itemInfo == null)
			{
				continue;
			}
			int valueInt = dropItem.Count.GetValueInt();
			if (valueInt > 0)
			{
				int num2 = UnityEngine.Random.Range(0, 100);
				RaycastHit hitInfo;
				if ((float)num2 < DataKeeper.DefaultMobDropChance && Physics.Raycast(base.transform.position + Vector3.up, -Vector3.up, out hitInfo, 100f, DataKeeper.I.ItemsCollisionsMask))
				{
					Quaternion rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, hitInfo.normal), Vector3.Cross(Vector3.up, hitInfo.normal));
					int num3 = ((itemInfo.Type == ItemType.Weapon) ? UnityEngine.Random.Range(0, 10) : 0);
					PhotonNetwork.InstantiateSceneObject("PhotonItem", hitInfo.point, rotation, 0, new object[4]
					{
						itemInfo.Id,
						(byte)valueInt,
						true,
						(byte)num3
					});
					num++;
				}
			}
		}
	}

	[PunRPC]
	private void CreateRagdoll()
	{
		if (_isDead)
		{
			return;
		}
		_isDead = true;
		_soundController.OnDeath();
		if (!(_viewModel != null))
		{
			return;
		}
		GameObject gameObject = (GameObject)Object.Instantiate(deadBody, base.transform.position, base.transform.rotation);
		ZombiesCustomizer component = _viewModel.GetComponent<ZombiesCustomizer>();
		if ((bool)component)
		{
			component.SetRagdollView(gameObject);
		}
		CopyTransformsRecurse(_viewModel.transform, gameObject.transform);
		if (_lastAttackerIsPlayer)
		{
			RaycastHit hitInfo;
			if (Physics.SphereCast(_lastAttackerPosition, 0.2f, _lastAttackDirection, out hitInfo, 750f, raymask) && (bool)hitInfo.rigidbody)
			{
				hitInfo.rigidbody.AddForce(_lastAttackDirection * 50f, ForceMode.Impulse);
			}
			else
			{
				Rigidbody[] componentsInChildren = gameObject.GetComponentsInChildren<Rigidbody>();
				if (componentsInChildren != null)
				{
					Rigidbody[] array = componentsInChildren;
					foreach (Rigidbody rigidbody in array)
					{
						if (rigidbody.transform.name == "Body_jnt" || rigidbody.transform.name == "Spine_jnt")
						{
							rigidbody.AddForce((base.transform.position - _lastAttackerPosition).normalized * 30f, ForceMode.Impulse);
							break;
						}
					}
				}
			}
		}
		RemoveBody component2 = gameObject.GetComponent<RemoveBody>();
		if ((bool)component2)
		{
			component2.bodyStayTime = 15f;
			component2.enabled = true;
		}
		DestroyViewModel();
	}
}
