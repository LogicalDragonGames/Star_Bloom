using UnityEngine;
using System.Collections;

// TODO: Event delegates
public class Timer : MonoBehaviour
{
	public float m_MaxTime = 0f;
	public float m_CurTime = 0f;
	bool m_IsRunning = false;
	public bool IsFinished { get { return m_CurTime >= m_MaxTime; } }

	public void Begin( float time )
	{
		m_MaxTime = time;
		m_CurTime = 0f;
		m_IsRunning = true;
	}

	public void Stop()
	{
		m_CurTime = 0f;
		m_IsRunning = false;
	}

	public void Pause()
	{
		m_IsRunning = false;
	}

	public void Resume()
	{
		m_IsRunning = true;
	}

	// Update is called once per frame
	void Update()
	{
		if( !m_IsRunning )
			return;

		m_CurTime = Mathf.Clamp( m_CurTime+Time.deltaTime, 0f, m_MaxTime );

		if( m_CurTime >= m_MaxTime )
			m_IsRunning = false;
	}
}
