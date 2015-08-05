using UnityEngine;
using System.Collections;

public class IsometricCamera : MonoBehaviour
{
	public Transform Target = null;
	public Transform CamTransform = null;
	public float Distance = 15.0f;
	public Vector3 Offset = new Vector3( 0.0f, 0.0f, 0.0f );
	public Vector3 Rotation = new Vector3( 45.0f, 45.0f, 0.0f );
	public float RotateTime = 0.5f;
	public float TranslateTime = 0.5f;
	public float CollisionBuffer = 2.0f;
	public float TargetCollisionBuffer = 2.0f;	// Buffer distance to prevent target collision
	public float MinCollisionDistance = 50.0f;
	public bool ClampOnStart = false;
	public bool DebugMode = false;
	private bool m_Enabled = true;

	// Use this for initialization
	void Start()
	{
		if( !CamTransform && Camera.main )
			CamTransform = Camera.main.transform;

		if( !CamTransform ) 
		{
			Debug.Log("Please assign a to the script or assign a main camera.");
			m_Enabled = false;	
		}

		if( Target == null )
			Target = transform;

		if( null == Target )
		{
			Debug.Log("Please assign a to the script or assign a main camera.");
			m_Enabled = false;	
		}

		if( m_Enabled && ClampOnStart )
		{
			float userTransTime = TranslateTime;
			float userRotTime = RotateTime;
			TranslateTime = RotateTime = 0.0f;

			Update();

			TranslateTime = userTransTime;
			RotateTime = userRotTime;
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if( !m_Enabled )
			return;
		
		UpdateRotation();
		UpdateTranslation();
		UpdateCollision();
	}

	private Vector3 GetTargetPos()
	{
		Vector3 targetRotation = CamTransform.rotation * Vector3.forward;
		return Target.position + Offset + targetRotation * -Distance;
	}

	private void UpdateTranslation()
	{
		if( TranslateTime > 0.0f )
			CamTransform.position = Vector3.Lerp( CamTransform.position, GetTargetPos(), Time.time * (1.0f/(TranslateTime*1000.0f)) );

		else if( TranslateTime == 0.0f )
			CamTransform.position = GetTargetPos();
	}

	private void UpdateRotation()
	{
		/*if( FocusTarget && RotateTime > 0.0f )
		{
			Quaternion targetRotation = Quaternion.FromToRotation( m_CamTransform.position, GetTargetPos() );
			m_CamTransform.rotation = Quaternion.Lerp( m_CamTransform.rotation, targetRotation, Time.time * (1.0f/(RotateTime*1000.0f)) );
		}

		else if( FocusTarget && RotateTime == 0.0f )
			m_CamTransform.rotation = Quaternion.FromToRotation( m_CamTransform.position, GetTargetPos() );

		else */

		if( RotateTime > 0.0f )
		{
			Quaternion targetRotation = Quaternion.Euler( Rotation );
			CamTransform.rotation = Quaternion.Lerp( CamTransform.rotation, targetRotation, Time.time * (1.0f/(RotateTime*1000.0f)) );
		}

		else if( RotateTime == 0.0f )
			CamTransform.rotation = Quaternion.Euler( Rotation );
	}

	private void UpdateCollision()
	{
		RaycastHit hitInfo = new RaycastHit();

		const int maxCollisions = 50;
		int handledCollisions = 0;

		// Debug
		ArrayList dbgCollisionPoints = null;
		if( DebugMode )
			dbgCollisionPoints = new ArrayList();

		Vector3 targetToCamera = CamTransform.position - Target.position;
		targetToCamera.Normalize();
		Vector3 targetCollisionOffset = targetToCamera * TargetCollisionBuffer;

		// Handle each collision entry
		while( Physics.Linecast( CamTransform.position, Target.position + targetCollisionOffset, out hitInfo )
		      && handledCollisions < maxCollisions )
		{
			if( DebugMode )
				dbgCollisionPoints.Add( hitInfo );

			if( hitInfo.transform == Target )
				break;

			if( null != Target )
				if( null != Target.parent )
					if( hitInfo.transform.IsChildOf(Target.parent.transform) )
						break;

			Vector3 correctionVector = Target.position - hitInfo.point;
			correctionVector.Normalize();

			CamTransform.position = hitInfo.point + correctionVector * CollisionBuffer;
		
			++handledCollisions;
		}

		if( DebugMode )
		{
			foreach( RaycastHit dbgHit in dbgCollisionPoints )
				Debug.DrawLine( dbgHit.point + Vector3.down * 1000, dbgHit.point + Vector3.up * 1000 );

			Debug.DrawLine( CamTransform.position, Target.position + targetCollisionOffset );
		}
	}
}
