using UnityEngine;
using System.Collections;

public class MenuScreenUpdater : MonoBehaviour
{
	protected bool m_IsMenuActive = false;
	protected dfPanel m_MenuPanel;
	protected dfTweenFloat m_FadeIn;
	protected dfTweenFloat m_FadeOut;
	protected PlayerMovementController m_PlayerController;
	protected bool m_SceneTimePaused = false;

	// Use this for initialization
	void Start()
	{
		m_MenuPanel = GetComponent<dfPanel>();
		if( null == m_MenuPanel )
		{
			Debug.LogError( "Failed to find 'dfPanel' on the in-game menu" );
			return;
		}

		dfTweenFloat[] tweens = GetComponents<dfTweenFloat>();

		foreach( dfTweenFloat tween in tweens )
		{
			if( tween.TweenName == "MenuFadeIn" )
				m_FadeIn = tween;
			else if( tween.TweenName == "MenuFadeOut" )
				m_FadeOut = tween;
		}
		
		if( null == m_FadeIn )
		{
			Debug.LogError( "Failed to find 'dfTweenFloat' MenuFadeIn on the in-game menu" );
			return;
		}

		if( null == m_FadeOut )
		{
			Debug.LogError( "Failed to find 'dfTweenFloat' MenuFadeIn on the in-game menu" );
			return;
		}

		m_FadeIn.TweenStarted += HandleFadeInStarted;
		m_FadeOut.TweenCompleted += HandleFadeOutCompleted;

		GameObject player = GameObject.Find( "Player" );
		if( null == player )
		{
			Debug.LogError( "Failed to find object marked with 'Player' tag" );
			return;
		}

		m_PlayerController = player.GetComponent<PlayerMovementController>();
		if( null == m_PlayerController )
		{
			Debug.LogError( "Failed to find component 'PlayerMovementController' attached to player" );
			return;
		}

		m_SceneTimePaused = WorldTime.Instance.TimePaused;
	}

	void HandleFadeInStarted (dfTweenPlayableBase sender)
	{
		m_MenuPanel.IsVisible = true;
	}

	void HandleFadeOutCompleted (dfTweenPlayableBase sender)
	{
		m_MenuPanel.IsVisible = false;
	}

	// Update is called once per frame
	void Update()
	{
		if( Input.GetButtonDown( "Menu" ) )
		{
			if( m_IsMenuActive )
				HideMenu();
			else
				ShowMenu();
		}
	}

	void HideMenu()
	{
		if( m_FadeIn.IsPlaying )
			m_FadeIn.Stop();

		m_FadeOut.Play();
		m_IsMenuActive = false;

		WorldTime.Instance.TimePaused = m_SceneTimePaused;
		m_PlayerController.SetTimedDisable( 0.5f );
	}

	void ShowMenu()
	{
		if( m_FadeOut.IsPaused )
			m_FadeOut.Stop();

		m_FadeIn.Play();
		m_IsMenuActive = true;

		WorldTime.Instance.TimePaused = true;
		m_PlayerController.SetDisabled();
	}
}
