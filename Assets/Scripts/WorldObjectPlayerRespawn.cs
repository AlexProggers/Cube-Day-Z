using UnityEngine;

public class WorldObjectPlayerRespawn : MonoBehaviour
{
	[SerializeField]
	private PhotonWorldObject _photonObject;

	[SerializeField]
	private PlayerSpawn _spawn;

	private bool _isInitialize;

	private void Update()
	{
		if (!_isInitialize && _photonObject.ObjInfo != null)
		{
			if (_photonObject.ObjInfo.OwnerUId == DataKeeper.backendInfo.playerId)
			{
				PlayerSpawnsController.I.SetSpawn(_spawn);
			}
			_isInitialize = true;
		}
	}
}
