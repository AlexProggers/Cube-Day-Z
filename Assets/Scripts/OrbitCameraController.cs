using UnityEngine;

public class OrbitCameraController : MonoBehaviour
{
	public GameObject MainCamera;

	public GameObject OrbitCamera;

	public GameObject FpsPlayer;

	public GameObject ModelPers;

	public GameObject FpsWeapons;

	public SmoothMouseLook SmoothMouseLookFirstPerson;

	public bool canUse = true;

	public static OrbitCameraController I;

	public bool _OrbitEnable;

	private void Awake()
	{
		if (I == null)
		{
			I = this;
		}
	}

	private void Start()
	{
		GameObject gameObject = GameControls.I.Player.transform.Find("Player(Clone)").transform.GetChild(0).gameObject;
		if (gameObject != null)
		{
			ModelPers = gameObject;
			OrbitCamera.GetComponent<MouseOrbitWithZoom>().target = gameObject.transform;
		}
	}

	private void Update()
	{
		if (canUse)
		{
			if (Input.GetKeyUp(KeyCode.O))
			{
				SwitchCameras(_OrbitEnable);
			}
			if (!_OrbitEnable && Input.GetMouseButtonDown(1))
			{
				SmoothMouseLookFirstPerson.enabled = false;
			}
			if (!_OrbitEnable && Input.GetMouseButtonUp(1))
			{
				SmoothMouseLookFirstPerson.enabled = true;
			}
		}
	}

	public void SwitchCameras(bool isOrbit)
	{
		if (isOrbit)
		{
			MainCamera.SetActive(false);
			OrbitCamera.SetActive(true);
			EnablePlayer(true);
			FpsWeapons.SetActive(false);
			_OrbitEnable = false;
		}
		else
		{
			OrbitCamera.SetActive(false);
			FpsWeapons.SetActive(true);
			EnablePlayer(false);
			MainCamera.SetActive(true);
			_OrbitEnable = true;
			SmoothMouseLookFirstPerson.enabled = true;
			
		}
	}

	private void EnablePlayer(bool flag)
	{
		if (ModelPers != null)
		{
			ModelPers.SetActive(flag);
		}
	}
}
