using UnityEngine;

public class CameraCullingDistance : MonoBehaviour
{
	private void Start()
	{
		float[] array = new float[32];
		array[LayerMask.NameToLayer("SkyDome")] = 1500f;
		array[LayerMask.NameToLayer("HerdSim")] = 30f;
		array[LayerMask.NameToLayer("Mob")] = 50f;
		array[LayerMask.NameToLayer("Plants")] = 30f;
		array[LayerMask.NameToLayer("SmallDetails")] = 70f;
		array[LayerMask.NameToLayer("PhotonPlayer")] = 100f;
		array[LayerMask.NameToLayer("Destructible")] = 200f;
		array[LayerMask.NameToLayer("StaticObjects")] = 30f;
		GetComponent<Camera>().layerCullDistances = array;
		GetComponent<Camera>().layerCullSpherical = true;
	}
}
