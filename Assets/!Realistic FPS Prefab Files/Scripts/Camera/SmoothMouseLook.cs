//SmoothMouseLook.cs by Azuline Studios© All Rights Reserved
//Smoothes mouse input, manages angle limits, enables/unlocks cursor on pause, and compensates for non-recovering weapon recoil. 
using UnityEngine;

using System.Collections;
//using System.Collections.Generic;

public class SmoothMouseLook : MonoBehaviour {

    public float sensitivity = 4.0f;
	[HideInInspector]
	public float sensitivityAmt = 4.0f;//actual sensitivity modified by IronSights Script

    private float minimumX = -360f;
    private float maximumX = 360f;

    private float minimumY = -85f;
    private float maximumY = 85f;
	[HideInInspector]
    public float rotationX = 0.0f;
	[HideInInspector]
    public float rotationY = 0.0f;
	[HideInInspector]
    public float inputY = 0.0f;
   
	public float smoothSpeed = 0.35f;
	
	private Quaternion originalRotation;
	private Transform myTransform;
	[HideInInspector]
	public float recoilX;//non recovering recoil amount managed by WeaponKick function of WeaponBehavior.cs
	[HideInInspector]
	public float recoilY;//non recovering recoil amount managed by WeaponKick function of WeaponBehavior.cs
	public bool lockCursor;

	private void OnEnable()
	{
		StartCoroutine("EnableOrbitCam");
	}

	private IEnumerator EnableOrbitCam()
	{
		yield return new WaitForSeconds(0.5f);
		if (OrbitCameraController.I != null)
		{
			OrbitCameraController.I.enabled = true;
		}
	}

	private void OnDisable()
	{
		if (OrbitCameraController.I != null)
		{
			OrbitCameraController.I.enabled = false;
		}
	}

	void Start(){         
        if (GetComponent<Rigidbody>()){GetComponent<Rigidbody>().freezeRotation = true;}
		
		myTransform = transform;//cache transform for efficiency
		
		originalRotation = myTransform.localRotation;
		//sync the initial rotation of the main camera to the y rotation set in editor
		Vector3 tempRotation = new Vector3(0,Camera.main.transform.eulerAngles.y,0);
		originalRotation.eulerAngles = tempRotation;
		
		sensitivityAmt = sensitivity;//initialize sensitivity amount from var set by player
		
		// Hide the cursor
		if(lockCursor)
			Cursor.visible = false;
    }

    void Update(){
		
		if(Time.timeScale > 0 && Time.smoothDeltaTime > 0){//allow pausing by setting timescale to 0
			//Hide the cursor
			if(lockCursor)
			{
				Screen.lockCursor = true;
				Cursor.visible = false;
			}

			float axisX = 0.0f;
			float axisY = 0.0f;

			// Read the mouse input axis
			if(!Application.isMobilePlatform)
			{
				axisX = Input.GetAxisRaw("Mouse X");
				axisY = Input.GetAxisRaw("Mouse Y");

				rotationX += axisX * sensitivityAmt * Time.timeScale;//lower sensitivity at slower time settings
				rotationY += axisY * sensitivityAmt * Time.timeScale;
			}else
			{
				axisX = MobileControl.I.LookX;
				axisY = MobileControl.I.LookY;

				rotationX += axisX * sensitivityAmt * Time.timeScale * 2.0f;//lower sensitivity at slower time settings
		        rotationY += axisY * sensitivityAmt * Time.timeScale * 2.0f;
			}
			
			//reset vertical recoilY value if it would exceed maximumY amount 
			if(maximumY - axisY * sensitivityAmt * Time.timeScale < recoilY){
				rotationY += recoilY;
				recoilY = 0.0f;	
			}
			//reset horizontal recoilX value if it would exceed maximumX amount 
			if(maximumX - axisX * sensitivityAmt * Time.timeScale < recoilX){
				rotationX += recoilX;
				recoilX = 0.0f;	
			}
			 
			rotationX = ClampAngle (rotationX, minimumX, maximumX);
			rotationY = ClampAngle (rotationY, minimumY - recoilY, maximumY - recoilY);
			
			inputY = rotationY + recoilY;//set public inputY value for use in other scripts
			 
			Quaternion xQuaternion = Quaternion.AngleAxis (rotationX + recoilX, Vector3.up);
			Quaternion yQuaternion = Quaternion.AngleAxis (rotationY + recoilY, -Vector3.right);
			
			//smooth the mouse input
			myTransform.rotation = Quaternion.Slerp(myTransform.rotation , originalRotation * xQuaternion * yQuaternion, smoothSpeed * Time.smoothDeltaTime * 60 / Time.timeScale);
			//lock mouselook roll to prevent gun rotating with fast mouse movements
			myTransform.rotation = Quaternion.Euler(myTransform.rotation.eulerAngles.x, myTransform.rotation.eulerAngles.y, 0.0f);
			
		}else{
			//Show the cursor
			Screen.lockCursor = false;
			Cursor.visible = true;	
		}
		
    }
   
	//function used to limit angles
    public static float ClampAngle (float angle, float min, float max){
        angle = angle % 360;
        if((angle >= -360F) && (angle <= 360F)){
            if(angle < -360F){
                angle += 360F;
            }
            if(angle > 360F){
                angle -= 360F;
            }         
        }
        return Mathf.Clamp (angle, min, max);
    }
	
}