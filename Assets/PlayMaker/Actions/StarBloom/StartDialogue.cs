using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("StarBloom")]
[Tooltip("Starts a dialogue with a dialog actor")]
public class StartDialogue : FsmStateAction
{
	[RequiredField]
	[CheckForComponent(typeof(DialogueActor))]
	[Tooltip("Object with a DialogActor component")]		
	public FsmOwnerDefault dialogueObject;

	[RequiredField]
	[Tooltip("Object speaking to the actor")]		
	public FsmOwnerDefault talker;

	[RequiredField]
	[Tooltip("Dialogue graph to use")]		
	public FsmString graph;
	
	[Tooltip("Changes the dialogue display type (narrative doesn't follow the entity)")]		
	public FsmBool isNarrative;
	
	[Tooltip("Whether the player is automatically locked")]		
	public FsmBool lockPlayer;
	
	[Tooltip("Whether the player is automatically released at the end of the conversation")]		
	public FsmBool autoUnlockPlayer;
	
	[Tooltip("Whether the npc will turn to face the talker")]		
	public FsmBool facesTalker;

	[Tooltip("Whether the npc will turn back to its original orientation")]		
	public FsmBool restoreFacing;

	[Tooltip("Shows or hides the nameplate")]		
	public FsmBool hasNameplate;

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
		graph = null;
		isNarrative = null;
		lockPlayer = null;
		autoUnlockPlayer = null;
		facesTalker = null;
		restoreFacing = null;
		dialogueStarted = null;
		dialogueContinued = null;
		dialogueFinished = null;
		m_Actor = null;
	}

	public override void OnEnter()
	{
		m_Actor = Fsm.GetOwnerDefaultTarget(dialogueObject).GetComponent<DialogueActor>();

		m_Actor.m_Speaker = Fsm.GameObject;
		m_Actor.m_Talker = talker.GameObject.Value;
		m_Actor.m_IsNarrative = isNarrative.Value;
		m_Actor.m_FacesTalker = facesTalker.Value;
		m_Actor.m_RestoreFacing = restoreFacing.Value;
		m_Actor.m_AutoUnlockPlayer = autoUnlockPlayer.Value;

		if( lockPlayer.Value )
		{
			GameObject.Find( "Player" ).GetComponent<PlayerMovementController>().SetDisabled( true );
		}

		m_Actor.m_WaitingForAnim = false;
		m_Actor.m_WaitingForCam = false;

		m_Actor.m_ConversationName = graph.Value;

		if( null != hasNameplate )
			m_Actor.m_HasNameplate = hasNameplate.Value;

		if( null != dialogueStarted )
			m_Actor.m_DialogStartedEvent = dialogueStarted;
		
		if( null != dialogueContinued )
			m_Actor.m_DialogContinuedEvent = dialogueContinued;
		
		if( null != dialogueFinished )
			m_Actor.m_DialogCompleteEvent = dialogueFinished;

		m_Actor.StartTalking();

		Finish();
	}
}