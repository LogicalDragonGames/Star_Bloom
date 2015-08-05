using UnityEngine;
using System.Collections;

public class CollisionDispatcher : MonoBehaviour
{
	public delegate void CollisionEnterHandler( Collision other );
	public CollisionEnterHandler CollisionEnter;

	public delegate void CollisionExitHandler( Collision other );
	public CollisionExitHandler CollisionExit;

	public delegate void TriggerEnterHandler( Collider other );
	public TriggerEnterHandler TriggerEnter;
	
	public delegate void TriggerExitHandler( Collider other );
	public TriggerExitHandler TriggerExit;

	void OnCollisionEnter( Collision other )
	{
		//Console.Instance.addGameChatMessage( "OnCollisionEnter: " + other.gameObject.name );
		if( null != CollisionEnter )
			CollisionEnter( other );
	}

	void OnCollisionExit( Collision other )
	{
		//Console.Instance.addGameChatMessage( "OnCollisionExit: " + other.gameObject.name );
		if( null != CollisionExit )
			CollisionExit( other );
	}

	void OnTriggerEnter( Collider other )
	{
		//Console.Instance.addGameChatMessage( "OnTriggerEnter: " + other.name );
		if( null != TriggerEnter )
			TriggerEnter( other );
	}

	void OnTriggerExit( Collider other )
	{
		//Console.Instance.addGameChatMessage( "OnTriggerExit: " + other.name );
		if( null != TriggerExit )
			TriggerExit( other );
	}
}
