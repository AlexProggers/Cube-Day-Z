using UnityEngine;

namespace BattleRoyale
{
	public class FarClippingManager : MonoBehaviour
	{
		public Camera mainCam;

		public static FarClippingManager I;

		private void Awake()
		{
			if (I == null)
			{
				I = this;
			}

			if (DataKeeper.GameType == GameType.SkyWars)
			{
				SetupFarClippingMainCam(300f);
			}

			else if (DataKeeper.GameType == GameType.BattleRoyale)
			{
				SetupFarClippingMainCam(800f);
			}
		}

		public void SetupFarClippingMainCam(float range)
		{
			mainCam.farClipPlane = range;
		}
	}
}
