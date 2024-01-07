using UnityEngine;

public static class ZombieBalanceService
{
	public static int SimpleZombieMaxCount = 225;

	public static int InfectiveZombieMaxCount = 25;

	public static int MegaZombieMaxCount = 1;

	public static float DayZombieSpeedFactor = 0.6f;

	public static float DayZombieAttackColdownFactor = 2f;

	public static float DayZombieCountFactor = 0.5f;

	public static int MaxZombieOnMap = 250;

	public static int MaxMobCountInSector = 10;

	public static int ZombiePerPlayerCount = 5;

	public static int CallculateSectorZombiesLimit(bool isDayTime, int zombiesOnMap, int availableZombiesInSector, int playersInRoom)
	{
		float num = ((!isDayTime) ? 1f : DayZombieCountFactor);
		int num2 = (int)((float)MaxZombieOnMap * num) - zombiesOnMap - Mathf.Max(0, ZombiePerPlayerCount * (playersInRoom - 1));
		if (num2 > 0)
		{
			int num3 = Mathf.Min(availableZombiesInSector, num2);
			if (num3 > 0)
			{
				return (int)Mathf.Min((float)num3 * num, MaxMobCountInSector);
			}
			return 0;
		}
		return 0;
	}
}
