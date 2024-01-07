using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JsonFx.Json;
using System.Runtime.ExceptionServices;

public class WorldObject
{
	public string ObjectId;

	public string Position;

	public string Rotation;

	public string AdditionInfo;

	public string OwnerId;
}

public class WorldInfo
{
	public string Id;

	public string Name;

	public List<WorldObject> Objects;

	public List<ObjectsSpawn> Spawns;
}

public class StatisticsInfo
{
	public int ZombieKills;

	public int CreatureKills;

	public int PlayerKills;

	public int CraftItems;

	public int PlantsPlanted;

	public int Die;

	public int Suicide;
}


public class WorldController : MonoBehaviour
{
	public static WorldController I;

	public const string ClothingPath = "Clothing/";

	public const string WeaponPath = "Weapon/";

	public const string ConsumablePath = "Consumable/";

	public const string WorldObjectsPath = "WorldObjects/";

	public const string MobsPath = "Mobs/";

	public const string PhotonItemPrefab = "PhotonItem";

	public const string LocalItemPrefab = "LocalItem";

	[SerializeField]
	private Transform _items;

	[SerializeField]
	private Transform _mobs;

	[SerializeField]
	private Transform _destructibleObjects;

	[SerializeField]
	private LayerMask _mobsSpawnLayerMask;

	[SerializeField]
	private PhotonObjectsManager _objectsManager;

	[SerializeField]
	private TOD_Sky _sky;

	[SerializeField]
	private TOD_Time _time;

	public Transform Items
	{
		get
		{
			return _items;
		}
	}

	public Transform Mobs
	{
		get
		{
			return _mobs;
		}
	}

	public Transform DestructibleObjects
	{
		get
		{
			return _destructibleObjects;
		}
	}

	public PhotonObjectsManager ObjectsManager
	{
		get
		{
			return _objectsManager;
		}
	}

	public Dictionary<int, PhotonMan> WorldPlayers = new Dictionary<int, PhotonMan>();

	[HideInInspector]
	public List<PhotonWorldObject> WorldObjects = new List<PhotonWorldObject>();
	[HideInInspector]
	public List<PhotonMob> WorldMobs = new List<PhotonMob>();

	[HideInInspector]
	public List<PhotonDropObject> WorldItems = new List<PhotonDropObject>();

	[HideInInspector]
	public List<LocalDropObject> LocalDropObjects = new List<LocalDropObject>();

	private bool _firstSpawn;

	private WorldInfo _worldInfo;

	public static bool IsDieNotCorrect;
    private string _myRoom;
    private bool _myRoomWithZombie;

    public StatisticsInfo Statistics { get; set; }
    public int ActiveMobCount { get; internal set; }


    public PhotonMan Player
    {
        get
        {
            return WorldPlayers[DataKeeper.backendInfo.playerId];
        }
    }

	public bool IsDay
	{
		get
		{
			return _sky.IsDay;
		}
	}

    public short[] GetItemsShortIds(string[] itemsIds)
	{
		List<Item> allItems = DataKeeper.Info.GetAllItems();
		short[] array = new short[itemsIds.Length];
		for (int i = 0; i < itemsIds.Length; i++)
		{
			array[i] = (short)allItems.FindIndex((Item obj) => obj.Id == itemsIds[i]);
		}
		return array;
	}

	public string[] GetItemsIdsByIndices(short[] itemsIndices)
	{
		List<Item> allItems = DataKeeper.Info.GetAllItems();
		string[] array = new string[itemsIndices.Length];
		for (int i = 0; i < itemsIndices.Length; i++)
		{
			array[i] = ((itemsIndices[i] >= 0) ? allItems[itemsIndices[i]].Id : null);
		}
		return array;
	}

    // Awake is called before the first frame update
    void Awake()
    {
		if (PlayerPrefs.HasKey("QuailityLvl"))
		{
			QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("QuailityLvl"));
		}
		StartCoroutine("DisableWeather");
		I = this;
		_firstSpawn = true;

		if (PhotonNetwork.room != null)
		{
			if (PhotonNetwork.isMasterClient)
			{
				PhotonNetwork.room.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { 
				{
					"startTime",
					PhotonNetwork.time
				} });
			}
		}
		else
		{
			Debug.LogError("room is null");
		}
		
		Statistics = new StatisticsInfo();
    }

	void Start()
	{
		OnJoinedRoom();
	}

	public IEnumerator DisableWeather()
	{
		yield return new WaitForSeconds(5f);
		if (DataKeeper.GameType == GameType.BattleRoyale || DataKeeper.GameType == GameType.SkyWars)
		{
			_time.DayLengthInMinutes = 2400f;
			_sky.Day.SunLightIntensity = 0.5f;
			_sky.Cycle.Hour = 17f;
		}
	}

	public void InstantiateLocalItem(string id, Vector3 position, Quaternion rotation, byte count, byte additionalCount)
	{
		LocalDropObject component = GameObject.Instantiate(Resources.Load<GameObject>(LocalItemPrefab), position, rotation).GetComponent<LocalDropObject>();
		component.Initialize(id, count, additionalCount);
	}

	private void InstantiateLocalItems()
	{
		if (_worldInfo.Spawns != null)
		{
			foreach (ObjectsSpawn spawn in _worldInfo.Spawns)
			{
				spawn.SpawnObjects(DataKeeper.Info, true);
			}
		}
	}

	public string GetResourceLoadPath(ItemType type)
	{
		switch (type)
		{
		case ItemType.Consumables:
			return "Consumable/";
		case ItemType.Weapon:
			return "Weapon/";
		case ItemType.Clothing:
			return "Clothing/";
		default:
			return string.Empty;
		}
	}

	private void CreateWorld()
	{
		_worldInfo = LoadWorldInfo();
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}

		if (_worldInfo.Objects != null)
		{
			foreach (WorldObject @object in _worldInfo.Objects)
			{
				DestructibleObject worldObjInfo = DataKeeper.Info.GetDestructibleObjectInfo(@object.ObjectId);
				short num = (short)DataKeeper.Info.DestructibleObjects.FindIndex((DestructibleObject wobj) => wobj.Id == worldObjInfo.Id);
				PhotonNetwork.InstantiateSceneObject("WorldObjects/" + worldObjInfo.Prefab, ParseUtils.Vector3FromString(@object.Position), Quaternion.Euler(ParseUtils.Vector3FromString(@object.Rotation)), 0, new object[5] { 0, num, null, null, null });
			}
		}
	}

	private IEnumerator AddMobsForPulling()
	{
		if (PhotonNetwork.room.CustomProperties.ContainsKey("zombies") && (bool)PhotonNetwork.room.CustomProperties["zombies"])
		{
			int simpleZombieCount = MobPullingSystem.Instance.GetAllMobsCountByType(ZombieType.Simple);
			int infectiveZombieCount = MobPullingSystem.Instance.GetAllMobsCountByType(ZombieType.Infective);
			int megaZombieCount = MobPullingSystem.Instance.GetAllMobsCountByType(ZombieType.Mega);
			for (int k = 0; k < ZombieBalanceService.SimpleZombieMaxCount - simpleZombieCount; k++)
			{
				PhotonNetwork.InstantiateSceneObject("SZ", Vector3.zero, Quaternion.identity, 0, null);
				yield return new WaitForSeconds(0.1f);
			}
			for (int j = 0; j < ZombieBalanceService.InfectiveZombieMaxCount - infectiveZombieCount; j++)
			{
				PhotonNetwork.InstantiateSceneObject("IZ", Vector3.zero, Quaternion.identity, 0, null);
				yield return new WaitForSeconds(0.1f);
			}
			for (int i = 0; i < ZombieBalanceService.MegaZombieMaxCount - megaZombieCount; i++)
			{
				PhotonNetwork.InstantiateSceneObject("MZ", Vector3.zero, Quaternion.identity, 0, null);
				yield return new WaitForSeconds(0.1f);
			}
		}
	}

	private void TrySyncTime()
	{
		if (PhotonNetwork.room != null)
		{
			if (PhotonNetwork.room.CustomProperties.ContainsKey("startTime"))
			{
				double num = (double)PhotonNetwork.room.CustomProperties["startTime"];
				_time.SyncTime((float)(PhotonNetwork.time - num));
			}
			else
			{
				Debug.Log("Time not sync!");
			}
		}
		else
		{
			Debug.LogError("room is null");
		}
	}

	private void OnJoinedRoom()
	{
		_myRoom = PhotonNetwork.room.Name;
		_myRoomWithZombie = false;

		TrySyncTime();

		if (PhotonNetwork.isMasterClient)
		{
			if (_firstSpawn)
			{
				CreateWorld();
			}

			StartCoroutine("AddMobsForPulling");
		}

		PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity, 0, new object[1]
        { 
            DataKeeper.backendInfo.playerId
        });

		if (_firstSpawn)
		{
			_worldInfo = LoadWorldInfo();
		    InstantiateLocalItems();

			PhotonSectorService.I.InitializeSpawnInfo(GetMobsSpawnInfo());
			PlayerSpawnsController.I.Respawn(_firstSpawn);

			_firstSpawn = false;
		}
	}

	private List<PhotonSpawnMobInfo> GetMobsSpawnInfo()
	{
		List<PhotonSpawnMobInfo> list = new List<PhotonSpawnMobInfo>();
		if (_worldInfo.Spawns != null)
		{
			foreach (ObjectsSpawn spawn in _worldInfo.Spawns)
			{
				SpawnType spawnType = spawn.SpawnType;
				if (spawnType == SpawnType.Mob)
				{
					list.AddRange(spawn.GetMobSpawnInfo(_mobsSpawnLayerMask));
				}
			}
		}
		return list;
	}

	public WorldInfo LoadWorldInfo()
	{
		TextAsset textAsset = null;

		if(DataKeeper.GameType == GameType.SkyWars)
		{
			textAsset = DataKeeper.I.worldSkywars;
		}

		else if(DataKeeper.GameType == GameType.Single || DataKeeper.GameType == GameType.Multiplayer || DataKeeper.GameType == GameType.BattleRoyale)
		{
			textAsset = DataKeeper.I.worldUnturned;
		}

		if (textAsset)
		{
			return JsonReader.Deserialize<WorldInfo>(textAsset.text);
		}
		return null;
	}
}
