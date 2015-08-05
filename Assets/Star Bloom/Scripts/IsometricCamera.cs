using UnityEngine;
using System.Collections;

public class IsometricCamera : MonoBehaviour
{
	public Transform Target = null;
	public Transform CamTransform = null;
	public float Distance = 15.0f;
	public Vector3 Offset = new Vector3( 0.0f, 0.0f, 0.0f );
	public Vector3 Rotation = new Vector3( 45.0f, 45.0f, 0.0f );
	protected float TranslateDuration = 0.5f; // Deprecated		// TODO: REMOVE REFS
	protected float RotateDuration = 0.5f;	// Deprecated	

	public float m_TranslateTime = 0.0f;
	public float m_RotateTime = 0.0f;

	public float CollisionBuffer = 2.0f;
	public float TargetCollisionBuffer = 2.0f;	// Buffer distance to prevent target collision
	public float MinCollisionDistance = 50.0f;

	public float m_SecondaryTransitionDurationIn = 1.0f;
	public float m_SecondaryTransitionDurationOut = 1.0f;

	public Interpolate.EaseType m_PrimaryEaseType = Interpolate.EaseType.Linear;
	public Interpolate.EaseType m_SecondaryEaseTypeIn = Interpolate.EaseType.Linear;
	public Interpolate.EaseType m_SecondaryEaseTypeOut = Interpolate.EaseType.Linear;
	
	public bool ClampOnStart = false;
	public bool DebugMode = false;

	protected Vector3 m_CurrentTarget = Vector3.zero;
	protected Vector3 m_PreviousTarget = Vector3.zero;
	protected Vector3 m_CurrentRotation = Vector3.zero;
	protected Vector3 m_PreviousRotation = Vector3.zero;
	protected bool m_HasMoved = false;
	private bool m_Enabled = true;

	protected float m_SecondaryTransitionTime = 0.0f;
	protected bool m_HasSecondaryTransition = false;
	protected bool m_SecondaryTransitionReturn = false;
	protected Vector3 m_SecondaryPosition;
	protected Vector3 m_SecondaryRotation;

	public delegate void CamTransitionHandler();
	public CamTransitionHandler SecondaryTransitionStarted;
	public CamTransitionHandler SecondaryTransitionCompleted;

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
			m_TranslateTime = TranslateDuration;
			m_RotateTime = RotateDuration;

			UpdateRotation();
			UpdateTranslation();
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if( !m_Enabled )
			return;

		UpdateSecondaryTarget();

		m_CurrentTarget = GetTargetPos();
		m_CurrentRotation = GetTargetRot();

		// Has the target moved since the last update?
		m_HasMoved = false;
		if( m_CurrentTarget != m_PreviousTarget )
		{
			m_TranslateTime = 0.0f;
			m_HasMoved = true;
		}

		if( m_CurrentRotation != m_PreviousRotation )
		{
			m_RotateTime = 0.0f;
			m_HasMoved = true;
		}

		// If the target hasn't moved and we're finished with our interpolations, we don't have
		// to do anything
		if( !m_HasMoved && m_TranslateTime >= TranslateDuration && m_RotateTime >= RotateDuration )
			return;
		
		m_TranslateTime = Mathf.Clamp( m_TranslateTime + Time.deltaTime, 0.0f, TranslateDuration );
		m_RotateTime = Mathf.Clamp( m_RotateTime + Time.deltaTime, 0.0f, RotateDuration );

		m_PreviousTarget = m_CurrentTarget;
		m_PreviousRotation = m_CurrentRotation;

		UpdateRotation();
		UpdateTranslation();
		UpdateCollision();
	}

	protected void UpdateSecondaryTarget()
	{
		if( !m_HasSecondaryTransition )
			return;

		if( m_SecondaryTransitionReturn )
		{
			m_SecondaryTransitionTime = Mathf.Clamp( m_SecondaryTransitionTime - Time.deltaTime, 0.0f, m_SecondaryTransitionDurationOut );
			if( m_SecondaryTransitionTime <= 0.0f )
				m_HasSecondaryTransition = false;
		}
		else
		{
			m_SecondaryTransitionTime = Mathf.Clamp( m_SecondaryTransitionTime + Time.deltaTime, 0.0f, m_SecondaryTransitionDurationIn );

			if( m_SecondaryTransitionTime >= m_SecondaryTransitionDurationIn )
				if( null != SecondaryTransitionCompleted )
					SecondaryTransitionCompleted();
		}
	}

	public void SetSecondaryTarget( Vector3 _pos, Vector3 _rot )
	{
		m_SecondaryPosition = _pos;
		m_SecondaryRotation = _rot;
		m_SecondaryTransitionReturn = false;
		m_SecondaryTransitionTime = 0.0f;
		m_HasSecondaryTransition = true;
		m_SecondaryTransitionReturn = false;

		if( null != SecondaryTransitionStarted )
			SecondaryTransitionStarted();
	}

	public void ClearSecondaryTarget()
	{
		m_SecondaryTransitionReturn = true;

		m_SecondaryTransitionTime = m_SecondaryTransitionDurationOut;
	}

	public float GetSecondaryDuration()
	{
		if( m_SecondaryTransitionReturn )
			return m_SecondaryTransitionDurationOut;
		else
			return m_SecondaryTransitionDurationIn;
	}

	public float GetSecondaryPerc()
	{
		float duration = GetSecondaryDuration();

		if( duration == 0.0f )
			duration = float.MinValue;

		return Mathf.Clamp01( m_SecondaryTransitionTime / duration );
	}

	private Vector3 GetTargetPos()
	{
		Vector3 targetRotation = CamTransform.rotation * Vector3.forward;
		Vector3 targetPos = Target.position + Offset + targetRotation * -Distance;
	
		if( m_HasSecondaryTransition )
		{
			Interpolate.Function ease = null;

			if( m_SecondaryTransitionReturn )
				ease = Interpolate.Ease( m_SecondaryEaseTypeOut );
			else
				ease = Interpolate.Ease( m_SecondaryEaseTypeIn );

			targetPos = Interpolate.Ease( ease, targetPos, m_SecondaryPosition-targetPos, m_SecondaryTransitionTime, GetSecondaryDuration() );
		}

		return targetPos;
	}
	
	private Vector3 GetTargetRot()
	{
		Vector3 rot = Rotation;

		if( m_HasSecondaryTransition )
		{
			Interpolate.Function ease = null;
			
			if( m_SecondaryTransitionReturn )
				ease = Interpolate.Ease( m_SecondaryEaseTypeOut );
			else
				ease = Interpolate.Ease( m_SecondaryEaseTypeIn );

			rot = Interpolate.Ease( ease, rot, m_SecondaryRotation-rot, m_SecondaryTransitionTime, GetSecondaryDuration() );
		}

		return rot;
	}

	private void UpdateTranslation()
	{
		Interpolate.Function ease = Interpolate.Ease( m_PrimaryEaseType );

		if( TranslateDuration > 0.0f || m_HasSecondaryTransition )
			CamTransform.position = Interpolate.Ease( ease, CamTransform.position, GetTargetPos() - CamTransform.position, 0.5f, 1.0f );
		else 
			CamTransform.position = GetTargetPos();
	}

	private void UpdateRotation()
	{
		Interpolate.Function ease = Interpolate.Ease( m_PrimaryEaseType );

		if( RotateDuration > 0.0f || m_HasSecondaryTransition )
		{
			Vector3 targetRotation = GetTargetRot();
			CamTransform.rotation = Quaternion.Euler( Interpolate.Ease( ease, CamTransform.rotation.eulerAngles, targetRotation - CamTransform.rotation.eulerAngles, 0.5f, RotateDuration ) );
		}
		else
			CamTransform.rotation = Quaternion.Euler( GetTargetRot() );
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
