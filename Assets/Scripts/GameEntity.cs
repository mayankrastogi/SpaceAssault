using UnityEngine;
using System.Collections;

public class GameEntity : MonoBehaviour
{
	// Inspector assigned
	public GameObject explosion = null;			// Explosion particle effect for the object this script is attached to
	public int scoreValue = 0;					// Points added to score on destroying the object this script is attached to
	public int weight = 100;					// Probability of spawning the object this script is attached to
	public float powerupDropChance = 0.0f;		// Percentage chance of leaving behind a powerup

	// Internals
	protected float chance;
	protected bool dying = false;

	// Reference cache
	protected GameController gameController = null;
	protected PlayerController playerController = null;

	protected virtual void Start()
	{
		gameController = GameController.instance;
		playerController = PlayerController.instance;
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		// Don't process event on collision with boundary
		if(dying || other.tag == "Boundary")
			return;

		// Set dying to true
		dying = true;

		// Destroy the entity
		DestroyEntity ();

		// Instantiate a powerup
		if (Random.Range (0.0f, 100.0f) < powerupDropChance)
			gameController.InstantiatePowerup (transform.position);
	}

	// Method to destroy the Game entity
	public virtual void DestroyEntity()
	{
		// If an explosion prefab is assigned, instantiate it
		if (explosion != null)
		{
			Instantiate(explosion, transform.position, transform.rotation);
		}

		// Add enitity's points to the game controller
		gameController.AddPoints (scoreValue);

		// Destroy the game object
		Destroy (gameObject);
	}
}
