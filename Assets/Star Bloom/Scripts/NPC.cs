using UnityEngine;
using System.Collections;

public class NPC : MonoBehaviour
{
	public string Name = "";
	public string[] PlayerTalkMessages = {""};

	public int LoseFocusSqDist = 1;
	public float LoseFocusAngle = 20;

	private GameObject m_LastKnownInteractor;
	private Quaternion m_LastKnownInteractorRotation;
	private Vector3 m_LastKnownInteractorPosition;
	private bool m_IsTalking = false;
	private Animator anim = null;


	// Use this for initialization
	void Start()
	{
		anim = GetComponent<Animator>();

		InteractionHandler ih = GetComponent<InteractionHandler>();

		if( ih )
			ih.InteractionOccurred += OnInteraction;
		else
			Debug.LogError( "Could not find 'InteractionHandler' component" );
	}
	
	// Update is called once per frame
	void Update()
	{
		if( m_IsTalking )
		{
			if( (m_LastKnownInteractor.transform.position - m_LastKnownInteractorPosition).sqrMagnitude > LoseFocusSqDist ||
			   !m_LastKnownInteractor.transform.rotation.AlmostEquals( m_LastKnownInteractorRotation, LoseFocusAngle ) )
			{
				m_IsTalking = false;

				if( anim )
					anim.SetBool( "Talking", false );
			}
		}

	}

	string TranslateMessageArgs( string message )
	{
		string translated = message;
		string playerName = "NAME_UNKNOWN";
		string hourMinTime = WorldTime.Instance.GetHourMinuteTime();
		
		GameObject player = GameObject.Find( "Player" );
		
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
		translated = translated.Replace( "{{NPCNAME}}", Name );

		return translated;
	}

	public void OnInteraction( Tool tool )
	{
		if( tool.GetType() == typeof(HandTool) )
		{
			Talk( tool.transform.root.gameObject );
		}
	}

	public void Talk( GameObject obj )
	{
		if( obj.tag == "Player" )
		{
			m_LastKnownInteractor = obj;
			m_LastKnownInteractorPosition = obj.transform.position;
			m_LastKnownInteractorRotation = obj.transform.rotation;
			m_IsTalking = true;

			if( anim )
				anim.SetBool( "Talking", true );

			string compoundMessage = "";

			for( uint i = 0; i < PlayerTalkMessages.Length; ++i )
				if( i == PlayerTalkMessages.Length - 1 )
					compoundMessage += TranslateMessageArgs( PlayerTalkMessages[i] );	
				else
			compoundMessage += TranslateMessageArgs( PlayerTalkMessages[i] ) + "\n\t"; // Only newline if not the final message

			Console.Instance.SendChat( Name, compoundMessage );
		}
	}
}
