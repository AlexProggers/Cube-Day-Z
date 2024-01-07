using UnityEngine;

public class DropTestInfo
{
	public string Id;

	public int Amount;

	public string Model;

	public Vector3 ThrowVector;

	public DropTestInfo(string id, int amount, string model, Vector3 throwVector)
	{
		Id = id;
		Amount = amount;
		Model = model;
		ThrowVector = throwVector;
	}
}

public class DropTestController : MonoBehaviour
{
	private DropTestInfo _info;

	private bool _isAvailable = true;

	private bool _isInitialized;

	public void Initialize(DropTestInfo info)
	{
		_info = info;
		GetComponent<Rigidbody>().useGravity = true;
		GetComponent<Rigidbody>().AddForce(info.ThrowVector);
		_isInitialized = true;
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (_isInitialized && _isAvailable)
		{
			_isAvailable = false;
			Quaternion rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, collision.contacts[0].normal), Vector3.Cross(Vector3.up, collision.contacts[0].normal));
			int num = ((DataKeeper.Info.GetWeaponInfo(_info.Id) != null) ? UnityEngine.Random.Range(0, 10) : 0);
			PhotonNetwork.InstantiateSceneObject(_info.Model, collision.contacts[0].point, rotation, 0, new object[4]
			{
				_info.Id,
				(byte)_info.Amount,
				true,
				(byte)num
			});
			Object.Destroy(base.gameObject);
		}
	}
}
