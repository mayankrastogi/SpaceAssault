using UnityEngine;
using System.Collections;

public class WeaponController : MonoBehaviour
{
	// Inspector assigned
	public GameObject shot = null;		// Ship's weapon
	public Transform shotSpawn = null;	// Position where shots are spawned
	public float fireRate = 1.5f;		// Rate at which the weapon is fired
	public float delay = 0.5f;			// Time delay before first shot is fired

	void Start()
	{
		InvokeRepeating ("Fire", delay, fireRate);
	}

	void Fire()
	{
		Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
	}
}
