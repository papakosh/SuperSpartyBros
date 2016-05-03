using UnityEngine;
using System.Collections;

public class EnemyStun : MonoBehaviour {

	// Handle collisions with the stun point on an enemy
	void OnCollisionEnter2D(Collision2D other)
	{
		// if Player hits the stun point of the enemy, then call Stunned on the enemy
		if (other.gameObject.tag == "Player") {
			// tell the enemy to be stunned
			this.GetComponentInParent<Enemy> ().Stunned ();

			// make the player the bounce off the enemy (aka jump)
			other.gameObject.GetComponent<CharacterController2D> ().EnemyBounce ();

		} else if (other.gameObject.tag == "Dagger") { // or if stun point on enemy is hit by a dagger, damage enemy
			// apply damage to enemy
			this.GetComponentInParent<Enemy> ().ApplyDamage (1);

			// destroy the dagger
			Destroy(other.gameObject);
		}
	}
}