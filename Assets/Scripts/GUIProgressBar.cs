using UnityEngine;
using System.Collections;

public class GUIProgressBar : MonoBehaviour
{
	public float smoothing = 1.0f;			// How quickly the current value reaches the correct value
	public GUITexture[] bars = null;		// Array of bars
	public Texture2D emptyTexture = null;	// Texture to tile for empty area
	public Texture2D filledTexture = null;	// Texture to tile for filled area

	// Internals
	protected float value = 0.0f;			// Value to be represented by the progress bar
	protected float maxValue = 100.0f;		// Maximum value that can be represented
	protected float currentValue = 0.0f;	// Current interpolated value
	protected int numOfBarsFilled = 0;		// Number of bars to be drawn with filled texture
	private int i = 0;						// Array index
	
	// Interpolate value to be represented and determine number of bars to fill
	protected virtual void Update()
	{
		currentValue = Mathf.Lerp (currentValue, value, Time.deltaTime * smoothing);
		numOfBarsFilled = Mathf.RoundToInt(bars.Length * currentValue / maxValue);
	}

	// Draw the progress bar
	protected virtual void OnGUI()
	{
		for (i = 0; i < bars.Length; i++)
		{
			if(i < numOfBarsFilled)
				bars[i].texture = filledTexture;
			else
				bars[i].texture = emptyTexture;
		}
	}
}
