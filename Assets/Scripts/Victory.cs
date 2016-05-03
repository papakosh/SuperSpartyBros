using UnityEngine;
using System.Collections;

public class Victory : MonoBehaviour {

	public bool taken = false;
	public GameObject victoryExplosion;

	// if the player touches the victory object, it has not already been taken, and the player can move (not dead or victory)
	// then the player has reached the victory point of the level
	void OnTriggerEnter2D (Collider2D other)
	{
		if ((other.tag == "Player" ) && (!taken) && (other.gameObject.GetComponent<CharacterController2D>().playerCanMove))
		{
			// mark as taken so doesn't get taken multiple times
			taken=true;

			// if explosion prefab is provide, then instantiate it
			if (victoryExplosion)
			{
				Instantiate(victoryExplosion,transform.position,transform.rotation);
			}

			// do the player victory thing
			other.gameObject.GetComponent<CharacterController2D>().Victory();

			// destroy the victory gameobject
			Destroy(this.gameObject);
		}
	}

}
