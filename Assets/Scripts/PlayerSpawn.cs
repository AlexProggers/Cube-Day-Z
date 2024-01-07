using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
	private const float RayDistance = 150f;

	private const int MaxSpawnRetries = 10;

	[SerializeField]
	private Vector3 _spawnArea;

	[SerializeField]
	private float _playerRadius;

	[SerializeField]
	private float _heightOffset;

	[SerializeField]
	private float _playerHeight;

	[SerializeField]
	private LayerMask _cullingMask;

	[SerializeField]
	private LayerMask _spawnOn;

    public bool Respawn(GameObject player, bool firstSpawn)
	{
		Vector3 randomSpawnPosition = GetRandomSpawnPosition();
		for (int i = 0; i < 10; i++)
		{
			if (!Physics.CheckSphere(randomSpawnPosition, _playerRadius, _cullingMask))
			{
				break;
			}
			randomSpawnPosition = GetRandomSpawnPosition();
		}
		if (Physics.CheckSphere(randomSpawnPosition, _playerRadius, _cullingMask))
		{
			Debug.Log("Can't spawn player here!");
			return false;
		}
		player.transform.position = randomSpawnPosition;
		if (Camera.main != null)
		{
			CameraKick component = Camera.main.GetComponent<CameraKick>();
			component.OnRespawn();
		}
        WorldController.I.Player.Respawn(firstSpawn);
		return true;
	}

    private void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 0.3f);
		Gizmos.DrawCube(base.transform.position + new Vector3(0f, _heightOffset, 0f), _spawnArea);
	}

	private Vector3 GetRandomSpawnPosition()
	{
		Vector3 origin = new Vector3(base.transform.position.x + UnityEngine.Random.Range((0f - _spawnArea.x) / 2f, _spawnArea.x / 2f), base.transform.position.y + _heightOffset + _spawnArea.y / 2f, base.transform.position.z + UnityEngine.Random.Range((0f - _spawnArea.z) / 2f, _spawnArea.z / 2f));
		Ray ray = new Ray(origin, -Vector3.up);
		RaycastHit hitInfo;
		if (Physics.Raycast(ray, out hitInfo, 150f, _spawnOn))
		{
			return VectorWithPlayerHeigthOffset(hitInfo.point);
		}
		return VectorWithPlayerHeigthOffset(base.transform.position);
	}

	private Vector3 VectorWithPlayerHeigthOffset(Vector3 vector3)
	{
		return new Vector3(vector3.x, vector3.y + _playerHeight, vector3.z);
	}
}
