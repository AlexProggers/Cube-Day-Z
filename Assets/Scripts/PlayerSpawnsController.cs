using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnsController : MonoBehaviour
{
    public static PlayerSpawnsController I;

	[SerializeField]
	private GameObject _player;

	[SerializeField]
	private int _maxSpawnRetries = 5;

	[SerializeField]
	private List<PlayerSpawn> _availableSpawns;

	[SerializeField]
	private PlayerSpawn _battleRoyaleLobbySpawn;

	private PlayerSpawn _mySpawnArea;

	private bool _respawning;

	public SphereCollider SpawnSphere;

	public Transform[] SkyWarsSpawnPoints;
    public bool CanRespawnAtBed
	{
		get
		{
			return _mySpawnArea != null;
		}
	}

	private bool _firstSpawn;

    void Awake()
    {
        I = this;
		_firstSpawn = true;
    }

    
	public Vector3 GetRandomSpawnForBattleRoyale()
	{
		float x = UnityEngine.Random.insideUnitSphere.x * SpawnSphere.radius + SpawnSphere.transform.position.x;
		float z = UnityEngine.Random.insideUnitSphere.z * SpawnSphere.radius + SpawnSphere.transform.position.z;
		return new Vector3(x, 400f, z);
	}

	public void SetSpawn(PlayerSpawn spawn)
	{
		_mySpawnArea = spawn;
	}
    
	public void OnRespawn()
	{
		if (_player != null)
		{
			_player.SetActive(true);
			GameControls.I.Initialize();
		}

		WorldController.I.Player.OnPlayerRespawn(false);
		_respawning = false;
	}

	private void DefaultRespawn(GameObject player, bool firstSpawn)
	{
		int i = 0;
		if (_availableSpawns.Count > 0)
		{
			bool flag = false;
			for (; i < _maxSpawnRetries; i++)
			{
				if (flag)
				{
					break;
				}
				if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars)
				{
					PlayerSpawn battleRoyaleLobbySpawn = _battleRoyaleLobbySpawn;
					flag = battleRoyaleLobbySpawn.Respawn(player, battleRoyaleLobbySpawn);
				}
				else
				{
					PlayerSpawn playerSpawn = _availableSpawns[UnityEngine.Random.Range(0, _availableSpawns.Count)];
					flag = playerSpawn.Respawn(player, firstSpawn);
				}
			}
			if (!flag)
			{
				_respawning = false;
				Debug.LogWarning("Can't spawn player!");
			}
		}
		else
		{
			Debug.Log("Not specified respawn points!");
		}
	}

	public void RespawnAtBed()
	{
		if (_respawning)
		{
			return;
		}
		GameObject player = GameControls.I.Player.gameObject;
		_respawning = true;
		int i = 0;
		if (_mySpawnArea != null)
		{
			bool flag = false;
			for (; i < _maxSpawnRetries; i++)
			{
				if (flag)
				{
					break;
				}
				flag = _mySpawnArea.Respawn(player, false);
			}
			if (!flag)
			{
				DefaultRespawn(player, false);
			}
		}
		else
		{
			DefaultRespawn(player, false);
		}
	}

	public void Respawn(bool firstSpawn, Vector3? positionFromSave = null)
	{
		if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars)
		{
			firstSpawn = true;
			positionFromSave = null;
		}
		if (_respawning)
		{
			return;
		}
		GameObject gameObject = ((!firstSpawn) ? GameControls.I.Player.gameObject : _player);
		_respawning = true;
		if (WorldController.IsDieNotCorrect)
		{
			Debug.Log("PlayerSpawnsController    " + WorldController.IsDieNotCorrect);
			DefaultRespawn(gameObject, firstSpawn);
		}
		else if (positionFromSave.HasValue)
		{
			gameObject.transform.position = positionFromSave.Value;
			if (Camera.main != null)
			{
				CameraKick component = Camera.main.GetComponent<CameraKick>();
				component.OnRespawn();
			}
			WorldController.I.Player.Respawn(firstSpawn);
		}
		else
		{
			DefaultRespawn(gameObject, firstSpawn);
		}
	}
}
