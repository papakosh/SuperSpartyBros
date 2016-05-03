using UnityEngine;
using System.Collections;

public class ThrowingDagger : MonoBehaviour {

	// if a thrown dagger touches a platform or moving platform, just destroy it
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Platform" || other.gameObject.tag == "MovingPlatform")
		{
			// destroy the dagger
			Destroy(this.gameObject);
		}
	}

}
