using UnityEngine;
using System.Collections;

public class DestroyByTime : MonoBehaviour
{
	public float lifetime = 2.0f;		// Time after which the object gets destroyed

	void Start()
	{
		// Destroy object automatically after lifetime seconds
		Destroy (gameObject, lifetime);
	}
}
