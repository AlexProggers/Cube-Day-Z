//AutoWaypoint.cs by Azuline Studios© All Rights Reserved
//Creates waypoint groups based on line of sight, draws editor waypoint lines,
//and finds the waypoint closest to a position.
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoWayPoint : MonoBehaviour {
	[HideInInspector]
	public List<AutoWayPoint> connected = new List<AutoWayPoint>();
	static public AutoWayPoint[] waypointsObj;//list containing all waypoints in the scene
	static float kLineOfSightCapsuleRadius = 0.25f;
	public int waypointGroup = 0;//waypoint group number of this waypoint
	public int waypointNumber = 0;//number of this waypoint in path sequence
	//LayerMask for only letting world collision objects block waypoint line of sight 
	private LayerMask capsuleMask = 10;
	
	public AutoWayPoint FindClosest ( Vector3 pos , int waypointsToFollow  ){
		// The closer two vectors, the larger the dot product will be.
		AutoWayPoint closest = null;
		float closestDistance = 75.0f;
		foreach(AutoWayPoint cur in connected){
			float distance = Vector3.Distance(cur.transform.position, pos);
			if (distance < closestDistance){
				//track this waypoint only if it is in our waypoint group 
				if(waypointsToFollow == cur.GetComponent<AutoWayPoint>().waypointGroup){
					closestDistance = distance;
					closest = cur;
				}
			}
		}
		return closest;
	}
	
	[ContextMenu ("Update Waypoints")]
	void UpdateWaypoints (){
		RebuildWaypointList();
	}
	
	void Awake (){
		RebuildWaypointList();
	}

	//draw the waypoint lines only when one of the waypoints is selected
	void OnDrawGizmosSelected (){
		
		RebuildWaypointList();
		
		foreach(AutoWayPoint p in connected) {
			if (Physics.Linecast(transform.position, p.transform.position)) {
				Gizmos.color = Color.red;
				Gizmos.DrawLine (transform.position, p.transform.position);
			} else {
				Gizmos.color = Color.green;
				Gizmos.DrawLine (transform.position, p.transform.position);
			}
		}
	}
	
	void RebuildWaypointList (){
		waypointsObj = FindObjectsOfType(typeof(AutoWayPoint)) as AutoWayPoint[];
		
		foreach(AutoWayPoint point in waypointsObj) {
			point.RecalculateConnectedWaypoints();
		}
	}
	
	void RecalculateConnectedWaypoints (){
		connected.Clear();
		foreach(AutoWayPoint other in waypointsObj) {
			// Do we have a clear line of sight?
			if (!Physics.CheckCapsule(transform.position, other.transform.position, kLineOfSightCapsuleRadius, capsuleMask)) {
				//only add this waypoint to the connected waypoint array if it is in our waypoint group
				if(other.GetComponent<AutoWayPoint>().waypointGroup == waypointGroup){
					connected.Add(other);
				}
			}
		}
	}
}