using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("StarBloom")]
[Tooltip("Settings for the NPC panel")]
public class SetNPCPanel : FsmStateAction
{
	[RequiredField]
	[Tooltip("Whether the NPC panel is visible")]		
	public FsmBool npcPanelVisible;
	
	[RequiredField]
	[Tooltip("The color of the background when fully faded in")]		
	public FsmColor backgroundColor;
	
	[Tooltip("The name of the NPC sprite")]		
	public FsmString npcSpriteName = "Invalid";
	
	[RequiredField]
	[Tooltip("Color modification to the NPC sprite")]		
	public FsmColor npcSpriteColor;

	[RequiredField]
	[Tooltip("The position of the npc on the screen")]		
	public FsmVector3 npcSpritePosition = Vector3.zero;

	///
	/// Temporarily Disabled - PENDING TESTING
	/// 
	[RequiredField]
	[Tooltip("The scale of the npc on the screen")]		
	protected FsmVector3 npcScale = Vector3.one;


	[RequiredField]
	[Tooltip("The fade in time for the background")]		
	public FsmFloat backgroundFadeInTime = 0.8f;
	
	[RequiredField]
	[Tooltip("The fade out time for the background")]		
	public FsmFloat backgroundFadeOutTime = 0.8f;
	
	[RequiredField]
	[Tooltip("The fade in time for the npc sprite")]		
	public FsmFloat npcFadeInTime = 0.4f;
	
	[RequiredField]
	[Tooltip("The fade out time for the npc sprite")]		
	public FsmFloat npcFadeOutTime = 0.4f;

	[Tooltip("Event fired when the background panel has started fading in")]		
	public FsmEvent backgroundFadeInStarted;

	[Tooltip("Event fired when the background panel has faded in")]		
	public FsmEvent backgroundFadedIn;

	[Tooltip("Event fired when the background panel has started fading out")]		
	public FsmEvent backgroundFadeOutStarted;
	
	[Tooltip("Event fired when the background panel has faded out")]		
	public FsmEvent backgroundFadedOut;

	[Tooltip("Event fired when the npc sprite has started fading in")]		
	public FsmEvent npcFadeInStarted;

	[Tooltip("Event fired when the npc sprite has faded in")]		
	public FsmEvent npcFadedIn;

	[Tooltip("Event fired when the npc sprite has started fading out")]		
	public FsmEvent npcFadeOutStarted;

	[Tooltip("Event fired when the npc sprite has faded out")]		
	public FsmEvent npcFadedOut;

	
	protected dfTweenFloat m_BGFadeIn;
	protected dfTweenFloat m_BGFadeOut;
	protected dfPanel m_BGPanel;
	protected dfSprite m_NPCSprite;
	protected dfTweenFloat m_NPCSpriteFadeIn;
	protected dfTweenFloat m_NPCSpriteFadeOut;
	
	public override void Reset ()
	{
		npcPanelVisible = null;
		backgroundColor = null;
		npcSpriteName = null;
		npcSpriteColor = null;
		npcSpritePosition = null;
		npcScale = null;
		backgroundFadeInTime = null;
		backgroundFadeOutTime = null;
		npcFadeInTime = null;
		npcFadeOutTime = null;
		backgroundFadeInStarted = null;
		backgroundFadedIn = null;
		backgroundFadeOutStarted = null;
		backgroundFadedOut = null;
		npcFadeInStarted = null;
		npcFadedIn = null;
		npcFadeOutStarted = null;
		npcFadedOut = null;
	}

	public override void OnEnter()
	{
		GameObject GUI = GameObject.Find( "GUI" );
		Transform npcPanelTransform = GUI.transform.Find( "NPCPanel" );
		dfPanel m_BGPanel = npcPanelTransform.GetComponent<dfPanel>();

		// Set bg info
		Transform bg = npcPanelTransform.Find( "BG" );
		dfSprite bgSprite = bg.GetComponent<dfSprite>();
		bgSprite.Color = backgroundColor.Value;
		bgSprite.Opacity = backgroundColor.Value.a;

		// Set npc sprite info
		Transform npc = npcPanelTransform.Find( "NPC" );
		m_NPCSprite = npc.GetComponent<dfSprite>();

		m_NPCSprite.SpriteName = npcSpriteName.Value;

		// If this sprite is invalid...
		if( null == m_NPCSprite.SpriteInfo )
			m_NPCSprite.SpriteName = "Invalid";

		m_NPCSprite.Width = m_NPCSprite.SpriteInfo.sizeInPixels.x;
		m_NPCSprite.Height = m_NPCSprite.SpriteInfo.sizeInPixels.y;
		m_NPCSprite.Color = npcSpriteColor.Value;
		
		npc.transform.position = Vector3.zero;

		if( null != npcScale )
			npc.transform.localScale = npcScale.Value;

		m_NPCSprite.RelativePosition = npcSpritePosition.Value;
		
		// Get the background fade tweens
		dfTweenFloat[] bgTweens = npcPanelTransform.GetComponents<dfTweenFloat>();

		foreach( dfTweenFloat tween in bgTweens )
		{
			if( tween.TweenName == "NPCPanelFadeIn" )
			{
				m_BGFadeIn = tween;
				m_BGFadeIn.Length = backgroundFadeInTime.Value;
			}
			else if( tween.TweenName == "NPCPanelFadeOut" )
			{
				m_BGFadeOut = tween;
				m_BGFadeOut.Length = backgroundFadeOutTime.Value;
			}
		}

		// Get the NPC fade tweens
		dfTweenFloat[] npcTweens = npc.GetComponents<dfTweenFloat>();
		
		foreach( dfTweenFloat tween in npcTweens )
		{
			if( tween.TweenName == "NPCFadeIn" )
			{
				m_NPCSpriteFadeIn = tween;
				m_NPCSpriteFadeIn.Length = npcFadeInTime.Value;
			}
			else if( tween.TweenName == "NPCFadeOut" )
			{
				m_NPCSpriteFadeOut = tween;
				m_NPCSpriteFadeOut.Length = npcFadeOutTime.Value;
			}
		}

		// Handle showing or hiding the panel
		if( npcPanelVisible.Value )
		{
			if( !m_BGPanel.IsVisible )
			{
				m_BGPanel.IsVisible = true;
				m_BGFadeIn.Reset();
				m_BGFadeIn.Play();
				m_BGFadeIn.TweenStarted += HandleBGFadeInStarted;
				m_BGFadeIn.TweenCompleted += HandleBGFadeInCompleted;
				m_NPCSpriteFadeIn.TweenStarted += HandleNPCFadeInStarted;
				m_NPCSpriteFadeIn.TweenCompleted += HandleNPCFadeInCompleted;
			}
		}
		else
		{
			if( m_BGPanel.IsVisible )
			{
				m_BGFadeOut.Reset();
				m_BGFadeOut.Play();
				m_BGFadeOut.TweenStarted += HandleBGFadeOutStarted;
				m_BGFadeOut.TweenCompleted += HandleBGFadeOutCompleted;
				m_NPCSpriteFadeOut.TweenStarted += HandleNPCFadeOutStarted;
				m_NPCSpriteFadeOut.TweenCompleted += HandleNPCFadeOutCompleted;
			}
		}

		Finish();
	}

	void HandleBGFadeInStarted( dfTweenPlayableBase sender )
	{
		if( null != backgroundFadeInStarted )
			Fsm.Event( backgroundFadeInStarted );

		m_BGFadeIn.TweenStarted -= HandleBGFadeInStarted;
	}

	void HandleBGFadeInCompleted( dfTweenPlayableBase sender )
	{
		if( null != backgroundFadedIn )
			Fsm.Event( backgroundFadedIn );

		m_BGFadeIn.TweenCompleted -= HandleBGFadeInCompleted;
	}

	void HandleBGFadeOutStarted( dfTweenPlayableBase sender )
	{
		if( null != backgroundFadeOutStarted )
			Fsm.Event( backgroundFadeOutStarted );
		
		m_BGFadeOut.TweenStarted -= HandleBGFadeOutStarted;
	}
	
	void HandleBGFadeOutCompleted( dfTweenPlayableBase sender )
	{
		if( null != backgroundFadedOut )
			Fsm.Event( backgroundFadedOut );

		GameObject GUI = GameObject.Find( "GUI" );
		Transform npcPanelTransform = GUI.transform.Find( "NPCPanel" );
		dfPanel m_BGPanel = npcPanelTransform.GetComponent<dfPanel>();

		if( null != m_BGPanel )
			m_BGPanel.IsVisible = false;

		m_BGFadeOut.TweenCompleted -= HandleBGFadeOutCompleted;
	}
	
	void HandleNPCFadeInStarted (dfTweenPlayableBase sender)
	{
		if( null != npcFadeInStarted )
			Fsm.Event( npcFadeInStarted );

		if( null != m_NPCSprite )
			m_NPCSprite.IsVisible = true;

		m_NPCSpriteFadeIn.TweenStarted -= HandleNPCFadeInStarted;
	}
	
	void HandleNPCFadeInCompleted (dfTweenPlayableBase sender)
	{
		if( null != npcFadedIn )
			Fsm.Event( npcFadedIn );
		
		m_NPCSpriteFadeIn.TweenCompleted -= HandleNPCFadeInCompleted;
	}
	
	void HandleNPCFadeOutStarted (dfTweenPlayableBase sender)
	{
		if( null != npcFadeOutStarted )
			Fsm.Event( npcFadeOutStarted );
		
		m_NPCSpriteFadeOut.TweenStarted -= HandleNPCFadeOutStarted;
	}
	
	void HandleNPCFadeOutCompleted (dfTweenPlayableBase sender)
	{
		if( null != npcFadedOut )
			Fsm.Event( npcFadedOut );

		if( null != m_NPCSprite )
			m_NPCSprite.IsVisible = false;

		m_NPCSpriteFadeOut.TweenCompleted -= HandleNPCFadeInCompleted;
	}
}