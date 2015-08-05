using UnityEngine;
using System.Collections;

[USequencerFriendlyName("PlayMaker Set Variable")]
[USequencerEvent("PlayMaker/Set Variable")]
public class USPlayMakerSetVariable : USEventBase 
{	
	public string VariableName;
	public string Value;
	
	public override void FireEvent()
	{	
		if(!AffectedObject)
			return;
		
		PlayMakerFSM fsm = AffectedObject.GetComponent<PlayMakerFSM>();
		
		if( null == fsm )
		{
			Debug.LogError( "USequencer Event: Object has no FSM" );
			return;
		}

		HutongGames.PlayMaker.FsmString var = fsm.FsmVariables.GetFsmString( VariableName );
		var.Value = Value;
	}
	
	public override void ProcessEvent(float deltaTime)
	{	
	}
	
	public override void StopEvent()
	{
		UndoEvent();
	}
	
	public override void UndoEvent()
	{
	}
}