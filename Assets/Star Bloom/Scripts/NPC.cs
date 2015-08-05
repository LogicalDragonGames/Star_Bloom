using UnityEngine;
using System.Collections;

public class NPC : DialogueActor
{
	public Relationship m_Relationship;
	public GameObject m_Player;
	
	public string m_NPCName = "";
	public int LoseFocusSqDist = 1;
	public float LoseFocusAngle = 20;

	private Animator anim = null;

	protected IsometricCamera m_Camera;
	protected PlayerTool m_LastInteractedTool = null;

	// Use this for initialization
	new void Start()
	{
		base.Start();

		anim = GetComponent<Animator>();

		InteractionHandler ih = GetComponent<InteractionHandler>();

		if( ih )
			ih.InteractionOccurred += OnInteraction;
		else
			Debug.LogError( "Could not find 'InteractionHandler' component" );

		m_Player = GameObject.FindGameObjectWithTag( "Player" );
		m_Camera = m_Player.GetComponent<IsometricCamera>();
	}

	public override void StartTalking()
	{
		if( anim )
			anim.SetBool( "Talking", true );
		
		LockPlayer();

		base.StartTalking();
	}

	public override void StopTalking()
	{
		if( anim )
			anim.SetBool( "Talking", false );
		
		UnlockPlayer();

		base.StopTalking();
	}

	protected void LockPlayer()
	{
		m_Player.GetComponent<PlayerMovementController>().SetDisabled( true );
	}

	protected void UnlockPlayer()
	{
		m_Player.GetComponent<PlayerMovementController>().SetDisabled( false );
	}

	public void OnInteraction( PlayerTool tool )
	{
		// Ignore interactions while talking
		if( m_IsTalking || m_WaitingForAnim || m_WaitingForCam )
			return;

		m_LastInteractedTool = tool;

		PlayMakerFSM fsm = GetComponent<PlayMakerFSM>();

		if( null == fsm )
			return;
		
		if( m_LastInteractedTool.GetType() == typeof(HandTool) )
			fsm.SendEvent( "HAND_TOOL_USED" );
		else
			fsm.SendEvent( "INTERACTED" );
	}

	public void GetHeldItem()
	{
		PlayMakerFSM fsm = GetComponent<PlayMakerFSM>();
		
		if( null != fsm )
		{
			string heldItem = "";

			if( m_LastInteractedTool.GetType() == typeof(HandTool) )
				heldItem = (m_LastInteractedTool as HandTool).GetHeldItem();

			fsm.FsmVariables.GetFsmString( "HELD_ITEM" ).Value = heldItem;
		}
	}

	public void AddRelationshipProgress( float add )
	{
		m_Relationship.AddProgress( add );
		RelationshipManager.Instance.UpdateRelationship( m_NPCName, m_Relationship );
	}

	void SetCamTransition()
	{
		Matrix4x4 mat = m_Speaker.transform.localToWorldMatrix;
		Vector3 offset = mat.MultiplyPoint3x4( m_InteractCamOffset );
		Quaternion rot = m_Speaker.transform.rotation;
		rot = rot * Quaternion.Euler( m_InteractCamRotation );
		m_Camera.SetSecondaryTarget( offset, rot.eulerAngles );
		m_Camera.SecondaryTransitionCompleted += OnCamTransitionFinished;
		
		m_WaitingForCam = true;
	}
	
	void OnCamTransitionFinished()
	{
		/*m_Camera.SecondaryTransitionCompleted -= OnCamTransitionFinished;
		m_WaitingForCam = false;

		ContinueTalking();*/
	}
}