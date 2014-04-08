using UnityEngine;
using System.Collections;

public class Expander : MonoBehaviour {

	// Inspector Assigned
	public float rate = 1.0f;					// Rate of expansion
	public Vector3 finalScale = Vector3.one;	// Final scale of the object

	// Reference Cache
	private Transform myTransform;
	private Vector3 myScale;

	void Start ()
	{
		myTransform = transform; 
		myScale = myTransform.localScale;
	}

	void Update ()
	{
		myScale = Vector3.Lerp (myScale, finalScale, rate * Time.deltaTime);
		myTransform.localScale = myScale;
	}
}
