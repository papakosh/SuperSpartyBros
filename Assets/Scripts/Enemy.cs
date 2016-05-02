using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	[Range(0f,10f)]
	public float moveSpeed = 4f;  // enemy move speed when moving
	public int damageAmount = 10; // probably deal a lot of damage to kill player immediately
	public int enemyHealth = 1;


	[Tooltip("Child gameObject for detecting stun.")]
	public GameObject stunnedCheck; // what gameobject is the stunnedCheck

	public float stunnedTime = 3f;   // how long to wait at a waypoint
	
	public string stunnedLayer = "StunnedEnemy";  // name of the layer to put enemy on when stunned
	public string playerLayer = "Player";  // name of the layer to put enemy on when stunned

	[HideInInspector]
	public bool isStunned = false;  // flag for isStunned
	[HideInInspector]
	public bool enemyCanAttack = true;


	public GameObject[] myWaypoints; // to define the movement waypoints
	[Tooltip("How much time in seconds to wait at each waypoint")]
	public float waitAtWaypointTime = 1f;   // how long to wait at a waypoint
	
	public bool loopWaypoints = true; // should it loop through the waypoints
	
	// SFXs
	public AudioClip stunnedSFX;
	public AudioClip attackSFX;
	public AudioClip deathSFX;
	public AudioClip hitSFX;
		
	// private variables below
	
	// store references to components on the gameObject
	Transform _transform;
	Rigidbody2D _rigidbody;
	Animator _animator;
	AudioSource _audio;
	
	// movement tracking
	[SerializeField]
	int _myWaypointIndex = 0; // used as index for My_Waypoints
	float _moveTime; 
	float _vx = 0f;
	bool _moving = true;
	
	// store the layer number the enemy is on (setup in Awake)
	int _enemyLayer;

	// store the layer number the enemy should be moved to when stunned
	int _stunnedLayer;
	
	void Awake() {
		// get a reference to the components we are going to be changing and store a reference for efficiency purposes
		_transform = GetComponent<Transform> ();
		
		_rigidbody = GetComponent<Rigidbody2D> ();
		if (_rigidbody==null) // if Rigidbody is missing
			Debug.LogError("Rigidbody2D component missing from this gameobject");
		
		_animator = GetComponent<Animator>();
		if (_animator==null) // if Animator is missing
			Debug.LogError("Animator component missing from this gameobject");
		
		_audio = GetComponent<AudioSource> ();
		if (_audio==null) { // if AudioSource is missing
			Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
			// let's just add the AudioSource component dynamically
			_audio = gameObject.AddComponent<AudioSource>();
		}

		if (stunnedCheck==null) {
			Debug.LogError("stunnedCheck child gameobject needs to be setup on the enemy");
		}
		
		// setup moving defaults
		_moveTime = 0f;
		_moving = true;
		
		// determine the enemies specified layer
		_enemyLayer = this.gameObject.layer;

		// determine the stunned enemy layer number
		_stunnedLayer = LayerMask.NameToLayer(stunnedLayer);

		// make sure collision are off between the playerLayer and the stunnedLayer
		// which is where the enemy is placed while stunned
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), _stunnedLayer, true); 
	}
	
	// if not stunned then move the enemy when time is > _moveTime
	void Update () {
		if (!isStunned)
		{
			if (Time.time >= _moveTime) {
				if (gameObject.tag == "Boss") {
					if (GameManager.gm.IsPlayerAtBoss () && GameManager.gm.IsInitialBossEngagement ()) {
						StartCoroutine (BossUIDisplay  ());

					} else if (GameManager.gm.CanBossEngagePlayer()) {
						BossMovement ();
					}
				}else
					EnemyMovement();
			} else {
				_animator.SetBool("Moving", false);
				_animator.SetBool("MoveAndAttack", false);
			}
		}
	}
	
	// Move the enemy through its rigidbody based on its waypoints
	void EnemyMovement() {
		// if there isn't anything in My_Waypoints
		if ((myWaypoints.Length != 0) && (_moving)) {
			
			// make sure the enemy is facing the waypoint (based on previous movement)
			Flip (_vx);
			
			// determine distance between waypoint and enemy
			_vx = myWaypoints[_myWaypointIndex].transform.position.x-_transform.position.x;
			
			// if the enemy is close enough to waypoint, make it's new target the next waypoint
			if (Mathf.Abs(_vx) <= 0.05f) {
				// At waypoint so stop moving
				_rigidbody.velocity = new Vector2(0, 0);
				
				// increment to next index in array
				_myWaypointIndex++;
				
				// reset waypoint back to 0 for looping
				if(_myWaypointIndex >= myWaypoints.Length) {
					if (loopWaypoints)
						_myWaypointIndex = 0;
					else
						_moving = false;
				}
				
				// setup wait time at current waypoint
				_moveTime = Time.time + waitAtWaypointTime;
			} else {
				// enemy is moving
				_animator.SetBool("Moving", true);
				
				// Set the enemy's velocity to moveSpeed in the x direction.
				_rigidbody.velocity = new Vector2(_transform.localScale.x * moveSpeed, _rigidbody.velocity.y);
			}
			
		}
	}

	// Have the boss chase the player
	void BossMovement() {
		// if there isn't anything in My_Waypoints
		if (_moving) {

			// make sure the enemy is facing the waypoint (based on previous movement)
			Flip (_vx);

			// determine distance between waypoint and enemy
			_vx = GameObject.FindGameObjectWithTag("Player").transform.position.x-_transform.position.x;

			// if the enemy is close enough to waypoint, make it's new target the next waypoint
			if (Mathf.Abs(_vx) <= 0.05f) {
				// At waypoint so stop moving
				_rigidbody.velocity = new Vector2(0, 0);

				// setup wait time at current waypoint
				_moveTime = Time.time + waitAtWaypointTime;
			} else {
				// enemy is moving
				_animator.SetBool("MoveAndAttack", true);

				// Set the enemy's velocity to moveSpeed in the x direction.
				_rigidbody.velocity = new Vector2(_transform.localScale.x * moveSpeed, _rigidbody.velocity.y);
			}

		}
	}


	// flip the enemy to face torward the direction he is moving in
	void Flip(float _vx) {
		
		// get the current scale
		Vector3 localScale = _transform.localScale;
		
		if ((_vx>0f)&&(localScale.x<0f))
			localScale.x*=-1;
		else if ((_vx<0f)&&(localScale.x>0f))
			localScale.x*=-1;
		
		// update the scale
		_transform.localScale = localScale;
	}
	
	// Attack player
	void OnTriggerEnter2D(Collider2D collision)
	{
		if (!enemyCanAttack) {
			if (collision.tag == "Dagger")
				Destroy (collision.gameObject);
			return;
		}

		if ((collision.tag == "Player") && !isStunned) {
			CharacterController2D player = collision.gameObject.GetComponent<CharacterController2D> ();
			if (player.playerCanMove) {
				// Make sure the enemy is facing the player on attack
				Flip (collision.transform.position.x - _transform.position.x);
				
				// attack sound
				PlaySound (attackSFX);
				
				// stop moving
				_rigidbody.velocity = new Vector2 (0, 0);
				
				// apply damage to the player
				player.ApplyDamage (damageAmount);
				
				// stop to enjoy killing the player
				_moveTime = Time.time + stunnedTime;
			}
		} else if (collision.tag == "Dagger") {
			if (gameObject.tag == "Boss") {
				if (GameManager.gm.CanBossEngagePlayer ()) {
					ApplyDamage (1);
					Destroy (collision.gameObject);
				} else {
					Destroy (collision.gameObject);
				}
			} else {
				ApplyDamage (1);
				Destroy (collision.gameObject);
			}
		}

	}
	
	// if the Enemy collides with a MovingPlatform, then make it a child of that platform
	// so it will go for a ride on the MovingPlatform
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = other.transform;
		}
	}
	
	// if the enemy exits a collision with a moving platform, then unchild it
	void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = null;
		}
	}
	
	// play sound through the audiosource on the gameobject
	void PlaySound(AudioClip clip)
	{
		_audio.PlayOneShot(clip);
	}
	
	// setup the enemy to be stunned
	public void Stunned()
	{
		if (!isStunned) 
		{
			isStunned = true;
			
			// provide the player with feedback that enemy is stunned
			PlaySound(stunnedSFX);
			_animator.SetTrigger("Stunned");
			
			// stop moving
			_rigidbody.velocity = new Vector2(0, 0);
			
			// switch layer to stunned layer so no collisions with the player while stunned
			this.gameObject.layer = _stunnedLayer;
			stunnedCheck.layer = _stunnedLayer;

			// start coroutine to stand up eventually
			StartCoroutine (Stand ());
		}
	}
	
	// coroutine to unstun the enemy and stand back up
	IEnumerator Stand()
	{
		yield return new WaitForSeconds(stunnedTime); 
		
		// no longer stunned
		isStunned = false;
		
		// switch layer back to regular layer for regular collisions with the player
		this.gameObject.layer = _enemyLayer;
		stunnedCheck.layer = _enemyLayer;
		
		// provide the player with feedback
		_animator.SetTrigger("Stand");

		// adjust boss speed based on health remaining 
		// *NOTE* added this here as well as apply damage because of
		// the possibility that when transitioning from damaged to stunned the speed change may not have been applied
		if (gameObject.tag == "Boss") {
			AdjustSpeedBasedOnRemainingHealth ();
		}
	}

	public void ApplyDamage (int damage){
		if (!isStunned) {
			enemyHealth -= damage;

			if (enemyHealth <= 0) { // enemy is now dead, so start dying
				PlaySound (deathSFX);
				StartCoroutine (KillEnemy ());
			} else {
				PlaySound (hitSFX);
				StartCoroutine (DamageEnemy ());
			}
		}
	}

	// coroutine to kill the player
	IEnumerator KillEnemy()
	{
		if (!isStunned)
		{
			// freeze the enemy
			FreezeMotion();

			// play the enemy death animation
			_animator.SetTrigger("Death");

			yield return new WaitForSeconds(1.0f);

			if (gameObject.tag == "Boss") // if boss killed, then game victory
				GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>().Victory();

			Destroy (gameObject);
		}
	}

	// coroutine to damage the enemy
	IEnumerator DamageEnemy()
	{
		if (!isStunned)
		{
			// play the hit animation
			_animator.SetTrigger("Hit");

			// After waiting tell the GameManager to reset the game
			yield return new WaitForSeconds(0.5f);

			// provide the player with feedback
			_animator.SetTrigger("Stand");

			//adjust boss speed based on health remaining
			if (gameObject.tag == "Boss") {
				AdjustSpeedBasedOnRemainingHealth ();
			}
		}
	}

	void FreezeMotion() {
		enemyCanAttack = false;
		_moving = false;
		_rigidbody.isKinematic = true;
	}

	void AdjustSpeedBasedOnRemainingHealth (){
		switch (enemyHealth) {
		case 38:
			moveSpeed = 0.2f;
			break;
		case 25:
			moveSpeed = 0.3f;
			break;
		case 13:
			moveSpeed = 0.4f;
			break;
		case 5:
			moveSpeed = 0.5f;
			break;
		default: 
			break;
		}
	}

	IEnumerator BossUIDisplay (){
		Flip (-1);
		GameManager.gm.ShowBossFightUI ();
		yield return new WaitForSeconds (3.0f);
		GameManager.gm.HideBossFightUI ();
	}
}
