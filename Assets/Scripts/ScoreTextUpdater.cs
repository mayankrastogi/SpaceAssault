using UnityEngine;
using System.Collections;

public class ScoreTextUpdater : MonoBehaviour
{
	// Inspector assigned
	public GUIText scoreText = null;	// Reference to Score GUI Text object
	public float smoothing = 3.0f;		// How quickly score reaches to the correct value

	// Internals
	private float currentValue = 0.0f;	// Current interpolated score value
	private string displayValue = "0";	// Value to be displayed

	// Reference cache
	private GameController gameController = null;

	void Start()
	{
		gameController = GameController.instance;
	}

	void Update()
	{
		// Interpolate current score
		currentValue = Mathf.Lerp (currentValue, gameController.GetScore (), Time.deltaTime * smoothing);
		displayValue = Mathf.RoundToInt (currentValue).ToString ();
	}

	void OnGUI()
	{
		// Round the fractional score value to integer and update score text
		scoreText.text = displayValue;
	}
}
