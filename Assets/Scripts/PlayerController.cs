using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
	// Inspector assigned
	public float hitPoints = 100.0f;			// Amount of damage that the ship can currently take
	public float maxHitPoints = 100.0f;			// Maximum damage the ship can take
	public float speed = 10.0f;					// Current speed of player ship
	public Boundary boundary = null;			// Screen region in which the player ship can move
	public float tilt = 4.0f;					// Maximum tilt of the ship while banking left and right
	public Transform shotSpawn = null;			// Transform where weapon shots will be spawned when fired
	public GameObject explosion = null;			// Explosion particle effect for the player ship
	public GameObject forceField = null;		// The force-field effect for the player ship

	// Weapons
	public GameObject primaryWeapon = null;		// Current primary weapon of player ship
	public GameObject secondaryWeapon = null;	// Current secondary weapon of player ship

	// Weapon Fire Rate (Time delay between two consecutive shots
	public float primaryFireRate = 0.25f;		// Fire rate of primary weapon
	public float secondaryFireRate = 0.5f;		// Fire rate of primary weapon

	// Internal
	private float moveHorizontal = 0.0f;		// Amount of movement in x-direction
	private float moveVertical = 0.0f;			// Amount of movement in z-direction
	private float primaryNextFire = 0.0f;		// Time after which next primary weapon shot can be fired
	private float secondaryNextFire = 0.0f;		// Time after which next secondary weapon shot can be fired

	// Reference cache
	private Rigidbody myRigidbody = null;
	private GameController gameController = null;

	// Static Singleton Instance
	private static PlayerController _instance = null;

	public static PlayerController instance
	{
		get
		{
			if(_instance == null)
				_instance = FindObjectOfType<PlayerController> ();
			return _instance;
		}
	}

	void Start()
	{
		myRigidbody = rigidbody;
		gameController = GameController.instance;
		hitPoints = maxHitPoints;

		gameController.RegisterTimer ("Invincible");
	}

	void Update()
	{
		// If the game is not in playing state then return
		if (gameController.currentState != GameState.Playing)
			return;

		// Check if primary weapon fire key is pressed
		if (Input.GetButton ("Fire1") && Time.time > primaryNextFire && primaryWeapon)
		{
			primaryNextFire = Time.time + primaryFireRate;
			Instantiate (primaryWeapon, shotSpawn.position, shotSpawn.rotation);
		}
		
		// Check if secondary weapon fire key is pressed
		if(Input.GetButton ("Fire2") && Time.time > secondaryNextFire && secondaryWeapon)
		{
			secondaryNextFire = Time.time + secondaryFireRate;
			Instantiate (secondaryWeapon, shotSpawn.position, shotSpawn.rotation);
		}
	}
	
	void FixedUpdate()
	{
		// If the game is not in playing state then return
		if (gameController.currentState != GameState.Playing)
			return;

		moveHorizontal = Input.GetAxis ("Horizontal");
		moveVertical = Input.GetAxis ("Vertical");

		// Change velocity of player ship
		myRigidbody.velocity = new Vector3 (moveHorizontal, 0.0f, moveVertical) * speed;

		// Clamp ship's position to remain inside the boundary
		myRigidbody.position = new Vector3
		(
			Mathf.Clamp (myRigidbody.position.x, boundary.xMin, boundary.xMax),
			0.0f,
			Mathf.Clamp (myRigidbody.position.z, boundary.zMin, boundary.zMax)
		);

		// Rotate the ship when it is moving left or right
		myRigidbody.rotation = Quaternion.Euler (0.0f, 0.0f, myRigidbody.velocity.x * -tilt);
	}

	void LateUpdate()
	{
		// If the invincible timer is set, activate the force-field object
		if (gameController.GetTimer("Invincible") > 0.0f)
		{
			if(forceField != null && forceField.activeSelf == false)
				forceField.SetActive (true);
		}
		// otherwise turn it off
		else
		{
			if(forceField != null && forceField.activeSelf == true)
				forceField.SetActive (false);
		}
	}

	public void TakeDamage(float damage)
	{
		// Don't take damage if the player is invincible
		if (gameController.GetTimer ("Invincible") > 0.0f)
			return;

		// Decrease hit points
		hitPoints -= damage;

		// Check if player is dead
		if(hitPoints < 0.0f)
		{
			DestroyShip();
		}
		else
		{
			// Damage was taken, play the sound effect
			if (audio)
				audio.Play ();
		}
	}

	public void DestroyShip()
	{
		// Don't take damage if the player is invincible
		if (gameController.GetTimer ("Invincible") > 0.0f)
			return;

		// If an explosion prefab is assigned, instantiate it
		if (explosion != null)
		{
			Instantiate (explosion, myRigidbody.position, myRigidbody.rotation);
		}

		// Destroy the game object
		Destroy (gameObject);

		// Tell game controller that the game is over
		gameController.GameOver ();
	}
}
