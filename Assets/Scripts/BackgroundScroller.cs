using UnityEngine;
using System.Collections;

// ---------------------------------------------------------------------------------------
// Class : BackgroundScroller
// Desc	 : Scrolls the background at the given speed and tiles it automatically
// ---------------------------------------------------------------------------------------
public class BackgroundScroller : MonoBehaviour
{
	// Inspector Assigned
	public float scrollSpeed = -0.25f;				// Scrolling speed of background
	public float tileSizeZ = 30.0f;					// Size of background tile along Z-axis

	// Internals
	private Vector3 startPosition = Vector3.zero;	// Initial position of the background
	private float newPosition = 0.0f;				// Used to store calculated new position

	// Reference Cache
	private Transform myTransform = null;
	private GameController gameController = null;

	// Initialization
	void Start ()
	{
		myTransform = transform;
		startPosition = myTransform.position;

		gameController = GameController.instance;
	}
	
	// Update is called once per frame
	void Update ()
	{
		// If the script is being used in game scene, move the background according
		// to the World Speed Scale
		if(gameController)
		{
			newPosition = Mathf.Repeat (myTransform.position.z + Time.deltaTime * scrollSpeed * gameController.GetSpeedScale (), tileSizeZ);
		}
		// otherwise simply scroll with a fixed speed
		else
		{
			newPosition = Mathf.Repeat (Time.time * scrollSpeed, tileSizeZ);
		}
		myTransform.position = startPosition + Vector3.forward * newPosition;
	}
}
