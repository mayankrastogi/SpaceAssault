using UnityEngine;
using System.Collections;

public class ShipDamageMeter : GUIProgressBar
{
	// Reference cache
	private PlayerController playerController = null;

	void Start()
	{
		playerController = PlayerController.instance;
	}

	protected override void Update ()
	{
		// Update value and maxValue to player's hitPoints and maxHitPoints
		if(playerController != null)
		{
			value = playerController.hitPoints;
			maxValue = playerController.maxHitPoints;
		}
		else
		{
			value = 0;
		}

		// Call base class function to take care of computation
		base.Update ();
	}
}