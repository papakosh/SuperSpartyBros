using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Shooter : MonoBehaviour {

	public GameObject projectile; // Reference to projectile prefab to shoot
	public float power = 10.0f; // Amount of power to shoot projectile
	public float rateOfFire = 0.5f; // How often to shoot each time user presses 'shoot'

	public AudioClip shootSFX; // Reference to AudioClip to play

	// Private variables
	float _lastTimeFired = 0f;

	// Update is called once per frame
	void Update () {
		//Detect if player can fire
		if (!GetComponent<CharacterController2D> ().canFire ())
			return;
		
		// Detect if fire button is pressed and enough time has passed since last firing
		if (CrossPlatformInputManager.GetButtonDown("Fire1") && (Time.fixedTime - _lastTimeFired >= rateOfFire))
		{	
			// Set last time fired to current elapsed time (in secs)
			_lastTimeFired = Time.fixedTime;

			// if projectile is specified
			if (projectile)
			{
				int daggerRotation = 0;
				if (GetComponent<CharacterController2D> ().facingRight ()) // -90 rotation when player facing right
					daggerRotation = -90;
				else // otherwise 90 when player facing left
					daggerRotation = 90;

				// Instantiante projectile at the player + 1 meter forward with proper rotation to have it always pointing forwards
				GameObject newProjectile = Instantiate(projectile, 
											transform.position + transform.forward, Quaternion.Euler(0, 0, daggerRotation)) as GameObject;

				// if the projectile does not have a rigidbody component, add one
				if (!newProjectile.GetComponent<Rigidbody2D>()) 
				{
					newProjectile.AddComponent<Rigidbody2D>();
				}
				// fire in the direction the player is facing - right or left
				if (GetComponent<CharacterController2D> ().facingRight())
					newProjectile.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 0) * power, ForceMode2D.Force);
				else
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