using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("StarBloom")]
[Tooltip("Settings for the decision panel")]
public class SetDecisionPanel : FsmStateAction
{
	[Tooltip("The name of background sprite")]		
	public FsmString backgroundSprite = "Invalid";
	
	[Tooltip("The color modifier of the panel")]		
	public FsmColor backgroundColor;
	
	[Tooltip("The text color when in the normal state")]		
	public FsmColor normalTextColor;

	[Tooltip("The text color when in the hover state")]		
	public FsmColor hoverTextColor;

	[Tooltip("The text color when in the focused state")]		
	public FsmColor focusTextColor;

	[Tooltip("The text color when in the pressed state")]		
	public FsmColor pressedTextColor;

	[Tooltip("The text color when in the disabled state")]		
	public FsmColor disabledTextColor;
	
	dfLabel m_ChatText;
	
	public override void Reset ()
	{
		backgroundSprite = null;
		backgroundColor = null;
		normalTextColor = null;
		hoverTextColor = null;
		focusTextColor = null;
		pressedTextColor = null;
		disabledTextColor = null;
	}
	
	public override void OnEnter()
	{
		GameObject GUI = GameObject.Find( "GUI" );
		Transform decisionPanelTransform = GUI.transform.Find( "DecisionPanel" );
		Transform optionPanelTransform = decisionPanelTransform.transform.Find( "Options" );
		UIDecisionBinding binding = optionPanelTransform.GetComponent<UIDecisionBinding>();
		
		// Set bg info

		if( null != backgroundColor )
			binding.SetBackgroundColor( backgroundColor.Value );

		binding.SetOptionTextColor( normalTextColor.Value, hoverTextColor.Value, focusTextColor.Value, pressedTextColor.Value, disabledTextColor.Value );

		if( null != backgroundSprite )
			binding.SetBackgroundSprite( backgroundSprite.Value );
		
		Finish();
	}
}