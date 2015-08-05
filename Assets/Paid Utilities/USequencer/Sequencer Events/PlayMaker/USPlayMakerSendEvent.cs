using UnityEngine;
using System.Collections;

[USequencerFriendlyName("PlayMaker Send Event")]
[USequencerEvent("PlayMaker/Send Event")]
public class USPlayMakerSendEvent : USEventBase 
{	
	public string PlayMakerEvent;
	
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

		fsm.SendEvent( PlayMakerEvent );
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