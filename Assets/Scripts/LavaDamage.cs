using UnityEngine;
using System.Collections;

public class LavaDamage : MonoBehaviour {

	public bool destroyNonPlayerObjects = true;

	// Handle gameobjects collider with a deathzone object
	void OnCollisionEnter2D (Collision2D other) {
		if (other.gameObject.tag == "Player") {
			// if player then tell the player to do its FallDeath
			other.gameObject.GetComponent<CharacterController2D> ().ApplyDamage (1);
		} else if (destroyNonPlayerObjects) { // not playe so just kill object - could be falling enemy for example
			DestroyObject(other.gameObject);
		}
	}
}
