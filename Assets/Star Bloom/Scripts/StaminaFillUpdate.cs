using UnityEngine;
using System.Collections;

public class StaminaFillUpdate : MonoBehaviour
{
	protected GameObject m_Player;
	protected PlayerFatigue m_StaminaScript;
	protected dfTweenFloat m_StaminaFill;

	// Use this for initialization
	void Awake()
	{
		m_Player = GameObject.Find( "Player" );
		m_StaminaScript = m_Player.GetComponent<PlayerFatigue>();
		m_StaminaFill = GetComponent<dfTweenFloat>();

		m_StaminaScript.StaminaChanged += HandleStaminaChanged;

		HandleStaminaChanged( 0.0f );
	}

	void HandleStaminaChanged( float delta )
	{
		m_StaminaFill.EndValue = m_StaminaScript.PercentStamina;
		m_StaminaFill.Play();
	}
}
