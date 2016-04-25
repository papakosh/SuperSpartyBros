using UnityEngine;
using System.Collections;

public class BossStageZone : MonoBehaviour {

	public AudioClip waterfallSFX;
	AudioSource _audio;
	public AudioClip bossMusic;

	public void Awake () {
		_audio = GetComponent<AudioSource> ();
		if (_audio==null) { // if AudioSource is missing
			Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
			// let's just add the AudioSource component dynamically
			_audio = gameObject.AddComponent<AudioSource>();
		}

	}

	// Handle gameobjects collider with a deathzone object
	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Player")
		{
			PlaySound (waterfallSFX);
			// if player then tell the player to do its FallDeath
			//other.gameObject.GetComponent<CharacterController2D>().FallDeath ();
		} 
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Player")
		{
			_audio.Stop ();
			// if player then tell the player to do its FallDeath
			//other.gameObject.GetComponent<CharacterController2D>().FallDeath ();
		} 
	}

	void PlaySound(AudioClip clip)
	{
		_audio.PlayOneShot(clip,0.05f);
		Camera.main.GetComponent<AudioSource> ().clip = bossMusic;
		Camera.main.GetComponent<AudioSource> ().Play ();

		Camera.main.GetComponent<AudioSource> ().volume = 1.0f;
		Camera.main.GetComponent<AudioSource> ().loop = true;
	}
}
