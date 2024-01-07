//DragRigidbody.cs by Azuline Studios© All Rights Reserved
//Drags rigidbody objects in fron of player, throws held obejcts, and drops objects.
//Also stores object's original drag and angularDrag values and restores them on drop.
using UnityEngine;
using System.Collections;

public class DragRigidbody : MonoBehaviour {
	private FPSRigidBodyWalker FPSWalkerComponent;
	private Ironsights IronsightsComponent;
	private FPSPlayer FPSPlayerComponent;
	private float spring = 75.0f;
	private float damper = 1.0f;
	private float drag = 10.0f;
	private float angularDrag = 5.0f;
	private float distance = 0.0f;
	public float reachDistance = 2.5f;//max distance to drag objects (scaled by playerHeightMod amount of FPSRigidBodyWalker.cs)
	private float reachDistanceAmt;//max distance to drag objects
	public float throwForce = 7.0f;//physics force to apply to thrown objects
	private bool attachToCenterOfMass = false;
	private SpringJoint springJoint;
    private float oldDrag;
    private float oldAngularDrag;
    private bool dragState;
	
	public LayerMask layersToDrag = 0;//only check these layers for draggable objects
	private Transform mainCamTransform;
	
	void Start(){
		FPSWalkerComponent = GetComponent<FPSRigidBodyWalker>();
		FPSPlayerComponent = GetComponent<FPSPlayer>();
		mainCamTransform = Camera.main.transform;
		//proportionately scale reachDistance by playerHeightMod amount
		reachDistanceAmt = reachDistance / (1 - (FPSWalkerComponent.playerHeightMod / FPSWalkerComponent.capsule.height));
	}
       
    void Update(){
	
		// Make sure the user pressed the mouse down
		if(!Input.GetKeyDown(FPSPlayerComponent.moveObject) || FPSPlayerComponent.zoomed || FPSPlayerComponent.hitPoints < 1.0f){
			return;
		}
			
		// We need to actually hit an object
        RaycastHit hit;
        if(!Physics.Raycast(mainCamTransform.position, ((mainCamTransform.position + mainCamTransform.forward * reachDistanceAmt) - mainCamTransform.position).normalized, out hit, reachDistanceAmt, layersToDrag)){
            return;
        }
		// We need to hit a rigidbody that is not kinematic
		if(!hit.rigidbody || hit.rigidbody.isKinematic){
			return;
		}
		
		if(!springJoint){
			GameObject go = new GameObject("Rigidbody dragger");
			Rigidbody body = go.AddComponent <Rigidbody>() as Rigidbody;
			springJoint = go.AddComponent <SpringJoint>() as SpringJoint;
			body.isKinematic = true;
		}
		
		springJoint.connectedBody = hit.rigidbody;
		springJoint.transform.position = hit.point;
		
		if(attachToCenterOfMass){
			Vector3 anchor = transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
			anchor = springJoint.transform.InverseTransformPoint(anchor);
			springJoint.anchor = anchor;
		}else{
			springJoint.anchor = Vector3.zero;
		}
		
		springJoint.spring = spring;
		springJoint.damper = damper;
		springJoint.maxDistance = distance;
		
		StartCoroutine ("DragObject", hit.distance);
	}
	
    IEnumerator DragObject ( float distance  ){
		
		FPSPlayer FPSPlayerComponent = GetComponent<FPSPlayer>();
		
        if(!dragState){
            oldDrag = springJoint.connectedBody.drag;
            oldAngularDrag = springJoint.connectedBody.angularDrag;
            dragState = true;
        }
		
		//allow floating objects like apples to fall once "picked" by dragging
		if(!springJoint.connectedBody.useGravity){
			if(springJoint.connectedBody.GetComponent<AudioSource>()){//play "picking" sound effect
				springJoint.connectedBody.GetComponent<AudioSource>().pitch = Random.Range(0.75f * Time.timeScale, 1.0f * Time.timeScale);//add slight random value to sound pitch for variety
				springJoint.connectedBody.GetComponent<AudioSource>().Play();
			}
			springJoint.connectedBody.useGravity = true;
		}
		
        springJoint.connectedBody.drag = drag;
        springJoint.connectedBody.angularDrag = angularDrag;
		
		while(Input.GetKey(FPSPlayerComponent.moveObject) 
		&& springJoint.connectedBody 
		&& !FPSPlayerComponent.zoomed
		&& FPSPlayerComponent.hitPoints > 0.0f){
            Ray ray = new Ray (mainCamTransform.position, ((mainCamTransform.position + mainCamTransform.forward * reachDistanceAmt) - mainCamTransform.position).normalized);
            springJoint.transform.position = ray.GetPoint(distance);
            if(!Input.GetKey(FPSPlayerComponent.throwObject)){
				//let go of object if we are out of grabbing range
				if( Vector3.Distance(springJoint.connectedBody.transform.position, mainCamTransform.position) < reachDistanceAmt * 1.4f ){
					FPSWalkerComponent.holdingObject = true;
					yield return 0;
				}else{
					break;
				}
			}else{//throw object
				float throwForceAmt;
				if(springJoint.connectedBody.mass < 1){
					throwForceAmt = throwForce/2;
				}else{
					throwForceAmt = throwForce * springJoint.connectedBody.mass;
				}
				springJoint.connectedBody.AddForceAtPosition((throwForceAmt * mainCamTransform.forward), springJoint.transform.position,ForceMode.Impulse);
				break;
			}
		}
		if (springJoint.connectedBody){//stop dragging object
			DropObject();
		}
	}
	//if dragged object contacts player object, stop dragging to prevent pushing or lifting player
	void OnCollisionStay(Collision col){
		if(springJoint){
			if(springJoint.connectedBody){//stop dragging object
				if(col.gameObject.GetComponent<Rigidbody>() == springJoint.connectedBody){
					DropObject();
				}		
			}
		}
    }
	
	void DropObject(){
		FPSWalkerComponent.holdingObject = false;
		springJoint.connectedBody.drag = oldDrag;
        springJoint.connectedBody.angularDrag = oldAngularDrag;
        springJoint.connectedBody = null;
        dragState = false;
	}

}