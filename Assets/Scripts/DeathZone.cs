using UnityEngine;
using System.Collections;

public class DeathZone : MonoBehaviour {

	public bool destroyNonPlayerObjects = true;

	// Handle gameobjects colliding with a deathzone object
	void OnCollisionEnter2D (Collision2D other) {
		// if player then tell the player to do its FallDeath
		if (other.gameObject.tag == "Player")
		{
			other.gameObject.GetComponent<CharacterController2D>().FallDeath ();
		} else if (destroyNonPlayerObjects) { // or when not the player, just kill the object - could be falling enemy for example
			Destroy(other.gameObject);
		}
	}
}
