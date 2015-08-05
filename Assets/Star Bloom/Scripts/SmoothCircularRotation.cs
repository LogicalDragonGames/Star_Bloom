using UnityEngine;
using System.Collections;

public class SmoothCircularRotation : MonoBehaviour
{
	public Transform target = null;
	public float radius = 1.0f;
	public float speed = 1.0f;
	public Vector3 angleOffset = Vector3.zero;
	public float angularVelocityX = 0.0f;
	public float angularVelocityZ = 0.0f;

	public bool debug = false;

	void Start()
	{
		transform.Rotate( angleOffset );

		if( null == target )
			if( transform.parent != null )
				target = transform.parent;
			else
				Debug.LogError( "No assigned target and no parent object" );
	}

	// Update is called once per frame
	void Update()
	{
		angleOffset.x += Time.deltaTime * angularVelocityX;
		angleOffset.y += Time.deltaTime * speed;
		angleOffset.z += Time.deltaTime * angularVelocityZ;
		
		transform.rotation = Quaternion.identity;
		transform.localRotation = Quaternion.identity;

		transform.RotateAround( target.position, Vector3.right, angleOffset.x );
		transform.RotateAround( target.position, Vector3.forward, angleOffset.z );
		transform.RotateAround( target.position, transform.up, angleOffset.y );

		transform.position = target.position + transform.forward * radius;

		//Debug.DrawLine( transform.parent.position, transform.parent.position + transform.forward * radius );
	}
}
