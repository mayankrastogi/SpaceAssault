using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour
{
	// Inspector assigned
	public float speed = -5.0f;						// Speed by which the object travels
	public bool affectedBySpeedScale = true;		// Is the object affected by change in World Speed scale

	// Reference cache
	private GameController gameController = null;
	
	void Start()
	{
		gameController = GameController.instance;

		if (affectedBySpeedScale)
		{
			speed *= gameController.GetSpeedScale ();
		}

		rigidbody.velocity = transform.forward * speed;
	}
	
	public void UpdateVelocity()
	{
		if (affectedBySpeedScale)
		{
			speed *= gameController.GetSpeedScale ();
			rigidbody.velocity *= gameController.GetSpeedScale ();
		}
	}
	
	public void ResetVelocity()
	{
		if (affectedBySpeedScale)
		{
			speed /= gameController.GetSpeedScale ();
			rigidbody.velocity /= gameController.GetSpeedScale ();
		}
	}
}