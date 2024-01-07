using System.Collections.Generic;
using UnityEngine;

public class ZombieViewManager : MonoBehaviour
{
	public static ZombieViewManager I;

	[SerializeField]
	private List<GameObject> _zombiesPrefabs;

	private void Awake()
	{
		I = this;
	}

	public GameObject GetZombieObj(string prefabName)
	{
		return _zombiesPrefabs.Find((GameObject zombie) => zombie.name == prefabName);
	}
}
