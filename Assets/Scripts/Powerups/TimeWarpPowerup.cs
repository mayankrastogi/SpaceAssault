using UnityEngine;
using System.Collections;

public class TimeWarpPowerup : GameEntity
{
	// Inspector assigned
	public float duration = 10.0f;					// Duration of the effect
	public float speedScale = 0.5f;					// Speed scale value to set
	public AudioClip destroySound = null;			// Sound effect to play upon collision
	public float destroySoundVolume = 1.0f;			// Volume of the sound effect

	protected override void OnTriggerEnter (Collider other)
	{
		// Don't process event on collision with boundary
		if(dying || other.tag == "Boundary")
			return;
		
		if (other.tag == "Player")
		{
			// Set the World Speed timer and change speed scale
			gameController.SetSpeedScale(speedScale, duration);

			// Play the powerup pickup sound
			if(destroySound)
				AudioSource.PlayClipAtPoint (destroySound, transform.position, destroySoundVolume);
		}
		
		// Call base class version to do rest of the work
		base.OnTriggerEnter (other);
	}
}
