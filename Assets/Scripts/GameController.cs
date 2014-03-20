using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// The various states the game controller can be in
public enum GameState
{
	GetReady, Playing, Paused, GameOver
}

public class GameController : MonoBehaviour
{
	// GUI Skin to use for pause menu
	public GUISkin gameSkin = null;

	// Prefabs and objects
	public List<GameObject> hazards = new List<GameObject> ();	// List of hazards in the game
	public List<GameObject> powerups = new List<GameObject> ();	// List of powerups in the game
	public GameObject gameOverObject = null;					// The object containing game over text
	
	// Wave spawn values
	public Vector3 spawnValues = Vector3.zero;	// Region where the hazards will be spawned
	public int hazardCount = 10;				// Number of hazards spawned in one wave
	public float spawnWait = 0.5f;				// Delay between two hazards spawned in the same wave
	public float startWait = 2.0f;				// Time delay before the first wave starts
	public float waveWait = 4.0f;				// Delay between two waves

	// Game Mechanics
	public float speedupRate = 0.001f;						// Rate at which world speed increases
	public float maxSpeedScale = 5.0f;						// Maximum world speed for normal gameplay
	public GameState currentState = GameState.GetReady;		// Starting state when the scene is loaded

	// Internals
	public int score = 0;						// Current score of the player
	public float speedScale = 1.0f;			// Scaling factor to use for reducing/increasing world speed
	private float normalSpeedScale = 1.0f;		// Backup variable to store scaling factor prior to change due to powerups
	private bool isNormalSpeed = true;			// Boolean to tell if the scaling factor value is due to powerups

	// Probability Tables
	private float[] hazardWeightTable = null;	// Hazard spawn probability table
	private float[] powerupWeightTable = null;	// Powerup spawn probability table

	private Dictionary<string, Timer> timers = new Dictionary<string, Timer> ();	// A list of names used by the timers
	
	public PlayerController playerController = null;	// Reference to player controller instance
	private SceneManager_Base sceneManager = null;		// Reference to scene manager instance

	// GUI regions
	private Rect pauseBox_ScreenRect = new Rect( 75, 220, 250, 160);
	private Rect continue_ScreenRect = new Rect( 125, 280, 150, 30 );
	private Rect quit_ScreenRect = new Rect( 125, 330, 150, 30);

	// Static Singleton instance
	private static GameController _instance = null;

	// Property to get singleton instance
	public static GameController instance
	{
		get
		{
			if(_instance == null)
				_instance = FindObjectOfType<GameController> ();
			return _instance;
		}
	}

	void Awake()
	{
		// Cache reference to the scene manager
		sceneManager = SceneManager_Base.Instance;

		// Set Audio listener volume to zero
		AudioListener.volume = 0.0f;

		// Build the probability tables
		BuildHazardWeightTable();
		BuildPowerupWeightTable();
		
		RegisterTimer ("World Speed");
		speedScale = 1.0f;
	}

	void Start()
	{
		score = 0;

		// Start playing the background music
		if(audio)
			audio.Play ();

		StartCoroutine (StartGame ());
	}

	void Update()
	{
		// If game is over, restart the level if Restart key is pressed
		if (currentState == GameState.GameOver && Input.GetButtonDown("Restart"))
		{
			Application.LoadLevel (Application.loadedLevel);
		}

		// If the game is not playing yet don't animate anything
		if (currentState != GameState.Playing)
			return;

		// If the ESC key is pressed  we need to enter pause mode
		if (Input.GetButtonDown("Pause"))
		{
			// Set the time scale to zero so all time based animation
			// and rigidbodies are frozen
			Time.timeScale = 0.0f;
			
			// Set game state to paused (this will cause OnGUI to display the pause menu)
			currentState = GameState.Paused;
			
			// Set the screen managers fade to 0.7 so we darken down the scene to make the
			// pause menu stand out better.
			if (sceneManager)
				sceneManager.SetScreenFade( 0.7f );
		}

		if (GetTimer("World Speed") == 0.0f)
		{
			if(!isNormalSpeed)
			{
				Mover[] movers = FindObjectsOfType<Mover> ();
				foreach (Mover m in movers)
					m.ResetVelocity ();
				speedScale = normalSpeedScale;
				isNormalSpeed = true;
			}
			
			// Gradually increase speed scale
			speedScale += Time.deltaTime * speedupRate;

			// Clamp speed scale to maximum value
			speedScale = Mathf.Clamp(speedScale, 0, maxSpeedScale);
		}

		// Update the timers
		foreach (KeyValuePair<string, Timer> entry in timers)
		{
			entry.Value.Tick(Time.deltaTime);	
		}
	}

	void OnGUI()
	{	
		// If we are not paused return
		if (currentState != GameState.Paused)
			return;
		
		// Assign the custom GUI Skin used by our game
		if (gameSkin)
			GUI.skin = gameSkin;

		// Create the transparent options box below the header
		GUI.Box(pauseBox_ScreenRect, "GAME PAUSED");
		
		// Create and react to the Continue button being pressed
		if (GUI.Button(continue_ScreenRect, "CONTINUE"))		
		{
			// Make sure time scale is resumed to normal as this
			// was set to zero for pause mode
			Time.timeScale = 1.0f;
			
			// Transition from pause mode back into playing mode
			currentState = GameState.Playing;
			
			// Get rid of any screen fade
			if (sceneManager)
				sceneManager.SetScreenFade (0.0f);
		}
		
		// Create and react to the Quit button
		if (GUI.Button(quit_ScreenRect, "QUIT")) 
		{
			// Re-enable Timescale
			Time.timeScale = 1.0f;

			// Load the Main Menu scene
			Application.LoadLevel ("Main Menu");
		}
	}

	// The start game co-routine
	private IEnumerator StartGame()
	{
		// Do a 2 second fade in of the scene
		float timer = 2.0f;
		
		while (timer >= 0.0f)
		{
			// Decrement the timer
			timer -= Time.deltaTime;
			
			// Set the screen fade on the scene manager
			sceneManager.SetScreenFade(timer / 2.0f);
			
			// Fade in the audio listener
			AudioListener.volume = 1.0f - (timer / 2.0f);
			
			// yield control
			yield return null;
		}
		
		// Transition into playing state
		currentState = GameState.Playing;
		
		// Start spawning waves
		StartCoroutine (SpawnWaves ());
	}

	// Co-routine to spawn enemy waves
	IEnumerator SpawnWaves()
	{
		GameObject hazard = null;
		Vector3 spawnPosition = Vector3.zero;

		// Wait before spawning the first wave
		yield return new WaitForSeconds(startWait / speedScale);
		
		while(true)
		{
			for(int i=0; i<hazardCount; i++)
			{
				// Get the type of enemy to spawn
				hazard = hazards[GetNextHazardIndex()];

				// Get a random spawn position
				spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);

				// Instantiate the enmey
				Instantiate (hazard, spawnPosition, Quaternion.identity);

				// Wait a few seconds before spawning next enemy
				yield return new WaitForSeconds(spawnWait / speedScale);
			}

			// Stop spawning if game gets over
			if(currentState == GameState.GameOver)
				break;

			// Wait before spawning the next wave
			yield return new WaitForSeconds(waveWait / speedScale);
		}
	}

	// Update high score if necessary when game is over
	public void GameOver()
	{
		// Change game state to Game Over state
		currentState = GameState.GameOver;

		// Show game over message
		gameOverObject.SetActive (true);

		// Load current high score
		int highScore = PlayerPrefs.GetInt ("High Score", 0);

		// Update high score if current score is higher
		if(score > highScore)
			PlayerPrefs.SetInt("High Score", score);
	}

	public void InstantiatePowerup(Vector3 position)
	{
		Instantiate (powerups [GetNextPowerupIndex ()], position, Quaternion.identity);
	}

	public float GetSpeedScale()
	{
		return speedScale;
	}
	
	public void SetSpeedScale(float newSpeedScale, float duration)
	{
		// If current speed scale is the normal value, make a backup copy for it
		if (isNormalSpeed)
		{
			normalSpeedScale = speedScale;
			isNormalSpeed = false;
		}
		// If the current value is different from the new value, update the speed
		// of all moving bodies
		if (speedScale != newSpeedScale)
		{
			speedScale = newSpeedScale;

			Mover[] movers = FindObjectsOfType<Mover> ();
			foreach (Mover m in movers)
				m.UpdateVelocity ();
		}

		SetTimer ("World Speed", duration);
	}

	// Method to increase score
	public void AddPoints(int points)
	{
		score += points;
	}

	// Get current score
	public float GetScore()
	{
		return score;
	}

	// Can be called by any object to register a special timer with the game controller
	// with the specified name.
	public void RegisterTimer(string key)
	{
		// If a timer with this name does not already exist
		// in our hash table
		if (!timers.ContainsKey (key))
		{
			// Store the name and the index of the timer in the dictionary table
			timers.Add(key, new Timer());
		}
	}
	
	// If a timer exists with the passed name fetch its Timer object from the
	// Dictionary and return its time.
	public float GetTimer(string key)
	{
		Timer timer = null;
		
		// Does a timer exist with the given name
		if (timers.TryGetValue (key, out timer))
		{
			// Return its time
			return timer.GetTime();
		}
		
		// No timer found
		return -1.0f;
	}
	
	// Add time onto a timer with the passed name
	public void UpdateTimer(string key, float seconds)
	{
		Timer timer = null;
		if (timers.TryGetValue (key, out timer))
		{
			timer.AddTime(seconds);
		}
	}

	// Set time of a timer with the passed name
	public void SetTimer(string key, float seconds)
	{
		Timer timer = null;
		if (timers.TryGetValue (key, out timer))
		{
			timer.SetTime(seconds);
		}
	}

	private void BuildHazardWeightTable()
	{
		BuildWeightTable (hazards, out hazardWeightTable);
	}
	
	private int GetNextHazardIndex()
	{
		return GetNextObjectIndex (hazards, hazardWeightTable);
	}
	
	private void BuildPowerupWeightTable()
	{
		BuildWeightTable (powerups, out powerupWeightTable);
	}
	
	private int GetNextPowerupIndex()
	{
		return GetNextObjectIndex (powerups, powerupWeightTable);
	}
	
	// Helper function to build probability tables
	private void BuildWeightTable(List<GameObject> objectList, out float[] weightTable)
	{
		GameEntity gameEntity = null;

		// Get number of objects available
		int noOfPrefabs = objectList.Count;
		int sum = 0, i = 0;
		
		// Allocate the table to hold a weight for each object
		weightTable = new float[noOfPrefabs];
		
		// Pass 1 add up probabilities
		for(i = 0; i < noOfPrefabs; i++)
		{
			// Get the script from the object
			gameEntity = objectList[i].GetComponent<GameEntity>();
			if(gameEntity != null)
			{
				// Fetch its assigned weight and add it to the current sum of weights
				sum += gameEntity.weight;
				
				// Store the weight in the weight table
				weightTable[i] = (float)gameEntity.weight;
			}
		}
		
		// Weight table now stores all the weights for each object as assigned in the inspector
		// and we have the sum of all weights. Now we do a second pass through the weight table
		// and divide each weight by the sum. This will map all weights into the 0-1 range.
		
		// Pass 2 normalize probabilities
		for (i = 0; i < noOfPrefabs; i++)
		{
			weightTable[i] /= sum;
		}
	}
	
	// Helper function to get the next object to spwan
	private int GetNextObjectIndex(List<GameObject> objectList, float[] weightTable)
	{
		// Choose a random number between 0 & 1
		float t = Random.value;
		
		// q is used to record how far we have searched
		// through the 0 & 1 range
		float q = 0.0f;
		
		// Loop through all our prefabs until we find the
		// one that our t value maps to
		for (int i = 0; i < objectList.Count; i++)
		{
			// Increment q with the normalized weight of
			// the current brick
			q += weightTable[i];
			
			// if t is smaller (or equal) to q then we have
			// found the brick whose weight is mapped to this
			// region of the 0 to 1 range.
			if(t <= q)
				return i;
		}
		return 0;
	}
}
