using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchControlsKit;

public class MobileControl : MonoBehaviour
{
	[SerializeField] string joystickName  = "Joystick";
	[SerializeField] string touchpadName  = "Touchpad";
	[SerializeField] string fireBtnName   = "fireBtn";
	[SerializeField] string jumpBtnName   = "jumpBtn";
	[SerializeField] string reloadBtnName = "reloadBtn";
	[SerializeField] string crouchBtnName = "crouchBtn";
	[SerializeField] string zoomBtnName   = "zoomBtn";
	public static MobileControl I;
	public float MovementX
	{
		get
		{
			return TCKInput.GetAxis(joystickName, AxisType.X );
		}
	}

	public float MovementY
	{
		get
		{
			return TCKInput.GetAxis(joystickName, AxisType.Y );
		}
	}

	public float LookX
	{
		get
		{
			return TCKInput.GetAxis(touchpadName, AxisType.X );
		}
	}

	public float LookY
	{
		get
		{
			return TCKInput.GetAxis(touchpadName, AxisType.Y );
		}
	}

	public bool HasFire
	{
		get
		{
			return TCKInput.GetButton(fireBtnName);
		}
	}

	public bool HasFireDown
	{
		get
		{
			return TCKInput.GetButtonDown(fireBtnName);
		}
	}

	public bool HasJump
	{
		get
		{
			return TCKInput.GetButton(jumpBtnName);
		}
	}

	public bool HasReload
	{
		get
		{
			return TCKInput.GetButton(reloadBtnName);
		}
	}

	public bool HasCrouch
	{
		get
		{
			return TCKInput.GetButtonDown(crouchBtnName);
		}
	}

	public bool HasZoom
	{
		get
		{
			return TCKInput.GetButton(zoomBtnName);
		}
	}

    void Awake()
    {
		I = this;
    }
}
