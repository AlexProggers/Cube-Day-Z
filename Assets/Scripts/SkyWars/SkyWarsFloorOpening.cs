using System.Collections;
using UnityEngine;

namespace SkyWars
{
	public class SkyWarsFloorOpening : MonoBehaviour
	{
		[SerializeField]
		private float _waitTime = 5f;

		private void OnTriggerEnter(Collider collider)
		{
			if (collider.name.Contains("Player"))
			{
				StartCoroutine("OpenFloor");
			}
		}

		private IEnumerator OpenFloor()
		{
			yield return new WaitForSeconds(_waitTime);
			base.gameObject.SetActive(false);
			SkyWarsSetupOptions.AddWoodenBlocksToInventary();
			SkyWarsGameLogic.I.CallStartBridgesMoving();
		}
	}
}
