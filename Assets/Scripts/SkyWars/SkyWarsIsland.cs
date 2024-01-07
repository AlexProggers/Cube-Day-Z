using UnityEngine;

namespace SkyWars
{
	public class SkyWarsIsland : MonoBehaviour
	{
		[SerializeField]
		private BoxCollider[] _colliders;

		[SerializeField]
		private Rigidbody _rb;

		public void DestroyIsland()
		{
			_rb.isKinematic = false;
			for (int i = 0; i < _colliders.Length; i++)
			{
				_colliders[i].enabled = false;
			}
		}
	}
}
