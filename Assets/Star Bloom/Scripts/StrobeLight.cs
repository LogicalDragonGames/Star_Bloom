using UnityEngine;
using System.Collections;

public class StrobeLight : MonoBehaviour
{
	public float m_TimeScale = 1.0f;
	public float m_FlickerRate = 1.0f;
	public float m_OnOffRatio = 0.5f;
	public bool m_OnFirst = true;
	public float invFlickerRate = 0.0f;

	protected bool m_StateOn = true;
	protected float m_LocalOffDuration = 0.0f;
	protected float m_LocalOnDuration = 0.0f;
	protected float m_ElapsedTime = 0.0f;

	// Use this for initialization
	void Start()
	{
	}
	
	// Update is called once per frame
	void Update()
	{
		if( null == this.light )
			return;

		// Determine on/off times
		invFlickerRate = 1/m_FlickerRate;
		m_OnOffRatio = Mathf.Clamp( m_OnOffRatio, 0.0f, 1.0f );
		m_LocalOnDuration = m_OnOffRatio * invFlickerRate;
		m_LocalOffDuration = (1.0f - m_OnOffRatio) * invFlickerRate;

		// Translate current time to localized on-off state
		m_ElapsedTime += Time.deltaTime * m_TimeScale;
		float localTime = Mathf.Repeat( m_ElapsedTime, invFlickerRate );

		if( m_OnFirst )
			m_StateOn = localTime < m_LocalOnDuration;
		else
			m_StateOn = localTime < m_LocalOffDuration;

		this.light.enabled = m_StateOn;
	}
}
