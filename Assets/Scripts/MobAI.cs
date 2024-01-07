using System.Collections;
using System.Collections.Generic;
using Photon;
using UnityEngine;

public enum AIState
{
	Seek = 0,
	Follow = 1
}

public class MobAI : Photon.MonoBehaviour
{
	[SerializeField]
	private ZombieSoundController _soundController;

	[SerializeField]
	private bool _agentUpdateRotation;

	[SerializeField]
	private float _rotationSpeed = 5f;

	[SerializeField]
	private float _attackHeightOffset;

	[SerializeField]
	private UnityEngine.AI.NavMeshAgent _agent;

	public LayerMask AttackCullingMask;

	public bool Aggressive;

	public float WalkSpeed;

	public float SeekDistanceValue;

	public int SeekPointsCount;

	public float SeekTime;

	public float AttackDistance;

	public float AttackCooldown;

	public int PriorityTreshold;

	private List<PhotonMan> _currentEnemys = new List<PhotonMan>();

	private PhotonMan _currentPlayer;

	private Transform _currentTarget;

	private Vector3 _targetPosition;

	private Vector3 _defaultPosition;

	private float _damage;

	private byte _radiation;

	private bool _canAttack;

	private bool _attacking;

	private float _attackCooldown;

	private float _currentRotationSpeed;

	private bool _isDayLogic;

	public AIState CurrentState { get; private set; }

	public bool Enabled { get; private set; }

	public List<PhotonMan> Enemys
	{
		get
		{
			return _currentEnemys;
		}
	}

	public Vector3 Speed
	{
		get
		{
			return _agent.velocity;
		}
	}

	public float TurnSpeed
	{
		get
		{
			return _currentRotationSpeed;
		}
	}

	private void Awake()
	{
		_canAttack = true;
		_agent.updateRotation = _agentUpdateRotation;
	}

	public void SetInfo(Vector3 defaultPosition, float damage, byte radiation)
	{
		_defaultPosition = defaultPosition;
		_damage = damage;
		_radiation = radiation;
	}

	public void EnableAI()
	{
		if (!Enabled)
		{
			_agent.enabled = true;
			StartCoroutine(DetectionLogic());
			Enabled = true;
		}
	}

	public void DisableAI()
	{
		Enabled = false;
		_agent.enabled = false;
		CurrentState = AIState.Seek;
		_currentTarget = null;
		_canAttack = true;
		StopAllCoroutines();
	}

	public void Push(Vector3 direction, float force)
	{
		_agent.isStopped = true;
		GetComponent<Rigidbody>().AddForce(direction * force, ForceMode.Impulse);
		float waitTime = 0.3f;
		StartCoroutine(OnPushCallback(waitTime));
	}

	private IEnumerator OnPushCallback(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		GetComponent<Rigidbody>().Sleep();
		GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
		GetComponent<Rigidbody>().velocity = Vector3.zero;
		_agent.isStopped = false;
	}

	private void SetCurrentTarget()
	{
		RemoveAllEnemysNull();
		if (_currentEnemys.Count > 0)
		{
			if (_currentEnemys.Count > 1)
			{
				int num = 0;
				int num2 = 0;
				PhotonMan photonMan = _currentPlayer;
				foreach (PhotonMan currentEnemy in _currentEnemys)
				{
					if (!(currentEnemy == null) && !currentEnemy.IsDead)
					{
						float currentHp = ((currentEnemy.ManInfo == null) ? 100f : ((float)(short)currentEnemy.ManInfo.HealthPoints));
						int num3 = CalculatePlayerPriority(currentHp, 100f, currentEnemy.transform.position);
						if (photonMan == null || photonMan.IsDead)
						{
							photonMan = currentEnemy;
							_currentPlayer = currentEnemy;
							num = num3;
							_currentTarget = currentEnemy.transform;
						}
						else if (photonMan == _currentPlayer)
						{
							num = num3;
						}
						if (num3 > num2)
						{
							num2 = num3;
							photonMan = currentEnemy;
						}
					}
				}
				if (photonMan != null && !photonMan.IsDead)
				{
					if (photonMan != _currentPlayer && num2 - num >= PriorityTreshold)
					{
						_currentTarget = photonMan.transform;
						_currentPlayer = photonMan;
					}
				}
				else
				{
					_currentTarget = null;
					_currentPlayer = null;
				}
			}
			else if (_currentEnemys[0] != null && !_currentEnemys[0].IsDead)
			{
				_currentTarget = _currentEnemys[0].transform;
				_currentPlayer = _currentEnemys[0];
			}
			else
			{
				_currentTarget = null;
			}
		}
		else
		{
			_currentTarget = null;
		}
		if (_currentTarget != null)
		{
			if (CurrentState != AIState.Follow)
			{
				base.photonView.RPC("OnFindTarget", PhotonTargets.All);
				StartCoroutine(Follow());
			}
		}
		else
		{
			SeekPlayer();
		}
	}

	private void RemoveAllEnemysNull()
	{
		_currentEnemys.RemoveAll((PhotonMan enemy) => enemy == null);
	}

	private int CalculatePlayerPriority(float currentHp, float maxHp, Vector3 position)
	{
		return (int)(100f * (1f - currentHp / maxHp)) + (int)(Vector3.Distance(base.transform.position, position) * 100f / 10f);
	}

	private void SeekPlayer()
	{
		if (CurrentState != 0)
		{
			CurrentState = AIState.Seek;
			for (int i = 0; i < SeekPointsCount; i++)
			{
				float num = UnityEngine.Random.Range(0f - SeekDistanceValue, SeekDistanceValue);
				float num2 = UnityEngine.Random.Range(0f - SeekDistanceValue, SeekDistanceValue);
				Vector3 target = new Vector3(_targetPosition.x + num, _targetPosition.y, _targetPosition.z + num2);
				MoveTo(target, true);
			}
			StartCoroutine(ReturnToDefaultPosition());
		}
	}

	private void LookAtTarget()
	{
		Vector3 zero = Vector3.zero;
		zero = ((!((double)_agent.velocity.magnitude < 0.1)) ? _agent.velocity : ((!(_currentTarget != null)) ? _agent.velocity : (_currentTarget.position - base.transform.position)));
		zero.y = 0f;
		if (zero != Vector3.zero)
		{
			base.transform.rotation = Quaternion.Lerp(base.transform.rotation, Quaternion.LookRotation(zero), _rotationSpeed * Time.deltaTime);
			_currentRotationSpeed = _rotationSpeed * Time.deltaTime;
		}
		else
		{
			_currentRotationSpeed = 0f;
		}
	}

	private void MoveTo(Vector3 target, bool appendAsWayPoint)
	{
		if (!appendAsWayPoint)
		{
			_agent.SetDestination(target);
		}
	}

	private IEnumerator DetectionLogic()
	{
		SetCurrentTarget();
		yield return new WaitForSeconds(2f);
		StartCoroutine(DetectionLogic());
	}

	private IEnumerator ReturnToDefaultPosition()
	{
		yield return new WaitForSeconds(SeekTime);
		if (CurrentState == AIState.Seek)
		{
			MoveTo(_defaultPosition, false);
		}
	}

	private IEnumerator CooldownAttack()
	{
		yield return new WaitForSeconds(_attackCooldown);
		_canAttack = true;
	}

	private IEnumerator Follow()
	{
		if (_currentTarget == null)
		{
			yield return null;
		}
		_targetPosition = _currentTarget.transform.position;
		CurrentState = AIState.Follow;
		if (!_attacking)
		{
			MoveTo(_targetPosition, false);
		}
		yield return new WaitForSeconds(0.5f);
		if (_currentTarget != null)
		{
			StartCoroutine(Follow());
		}
		else
		{
			SeekPlayer();
		}
	}

	private void AttackProcess()
	{
		if (!_canAttack || !(_currentTarget != null))
		{
			return;
		}
		Vector3 vector = base.transform.position + new Vector3(0f, _attackHeightOffset, 0f);
		if (!(Vector3.Distance(_currentTarget.position, vector) <= AttackDistance))
		{
			return;
		}
		_canAttack = false;
		_attacking = true;
		Vector3 direction = _currentTarget.position - vector;
		if (Vector3.Dot(direction.normalized, base.transform.forward) > 0.4f)
		{
			RaycastHit hitInfo;
			if (Physics.Raycast(vector, direction, out hitInfo, AttackDistance, AttackCullingMask))
			{
				switch (hitInfo.collider.gameObject.layer)
				{
				case 11:
				    WorldController.I.Player.HitPlayer((short)_damage, _radiation, 0);
					base.photonView.RPC("OnAttackTarget", PhotonTargets.All);
					break;
				case 25:
				{
					PhotonMan component = hitInfo.transform.GetComponent<PhotonMan>();
					if ((bool)component && !component.IsDead)
					{
						component.HitPlayer((short)_damage, _radiation, 9);
						base.photonView.RPC("OnAttackTarget", PhotonTargets.All);
					}
					break;
				}
				}
				StartCoroutine(CooldownAttack());
			}
			else
			{
				_canAttack = true;
			}
		}
		_attacking = false;
	}

	private void DayTimeBalanceLogic()
	{
		if (WorldController.I != null)
		{
			if (WorldController.I.IsDay)
			{
				if (!_isDayLogic)
				{
					_agent.speed = WalkSpeed * ZombieBalanceService.DayZombieSpeedFactor;
					_attackCooldown = AttackCooldown * ZombieBalanceService.DayZombieAttackColdownFactor;
					_isDayLogic = true;
				}
			}
			else if (_isDayLogic)
			{
				_agent.speed = WalkSpeed;
				_attackCooldown = AttackCooldown;
				_isDayLogic = false;
			}
		}
	}

	private void Update()
	{
		if (!Enabled)
		{
			return;
		}

		DayTimeBalanceLogic();
		if (!_agent.updateRotation)
		{
			LookAtTarget();
		}
		if (Aggressive)
		{
			AttackProcess();
		}
	}

	[PunRPC]
	private void OnAttackTarget()
	{
		_soundController.OnAttack();
	}

	[PunRPC]
	private void OnFindTarget()
	{
		_soundController.OnFindTarget();
	}
}
