using UnityEngine;

namespace SkyWars
{
	public class SkyWarsBreageMoving : MonoBehaviour
	{
		private Transform _myTransform;

		private void Awake()
		{
			_myTransform = base.transform;
		}

		private void Update()
		{
			if (_myTransform.localScale.x >= SkyWarsSetupOptions.BridgeMaxScale - 2f)
			{
				base.enabled = false;
			}
			_myTransform.localScale = Vector3.Lerp(_myTransform.localScale, new Vector3(SkyWarsSetupOptions.BridgeMaxScale, _myTransform.localScale.y, _myTransform.localScale.z), SkyWarsSetupOptions.BridgeMovingSpeed * Time.deltaTime);
		}
	}
}
