using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace BattleRoyale
{
	public class PlayerFogAreaChecker : MonoBehaviour
	{
		public GameObject LocalFog;

		private void OnEnable()
		{
			LocalFog.SetActive(false);
			if (DataKeeper.GameType != GameType.BattleRoyale)
			{
				base.enabled = false;
			}
			else
			{
				StartCoroutine("StartCheckFog");
			}
		}

		private IEnumerator StartCheckFog()
		{
			while (true)
			{
				if (BattleRoyaleFogManager.I != null && BattleRoyaleGameManager.I != null && !BattleRoyaleGameManager.I.IsLobby())
				{
					if (BattleRoyaleFogManager.I.IfObjectInsideTheGasArea(base.transform))
					{
						LocalFog.SetActive(true);
						short damage = (short)BattleRoyaleFogManager.I.GetCurrentDamage();
						if ((short)damage > 0)
						{
							WorldController.I.Player.HitPlayer(damage, (byte)0, -1);
						}
					}
					else
					{
						LocalFog.SetActive(false);
					}
				}
				yield return new WaitForSeconds(1f);
			}
		}
	}
}
