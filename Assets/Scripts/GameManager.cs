using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace so can reference UI elements

public class GameManager : MonoBehaviour {

	// static reference to game manager so can be called from other scripts directly (not just through gameobject component)
	public static GameManager gm;

	// levels to move to on victory and lose
	public string levelAfterVictory;
	public string levelAfterGameOver;

	// game performance
	public int score = 0;
	public int highscore = 0;
	public int startLives = 3;
	public int lives = 3;

	// UI elements to control
	public Text UIScore;
	public Text UIHighScore;
	public Text UILevel;
	public GameObject UIPickup;
	public GameObject[] UIExtraLives;
	public GameObject UIGamePaused;
	public GameObject BossFightUI;

	//Game Background Music
	public AudioClip Music_Background;

	// private variables
	GameObject _player;
	Vector3 _spawnLocation;
	bool bossCondition = false;
	bool bossCanEngageCondition = false;
	float _deathPositionX = 0f;
	bool initialBossEngagement = true;

	// set things up here
	void Awake () {
		// setup reference to game manager
		if (gm == null)
			gm = this.GetComponent<GameManager>();

		// setup all the variables, the UI, and provide errors if things not setup properly.
		setupDefaults();
	}

	// game loop
	void Update() {
		// if ESC pressed then pause the game
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (Time.timeScale > 0f) {
				UIGamePaused.SetActive(true); // this brings up the pause UI
				Time.timeScale = 0f; // this pauses the game action
			} else {
				Time.timeScale = 1f; // this unpauses the game action (ie. back to normal)
				UIGamePaused.SetActive(false); // remove the pause UI
			}
		}
	}

	// setup all the variables, the UI, and provide errors if things not setup properly.
	void setupDefaults() {
		// setup reference to player
		if (_player == null)
			_player = GameObject.FindGameObjectWithTag("Player");
		
		if (_player==null)
			Debug.LogError("Player not found in Game Manager");
		
		// get initial _spawnLocation based on initial position of player
		_spawnLocation = _player.transform.position;

		// if levels not specified, default to current level
		if (levelAfterVictory=="") {
			Debug.LogWarning("levelAfterVictory not specified, defaulted to current level");
			//levelAfterVictory = Application.loadedLevelName;
			levelAfterVictory = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		}
		
		if (levelAfterGameOver=="") {
			Debug.LogWarning("levelAfterGameOver not specified, defaulted to current level");
			//levelAfterGameOver = Application.loadedLevelName;
			levelAfterGameOver = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		}

		// friendly error messages
		if (UIScore==null)
			Debug.LogError ("Need to set UIScore on Game Manager.");
		
		if (UIHighScore==null)
			Debug.LogError ("Need to set UIHighScore on Game Manager.");
		
		if (UILevel==null)
			Debug.LogError ("Need to set UILevel on Game Manager.");
		
		if (UIGamePaused==null)
			Debug.LogError ("Need to set UIGamePaused on Game Manager.");
		
		// get stored player prefs
		refreshPlayerState();

		// get the UI ready for the game
		refreshGUI();
	}

	// get stored Player Prefs if they exist, otherwise go with defaults set on gameObject
	void refreshPlayerState() {
		lives = PlayerPrefManager.GetLives();

		// special case if lives <= 0 then must be testing in editor, so reset the player prefs
		if (lives <= 0) {
			PlayerPrefManager.ResetPlayerState(startLives,false);
			lives = PlayerPrefManager.GetLives();
		}
		score = PlayerPrefManager.GetScore();
		highscore = PlayerPrefManager.GetHighscore();

		// save that this level has been accessed so the MainMenu can enable it
		PlayerPrefManager.UnlockLevel();
	}

	// refresh all the GUI elements
	void refreshGUI() {
		// set the text elements of the UI
		UIScore.text = "Score: "+score.ToString();
		UIHighScore.text = "Highscore: "+highscore.ToString ();
		//UILevel.text = Application.loadedLevelName;
		UILevel.text = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		// turn on the appropriate number of life indicators in the UI based on the number of lives left
		for(int i=0;i<UIExtraLives.Length;i++) {
			if (i<(lives-1)) { // show one less than the number of lives since you only typically show lifes after the current life in UI
				UIExtraLives[i].SetActive(true);
			} else {
				UIExtraLives[i].SetActive(false);
			}
		}
	}

	// public function to add points and update the gui and highscore player prefs accordingly
	public void AddPoints(int amount)
	{
		// increase score
		score+=amount;

		// update UI
		UIScore.text = "Score: "+score.ToString();

		// if score>highscore then update the highscore UI too
		if (score>highscore) {
			highscore = score;
			UIHighScore.text = "Highscore: "+score.ToString();
		}
	}

	// public function to remove player life and reset game accordingly
	public void ResetGame() {
		// remove life and update GUI
		lives--;
		refreshGUI();

		if (lives <= 0) { // no more lives
			// save the current player prefs before going to GameOver
			PlayerPrefManager.SavePlayerState (score, highscore, lives);

			// load the gameOver screen
			UnityEngine.SceneManagement.SceneManager.LoadScene(levelAfterGameOver);
		} else { // tell the player to respawn
			string current_level = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
			// level 1
			StageBoundary lvl1Stage1Bounds = new StageBoundary(0f,7.85f,0f);  
			//float _lvl1_stg1_bnd_end = 7.85f;
			//float _lvl1_stg1_res_x = 0.00f;
			StageBoundary lvl1Stage2Bounds = new StageBoundary(7.86f,21.00f,11.00f);  

			//float _lvl1_stg2_bnd_start = 7.86f;
			//float _lvl1_stg2_bnd_end = 21.00f;
			//float _lvl1_stg2_res_x = 11.00f;
			StageBoundary lvl1Stage3Bounds = new StageBoundary(21.05f,41.00f,24.75f);  
			//float _lvl1_stg3_bnd_start = 21.05f;
			//float _lvl1_stg3_bnd_end = 41.00f;
			//float _lvl1_stg3_res_x = 24.75f;
			StageBoundary lvl1Stage4Bounds = new StageBoundary(41.05f,0f,41.10f);  
			//float _lvl1_stg4_bnd_start = 41.05f;
			//float _lvl1_stg4_res_x = 41.10f;

			// level 2
			//float _lvl2_stg1_res_x = -8.00f;
			//float _lvl2_stg1_bnd_end = 6.85f;
			StageBoundary lvl2Stage1Bounds = new StageBoundary(0f,6.85f,-8.00f);  

			StageBoundary lvl2Stage2Bounds = new StageBoundary(6.90f,21.90f,7.60f);  
			//float _lvl2_stg2_bnd_start = 6.90f;
			//float _lvl2_stg2_bnd_end = 21.90f;
			//float _lvl2_stg2_res_x = 7.60f;
			StageBoundary lvl2Stage3Bounds = new StageBoundary(22.85f,37.89f,23.70f);  

			//float _lvl2_stg3_bnd_start = 22.85f;
			//float _lvl2_stg3_bnd_end = 37.89f;
			//float _lvl2_stg3_res_x = 23.70f;
			StageBoundary lvl2Stage4Bounds = new StageBoundary(37.90f,0f,37.90f);  

			//float _lvl2_stg4_bnd_start = 37.90f;
			//float _lvl2_stg4_res_x = 37.90f;

			// level 3
			StageBoundary lvl3Stage1Bounds = new StageBoundary(0f,6.85f,0f);  

			//float _lvl3_stg1_res_x = 0.00f;
			//float _lvl3_stg1_bnd_end = 6.85f;
			StageBoundary lvl3Stage2Bounds = new StageBoundary(7.60f,17.80f,8.50f);  

			//float _lvl3_stg2_bnd_start = 7.60f;
			//float _lvl3_stg2_bnd_end = 17.80f;
			//float _lvl3_stg2_res_x = 8.50f;
			StageBoundary lvl3Stage3Bounds = new StageBoundary(18.00f,39.50f,18.50f);  

			//float _lvl3_stg3_bnd_start = 18.00f;
			//float _lvl3_stg3_bnd_end = 39.50f;
			//float _lvl3_stg3_res_x = 18.50f;
			StageBoundary lvl3Stage4Bounds = new StageBoundary(40.00f,64.00f,40.00f);  

			//float _lvl3_stg4_bnd_start = 40.00f;
			//float _lvl3_stg4_bnd_end = 64.00f;
			//float _lvl3_stg4_res_x = 40.00f;

			if (current_level == "Level 1") {
				if (_deathPositionX <= lvl1Stage1Bounds.endPosX)
					_spawnLocation = new Vector3 (lvl1Stage1Bounds.spawnPosX, -1.73f, 0f);
				else if ((lvl1Stage2Bounds.startPosX <= _deathPositionX) && (_deathPositionX <= lvl1Stage2Bounds.endPosX))
					_spawnLocation = new Vector3 (lvl1Stage2Bounds.spawnPosX, -1.73f, 0f);
				else if ((lvl1Stage3Bounds.startPosX <= _deathPositionX) && (_deathPositionX <= lvl1Stage3Bounds.endPosX))
					_spawnLocation = new Vector3 (lvl1Stage3Bounds.spawnPosX, -1.73f, 0f);
				else if ((lvl1Stage4Bounds.startPosX <= _deathPositionX))
					_spawnLocation = new Vector3 (lvl1Stage4Bounds.spawnPosX, -1.73f, 0f);
			} else if (current_level == "Level 2") {
				if (_deathPositionX <= lvl2Stage1Bounds.endPosX)
					_spawnLocation = new Vector3 (lvl2Stage1Bounds.spawnPosX, -1.73f, 0f);
				else if ((lvl2Stage2Bounds.startPosX <= _deathPositionX) && (_deathPositionX <= lvl2Stage2Bounds.endPosX))
					_spawnLocation = new Vector3 (lvl2Stage2Bounds.spawnPosX, -1.73f, 0f);
				else if ((lvl2Stage3Bounds.startPosX <= _deathPositionX) && (_deathPositionX <= lvl2Stage3Bounds.endPosX))
					_spawnLocation = new Vector3 (lvl2Stage3Bounds.spawnPosX, -1.73f, 0f);
				else if ((lvl2Stage4Bounds.startPosX <= _deathPositionX))
					_spawnLocation = new Vector3 (lvl2Stage4Bounds.spawnPosX, -1.73f, 0f);
			} else if (current_level == "Level 3" && !bossCondition) {
				if (_deathPositionX <= lvl3Stage1Bounds.endPosX)
					_spawnLocation = new Vector3 (lvl3Stage1Bounds.spawnPosX, -1.73f, 0f);
				else if ((lvl3Stage2Bounds.startPosX <= _deathPositionX) && (_deathPositionX <= lvl3Stage2Bounds.endPosX))
					_spawnLocation = new Vector3 (lvl3Stage2Bounds.spawnPosX, -1.73f, 0f);
				else if ((lvl3Stage3Bounds.startPosX <= _deathPositionX) && (_deathPositionX <= lvl3Stage3Bounds.endPosX))
					_spawnLocation = new Vector3 (lvl3Stage3Bounds.spawnPosX, -1.73f, 0f);
				else if ((lvl3Stage4Bounds.startPosX <= _deathPositionX) && (_deathPositionX <= lvl3Stage4Bounds.endPosX)) {
					_spawnLocation = new Vector3 (lvl3Stage4Bounds.spawnPosX, -1.73f, 0f);
					GameObject platform = GameObject.Find ("BossStageSetupPlatform").gameObject;
					platform.GetComponent<BossStagePlatformMover> ().resetMovement (42.38f, -2.807f);
					Camera.main.GetComponent<AudioSource> ().clip = Music_Background;
					Camera.main.GetComponent<AudioSource> ().Play ();
					Camera.main.GetComponent<AudioSource> ().volume = 0.2f;
				}
			} else if (bossCondition) {
				GameObject bossObject = GameObject.FindGameObjectWithTag("Boss");
				float bossStageBoundaryStartPositionX = 66f;
				float bossStageBoundaryEndPositionX = 77f;
				float playerSpawnPositionX = 67f;

				float bossPositionX = bossObject.transform.position.x;
				float deathDistanceFromLeftBoundary = _deathPositionX - bossStageBoundaryStartPositionX;
				float deathDistanceFromRightBoundary = bossStageBoundaryEndPositionX - _deathPositionX;

				if ((deathDistanceFromLeftBoundary > deathDistanceFromRightBoundary)) {
					// player die's to the right of boss, spawn 2 units to his left
					 playerSpawnPositionX  = bossPositionX - 2;
					_spawnLocation = new Vector3 (playerSpawnPositionX, -1.73f, 0f);
				} else { // player die's to the left of boss, spawn 2 units to his right
					playerSpawnPositionX  = bossPositionX + 2;
					_spawnLocation = new Vector3 (playerSpawnPositionX, -1.73f, 0f); 
				}
			}
			_player.GetComponent<CharacterController2D>().Respawn(_spawnLocation);
		}
	}

	// public function for level complete
	public void LevelCompete() {
		// save the current player prefs before moving to the next level
		PlayerPrefManager.SavePlayerState(score,highscore,lives);

		// use a coroutine to allow the player to get fanfare before moving to next level
		StartCoroutine(LoadNextLevel());
	}

	public void Pickup(string pickupText, bool active){
		UIPickup.GetComponent<Text> ().text = pickupText;
		UIPickup.SetActive (active);
		if (active)
			StartCoroutine (HidePickupUI ());
	}

	public void PlayerAtBoss (){
		bossCondition = true;
	}
		
	public bool IsPlayerAtBoss (){
		return bossCondition;
	}

	// load the nextLevel after delay
	IEnumerator LoadNextLevel() {
		yield return new WaitForSeconds(3.5f); 
		UnityEngine.SceneManagement.SceneManager.LoadScene(levelAfterVictory);
	}

	IEnumerator HidePickupUI() {
		// destroy the dagger gameobject
		yield return new WaitForSeconds(2.5f);
		Pickup ("", false);
	}

	public void SetDeathPositionX (float positionX){
		_deathPositionX = positionX;
	}

	public bool IsInitialBossEngagement (){
		return initialBossEngagement;
	}

	public void ShowBossFightUI (){
		BossFightUI.SetActive (true);
	}

	public void HideBossFightUI () {
		initialBossEngagement = false;
		BossFightUI.SetActive (false);
		bossCanEngageCondition = true;
	}

	public bool CanBossEngagePlayer(){
		return bossCanEngageCondition;
	}

	public class StageBoundary {
		public float startPosX;
		public float endPosX;
		public float spawnPosX;

		public StageBoundary (float startX, float endX, float spawnX){
			startPosX = startX;
			endPosX = endX;
			spawnPosX = spawnX;
		}
	}
}
