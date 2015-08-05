using UnityEngine;
using System.Collections;

public class OffsetAnimator : MonoBehaviour
{
	public Interpolate.EaseType EaseType = Interpolate.EaseType.Linear;
	public Vector3 MaxOffset = Vector3.zero;
	public float Duration = 1.0f;

	private Interpolate.Function EaseFn = null;
	private Vector3 PreviousOffset = Vector3.zero;
	private float ElapsedTime = 0.0f;
	private bool AnimatingUp = true;
	
	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
		if( Duration == 0.0f )
			Duration = float.MinValue;

		EaseFn = Interpolate.Ease( EaseType );

		// Negate the previous offset
		this.transform.position -= PreviousOffset;
		
		// Increment time based on our animation direction
		if( AnimatingUp )
			ElapsedTime += Time.deltaTime;
		else
			ElapsedTime -= Time.deltaTime;

		// Flip our animation direction if the duration is exceeded
		if( ElapsedTime >= Duration )
		{
			ElapsedTime = Duration;
			AnimatingUp = false;
		}
		else if( ElapsedTime <= 0.0f )
		{
			ElapsedTime = 0.0f;
			AnimatingUp = true;
		}

		// Interpolate by a magnitude in the direction of the offset,
		// with the maximum being the magnitude of the given offset amount
		float maxOffsetMag = MaxOffset.magnitude;
		float curOffsetMag = EaseFn( 0.0f, maxOffsetMag, ElapsedTime, Duration );

		// Apply the interpolated offset as a magnitude to the offset normal
		Vector3 offsetNormal = (MaxOffset - Vector3.zero).normalized;
		PreviousOffset = offsetNormal * curOffsetMag;
		transform.position += PreviousOffset;
	}
}
