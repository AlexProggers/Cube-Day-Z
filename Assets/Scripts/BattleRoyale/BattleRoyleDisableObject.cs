using UnityEngine;

namespace BattleRoyale
{
	public class BattleRoyleDisableObject : MonoBehaviour
	{
		public bool reverse;

		private void OnEnable()
		{
			if (DataKeeper.GameType != GameType.BattleRoyale && !reverse)
			{
				base.gameObject.SetActive(false);
			}
			if (reverse && DataKeeper.GameType == GameType.BattleRoyale)
			{
				base.gameObject.SetActive(false);
			}
		}
	}
}
