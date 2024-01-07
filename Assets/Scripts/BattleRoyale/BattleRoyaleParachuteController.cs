using UnityEngine;

namespace BattleRoyale
{
	public class BattleRoyaleParachuteController : MonoBehaviour
	{
		public Transform Player;

		public Rigidbody RigidPlayer;

		private void FixedUpdate()
		{
			if (Input.GetKey(KeyCode.W))
			{
				RigidPlayer.AddForce(Player.transform.forward * 13f, ForceMode.Force);
			}
		}
	}
}
