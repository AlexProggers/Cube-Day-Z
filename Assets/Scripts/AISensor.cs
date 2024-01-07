using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AISensor : MonoBehaviour
{
	private MobAI _ai;

	public float SensorRadius { get; private set; }

	public void Initialize(MobAI ai)
	{
		_ai = ai;
		SphereCollider sphereCollider = GetComponent<Collider>() as SphereCollider;
		if ((bool)sphereCollider)
		{
			SensorRadius = sphereCollider.radius;
		}
	}

	private void OnTriggerEnter(Collider enemy)
	{
		PhotonMan componentInParent = enemy.GetComponentInChildren<PhotonMan>();
		if ((bool)componentInParent && !_ai.Enemys.Contains(componentInParent))
		{
			Debug.Log("seek player!");
			_ai.Enemys.Add(componentInParent);
		}
	}

	private void OnTriggerExit(Collider enemy)
	{
		PhotonMan componentInParent = enemy.GetComponentInChildren<PhotonMan>();
		if ((bool)componentInParent && _ai.Enemys.Contains(componentInParent))
		{
			_ai.Enemys.Remove(componentInParent);
		}
	}
}
