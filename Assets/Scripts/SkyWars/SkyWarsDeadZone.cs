
using UnityEngine;

namespace SkyWars
{
	public class SkyWarsDeadZone : MonoBehaviour
	{
		private void OnTriggerEnter(Collider collider)
		{
			if (collider.name.Contains("Player"))
			{
				short damage = 500;
				WorldController.I.Player.HitPlayer(damage, (byte)1, -1);
			}
		}
	}
}
