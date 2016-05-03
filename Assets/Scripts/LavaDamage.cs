using UnityEngine;
using System.Collections;

public class LavaDamage : MonoBehaviour {

	public bool destroyNonPlayerObjects = true;

	// Handle gameobjects collider with a lavazone object
	void OnCollisionEnter2D (Collision2D other) {
		if (other.gameObject.tag == "Player") { // if player collides then have the player to take damage
			other.gameObject.GetComponent<CharacterController2D> ().ApplyDamage (1);
		} else if (destroyNonPlayerObjects) { // or if not the player just kill object
			Destroy(other.gameObject);
		}
	}
}
