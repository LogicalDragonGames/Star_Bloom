using UnityEngine;
using System.Collections;

public class RocketBox : MonoBehaviour
{
	public float force = 4000.0f;
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void OnTriggerEnter( Collider collider )
	{
		if( collider.gameObject.rigidbody != null )
			collider.gameObject.rigidbody.AddForce( transform.forward * force );
	}
}
