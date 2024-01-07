using UnityEngine;

public class PositionInHandInfo : MonoBehaviour
{
	public Transform LeftHandPoint;

	public Transform RightHandPoint;

	public Vector3 ItemPositionOffset;

	public Vector3 ItemRotationOffset;

	public Vector3 ItemScaleOffset = Vector3.one;

	public Transform MuzzleFlash;

	public Light MuzzleFlashLight;
}
