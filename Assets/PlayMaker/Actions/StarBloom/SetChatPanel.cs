using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("StarBloom")]
[Tooltip("Settings for the chat panel")]
public class SetChatPanel : FsmStateAction
{
	[Tooltip("The name of background sprite")]		
	public FsmString backgroundSprite = "Invalid";

	[Tooltip("The color modifier of the chat panel")]		
	public FsmColor backgroundColor;

	dfLabel m_ChatText;

	public override void Reset ()
	{
		backgroundSprite = null;
		backgroundColor = null;
	}

	public override void OnEnter()
	{
		GameObject GUI = GameObject.Find( "GUI" );
		Transform chatPanelTransform = GUI.transform.Find( "ChatPanel" );
		Transform textTransform = chatPanelTransform.transform.Find( "Text" );
		m_ChatText = textTransform.GetComponent<dfLabel>();

		// Set bg info
		if( null != backgroundSprite )
			m_ChatText.BackgroundSprite = backgroundSprite.Value;

		if( null != backgroundColor )
			m_ChatText.BackgroundColor = backgroundColor.Value;

		Finish();
	}
}