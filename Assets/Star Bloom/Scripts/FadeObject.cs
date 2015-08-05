/*
	FadeObjectInOut.cs
 	Hayden Scott-Baron (Dock) - http://starfruitgames.com
 	6 Dec 2012 
 
	This allows you to easily fade an object and its children. 
	If an object is already partially faded it will continue from there. 
	If you choose a different speed, it will use the new speed. 
 
	NOTE: Requires materials with a shader that allows transparency through color.  
*/

using UnityEngine;
using System.Collections;

public class FadeObject : MonoBehaviour
{
	// publically editable speed
	public Color m_FadeInColor = Color.white;
	public Color m_FadeOutColor = Color.black;
	public float m_FadeInDuration = 0.5f; 
	public float m_FadeOutDuration = 0.5f;
	public bool m_TestFadeIn = false;
	public bool m_TestFadeOut = false;
	public bool m_FadeChildren = false;
	public Interpolate.EaseType m_EaseType = Interpolate.EaseType.Linear;

	protected Timer m_Timer;

	public enum FadeState
	{
		None,
		FadingIn,
		FadingOut
	};

	public FadeState m_State = FadeState.None;

	public void Start()
	{
		if( null == m_Timer )
			m_Timer = (Timer)gameObject.AddComponent<Timer>();
	}

	public void FadeIn()
	{
		m_State = FadeState.FadingIn;
		m_Timer.Begin( m_FadeInDuration );
	}

	public void FadeOut()
	{
		m_State = FadeState.FadingOut;
		m_Timer.Begin( m_FadeOutDuration );
	}

	void Update()
	{
		if( m_TestFadeIn )
		{
			FadeIn();
			m_TestFadeIn = false;
		}
		else if( m_TestFadeOut )
		{
			FadeOut();
			m_TestFadeOut = false;
		}

		if( m_State == FadeState.None )
			return;

		Color interpColor = GetInterpolatedColor();

		if( m_FadeChildren )
		{
			Renderer[] renderObjects = GetComponentsInChildren<Renderer>();

			foreach( Renderer obj in renderObjects )
				obj.material.SetColor("_Color", interpColor); 
		}
		else
			renderer.material.SetColor("_Color", interpColor); 

		if( m_Timer.IsFinished )
			m_State = FadeState.None;
	}

	protected Color GetInterpolatedColor()
	{
		
		Interpolate.Function easeFn = Interpolate.Ease( m_EaseType );
		
		Color startColor = m_FadeInColor;
		Color endColor = m_FadeOutColor;
		
		if( m_State == FadeState.FadingIn )
		{
			startColor = m_FadeOutColor;
			endColor = m_FadeInColor;
		}

		Vector3 startVec = new Vector3( startColor.r, startColor.g, startColor.b );
		Vector3 endVec = new Vector3( endColor.r, endColor.g, endColor.b );

		Vector3 rgb = Interpolate.Ease ( easeFn, startVec, endVec-startVec, m_Timer.m_CurTime, m_Timer.m_MaxTime );
		float alpha = easeFn( startColor.a, endColor.a - startColor.a, m_Timer.m_CurTime, m_Timer.m_MaxTime );

		return new Color( rgb.x, rgb.y, rgb.z, alpha );
	}
}