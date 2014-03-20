using UnityEngine;
using System.Collections;

public class GameEntity_Enemy : GameEntity
{
	// Inspector assigned
	public float damage = 0.0f;		// Amount of damage done to player on collision

	protected override void OnTriggerEnter (Collider other)
	{
		// Don't process event on collision with boundary
		if(dying || other.tag == "Boundary")
			return;

		if (other.tag == "Player")
		{
			// Apply damage to the player
			playerController.TakeDamage (damage);
		}

		// Call base class version to do rest of the work
		base.OnTriggerEnter (other);
	}
}