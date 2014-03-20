using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour
{
	public float tumble = 5.0f;			// Speed of rotation

	void Start()
	{
		// Set initial angular velocity
		rigidbody.angularVelocity = Random.insideUnitSphere * tumble;
	}
}
