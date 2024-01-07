using JsonFx.Json;
using UnityEngine;

public enum Language
{
    Russian = 0,
    English = 1
}

public enum GameType
{
    FirstScene   = 0,
    Single       = 1,
    Multiplayer  = 2,
    Tutorial     = 3,
    BattleRoyale = 4,
    SkyWars      = 5
}

public enum Sex
{
    Male,
    Female
}

[System.Serializable]
public class KeyValueTexture
{
	public string Key;

	public Texture Value;
}


public class DataKeeper : MonoBehaviour
{
	public static DataKeeper I;

    public static bool IsBattleRoyaleClick = false;

    public static bool IsSkyWarsClick = false;

    public static bool IsNewGameClick = false;

    public static bool IsContinueClick = false;

    public static string BuildVersion = "2.144 A23";

    public static string ChatAppVersion = "1.216";

    public static float AutoSaveTime = 60f;

    public static float AutoSaveWorldTime = 30f;

    public static float AutoSaveWorldTimeTesting = 30f;

    public static short MaxPlayerHp = 100;

    public static float BR_MoreWeaponsChancePerSpawnZone = 0.2f;

    public static float LocalLootFactor = 0.75f;

    public static float ZombieLootFactor = 0.5f;

    public static float DefaultMobDropChance = 15f;

    public static float DistanceToDropLocalLoot = 75f;

    public static float DefaultPhotonItemLifetime = 180f;

    public static int PhotonDropChanceOnPremium = 10;

    public static Language Language = Language.Russian;

    public static GameType GameType = GameType.FirstScene;

    public static string NoIcon = "noIcon";

    public static bool OfflineMode = true;

    public static Sex Sex;

    public static byte FaceIndex;

    public static byte SkinColorIndex;

	public static ContentInfo Info;

	public LayerMask ItemsCollisionsMask;

    [SerializeField]
	private Texture _defaultClothingTex;
    
	[SerializeField]
	private System.Collections.Generic.List<KeyValueTexture> _clothingTextures;

	[SerializeField]
	private System.Collections.Generic.List<KeyValueInt> _weaponsIdOrder;

	public TextAsset worldContent;

	public TextAsset worldSkywars;

	public TextAsset worldUnturned;

    [SerializeField]
	private int _hungerStepTime = 30;

	[SerializeField]
	private int _thirstStepTime = 20;

	[SerializeField]
	private int _hallucinationTime = 60;

    public static BackendInfo backendInfo;

    public int HungerStepTime
	{
		get
		{
			return _hungerStepTime;
		}
	}

	public int ThirstStepTime
	{
		get
		{
			return _thirstStepTime;
		}
	}

	public int HallucinationTime
	{
		get
		{
			return _hallucinationTime;
		}
	}

	private void Awake()
	{
		I = this;
        DontDestroyOnLoad(gameObject);

		if(worldContent != null)
		{
			Initialize();
		}
	}

	private void Initialize()
	{
		Info = JsonReader.Deserialize<ContentInfo>(worldContent.text);
        Info.Initialize();

		backendInfo = new BackendInfo();
		backendInfo.playerId = UnityEngine.Random.Range(1, 999999);
	}

    public void AddWeaponOrder(string key, int id)
	{
		KeyValueInt keyValueInt = new KeyValueInt();
		keyValueInt.Key = key;
		keyValueInt.Value = id;
		_weaponsIdOrder.Add(keyValueInt);
	}

    public int GetWeaponIndex(string id)
	{
		KeyValueInt keyValueInt = _weaponsIdOrder.Find((KeyValueInt weaponInfo) => weaponInfo.Key == id);
		if (keyValueInt != null)
		{
			return keyValueInt.Value;
		}
		return 0;
	}

	public Texture GetClothingTexture(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		KeyValueTexture keyValueTexture = _clothingTextures.Find((KeyValueTexture texture) => texture.Key == id);
		return (keyValueTexture != null) ? keyValueTexture.Value : _defaultClothingTex;
	}

	public string GetClothingTextureId(Texture texture)
	{
		KeyValueTexture keyValueTexture = _clothingTextures.Find((KeyValueTexture tex) => tex.Value == texture);
		if (keyValueTexture != null)
		{
			return keyValueTexture.Key;
		}
		return null;
	}
}
