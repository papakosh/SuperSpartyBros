﻿using UnityEngine;
using System.Collections;

public class BossStagePlatformMover : MonoBehaviour {

	public GameObject platform; // reference to the platform to move

	public GameObject[] myWaypoints; // array of all the waypoints

	[Range(0.0f, 10.0f)] // create a slider in the editor and set limits on moveSpeed
	public float moveSpeed = 5f; // enemy move speed
	public float waitAtWaypointTime = 1f; // how long to wait at a waypoint before _moving to next waypoint

	// private variables
	Transform _transform;
	int _myWaypointIndex = 0;		// used as index for My_Waypoints
	float _moveTime;
	bool _moving = true;

	// Use this for initialization
	void Start () {
		_transform = platform.transform;
		_moveTime = 0f;
		//if (!loop)
			_moving = false;
		//else
		//	_moving = true;
	}
	
	// game loop
	void Update () {
		// 
		if (_myWaypointIndex == 0  && !_moving && _transform.childCount > 0) { 
			// if at start waypoint, not moving yet, and player is onboard, then start moving and set boss condition
			_moving = true;
		}else if (_myWaypointIndex==2 &&_transform.childCount == 0) {
			// if final waypoint and player is not onboard, then start boss scene and destroy platform
			GameManager.gm.PlayerAtBoss ();
			Destroy (gameObject);
		}else if (Time.time >= _moveTime && _moving) {
			// if beyond _moveTime, then start moving
			Movement();
		}
	}

	void Movement() {
		// if there isn't anything in My_Waypoints
		if ((myWaypoints.Length != 0) && (_moving)) {

			// move towards waypoint
			_transform.position = Vector3.MoveTowards(_transform.position, myWaypoints[_myWaypointIndex].transform.position, moveSpeed * Time.deltaTime);

			// if the enemy is close enough to waypoint, make it's new target the next waypoint
			if(Vector3.Distance(myWaypoints[_myWaypointIndex].transform.position, _transform.position) <= 0) {
				_myWaypointIndex++;
				_moveTime = Time.time + waitAtWaypointTime;
			}
			
			// if at final waypoint, stop moving
			if(_myWaypointIndex >= myWaypoints.Length) {
				_moving = false;
			}
		}
	}

	public void resetMovement (float xVal, float yVal){
		_myWaypointIndex = 0;
		_moving = false;
		_transform.position = new Vector3(xVal, yVal, 0.00f); 
	}
}
