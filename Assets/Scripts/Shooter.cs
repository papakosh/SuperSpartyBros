using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Shooter : MonoBehaviour {

	// Reference to projectile prefab to shoot
	public GameObject projectile;
	public float power = 10.0f;
	public float rateOfFire = 0.5f;

	// Reference to AudioClip to play
	public AudioClip shootSFX;

	// Private variables
	float _lastTimeFired = 0f;

	// Update is called once per frame
	void Update () {
		//Detect if player can fire
		if (!GetComponent<CharacterController2D> ().canFire ())
			return;
		// Detect if fire button is pressed
		if (CrossPlatformInputManager.GetButtonDown("Fire1") && (Time.fixedTime - _lastTimeFired >= rateOfFire))
		{	
			// Set last time fired to current elapsed time (in secs)
			_lastTimeFired = Time.fixedTime;

			// if projectile is specified
			if (projectile)
			{
				// Instantiante projectile at the player + 1 meter forward with -90 rotation to have it point forwards
				GameObject newProjectile = Instantiate(projectile, transform.position + transform.forward, Quaternion.Euler(0, 0, -90)) as GameObject;

				// if the projectile does not have a rigidbody component, add one
				if (!newProjectile.GetComponent<Rigidbody2D>()) 
				{
					newProjectile.AddComponent<Rigidbody2D>();
				}
				// fire in the direction the player is facing - right or left
				if (GetComponent<CharacterController2D> ().facingRight())
					// Apply force to the newProjectile's Rigidbody component if it has one
					newProjectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 0) * power, ForceMode2D.Force);
				else
					// Apply force to the newProjectile's Rigidbody component if it has one
					newProjectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 0) * power, ForceMode2D.Force);
				
				// play sound effect if set
				if (shootSFX)
				{
					if (newProjectile.GetComponent<AudioSource> ()) { // the projectile has an AudioSource component
						// play the sound clip through the AudioSource component on the gameobject.
						// note: The audio will travel with the gameobject.
						newProjectile.GetComponent<AudioSource> ().PlayOneShot (shootSFX, 1.0f);
					} else {
						// dynamically create a new gameObject with an AudioSource
						// this automatically destroys itself once the audio is done
						AudioSource.PlayClipAtPoint (shootSFX, newProjectile.transform.position, 1.0f);
					}
				}
			}
		}
	}
}
