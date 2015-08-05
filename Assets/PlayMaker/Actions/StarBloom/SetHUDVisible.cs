using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("StarBloom")]
[Tooltip("Sets the visibility of the game HUD")]
public class SetHUDVisible : FsmStateAction
{
	[RequiredField]
	[Tooltip("Whether the HUD is visible")]		
	public FsmBool hudVisible;

	public override void Reset ()
	{
		hudVisible = null;
	}

	public override void OnEnter()
	{
		GameObject GUI = GameObject.Find( "GUI" );
		Transform HUD = GUI.transform.Find( "HUD" );
		dfPanel hudPanel = HUD.GetComponent<dfPanel>();

		hudPanel.IsVisible = hudVisible.Value;

		Finish();
	}
}