using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("StarBloom")]
[Tooltip("Receives and handles Dialogue events from a DialogueActor")]
public class DialogueEventHandler : FsmStateAction
{
	[RequiredField]
	[Tooltip("Object with a DialogActor component")]		
	public FsmOwnerDefault dialogueObject;

	[Tooltip("Sent when the dialogue starts")]		
	public FsmEvent dialogueStarted;
	
	[Tooltip("Sent when a dialogue is interacted with / continued")]		
	public FsmEvent dialogueContinued;

	[Tooltip("Sent when the dialogue finishes")]		
	public FsmEvent dialogueFinished;

	protected DialogueActor m_Actor;

	public override void Reset ()
	{
		dialogueObject = null;
		dialogueStarted = null;
		dialogueContinued = null;
		dialogueFinished = null;
		m_Actor = null;
	}

	public override void OnEnter()
	{
		m_Actor = Fsm.GetOwnerDefaultTarget(dialogueObject).GetComponent<DialogueActor>();

		if( null != dialogueStarted )
			m_Actor.m_DialogStartedEvent = dialogueStarted;

		if( null != dialogueContinued )
			m_Actor.m_DialogContinuedEvent = dialogueContinued;

		if( null != dialogueFinished )
			m_Actor.m_DialogCompleteEvent = dialogueFinished;

		Finish();
	}
}