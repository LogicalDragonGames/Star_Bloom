using UnityEngine;
using System.Collections;

public class BounceListener : MonoBehaviour
{
	// Use this for initialization
	void Start()
	{
		Console.Instance.CommandSent += new Console.CommandEventHandler(OnCommand);
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void OnCommand( string message )
	{
		if( message == "bounce" )
			rigidbody.velocity += Vector3.up * 10;

		else if( message == "float" )
		{
			transform.position += Vector3.up * 2;
			rigidbody.constraints = RigidbodyConstraints.FreezePosition;
		}

		else if( message == "spin" )
		{
			rigidbody.AddTorque( (new Vector3(0.0f, 1.0f, 0.0f)).normalized * 10000.0f );
		}

		else if( message == "drop" )
		{
			rigidbody.constraints = RigidbodyConstraints.None;
		}
	}
}
