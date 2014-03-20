using UnityEngine;
using System.Collections;

public class Vibrator : MonoBehaviour {
	
	public float distance;
	public float rate;

	private float spawnPosition;
	private float targetPosition;
	private float newPosition;

	private Transform myTransform;

	void Start()
	{
		myTransform = transform;
		spawnPosition = myTransform.position.x;

		if (Random.Range (0, 2) == 0)
			targetPosition = spawnPosition + distance;
		else
			targetPosition = spawnPosition - distance;
	}

	void Update()
	{
		newPosition = Mathf.MoveTowards (myTransform.position.x, targetPosition, rate * Time.deltaTime);
		myTransform.position = new Vector3 (newPosition, 0.0f, myTransform.position.z);

		if(newPosition >= spawnPosition+distance)
			targetPosition = spawnPosition - distance;
		else if(newPosition <= spawnPosition-distance)
			targetPosition = spawnPosition+distance;
	}
}
