using UnityEngine;
using System.Collections;

public class BossStagePlatformMover : MonoBehaviour {

	public GameObject platform; // reference to the platform to move
	public GameObject[] myWaypoints; // array of all the waypoints

	[Range(0.0f, 10.0f)] // create a slider in the editor and set limits on moveSpeed
	public float moveSpeed = 5f; // platform move speed
	public float waitAtWaypointTime = 1f; // how long to wait at a waypoint before _moving to next waypoint

	// private variables
	Transform _transform;
	int _myWaypointIndex = 0; // used as index for My_Waypoints
	float _moveTime;
	bool _moving = true;

	// Use this for initialization
	void Start () {
		_transform = platform.transform;
		_moveTime = 0f;
		_moving = false; // initial moving state false since platform is one way and won't start until stepped on
	}
	
	// game event loop
	void Update () {
		// if at starting waypoint, not moving yet, and player is onboard, then platform can move
		if (_myWaypointIndex == 0  && !_moving && _transform.childCount > 0) { 
			_moving = true;
		}else if (_myWaypointIndex==2 &&_transform.childCount == 0) {
		// or if at final waypoint and player is not onboard, then start boss scene and destroy platform
			GameManager.gm.PlayerAtBoss ();
			Destroy (gameObject);
		}else if (Time.time >= _moveTime && _moving) {
		// or if beyond _moveTime, then start moving the platform
			Movement();
		}
	}

	// move platform
	void Movement() {
		// if there is anything in My_Waypoints and can move then move platform
		if ((myWaypoints.Length != 0) && (_moving)) {

			// move towards waypoint
			_transform.position = Vector3.MoveTowards(_transform.position, myWaypoints[_myWaypointIndex].transform.position, moveSpeed * Time.deltaTime);

			// if the platform is close enough to waypoint, make it's new target the next waypoint
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

	// public function to reset the position of the platform
	public void resetPosition (float xVal, float yVal){
		_myWaypointIndex = 0;
		_moving = false;
		_transform.position = new Vector3(xVal, yVal, 0.00f); 
	}
}
