using UnityEngine;
using System.Collections;

public class StrobeLightInterpolator : MonoBehaviour
{
	public enum InterpType
	{
		Ascending = 0,
		Descending,
		AscAndDesc
	};

	public Interpolate.EaseType EaseType = Interpolate.EaseType.Linear;
	public float m_Duration = 1.0f;
	public float m_TimeScaleMin = 1.0f;
	public float m_TimeScaleMax = 4.0f;
	public float m_FlickerRateMin = 1.0f;
	public float m_FlickerRateMax = 1.0f;
	public float m_OnOffRatioMin = 0.5f;
	public float m_OnOffRatioMax = 0.5f;
	public InterpType m_InterpolationType = InterpType.AscAndDesc;
	public bool m_EndLightActive = true;
	public bool m_EndStrobeActive = false;
	public bool m_Active = true;
	public bool m_Looping = true;

	protected Interpolate.Function EaseFn = null;
	protected StrobeLight strobeLight = null;
	protected float elapsedTime = 0.0f;
	protected bool ascending = false;

	// Use this for initialization
	void Start()
	{
		strobeLight = GetComponent<StrobeLight>();

		if( null == strobeLight )
		{
			Debug.LogError( "Could not find 'StrobeLight' component" );
		}
	}
	
	// Update is called once per frame
	void Update()
	{
		if( !m_Active )
			return;

		if( !light.enabled )
			light.enabled = true;
		
		if( !strobeLight.enabled )
			strobeLight.enabled = true;

		if( ascending )
			elapsedTime += Time.deltaTime;
		else
			elapsedTime += -Time.deltaTime;

		elapsedTime = Mathf.Clamp( elapsedTime, 0.0f, m_Duration );

		
		if( elapsedTime >= m_Duration && ascending )
		{
			if( m_InterpolationType == InterpType.Ascending )
			{
				elapsedTime = 0.0f;

				HandleLoopEnd();
			}
			else
			{
				ascending = false;
			}
		}
		else if( elapsedTime <= 0.0f && !ascending )
		{
			if( m_InterpolationType == InterpType.Descending )
				elapsedTime = m_Duration;
			else
				ascending = true;

			HandleLoopEnd();
		}

		EaseFn = Interpolate.Ease( EaseType );

		strobeLight.m_TimeScale = EaseFn( m_TimeScaleMin, m_TimeScaleMax - m_TimeScaleMin, elapsedTime, m_Duration );
		strobeLight.m_FlickerRate = EaseFn( m_FlickerRateMin, m_FlickerRateMax - m_FlickerRateMin, elapsedTime, m_Duration );
		strobeLight.m_OnOffRatio = EaseFn( m_OnOffRatioMin, m_OnOffRatioMax - m_OnOffRatioMin, elapsedTime, m_Duration );
	}

	void HandleLoopEnd()
	{
		if( !m_Looping )
		{
			m_Active = false;

			light.enabled = m_EndLightActive;
			strobeLight.enabled = m_EndStrobeActive;
		}
	}
	
	public void Restart()
	{
		m_Active = true;
		strobeLight.enabled = true;

		if( m_InterpolationType == InterpType.Descending )
		{
			ascending = false;
			elapsedTime = m_Duration;
		}
		else
		{
			ascending = true;
			elapsedTime = 0.0f;
		}
	}
}