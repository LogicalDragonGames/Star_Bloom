using UnityEngine;
using System.Collections;

public class PositionLerper : MonoBehaviour
{
	public Vector3 MaxOffset = Vector3.zero;
	public float Duration = 1.0f;

	private Vector3 PreviousOffset = Vector3.zero;
	private float PercentCompletion = 0.0f;
	private bool LerpingUp = true;

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
		if( Duration == 0.0f )
			Duration = float.MinValue;

		// Negate the previous offset
		this.transform.position -= PreviousOffset;

		// Calculate our duration in the offset
		float transDuration = PercentCompletion * Duration;

		if( LerpingUp )
			transDuration += Time.deltaTime;
		else
			transDuration -= Time.deltaTime;

		PercentCompletion = transDuration / Duration;

		if( PercentCompletion > 1.0f )
		{
			PercentCompletion = 1.0f;
			LerpingUp = false;
		}
		else if( PercentCompletion < 0.0f )
		{
			PercentCompletion = 0.0f;
			LerpingUp = true;
		}

		PreviousOffset = PercentCompletion * MaxOffset;
		transform.position += PreviousOffset;
	}
}
