
using BattleRoyale;
using System.Collections.Generic;
using UnityEngine;

public enum ManState : byte
{
	HealthPoint = 0,
	Hunger = 1,
	Thirst = 2,
	Sickness = 3,
	Stamina = 4
}

public class PhotonManInfo
{
	public short HealthPoints;

	public Dictionary<string, string> Equipment;
}

public class CharacterViewInfo
{
	public Sex Sex;

	public byte SkinColorIndex;

	public string HeadTexId;

	public string BodyTexId;

	public string LegsTexId;

	public CharacterViewInfo()
	{

	}

	public CharacterViewInfo(Sex sex, byte skinColorIndex, string headTexId, string bodyTexId, string legsTexId)
	{
		Sex = sex;
		SkinColorIndex = skinColorIndex;
		HeadTexId = headTexId;
		BodyTexId = bodyTexId;
		LegsTexId = legsTexId;
	}
}

public class PhotonMan : Photon.MonoBehaviour
{
	internal struct State
	{
		internal double timestamp;

		internal Vector3 pos;

		internal Vector3 velocity;

		internal Quaternion rot;

		internal Vector3 angularVelocity;
	}
	private State[] _bufferedState = new State[20];

	[SerializeField]
    private PlayerPhotonSectorController _mySectorController;

	[SerializeField]
	private ModelClothingController _clothingController;

	
	[SerializeField]
	private ThirdPersonAttackController _thirdPersonAttackController;

	[SerializeField]
	private AudioSource _weaponAudioSource;

	[SerializeField]
	private GameObject _parachute;

    private const int OnDieMenuWaitTime = 2;

    private LayerMask _ragdollRaymask = 512;

	private double _interpolationBackTime = 0.1;

	private double _extrapolationLimit = 0.5;

    public PhotonManInfo ManInfo { get; private set; }

    private int _timestampCount;

    private int _hunger;

	private int _thirst;

	private int _sickness;

	private int _animatorSpeedId;

	private bool _isAnimatorHashInitialized;

    public int CurrentHunger
	{
		get
		{
			return _hunger;
		}
		private set
		{
			_hunger = value;
		}
	}

	public int CurrentThirst
	{
		get
		{
			return _thirst;
		}
		private set
		{
			_thirst = value;
		}
	}

	public int CurrentSickness
	{
		get
		{
			return _sickness;
		}
		private set
		{
			_sickness = value;
		}
	}

	public HallucinogenType CurrentHallucinogen { get; private set; }

    private string _name;

    private int _id;

    public string Name
	{
		get
		{
			return _name;
		}
	}

    public int Id
	{
		get
		{
			return _id;
		}
	}

	public double InstTime { get; private set; }

	public bool IsDead
	{
		get
		{
			return ManInfo == null || ManInfo.HealthPoints <= 0;
		}
	}

    [SerializeField]
    private GameObject _model;
	
	[SerializeField]
	private GameObject _ragdoll;

	[SerializeField]
	private Animator _animator;

	public Rigidbody rigBody;

	public CapsuleCollider capsCollider;

    public PlayerPhotonSectorController SectorController
	{
		get
		{
			return _mySectorController;
		}
	}

	public void ToggleAnimator(bool flag)
	{
		_animator.enabled = flag;
	}

	private void InitializeAnimatorHash()
	{
		_animatorSpeedId = Animator.StringToHash("Speed_f");
		_isAnimatorHashInitialized = true;
	}

	[PunRPC]
    public void OnRespawn()
    {
        _model.SetActive(true);
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
            if (GameControls.I != null && GameControls.I.PlayerTransform != null && !GameControls.I.Player.inTheCar)
            {
                stream.SendNext(GameControls.I.PlayerTransform.position);
                stream.SendNext(GameControls.I.PlayerTransform.rotation);
                stream.SendNext(GameControls.I.PlayerTransform.GetComponent<Rigidbody>().velocity);
                stream.SendNext(GameControls.I.PlayerTransform.GetComponent<Rigidbody>().angularVelocity);
            }

            return;
        }

		for (int num = _bufferedState.Length - 1; num >= 1; num--)
		{
			_bufferedState[num] = _bufferedState[num - 1];
		}

		Vector3 pos = (Vector3)stream.ReceiveNext();
		Quaternion rot =  (Quaternion)stream.ReceiveNext();
		Vector3 vel = (Vector3)stream.ReceiveNext();
		Vector3 ang = (Vector3)stream.ReceiveNext();
			
		State state           = default(State);
		state.timestamp       = info.timestamp;
		state.pos             = pos;
		state.rot             = rot;
		state.velocity        = vel;
		state.angularVelocity = ang;

		State state2 = state;
		_bufferedState[0] = state2;
		_timestampCount = Mathf.Min(_timestampCount + 1, _bufferedState.Length);
	}

	bool initialized = false;

    private void Update()
	{
		if (!base.photonView.isMine) {
			double num = PhotonNetwork.time - _interpolationBackTime;
			if (_bufferedState[0].timestamp > num)
			{
				for (int i = 0; i < _timestampCount; i++)
				{
					if (_bufferedState[i].timestamp <= num || i == _timestampCount - 1)
					{
						State state = _bufferedState[Mathf.Max(i - 1, 0)];
						State state2 = _bufferedState[i];
						double num2 = state.timestamp - state2.timestamp;
						float t = 0f;
						if (num2 > 0.0001)
						{
							t = (float)((num - state2.timestamp) / num2);
						}
						base.transform.localPosition = Vector3.Lerp(state2.pos, state.pos, t);
						base.transform.localRotation = Quaternion.Slerp(state2.rot, state.rot, t);
						return;
					}
				}
			}
			else
			{
				State state3 = _bufferedState[0];
				float num3 = (float)(num - state3.timestamp);

				if ((double)num3 < _extrapolationLimit || !initialized) {
					float angle = num3 * state3.angularVelocity.magnitude * 57.29578f;
					Quaternion quaternion = Quaternion.AngleAxis (angle, state3.angularVelocity);
					base.transform.position = state3.pos + state3.velocity * num3;
					base.transform.rotation = quaternion * state3.rot;
					GetComponent<Rigidbody> ().velocity = state3.velocity;
					GetComponent<Rigidbody> ().angularVelocity = state3.angularVelocity;
					initialized = true;
				}
			}
		}

		if (!_model.activeSelf || !_isAnimatorHashInitialized)
		{
			return;
		}

		float value = 0f;
		if (base.photonView.isMine)
		{
			if (GameControls.I != null && GameControls.I.Player != null)
			{
				value = GameControls.I.Player.GetComponent<Rigidbody>().velocity.magnitude;
			}
		}
		else
		{
			value = GetComponent<Rigidbody>().velocity.magnitude;
		}

		_animator.SetFloat(_animatorSpeedId, value);
	}

    
	private void OnPhotonInstantiate(PhotonMessageInfo info)
	{
  
		ManInfo = new PhotonManInfo
		{
			HealthPoints = (short)100,
			Equipment = new Dictionary<string, string>()
		};

        if (!WorldController.I.WorldPlayers.ContainsKey((int)base.photonView.instantiationData[0]))
		{
			WorldController.I.WorldPlayers.Add((int)base.photonView.instantiationData[0], this);
		}
		else if (WorldController.I.WorldPlayers[(int)base.photonView.instantiationData[0]] == null || WorldController.I.WorldPlayers[(int)base.photonView.instantiationData[0]].InstTime < InstTime)
		{
			WorldController.I.WorldPlayers[(int)base.photonView.instantiationData[0]] = this;
		}

        InitializeAnimatorHash();
        InstTime = info.timestamp;

        _name = info.sender.NickName;
        _id = (int)base.photonView.instantiationData[0];

        if (base.photonView.isMine)
		{
			GetComponent<Collider>().enabled = false;
			_model.SetActive(false);
			GetComponent<Rigidbody>().isKinematic = true;
		}
		else
		{
			GetComponent<Collider>().enabled = true;
			GetComponent<Rigidbody>().isKinematic = false;
		}
	}

    private void OnDestroy()
	{
		if (WorldController.I.WorldPlayers.ContainsKey(_id) && WorldController.I.WorldPlayers[_id] == this)
		{
			WorldController.I.WorldPlayers.Remove(_id);
		}
	}

    public void OnPlayerRespawn(bool afterReconnect)
    {
        base.transform.parent = GameControls.I.PlayerTransform;
        base.transform.localRotation = Quaternion.identity;
        base.transform.localPosition = Vector3.zero;
        base.transform.localScale = Vector3.one;

        if (base.photonView.isMine && !afterReconnect)
        {
            base.photonView.RPC("OnRespawn", PhotonTargets.Others);

            if (OrbitCameraController.I != null)
            {
                OrbitCameraController.I.canUse = true;
            }
        }

        ResetStateEffects();
    }

	private void EnableStateEffects(bool returnToDefault)
	{
		if (returnToDefault)
		{
			ResetStateEffects();
		}
		StartStateEffects();
	}

	
	public void Respawn(bool firstSpawn)
	{
		base.photonView.RPC("RespawnPlayer", PhotonTargets.All, firstSpawn);
	}

	[PunRPC]
	private void RespawnPlayer(bool firstSpawn)
	{
		ManInfo.HealthPoints = (short)100;
		if (base.photonView.isMine)
		{
			if (DataKeeper.GameType != GameType.Tutorial && DataKeeper.GameType != GameType.BattleRoyale && DataKeeper.GameType != GameType.SkyWars)
			{
				EnableStateEffects(!firstSpawn);
			}
			if (!firstSpawn)
			{
				GameUIController.I.ShowRespawnMenu(false);
			}
			
			PlayerSpawnsController.I.OnRespawn();
			GameControls.I.Walker.climbing = false;
			GameControls.I.Walker.noClimbingSfx = false;
		}
	}

	private void ResetStateEffects()
	{
		CurrentHunger = 0;
		CurrentThirst = 0;
		CurrentSickness = 0;
		GameControls.I.Walker.staminaForSprintAmt = GameControls.I.Walker.staminaForSprint;
		GameControls.I.Player.hitPoints = 100f;
		if (StatesView.I != null)
		{
			StatesView.I.UpdateState(ManState.HealthPoint, ManInfo.HealthPoints);
			StatesView.I.UpdateState(ManState.Hunger,      CurrentHunger);
			StatesView.I.UpdateState(ManState.Thirst,      CurrentThirst);
			StatesView.I.UpdateState(ManState.Sickness,    CurrentSickness);
		}
	}

	public void StartStateEffects()
	{
		StartCoroutine("Hunger");
		StartCoroutine("Thirst");
		StartCoroutine("StatesEffects");
	}

	private void DisableStateEffects()
	{
		Debug.Log("DisableStateEffects");
		StopAllCoroutines();
	}

	public void UseHallucinogen(HallucinogenType type)
	{
		if (type != 0)
		{
			StopCoroutine("Hallucination");
			StartCoroutine("Hallucination", type);
		}
	}

	private System.Collections.IEnumerator StatesEffects()
	{
		int damage3 = ((CurrentHunger >= 100) ? 10 : 0);
		damage3 += ((CurrentThirst >= 100) ? 10 : 0);
		damage3 += ((CurrentSickness >= 100) ? 10 : 0);
		yield return new WaitForSeconds(2f);
		if (damage3 > 0)
		{
			base.photonView.RPC("Hit", PhotonTargets.All, (short)damage3, (byte)0, null);
		}
		StartCoroutine("StatesEffects");
	}

	private System.Collections.IEnumerator Hallucination(HallucinogenType type)
	{
		CurrentHallucinogen = type;
		HallucinogenController.I.EnableHallucinogen(CurrentHallucinogen);
		yield return new WaitForSeconds(50.0f);
		HallucinogenController.I.DisableHallucinogen();
		CurrentHallucinogen = HallucinogenType.None;
	}

	public void AddSickness(byte value)
	{
		CurrentSickness = GetChangedState(CurrentSickness, value);
	}

	private System.Collections.IEnumerator Hunger()
	{
		yield return new WaitForSeconds(40.0f);
		ChangeState(ManState.Hunger, 1);
		StartCoroutine("Hunger");
	}

	private System.Collections.IEnumerator Thirst()
	{
		yield return new WaitForSeconds(30.0f);
		ChangeState(ManState.Thirst, 1);
		StartCoroutine("Thirst");
	}

	private int GetChangedState(int value, short additionalValue)
	{
		return Mathf.Max(0, Mathf.Min(100, value + additionalValue));
	}

	public void ChangeState(ManState state, short value)
	{
		switch (state)
		{
		case ManState.Hunger:
			CurrentHunger = GetChangedState(CurrentHunger, value);
			StatesView.I.UpdateState(state, CurrentHunger);
			break;
		case ManState.Thirst:
			CurrentThirst = GetChangedState(CurrentThirst, value);
			StatesView.I.UpdateState(state, CurrentThirst);
			break;
		case ManState.Sickness:
			CurrentSickness = GetChangedState(CurrentSickness, value);
			StatesView.I.UpdateState(state, CurrentSickness);
			break;
		case ManState.HealthPoint:
			break;
		case ManState.Stamina:
		{
			float num = (float)value * GameControls.I.Walker.staminaForSprint / 100f;
			GameControls.I.Walker.staminaForSprintAmt = Mathf.Min(GameControls.I.Walker.staminaForSprint, GameControls.I.Walker.staminaForSprintAmt + num);
			break;
		}
		}
	}

	public void SetStates(byte hp, byte hunger, byte thirst, byte sickness)
	{
		if (hp > 0)
		{
			byte obscuredByte = (byte)(hp - 100);
			ChangeState(ManState.HealthPoint, obscuredByte);
		}
		if (hunger > 0)
		{
			ChangeState(ManState.Hunger, hunger);
		}
		if (thirst > 0)
		{
			ChangeState(ManState.Thirst, thirst);
		}
		if (sickness > 0)
		{
			ChangeState(ManState.Sickness, sickness);
		}
	}

	public void EuqipItem(Clothing info)
	{
		string text = string.Empty;
		switch (info.BodyPart)
		{
		case ClothingBodyPart.Bodywear:
		case ClothingBodyPart.Legwear:
			text = info.Id;
			break;
		case ClothingBodyPart.Backpack:
		case ClothingBodyPart.Headwear:
		case ClothingBodyPart.Vest:
			text = info.Prefab;
			break;
		}
		base.photonView.RPC("EquipClothingSync", PhotonTargets.All, info.BodyPart.ToString(), text);
	}

	public void TakeOffItem(ClothingBodyPart bodyPart)
	{
		base.photonView.RPC("EquipClothingSync", PhotonTargets.All, bodyPart.ToString(), string.Empty);
	}

	public void HitPlayer(short damage, byte radiation, int attackerSocialId)
	{
		if (!IsDead)
		{
			base.photonView.RPC("Hit", PhotonTargets.All, damage, radiation, attackerSocialId);
		}
	}

	public void Suicide()
	{
		base.photonView.RPC("Hit", PhotonTargets.All, (short)1000, (byte)0, _id);
	}

	[PunRPC]
	private void PlaceItem(int playerUId, Vector3 position, Quaternion rotation, short itemIndex)
	{
		if (PhotonNetwork.isMasterClient && itemIndex >= 0 && itemIndex < DataKeeper.Info.DestructibleObjects.Count)
		{
			DestructibleObject destructibleObject = DataKeeper.Info.DestructibleObjects[itemIndex];
			if (DataKeeper.Info.DestructibleObjects.Count > itemIndex)
			{
				PhotonNetwork.InstantiateSceneObject("WorldObjects/" + destructibleObject.Prefab, position, rotation, 0, new object[5] { playerUId, itemIndex, null, null, null });
			}
		}
	}

	[PunRPC]
	private void DropItem(Vector3 position, string id, int count, byte additionalCount)
	{
		if (PhotonNetwork.isMasterClient)
		{
			Item itemInfo = DataKeeper.Info.GetItemInfo(id);
			RaycastHit hitInfo;
			if (itemInfo != null && Physics.Raycast(position, -Vector3.up, out hitInfo, 100f, DataKeeper.I.ItemsCollisionsMask))
			{
				Quaternion rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, hitInfo.normal), Vector3.Cross(Vector3.up, hitInfo.normal));
				PhotonNetwork.InstantiateSceneObject("PhotonItem", hitInfo.point, rotation, 0, new object[4]
				{
					itemInfo.Id,
					(byte)count,
					true,
					additionalCount
				});
			}
		}
	}

	public void PlayerDropItem(Vector3 position, string id, int count, byte additionalCount)
	{
		base.photonView.RPC("DropItem", PhotonTargets.MasterClient, position, id, count, additionalCount);
	}

	public void PlayerPlaceItem(Vector3 position, Quaternion rotation, string id)
	{
		short num = (short)DataKeeper.Info.DestructibleObjects.FindIndex((DestructibleObject obj) => obj.Id == id);
		base.photonView.RPC("PlaceItem", PhotonTargets.MasterClient, DataKeeper.backendInfo.playerId, position, rotation, num);
	}

	[PunRPC]
	private void ChangeItemInHandView(string info)
	{
		if (!string.IsNullOrEmpty(info))
		{
			_clothingController.AddInHand(info);
		}
		else
		{
			_clothingController.RemoveFromHand();
		}
	}

	[PunRPC]
	private void EquipClothingSync(string bodyPart, string info)
	{
		ClothingBodyPart bodyPart2 = (ClothingBodyPart)(byte)System.Enum.Parse(typeof(ClothingBodyPart), bodyPart);
		if (string.IsNullOrEmpty(info))
		{
			_clothingController.TakeOff(bodyPart2);
		}
		else
		{
			_clothingController.Equip(bodyPart2, info, base.photonView.isMine);
		}
	}

	public void ChangeItemInHand(string itemId)
	{
		base.photonView.RPC("ChangeItemInHandView", PhotonTargets.All, (string)itemId);
	}

	public void ShowPlayerAttack(Vector3? targetPoint)
	{
		if (!(PlayerSpawnsController.I != null) || !(WorldController.I.Player != null))
		{
			return;
		}

		PhotonPlayer[] nearPhotonPlayers = _mySectorController.GetNearPhotonPlayers();
		if (nearPhotonPlayers != null)
		{
			List<PhotonPlayer> list = new List<PhotonPlayer>(nearPhotonPlayers);
			if (list.Contains(PhotonNetwork.player))
			{
				list.Remove(PhotonNetwork.player);
			}
			if (targetPoint.HasValue)
			{
				base.photonView.RPC_ToListOfPlayers("ShowAttackAtPoint", list.ToArray(), (short)targetPoint.Value.x, (short)targetPoint.Value.y, (short)targetPoint.Value.z);
			}
			else
			{
				base.photonView.RPC_ToListOfPlayers("ShowAttack", list.ToArray());
			}
		}
	}

	[PunRPC]
	private void ShowAttackAtPoint(short targetX, short targetY, short targetZ)
	{
		if (PlayerSpawnsController.I == null || WorldController.I.Player == null)
		{
			return;
		}
		if (_clothingController == null)
		{
			Debug.LogError("_clothingController == null");
		}
		if (DataKeeper.Info == null)
		{
			Debug.LogError("DataKeeper.Info == null");
		}
		if (DataKeeper.I == null)
		{
			Debug.LogError("DataKeeper.I == null");
		}
		if (GameControls.I == null)
		{
			Debug.LogError("GameControls.I == null");
		}
		Weapon weapon = null;
		int num = 1;
		float maxDistance = 3f;
		if (!string.IsNullOrEmpty(_clothingController.ItemInHandId))
		{
			weapon = DataKeeper.Info.GetWeaponInfo(_clothingController.ItemInHandId);
			if (weapon != null)
			{
				num = DataKeeper.I.GetWeaponIndex(weapon.Id);
				maxDistance = weapon.Range;
			}
		}
		if (num < GameControls.I.PlayerWeapons.WeaponsBehavioursList.Count)
		{
			WeaponBehavior weaponBehavior = GameControls.I.PlayerWeapons.WeaponsBehavioursList[num];
			if (WorldController.I.Player != null && Vector3.Distance(WorldController.I.Player.transform.position, base.transform.position) < 100f)
			{
				_weaponAudioSource.clip = weaponBehavior.fireSnd;
				_weaponAudioSource.pitch = UnityEngine.Random.Range(0.96f * Time.timeScale, 1f * Time.timeScale);
				_weaponAudioSource.PlayOneShot(_weaponAudioSource.clip, 0.9f / _weaponAudioSource.volume);
				_thirdPersonAttackController.ShowAttackAnimation(weapon);
			}
			Vector3 vector = new Vector3(targetX, targetY, targetZ);
			Vector3 direction = vector - base.transform.position;
			RaycastHit hitInfo;
			if (Physics.Raycast(base.transform.position, direction, out hitInfo, maxDistance, weaponBehavior.bulletMask) && WorldController.I.Player != null && Vector3.Distance(WorldController.I.Player.transform.position, hitInfo.point) < 40f)
			{
				weaponBehavior.Effects.ImpactEffects(hitInfo);
				weaponBehavior.Effects.BulletMarks(hitInfo);
			}
		}
	}

	[PunRPC]
	private void ShowAttack()
	{
		if (WorldController.I == null || WorldController.I.Player == null)
		{
			return;
		}
		if (_clothingController == null)
		{
			Debug.LogError("_clothingController == null");
		}
		if (DataKeeper.Info == null)
		{
			Debug.LogError("DataKeeper.Info == null");
		}
		if (DataKeeper.I == null)
		{
			Debug.LogError("DataKeeper.I == null");
		}
		if (GameControls.I == null)
		{
			Debug.LogError("GameControls.I == null");
		}

		Weapon weapon = null;
		int num = 1;
		if (!string.IsNullOrEmpty(_clothingController.ItemInHandId))
		{
			weapon = DataKeeper.Info.GetWeaponInfo(_clothingController.ItemInHandId);
			if (weapon != null)
			{
				num = DataKeeper.I.GetWeaponIndex(weapon.Id);
			}
		}
		if (num < GameControls.I.PlayerWeapons.WeaponsBehavioursList.Count)
		{
			WeaponBehavior weaponBehavior = GameControls.I.PlayerWeapons.WeaponsBehavioursList[num];
			if (WorldController.I.Player != null && Vector3.Distance(WorldController.I.Player.transform.position, base.transform.position) < 100f)
			{
				_weaponAudioSource.clip = weaponBehavior.fireSnd;
				_weaponAudioSource.pitch = UnityEngine.Random.Range(0.96f * Time.timeScale, 1f * Time.timeScale);
				_weaponAudioSource.PlayOneShot(_weaponAudioSource.clip, 0.9f / _weaponAudioSource.volume);
				_thirdPersonAttackController.ShowAttackAnimation(weapon);
			}
		}
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

	private void SetRagdollPositions(GameObject ragdoll, int killerSocId)
	{
		CopyTransformsRecurse(_model.transform, ragdoll.transform);
		if (killerSocId != 0 && killerSocId != DataKeeper.backendInfo.playerId)
		{
			PhotonMan photonMan = ((!WorldController.I.WorldPlayers.ContainsKey(killerSocId)) ? null : WorldController.I.WorldPlayers[killerSocId]);
			if ((bool)photonMan)
			{
				Vector3 vector = photonMan.transform.position - base.transform.position;
				RaycastHit hitInfo;
				if (Physics.SphereCast(photonMan.transform.position, 0.2f, vector, out hitInfo, 750f, _ragdollRaymask) && (bool)hitInfo.rigidbody)
				{
					hitInfo.rigidbody.AddForce(vector * 50f, ForceMode.Impulse);
				}
				else
				{
					Rigidbody[] componentsInChildren = ragdoll.GetComponentsInChildren<Rigidbody>();
					if (componentsInChildren != null)
					{
						Rigidbody[] array = componentsInChildren;
						foreach (Rigidbody rigidbody in array)
						{
							if (rigidbody.transform.name == "Body_jnt" || rigidbody.transform.name == "Spine_jnt")
							{
								rigidbody.AddForce((base.transform.position - photonMan.transform.position).normalized * 30f, ForceMode.Impulse);
								break;
							}
						}
					}
				}
			}
		}
		RemoveBody component = ragdoll.GetComponent<RemoveBody>();
		if ((bool)component)
		{
			component.bodyStayTime = 15f;
			component.enabled = true;
		}
	}

	[PunRPC]
	private void Heal(byte value)
	{
		if ((short)ManInfo.HealthPoints > 0)
		{
			ManInfo.HealthPoints = (short)Mathf.Min(100, (short)ManInfo.HealthPoints + value);
			if (base.photonView.isMine)
			{
				StatesView.I.UpdateState(ManState.HealthPoint, (short)ManInfo.HealthPoints);
				GameControls.I.Player.HealPlayer((int)value);
			}
		}
	}

	[PunRPC]
	private void Hit(short damage, byte radiation, int attackerSocialId, PhotonMessageInfo info)
	{
		if (((DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars) && BattleRoyaleGameManager.I != null && BattleRoyaleGameManager.I.IsLobby()) || IsDead)
		{
			return;
		}

		ManInfo.HealthPoints = (short)Mathf.Max(0, (short)ManInfo.HealthPoints - damage);

		if (IsDead)
		{
			_model.SetActive(false);
			GameObject gameObject = GameObject.Instantiate(_ragdoll, base.transform.position, base.transform.rotation);

			_clothingController.SetRagdollView(gameObject);
			SetRagdollPositions(gameObject, attackerSocialId);
		}
		if (!base.photonView.isMine)
		{
			return;
		}
		StatesView.I.UpdateState(ManState.HealthPoint, (short)ManInfo.HealthPoints);
		if (damage != 0)
		{
			GameControls.I.Player.ApplyDamage(damage, delegate
			{
				StartCoroutine(OnDie(attackerSocialId, info.sender));
			});
		}
		if (radiation > 0)
		{
			ChangeState(ManState.Sickness, radiation);
		}
		if ((short)ManInfo.HealthPoints != (short)(float)GameControls.I.Player.hitPoints)
		{
			Debug.Log(string.Concat("HEALTH IN PHOTON MAN ", ManInfo.HealthPoints, " IN REALISTIC ", GameControls.I.Player.hitPoints));
		}
		PushPlayer(attackerSocialId, damage);
	}

	private void PushPlayer(int attackerSocialId, short damage)
	{
		if (DataKeeper.GameType == GameType.SkyWars && attackerSocialId != 0 && attackerSocialId != -1)
		{
			Vector3 vector = (GameControls.I.Player.transform.position - WorldController.I.WorldPlayers[attackerSocialId].transform.position).normalized;
			if (damage == 0)
			{
				vector = -vector;
			}
			GameControls.I.Player.GetComponent<Rigidbody>().AddForce(vector * SkyWars.SkyWarsSetupOptions.HitForce, ForceMode.Force);
		}
	}

	[PunRPC]
	private void AddPlayerFrag()
	{
		WorldController.I.Statistics.PlayerKills++;
	}

	[PunRPC]
	private void AddZombieFrag()
	{
		WorldController.I.Statistics.ZombieKills++;
	}

	private System.Collections.IEnumerator OnDie(int attackerSocialId, PhotonPlayer killer)
	{
		if (OrbitCameraController.I != null)
		{
			OrbitCameraController.I.SwitchCameras(false);
			OrbitCameraController.I.canUse = false;
		}

		bool killedByPlayer = false;
		string killerName = string.Empty;
		string weaponId = string.Empty;

		if (attackerSocialId == DataKeeper.backendInfo.playerId)
		{
			killedByPlayer = true;
		}
		else
		{
			killerName = killer.NickName;

			if (attackerSocialId >= 0)
			{
				PhotonMan playerKiller = WorldController.I.WorldPlayers[attackerSocialId];
				if ((bool)playerKiller)
				{
					weaponId = playerKiller._clothingController.ItemInHandId;
					killedByPlayer = true;
				}
			}
		}

		if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars)
		{
			BattleRoyaleGameManager.I.CalculateMyFinalePlace();
			if (attackerSocialId != DataKeeper.backendInfo.playerId)
			{
				BattleRoyaleGameManager.I.StartShowKillLog(killerName, _name, weaponId, attackerSocialId);
			}
		}

		HallucinogenController.I.DisableHallucinogen();
		CurrentHallucinogen = HallucinogenType.None;

		InventoryController.Instance.OnDie(killedByPlayer);
		GameUIController.I.ShowCharacterMenu(false, CharacterMenuType.Menu, false);

		yield return new WaitForSeconds(2f);
		
		DisableStateEffects();
		GameUIController.I.ShowRespawnMenu(true, killerName, weaponId);
	}

	public void CallShowParachute(bool flag)
	{
		base.photonView.RPC("ShowParachute", PhotonTargets.Others, flag);
	}

	[PunRPC]
	public void ShowParachute(bool flag)
	{
		if (_parachute != null)
		{
			_parachute.gameObject.SetActive(flag);
		}
	}
}