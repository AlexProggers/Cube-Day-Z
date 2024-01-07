using UnityEngine;

public class PlaceItemViewController : MonoBehaviour
{
	private const float RotateSpeed = 25f;

	private const string IgnoreRaycastLayer = "Ignore Raycast";

	[SerializeField]
	private Transform _placeItemHolder;

	private DestructibleObject _info;

	private GameObject _placeItem;

	private RaycastHit _hit;

	private bool _onPlayerLookOriented;

	private float _customYRotation;

	private int rotate_snap_counter;

	public Transform PlaceTransform
	{
		get
		{
			return (!(_placeItem != null)) ? null : _placeItem.transform;
		}
	}

	public void SetItem(string id, bool playerLookOriented)
	{
		_info = DataKeeper.Info.GetDestructibleObjectInfo(id);
		_onPlayerLookOriented = playerLookOriented;

		if (_info != null)
		{
			_placeItem = (GameObject)Object.Instantiate(Resources.Load("WorldObjects/" + _info.Prefab));
			Collider[] componentsInChildren = _placeItem.GetComponentsInChildren<Collider>();
			Collider[] array = componentsInChildren;
			foreach (Collider obj in array)
			{
				Object.Destroy(obj);
			}
			UnityEngine.AI.NavMeshObstacle[] componentsInChildren2 = _placeItem.GetComponentsInChildren<UnityEngine.AI.NavMeshObstacle>();
			UnityEngine.AI.NavMeshObstacle[] array2 = componentsInChildren2;
			foreach (UnityEngine.AI.NavMeshObstacle obj2 in array2)
			{
				Object.Destroy(obj2);
			}
			ChangeLayer(_placeItem);
		}
	}

	private void ChangeLayer(GameObject obj)
	{
		obj.layer = LayerMask.NameToLayer("Ignore Raycast");
		foreach (Transform item in obj.transform)
		{
			ChangeLayer(item.gameObject);
		}
	}

	public void Reset()
	{
		if (_placeItem != null)
		{
			Object.Destroy(_placeItem);
		}
		_customYRotation = 0f;
		_info = null;
	}

	private void Update()
	{
		if (_placeItem == null)
		{
			return;
		}
		Vector3 position = GameControls.I.Player.mainCamTransform.position;
		Vector3 forward = GameControls.I.Player.mainCamTransform.forward;
		if (!Physics.Raycast(position, forward, out _hit, 10f, DataKeeper.I.ItemsCollisionsMask))
		{
			return;
		}
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
		{
			if (Input.GetMouseButtonDown(1))
			{
				rotate_snap_counter++;
				_customYRotation = (float)rotate_snap_counter * 90f;
			}
		}
		else if (Input.GetMouseButton(1))
		{
			_customYRotation += 25f * Time.deltaTime;
		}
		_placeItem.transform.position = _hit.point;
		Vector3 axis = ((!_onPlayerLookOriented) ? Vector3.up : Vector3.Cross(Vector3.up, _hit.normal));
		Quaternion quaternion = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, _hit.normal), axis);
		_placeItem.transform.rotation = Quaternion.Euler(quaternion.eulerAngles.x, _customYRotation, quaternion.eulerAngles.z);
	}
}
