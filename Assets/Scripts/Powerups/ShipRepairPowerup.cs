using UnityEngine;
using System.Collections;

public class ShipRepairPowerup : GameEntity
{
	// Inspector Assigned
	public AudioClip destroySound = null;			// Sound effect to play upon collision
	public float destroySoundVolume = 1.0f;			// Volume of the sound effect

	protected override void OnTriggerEnter (Collider other)
	{
		// Don't process event on collision with boundary
		if(dying || other.tag == "Boundary")
			return;
		
		if (other.tag == "Player")
		{
			// Max-out player hit points
			playerController.hitPoints = playerController.maxHitPoints;
			
			// Play the powerup pickup sound
			if(destroySound)
				AudioSource.PlayClipAtPoint (destroySound, transform.position, destroySoundVolume);
		}
		
		// Call base class version to do rest of the work
		base.OnTriggerEnter (other);
	}
}
