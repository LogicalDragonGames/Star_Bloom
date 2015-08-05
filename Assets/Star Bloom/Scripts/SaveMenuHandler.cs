using UnityEngine;
using System.Collections;

public class SaveMenuHandler : MonoBehaviour 
{
	public dfTweenFloat m_ShowTween;
	public dfTweenFloat m_HideTween;

	public dfPanel m_SaveDialog;
	protected SaveDialogEventHandler m_SDScript;
	protected dfPanel m_Panel;

	void Start()
	{
		m_Panel = GetComponent<dfPanel>();
		m_SDScript = m_SaveDialog.GetComponent<SaveDialogEventHandler>();

		m_ShowTween.TweenStarted += HandleShowStarted;
		m_ShowTween.TweenCompleted += HandleShowCompleted;
		m_HideTween.TweenStarted += HandleHideStarted;
		m_HideTween.TweenCompleted += HandleHideCompleted;
	}

	void OnDestroy()
	{
		m_ShowTween.TweenStarted -= HandleShowStarted;
		m_ShowTween.TweenCompleted -= HandleShowCompleted;
		m_HideTween.TweenStarted -= HandleHideStarted;
		m_HideTween.TweenCompleted -= HandleHideCompleted;
	}

	void HandleShowStarted (dfTweenPlayableBase sender)
	{
		m_Panel.IsVisible = true;
	}
	
	void HandleShowCompleted (dfTweenPlayableBase sender)
	{
	}

	void HandleHideStarted (dfTweenPlayableBase sender)
	{
	}

	void HandleHideCompleted (dfTweenPlayableBase sender)
	{
		m_Panel.IsVisible = false;
	}

	public void Update()
	{
		if( m_Panel.IsVisible )
		{
			if( Input.GetButtonDown( "Back" ) && !m_SDScript.WasVisible )
			{
				HideMenu();
			}
		}
	}

	public void ShowMenu()
	{
		if( m_Panel.IsVisible )
			return;

		m_ShowTween.Play();
	}

	public void HideMenu()
	{
		if( !m_Panel.IsVisible )
			return;

		if( m_HideTween.IsPlaying )
			return;

		if( m_ShowTween.IsPlaying )
			return;

		m_HideTween.Play();
	}
}
