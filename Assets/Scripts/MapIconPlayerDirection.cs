using UnityEngine;

public class MapIconPlayerDirection : MonoBehaviour
{
	private void Update()
	{
		if (WorldController.I && WorldController.I.Player)
		{
			float y = WorldController.I.Player.transform.eulerAngles.y;
			Vector3 eulerAngles = base.transform.eulerAngles;
			eulerAngles.z = y;
			base.transform.eulerAngles = -eulerAngles;
		}
	}
}
