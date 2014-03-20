using UnityEngine;
using System.Collections;

public class InvincibilityPowerup : GameEntity
{
	// Inspector assigned
	public float duration = 10.0f;					// Duration of the effect

	protected override void OnTriggerEnter (Collider other)
	{
		// Don't process event on collision with boundary
		if(dying || other.tag == "Boundary")
			return;
		
		if (other.tag == "Player")
		{
			// Set Invincibility Timer
			gameController.SetTimer ("Invincible", duration);

			// Play the powerup pickup sound
			if(audio)
				audio.Play ();
		}
		
		// Call base class version to do rest of the work
		base.OnTriggerEnter (other);
	}
}
