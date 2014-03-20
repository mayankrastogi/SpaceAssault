using UnityEngine;
using System.Collections;

public class StarfieldScroller : MonoBehaviour
{
	// Array of starfield particle systems
	private ParticleSystem[] starfields = null;

	// Reference cache
	private GameController gameController = null;

	// Use this for initialization
	void Start ()
	{
		gameController = GameController.instance;
		starfields = GetComponentsInChildren<ParticleSystem> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		foreach (ParticleSystem starfield in starfields)
			starfield.playbackSpeed = gameController.GetSpeedScale ();
	}
}
