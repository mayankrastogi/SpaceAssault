using UnityEngine;
using System.Collections;

public class EvasiveManeuver : MonoBehaviour
{
	// Inspector assigned
	public Boundary boundary = null;			// Region within which the ship can move
	public float tilt = 10.0f;					// How much the ship tilts while banking
	public float dodge = 5.0f;					// Maximum distance the ship will bank
	public float smoothing = 7.5f;				// How quickly the ship reaches target distance
	public Vector2 startWait = Vector2.zero;	// Min and max time to wait before 1st maneuver
	public Vector2 maneuverTime = Vector2.zero;	// Min and max time taken to finish maneuver
	public Vector2 maneuverWait = Vector2.zero;	// Min and max time delay between two maneuvers

	// Internals
	private float currentManeuver = 0.0f;		// Current velocity of maneuver
	private float targetManeuver = 0.0f;		// Target velocity of maneuver
	private Mover mover = null;					// The mover component having the speed value

	// Reference Cache
	private Rigidbody myRigidbody = null;
	private Transform myTransform = null;
	private GameController gameController = null;
	
	void Start()
	{
		// Cache references
		myRigidbody = rigidbody;
		myTransform = transform;
		gameController = GameController.instance;
		mover = GetComponent<Mover> ();

		StartCoroutine (Evade ());
	}
	
	IEnumerator Evade()
	{
		// Wait before performing first maneuver
		yield return new WaitForSeconds (Random.Range (startWait.x, startWait.y) / gameController.GetSpeedScale ());
		
		while(true)
		{
			// Calculate target distance and direction of movement
			targetManeuver = Random.Range (1, dodge) * -Mathf.Sign (myTransform.position.x);

			// Wait for maneuver to complete
			yield return new WaitForSeconds(Random.Range (maneuverTime.x, maneuverTime.y) / gameController.GetSpeedScale ());

			// Reset target distance
			targetManeuver = 0;

			// Wait before performing next maneuver
			yield return new WaitForSeconds(Random.Range (maneuverWait.x, maneuverWait.y) / gameController.GetSpeedScale ());
		}
	}
	
	void FixedUpdate()
	{
		// Calculate new velocity of ship in x-direction
		currentManeuver = Mathf.MoveTowards (myRigidbody.velocity.x, targetManeuver, Time.deltaTime * smoothing * gameController.GetSpeedScale ());

		// Change the velocity
		myRigidbody.velocity = new Vector3 (currentManeuver, 0.0f, mover.speed);

		// Clamp the ship to remain within the boundary
		myRigidbody.position = new Vector3
		(
			Mathf.Clamp (myRigidbody.position.x, boundary.xMin, boundary.xMax),
			0.0f,
			Mathf.Clamp (myRigidbody.position.z, boundary.zMin, boundary.zMax)
		);

		// Tilt the ship
		myRigidbody.rotation = Quaternion.Euler (0.0f, 0.0f, myRigidbody.velocity.x * -tilt);
	}
}
