using UnityEngine;
using System.Collections;

public class IsoCameraTransitioner : MonoBehaviour
{
	public Interpolate.EaseType EaseType = Interpolate.EaseType.Linear;

	public float DistanceA = 15.0f;
	public float DistanceB = 15.0f;
	public Vector3 OffsetA = Vector3.zero;
	public Vector3 OffsetB = Vector3.zero;
	public Vector3 RotationA = new Vector3( 45.0f, 90.0f, 0.0f );
	public Vector3 RotationB = new Vector3( 45.0f, 90.0f, 0.0f );

	Interpolate.Function EaseFn = null;
	PlayerMovementController controllerScript;
	IsometricCamera camScript;
	CollisionField collisionField;

	// Use this for initialization
	void Start()
	{
		collisionField = GetComponent<CollisionField>();

		if( null == collisionField )
			Debug.LogError( "Could not find 'CollisionField' component" );
		
		collisionField.ObjectMoved += OnObjectMoved;
		collisionField.ObjectEntered += OnObjectEntered;
		collisionField.ObjectLeft += OnObjectLeft;
	}
	
	void OnObjectMoved( FieldObject obj )
	{
		if( obj.collider.tag == "Player" )
		{
			camScript = obj.collider.GetComponent<IsometricCamera>();

			if( null == camScript )
			{
				Debug.LogError( "Could not find 'IsometricCamera' component attached to object tagged 'Player'" );
				return;
			}

			EaseFn = Interpolate.Ease( EaseType );

			if( null == EaseFn )
			{
				Debug.LogError( "Failed to retrieve Ease function for EaseType '" + EaseType.ToString() + "'" );
				return;
			}

			camScript.Distance = EaseFn( DistanceA, DistanceB-DistanceA, obj.percent, 1.0f );
			camScript.Offset = Interpolate.Ease(EaseFn, OffsetA, OffsetB-OffsetA, obj.percent, 1.0f );
			camScript.Rotation = Interpolate.Ease(EaseFn, RotationA, RotationB-RotationA, obj.percent, 1.0f );
		}
	}

	void OnObjectEntered( FieldObject obj )
	{
		if( obj.collider.tag == "Player" )
		{
			controllerScript = obj.collider.GetComponent<PlayerMovementController>();
			
			if( null == controllerScript )
			{
				Debug.LogError( "Could not find 'PlayerMovementController' component attached to object tagged 'Player'" );
				return;
			}
			
			controllerScript.localAxesLocked = true;
		}
	}

	void OnObjectLeft( FieldObject obj )
	{
		if( obj.collider.tag == "Player" )
		{
			controllerScript = obj.collider.GetComponent<PlayerMovementController>();
			
			if( null == controllerScript )
			{
				Debug.LogError( "Could not find 'PlayerMovementController' component attached to object tagged 'Player'" );
				return;
			}
			
			controllerScript.localAxesLocked = false;
		}
	}
}
