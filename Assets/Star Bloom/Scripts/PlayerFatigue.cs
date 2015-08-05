using UnityEngine;
using System.Collections;

public class PlayerFatigue : MonoBehaviour
{
	public float m_Stamina = 0.0f;
	public float m_MaxStamina = 0.0f;

	public float PercentStamina
	{ 
		get{ return m_Stamina / m_MaxStamina; }
		set{ m_Stamina = Mathf.Clamp01( PercentToStamina( value ) ); }
	}

	public delegate void FValueChanged( float delta );
	public event FValueChanged StaminaChanged;

	void Update()
	{
	}

	public void SetStamina( float _stamina )
	{
		float prevStamina = m_Stamina;

		m_Stamina = Mathf.Clamp( _stamina, 0.0f, m_MaxStamina );

		if( m_Stamina == prevStamina )
			return;

		OnStaminaChanged( m_Stamina - prevStamina );
	}

	public void ModifyStamina( float _stamina )
	{
		if( _stamina == 0.0f )
			return;

		float prevStamina = m_Stamina;
		float prevStaminaPerc = PercentStamina;

		m_Stamina = Mathf.Clamp( m_Stamina+_stamina, 0f, m_MaxStamina );

		if( m_Stamina == prevStamina )
			return;

		OnStaminaChanged( m_Stamina - prevStamina );
	}

	public float PercentToStamina( float _perc )
	{
		return _perc * m_MaxStamina;
	}

	public void ModifyStaminaPerc( float _perc )
	{
		if( _perc == 0.0f )
			return;

		float deltaStamina = PercentToStamina( _perc );

		ModifyStamina( deltaStamina );
	}

	protected void OnStaminaChanged( float _delta )
	{
		if( null != StaminaChanged )
			StaminaChanged( _delta );

		PlayMakerFSM fsm = GetComponent<PlayMakerFSM>();

		if( null == fsm )
			return;

		fsm.SendEvent( "StaminaChanged" );
	}
}