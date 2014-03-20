using UnityEngine;
using System.Collections;

// ---------------------------------------------------------------------------------------
// Class : SceneManager_Menu
// Desc	 : Manages the sequence of events on the main menu screen and reacts to button 
//		   button selections
// ---------------------------------------------------------------------------------------
public class SceneManager_Menu : SceneManager_Base 
{	
	// Inspector Assigned
	public Transform shipEntryPoint = null;		// Position where the ship arrives to in the beginning
	public Transform camExitPoint = null;		// Position towards which the camera moves when play button is clicked
	public Transform shipObject = null;			// The space ship game object
	public GUITexture titleObject = null;		// The game object containing the Space Assault Title
	public GameObject menuObject = null;		// The parent menu object (containing the PLay and Quit text meshes as children)
	public TextMesh highScoreText = null;		// The text mesh for displaying highest score

	// Internals
	private Vector3 originalShipPosition = Vector3.zero;	// Position of the ship at scene startup
	private Vector3 targetShipPosition = Vector3.zero;		// Position the ship should move to
	private Vector3 originalCamPosition = Vector3.zero;		// Position of the camera at scene startup
	private Vector3 targetCamPosition = Vector3.zero;		// Position the camera should move to
	private Transform cameraTransform = null;				// Current transform of the camera

	void Awake()
	{
		// Make sure AudioListener has volume
		AudioListener.volume = 0.0f;
		
		// Get the main camera
		if (Camera.main)
		{
			// Store a reference to its transform so we can update it
			cameraTransform = Camera.main.transform; 
			
			// Store the original position vector of the camera at scene startup
			originalShipPosition = shipObject.position;
			originalCamPosition = cameraTransform.position;
		}
		
		// Store the final position vector we would like the camera to move to
		if (shipEntryPoint)
			targetShipPosition = shipEntryPoint.position;
		if (camExitPoint)
			targetCamPosition = camExitPoint.position;
	}

	void Start()
	{
		// No screen fade at startup
		ScreenFade = 1.0f;
		
		// Enable the showing of the mouse cursor so we can select menu items
		Screen.showCursor = true;

		// Fetch high score
		highScoreText.text = "High Score : " + PlayerPrefs.GetInt ("High Score", 0);

		// Start playing the background music
		if(audio)
			audio.Play ();

		// Load the Menu Screen
		StartCoroutine (LoadMenuScreen ());
	}

	public override void OnButtonSelect( string buttonName )
	{
		// If this has been called and we are already processing an action return
		if (ActionSelected) return;
		
		// If the play buttton has been selected start the coroutine that animates the
		// camera through the hole in the wall and then load the next scene.
		if (buttonName=="Play")
		{
			ActionSelected = true;
			StartCoroutine(	LoadGameScene() );
		}
		// Otherwise, if the Quit button is pressed start the Coroutine that fades
		// the scene and then loads in the Closing Credits scene.
		else if (buttonName=="Exit")
		{
			ActionSelected = true;
			StartCoroutine( QuitGame() );
		}
	}

	private IEnumerator LoadMenuScreen()
	{
		// Ddo a 2 second animation of the ship
		float timer = 2.0f;
		
		// While the timer has not expired
		while (timer>0.0f)
		{
			// Decrement timer
			timer-=Time.deltaTime;
			
			// Set the screen fade
			if (timer>=0.0f) ScreenFade = timer/2.0f;
			
			// Generate the camera position by lerping between the original and target camera position using the timer
			// as the interpolator.
			if (timer>=0.0f) shipObject.position = Vector3.Lerp( originalShipPosition, targetShipPosition, 1-(timer/2.0f));
			
			// Fade in volume of listener based on timer countdown
			AudioListener.volume = 1.0f-timer/2.0f;
			
			// Yield control
			yield return null;
		}

		// Fade in the menu in next 1 second

		// Get all the renderers of the menu object
		menuObject.SetActive (true);
		Renderer[] renderers = menuObject.GetComponentsInChildren<Renderer>();	
		
		// Perform a 1 second fade-out of the menu
		timer = 1.0f;
		
		// While the 1 second has still not expired
		while (timer>0.0f)
		{
			// Update the timer
			timer-=Time.deltaTime;
			
			// Loop through each renderer in the menu object
			foreach( Renderer r in renderers )
			{
				if (r && r.material)
				{
					// Fetch the material of the renderer
					Color col = r.material.color;
					
					// set the alpha of the material to the timer value
					col.a = 1.0f - timer;
					r.material.color = col;	
				}
			}
			
			// Yield control
			yield return null;
		}

		// Ddo a 1 second fade in of high score
		timer = 1.0f;
		highScoreText.gameObject.SetActive (true);
		// While the timer has not expired
		while (timer>0.0f)
		{
			// Decrement timer
			timer-=Time.deltaTime;
			
			Renderer r = highScoreText.renderer;
			if (r && r.material)
			{
				// Fetch the material of the renderer
				Color col = r.material.color;
				
				// set the alpha of the material to the timer value
				col.a = 1.0f - timer;
				r.material.color = col;	
			}
			
			// Yield control
			yield return null;
		}
	}

	private IEnumerator LoadGameScene()
	{
		// Get all the renderers of the menu object
		Renderer[] renderers = menuObject.GetComponentsInChildren<Renderer>();
		Renderer highScoreRenderer = highScoreText.renderer;
		
		// Perform a 1 second fade-out of the menu
		float timer = 1.0f;
		
		// While the 1 second has still not expired
		while (timer>0.0f)
		{
			// Update the timer
			timer-=Time.deltaTime;
			
			// Loop through each renderer in the menu object
			foreach( Renderer r in renderers )
			{
				if (r && r.material)
				{
					// Fetch the material of the renderer
					Color col = r.material.color;
					
					// set the alpha of the material to the timer value
					col.a = timer;
					r.material.color = col;	
				}
			}
			if (titleObject)
			{
				// Fetch the material of the renderer
				Color col = titleObject.color;
				
				// set the alpha of the material to the timer value
				col.a = timer;
				titleObject.color = col;	
			}
			if (highScoreRenderer && highScoreRenderer.material)
			{
				// Fetch the material of the renderer
				Color col = highScoreRenderer.material.color;
				
				// set the alpha of the material to the timer value
				col.a = timer;
				highScoreRenderer.material.color = col;	
			}
			
			// Yield control
			yield return null;
		}
		
		// Now do a 1.5 second animation of the camera
		timer = 1.5f;
		
		// While the timer has not expired
		while (timer > 0.0f)
		{
			// Decrement timer
			timer -= Time.deltaTime;
			
			// Set the screen fade
			if (timer >= 0.0f)
				ScreenFade = 1.0f - (timer / 1.5f);
			
			// Generate the camera position by lerping between the original and target camera position using the timer
			// as the interpolator.
			if (timer >= 0.0f)
				cameraTransform.position = Vector3.Lerp (originalCamPosition, targetCamPosition, 1.0f - (timer / 1.5f));
			
			// Fade out volume of listener based on timer countdown
			AudioListener.volume = timer / 1.5f;
			
			// Yield control
			yield return null;
		}
		
		// We are now in a black-out and can load the game scene.
		Application.LoadLevel("Game Scene");
	}

	private IEnumerator QuitGame()
	{
		// Fade out screen
		float timer = 1.0f;
		while (timer > 0.0f)
		{
			timer -= Time.deltaTime;
			if(timer >= 0.0f)
				ScreenFade = 1.0f - timer;
			AudioListener.volume = timer;

			if (titleObject)
			{
				// Fetch the material of the renderer
				Color col = titleObject.color;
				
				// set the alpha of the material to the timer value
				col.a = timer;
				titleObject.color = col;	
			}

			yield return null;
		}
		Application.Quit ();
	}
}
