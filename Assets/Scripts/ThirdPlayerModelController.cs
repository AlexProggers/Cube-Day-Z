using System.Collections;
using UnityEngine;

public class ThirdPlayerModelController : MonoBehaviour
{
    [SerializeField]
	private Animator _animator;

	private Transform _leftHand;

	private Transform _rightHand;

	private bool _isIkActive;

	[SerializeField]
	private AudioSource _audioSourceForStep;

	private bool block;

	public void EnableIk(Transform leftHandPoint, Transform rightHandPoint)
	{
		_leftHand = leftHandPoint;
		_rightHand = rightHandPoint;
		_isIkActive = true;
	}

	public void DisableIk()
	{
		_leftHand = null;
		_rightHand = null;
		_isIkActive = false;
	}

	void OnPlayerStep(int tmp)
	{
		if (!block)
		{
			StartCoroutine("PlaySound");
		}
	}

	IEnumerator PlaySound()
	{
		block = true;
		_audioSourceForStep.pitch = UnityEngine.Random.Range(0.96f * Time.timeScale, 1f * Time.timeScale);
		_audioSourceForStep.PlayOneShot(_audioSourceForStep.clip, 0.9f / _audioSourceForStep.volume);
		yield return new WaitForSeconds(0.4f);
		block = false;
	}

	void OnAnimatorIK(int layerIndex)
	{
		if (_isIkActive)
		{
			_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
			_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
			_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
			_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
			_animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHand.position);
			_animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHand.rotation);
			_animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHand.position);
			_animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHand.rotation);
		}
		else
		{
			_animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
			_animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
			_animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
			_animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
		}
	}
}
