using UnityEngine;
using System.Collections;

public class ThrowingDagger : MonoBehaviour {

	// if the player touches the coin, it has not already been taken, and the player can move (not dead or victory)
	// then take the coin
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Platform" || other.gameObject.tag == "MovingPlatform")
		{
			// destroy the dagger
			Destroy(this.gameObject);
		}
	}

}
