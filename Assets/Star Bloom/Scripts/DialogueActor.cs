using UnityEngine;
using System.Collections;
using DiaQ;

[System.Serializable]
public class DialogueActor : MonoBehaviour
{
	public bool m_IsNarrative = false;
	public bool m_HasNameplate = true;
	public string m_ActorName = "";

	[HideInInspector]
	public string m_ConversationName = "";

	[HideInInspector]
	public HutongGames.PlayMaker.FsmEvent m_DialogStartedEvent;

	[HideInInspector]
	public HutongGames.PlayMaker.FsmEvent m_DialogCompleteEvent;

	[HideInInspector]
	public HutongGames.PlayMaker.FsmEvent m_DialogContinuedEvent;

	[HideInInspector]
	public GameObject m_Talker;

	public bool m_FacesTalker = true;
	public bool m_RestoreFacing = true;
	public float m_TurnDuration = 0.5f;

	public Interpolate.EaseType m_TurningEaseType = Interpolate.EaseType.Linear;

	public GameObject m_LastKnownInteractor;
	public Vector3 m_LastKnownInteractorPosition;
	public Quaternion m_LastKnownInteractorRotation;

	public Vector3 m_InteractCamOffset = Vector3.zero;
	public Vector3 m_InteractCamRotation = Vector3.zero;
	public Vector3 m_ChatOffset = Vector3.zero;

	[HideInInspector]
	public bool m_AutoUnlockPlayer = false;

	[HideInInspector]
	public bool m_WaitingForCam = false;

	[HideInInspector]
	public bool m_WaitingForAnim = false;

	protected dfTweenVector3 m_CurGUIAnim;

	protected bool m_IsTalking = false;
	protected bool m_IsTurning = false;
	protected bool m_TurningToTalker = true;
	protected Vector3 m_ToTalkerRotation = Vector3.zero;
	protected Vector3 m_OriginalRotation = Vector3.zero;
	protected float m_TurnTime = 0.0f;

	protected GameObject m_GUI;
	protected dfPanel m_ChatPanel;
	protected UIDecisionBinding m_ChatDecisions;
	protected dfFollowObject m_ChatFollower;
	protected dfPanel m_NameplatePanel;
	protected dfLabel m_ChatNameplate;
	protected dfLabel m_ChatText;
	protected dfTweenVector3 m_ChatZoomIn;
	protected dfTweenVector3 m_ChatZoomOut;
	protected dfTweenVector2 m_ChatContinueIn;
	protected dfTweenVector2 m_ChatContinueOut;
	protected DiaQConversation m_Conversation = null;
	protected bool m_HasDecisionDelegateHooks = false;
	protected bool m_OptionSelected = false;
	protected bool m_FirstDialogueNode = true;
	protected string m_NameplateName = "";
	protected string m_TranslatedDialogue = "";

	[HideInInspector]
	public GameObject m_Speaker;

	public void Start()
	{
		m_GUI = GameObject.Find( "GUI" );
		if( null == m_GUI )
		{
			Debug.LogError( "Could not find 'GUI' object" );
			return;
		}
		
		Transform chatPanel = m_GUI.transform.Find( "ChatPanel" );
		if( null == chatPanel )
		{
			Debug.LogError( "Could not find 'ChatPanel' object" );
			return;
		}
		
		m_ChatPanel = chatPanel.GetComponent<dfPanel>();
		if( null == m_ChatPanel )
		{
			Debug.LogError( "Could not find 'dfPanel' component" );
			return;
		}
		
		dfTweenVector3[] v3Tweens = chatPanel.GetComponents<dfTweenVector3>();
		
		foreach( dfTweenVector3 tween in v3Tweens )
		{
			if( tween.TweenName == "ZoomIn" )
				m_ChatZoomIn = tween;
			else if( tween.TweenName == "ZoomOut" )
				m_ChatZoomOut = tween;
		}
		
		if( null == m_ChatZoomIn )
		{
			Debug.LogError( "Could not find 'dfTweenVector3' ZoomIn component" );
			return;
		}
		
		if( null == m_ChatZoomOut )
		{
			Debug.LogError( "Could not find 'dfTweenVector3' ZoomOut component" );
			return;
		}
		
		m_ChatFollower = m_ChatPanel.GetComponent<dfFollowObject>();
		if( null == m_ChatFollower )
		{
			Debug.LogError( "Could not find 'dfFollowObject' component" );
			return;
		}
		
		Transform textObj = chatPanel.Find( "Text" );
		if( null == textObj )
		{
			Debug.LogError( "Could not find 'Text' object" );
			return;
		}
		
		m_ChatText = textObj.GetComponent<dfLabel>();
		if( null == m_ChatText )
		{
			Debug.LogError( "Could not find 'dfLabel' component" );
			return;
		}

		dfTweenVector2[] fTweens = textObj.GetComponents<dfTweenVector2>();
		
		foreach( dfTweenVector2 tween in fTweens )
		{
			if( tween.TweenName == "ContinueIn" )
				m_ChatContinueIn = tween;
			else if( tween.TweenName == "ContinueOut" )
				m_ChatContinueOut = tween;
		}
		
		Transform nameplate = chatPanel.Find( "Nameplate" );
		if( null == nameplate )
		{
			Debug.LogError( "Could not find 'Nameplate' object" );
			return;
		}

		m_NameplatePanel = nameplate.GetComponent<dfPanel>();
		if( null == nameplate )
		{
			Debug.LogError( "Could not find 'Nameplate' panel object" );
			return;
		}

		Transform nameplateName = nameplate.Find( "Name" );
		if( null == nameplateName )
		{
			Debug.LogError( "Could not find 'Name' object" );
			return;
		}
		
		m_ChatNameplate = nameplateName.GetComponent<dfLabel>();
		if( null == m_ChatNameplate )
		{
			Debug.LogError( "Could not find 'dfLabel' component" );
			return;
		}

		m_ChatDecisions = m_GUI.transform.Find( "DecisionPanel" ).Find("Options").GetComponent<UIDecisionBinding>();
	}

	// Update is called once per frame
	void Update()
	{
		if( m_IsTurning )
		{
			m_TurnTime = Mathf.Clamp( m_TurnTime + Time.deltaTime, 0.0f, m_TurnDuration );
			
			Interpolate.Function ease = Interpolate.Ease( m_TurningEaseType );
			
			if( m_TurningToTalker )
				transform.rotation = Quaternion.Euler( Interpolate.Ease( ease, m_OriginalRotation, m_ToTalkerRotation-m_OriginalRotation, m_TurnTime, m_TurnDuration ) );
			else
				transform.rotation = Quaternion.Euler( Interpolate.Ease( ease, m_ToTalkerRotation, m_OriginalRotation-m_ToTalkerRotation, m_TurnTime, m_TurnDuration ) );
			
			if( m_TurnTime >= m_TurnDuration )
				m_IsTurning = false;
		}
		
		if( m_IsTalking )
		{
			// Stop talking if player moves
			/*if( (m_LastKnownInteractor.transform.position - m_LastKnownInteractorPosition).sqrMagnitude > LoseFocusSqDist ||
			   !m_LastKnownInteractor.transform.rotation.AlmostEquals( m_LastKnownInteractorRotation, LoseFocusAngle ) )
			{

			}*/
			
			if( !m_WaitingForAnim && !m_WaitingForCam )
			{
				if( Input.GetButtonDown( "Interact" ) || m_OptionSelected )
					ContinueTalking();

				m_OptionSelected = false;
			}

		}
	}

	string TranslateMessageArgs( string message )
	{
		if( message == "" )
			return message;
		
		string translated = message;
		string playerName = "NAME_UNKNOWN";
		string hourMinTime = WorldTime.Instance.GetHourMinuteTime();
		
		GameObject player = GameObject.FindGameObjectWithTag( "Player" );
		
		if( player )
		{
			UserDefinitions userDefs = player.GetComponent<UserDefinitions>();
			
			if( userDefs )
				playerName = userDefs.PlayerName;
			else
				Debug.LogError( "Could not find 'UserDefinitions' component" );
		}
		else
			Debug.LogError( "Could not find 'Player' entity" );
		
		translated = translated.Replace( "{{PLAYER}}", playerName );
		translated = translated.Replace( "{{TIME}}", hourMinTime );
		translated = translated.Replace( "{{NAME}}", m_ActorName );
		
		return translated;
	}

	public virtual void StartTalking()
	{
		if( m_IsTalking )
			return;
		
		PlayMakerFSM fsm = m_Speaker.GetComponent<PlayMakerFSM>();
		
		if( fsm == null )
			return;

		m_LastKnownInteractor = m_Speaker;
		m_LastKnownInteractorPosition = m_Speaker.transform.position;
		m_LastKnownInteractorRotation = m_Speaker.transform.rotation;
		m_IsTalking = true;

		if( m_FacesTalker )
			TurnToTalker();
		
		ShowChatBubble( "" );
		//SetCamTransition();
		
		fsm.Fsm.Event( m_DialogStartedEvent );

		// Enter the first dialog segment
		ContinueTalking();
	}
	
	public virtual void ContinueTalking()
	{
		if( m_WaitingForCam )
			return;

		bool hadChoices = false;

		// First dialog
		if( null == m_Conversation )
		{
			m_FirstDialogueNode = true;
			
			m_Conversation = DiaQEngine.Instance.StartGraph( m_ConversationName, false );
		}
		// Otherwise continue the conversation
		else
		{
			m_FirstDialogueNode = false;
			
			if( null == m_Conversation.choices )
			{
				DiaQEngine.Instance.EndGraph();
				m_Conversation = null;
			}
			else if( m_Conversation.choices.Length == 1 )
			{
				m_Conversation = DiaQEngine.Instance.SubmitChoice( 0 );
			}
			else
			{
				int optIndex = m_ChatDecisions.GetCurrentOptionIndex();

				if( (-1) == optIndex )
					return;				// Don't continue until an option has been selected

				hadChoices = true;
				m_Conversation = DiaQEngine.Instance.SubmitChoice( optIndex );
			}
		}

		bool hasChoices = m_Conversation != null && m_Conversation.choices != null && m_Conversation.choices.Length > 1;

		if( hasChoices )
			m_ChatDecisions.SetOptions( m_Conversation.choices );

		if( !hadChoices && hasChoices  )
		{
			m_ChatDecisions.Show();

			if( !m_HasDecisionDelegateHooks )
				m_ChatDecisions.OptionSelected += OnChatOptionSelected;
		}
		else if( hadChoices && !hasChoices )
		{
			if( m_HasDecisionDelegateHooks )
				m_ChatDecisions.OptionSelected -= OnChatOptionSelected;

			m_ChatDecisions.Hide();
		}
		
		if( null == m_Conversation )
			StopTalking();
		else
			SetChatBubbleMessage( TranslateMessageArgs( m_Conversation.conversationText ), !m_FirstDialogueNode ); // Don't play anim on first message
	}

	void OnChatOptionSelected ()
	{
		m_OptionSelected = true;
	}
	
	public virtual void StopTalking()
	{
		m_IsTalking = false;
		m_WaitingForCam = false;

		if( m_AutoUnlockPlayer )
			GameObject.Find( "Player" ).GetComponent<PlayerMovementController>().SetDisabled( false );

		if( null != m_Conversation )
			DiaQEngine.Instance.EndGraph();

		if( m_FacesTalker && m_RestoreFacing )
			TurnToOriginal();
		
		HideChatBubble();
	}
	
	float AngleDir( Vector3 fwd, Vector3 targetDir, Vector3 up )
	{
		Vector3 perp = Vector3.Cross(fwd, targetDir);
		float dir = Vector3.Dot(perp, up);
		
		if( dir >= 0.0f )
			return 1.0f;
		else
			return -1f;
	}
	
	public void TurnToTalker()
	{
		m_IsTurning = true;
		m_TurningToTalker = true;
		m_TurnTime = 0.0f;
		
		m_OriginalRotation = transform.rotation.eulerAngles;
		
		Vector2 actorPos = new Vector2( transform.position.x, transform.position.z );
		Vector2 talkerPos = new Vector2( m_Talker.transform.position.x, m_Talker.transform.position.z );
		
		Vector2 toTalker = (talkerPos - actorPos).normalized;
		Vector2 actorFwd = new Vector2( transform.forward.x, transform.forward.z ).normalized;
		float angle = Vector2.Angle( actorFwd, toTalker );
		
		float angleDir = AngleDir( new Vector3( actorFwd.x, 0.0f, actorFwd.y ), new Vector3( toTalker.x, 0.0f, toTalker.y ), Vector3.up );
		angle *= angleDir;
		
		m_ToTalkerRotation = m_OriginalRotation;
		m_ToTalkerRotation.y += angle;
	}
	
	public void TurnToOriginal()
	{
		m_IsTurning = true;
		m_TurningToTalker = false;
		m_TurnTime = 0.0f;
	}

	void HandleTweenCompleted(dfTweenPlayableBase sender)
	{
		m_WaitingForAnim = false;
		
		if( m_CurGUIAnim != null )
		{
			m_CurGUIAnim.TweenCompleted -= HandleTweenCompleted;

			if( m_CurGUIAnim == m_ChatZoomOut )
			{
				m_CurGUIAnim = null;
				PlayMakerFSM fsm = m_Speaker.GetComponent<PlayMakerFSM>();
				
				if( null != fsm )
					fsm.Fsm.Event( m_DialogCompleteEvent );
			}

			m_CurGUIAnim = null;
		}
	}
	
	public void ShowChatBubble( string message )
	{
		ShowChatBubble( m_ActorName, message );
	}
	
	public void ShowChatBubble( string actorName, string message )
	{
		m_ChatText.AutoSize = true;
		
		m_ChatPanel.Pivot = dfPivotPoint.TopLeft;
		m_ChatPanel.enabled = true;

		if( m_IsNarrative )
		{
			m_ChatFollower.enabled = false;
			m_ChatPanel.transform.position = Vector3.zero;
			m_ChatPanel.transform.localScale = Vector3.zero;
			m_ChatPanel.RelativePosition = m_ChatOffset;
		}
		else
		{
			m_ChatFollower.enabled = true;
			m_ChatFollower.attach = this.gameObject;
			m_ChatFollower.offset = m_ChatOffset;
		}

		if( m_HasNameplate )
		{
			m_NameplatePanel.IsVisible = true;
			m_ChatNameplate.Text = actorName;
		}
		else
		{
			m_NameplatePanel.IsVisible = false;
		}

		if( null != m_CurGUIAnim )
		{
			m_CurGUIAnim.TweenCompleted -= HandleTweenCompleted;
			m_CurGUIAnim = null;
		}

		m_ChatText.Text = message;
		m_ChatPanel.Update();
		m_ChatZoomOut.Reset();
		m_ChatZoomOut.Stop();
		m_ChatZoomIn.Reset();
		m_ChatZoomIn.Play();
		m_CurGUIAnim = m_ChatZoomIn;
		m_CurGUIAnim.TweenCompleted += HandleTweenCompleted;
		m_WaitingForAnim = true;
		m_ChatPanel.IsVisible = true;
		Console.Instance.SendChat( actorName, message );
	}
	
	public void SetChatBubbleMessage( string message, bool useAnim = true )
	{
		SetChatBubbleMessage( m_ActorName, message, useAnim );
	}
	
	public void SetChatBubbleMessage( string npcName, string message, bool useAnim = true )
	{
		m_NameplateName = npcName;
		m_TranslatedDialogue = message;

		if( !useAnim )
		{
			m_ChatNameplate.Text = npcName;
			m_ChatText.Text = message;
		}
		else
		{
			// Set tween start/end values
			// We get our target values by using the DF auto size

			m_ChatContinueOut.StartValue = m_ChatText.Size;
			m_ChatContinueOut.EndValue = new Vector2( m_ChatText.Size.x, m_ChatContinueOut.EndValue.y );
			m_ChatContinueIn.StartValue = m_ChatContinueOut.EndValue;
		
			m_ChatText.AutoSize = true;
			string curText = m_ChatText.Text;
			m_ChatText.Text = message;
			m_ChatText.Update();

			m_ChatContinueIn.EndValue = m_ChatText.Size;

			m_ChatText.Text = curText;
			m_ChatText.Update();
			m_ChatText.AutoSize = false;

			m_ChatText.Size = m_ChatContinueOut.StartValue;

			m_WaitingForAnim = true;
			m_ChatContinueOut.Reset();
			m_ChatContinueOut.Play();
			m_ChatContinueOut.TweenCompleted += HandleChatContinueOutTweenComplete;
		}
	}

	void HandleChatContinueOutTweenComplete( dfTweenPlayableBase sender )
	{
		m_ChatContinueOut.TweenCompleted -= HandleChatContinueOutTweenComplete;

		m_ChatNameplate.Text = m_NameplateName;
		m_ChatText.Text = m_TranslatedDialogue;

		m_ChatContinueIn.Reset();
		m_ChatContinueIn.Play();
		m_ChatContinueIn.TweenCompleted += HandleChatContinueInTweenComplete;
	}

	void HandleChatContinueInTweenComplete (dfTweenPlayableBase sender)
	{
		m_WaitingForAnim = false;
		m_ChatContinueIn.TweenCompleted -= HandleChatContinueInTweenComplete;
		m_ChatText.AutoSize = true;
		//m_ChatText.AutoSize = true;
	}
	
	public void HideChatBubble()
	{
		//m_ChatFollower.attach = m_Player;
		m_ChatPanel.Pivot = dfPivotPoint.BottomCenter;
		m_ChatFollower.enabled = false;
		m_ChatZoomIn.Reset();
		m_ChatZoomIn.Stop();
		m_ChatZoomOut.Reset();
		m_ChatZoomOut.Play();
		m_CurGUIAnim = m_ChatZoomOut;
		m_CurGUIAnim.TweenCompleted += HandleTweenCompleted;
		m_WaitingForAnim = true;
		//m_Camera.ClearSecondaryTarget();
	}
}
