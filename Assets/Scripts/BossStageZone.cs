using UnityEngine;
using System.Collections;

public class BossStageZone : MonoBehaviour {

	public AudioClip waterfallSFX; // waterfall sound to play in zone
	public AudioClip bossMusic; // boss stage music to play in zone

	// store references to components on the gameObject
	AudioSource _audio;

	public void Awake () {
		_audio = GetComponent<AudioSource> ();
		if (_audio==null) { // if AudioSource is missing
			Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");

			// let's just add the AudioSource component dynamically
			_audio = gameObject.AddComponent<AudioSource>();
		}
	}

	// On entering trigger, play sounds
	void OnTriggerEnter2D (Collider2D other) {
		if (other.gameObject.tag == "Player")
		{
			PlaySound (waterfallSFX);
			ChangeMainCameraMusic (bossMusic);
		} 
	}

	// On exiting trigger, stop sounds
	void OnTriggerExit2D (Collider2D other) {
		if (other.gameObject.tag == "Player")
		{
			_audio.Stop ();
		} 
	}

	// play one shot sound
	void PlaySound(AudioClip clip)
	{
		_audio.PlayOneShot(clip,0.05f);
	}

	// change camera music
	void ChangeMainCameraMusic (AudioClip clip){
		Camera.main.GetComponent<AudioSource> ().clip = clip;
		Camera.main.GetComponent<AudioSource> ().volume = 1.0f;
		Camera.main.GetComponent<AudioSource> ().loop = true;
		Camera.main.GetComponent<AudioSource> ().Play ();
	}
}
