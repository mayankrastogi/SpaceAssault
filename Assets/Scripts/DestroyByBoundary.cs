using UnityEngine;
using System.Collections;

public class DestroyByBoundary : MonoBehaviour
{
	// Destroy object when it leaves the boundary
	void OnTriggerExit(Collider other)
	{
		Destroy (other.gameObject);
	}
}
