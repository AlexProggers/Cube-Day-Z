using UnityEngine;

namespace SkyWars
{
	public class SkyWarsRedButton : MonoBehaviour
	{
		private void OnTriggerEnter(Collider collider)
		{
			if (collider.name.Contains("Player"))
			{
				SkyWarsGameLogic.I.StartDestroyIslands();
			}
		}
	}
}
