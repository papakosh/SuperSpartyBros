using UnityEngine;
using System.Collections;

public class DaggerPickup : MonoBehaviour {

	public bool taken = false;
	public GameObject explosion;

	// if the player touches the victory object, it has not already been taken, and the player can move (not dead or victory)
	// then the player has reached the victory point of the level
	void OnTriggerEnter2D (Collider2D other)
	{
		if ((other.tag == "Player" ) && (!taken) && (other.gameObject.GetComponent<CharacterController2D>().playerCanMove))
		{
			// mark as taken so doesn't get taken multiple times
			taken=true;

			// do the dagger thing
			other.gameObject.GetComponent<CharacterController2D>().DaggerTaken();
			GameManager.gm.Pickup ("Dagger Picked up. Press 'Fire' to shoot.", true);
			Destroy(gameObject);
		}
	}

}
