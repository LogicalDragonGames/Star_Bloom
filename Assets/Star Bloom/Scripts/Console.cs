using UnityEngine;
using System.Collections;

public class Console : Singleton<Console>
{
	struct FPSChatEntry
	{
		public string name;
		public string text;  
	}

	//Private vars used by the script
	private ConsoleSettings settings = null;
	private string inputField = "";
	private Vector2 scrollPosition;
	private string playerName;
	private float lastUnfocus = 0;
	private Rect window;
	//private float lastEntry = 0.0f;
	private static Console thisScript;
	private ArrayList chatEntries = new ArrayList();

	public delegate void ChatEventHandler( string message );
	public ChatEventHandler ChatSent;

	public delegate void CommandEventHandler( string message );
	public CommandEventHandler CommandSent;

	public void Awake()
	{
		GameObject sceneMaster = GameObject.Find( "SceneMaster" );

		if( null == sceneMaster )
		{
			Debug.LogError ( "Could not find SceneMaster instance" );
			return;
		}

		settings = (ConsoleSettings) sceneMaster.GetComponent( typeof(ConsoleSettings) );

		if( null == settings )
		{
			Debug.LogError( "Could not find ConsoleSettings instance" );
			return;
		}

		settings.usingChat = false;
		thisScript = this;

		if( settings.centerHorizontal )
			settings.x = Screen.width / 2 - settings.width / 2 - settings.x;
		else if( settings.clampRight )
			settings.x = Screen.width - settings.width - settings.x;

		if (settings.centerVertical)
						settings.y = Screen.height / 2 - settings.height / 2 - settings.y; 
		else if( settings.clampBottom )
			settings.y = Screen.height - settings.height - settings.y;

		window = new Rect( settings.x, settings.y, settings.width, settings.height );
		//lastEntry = Time.time;
		playerName = PlayerPrefs.GetString( "playerName", "" );
		
		if( playerName == "" )
			playerName = "Player";
	}
	
	public void CloseChatWindow()
	{
		settings.showChat = false;
		inputField = "";
		chatEntries = new ArrayList();
	}
	
	public void ShowChatWindow()
	{
		settings.showChat = true;
		inputField = "";
		chatEntries = new ArrayList();
	}
	
	public void OnGUI()
	{
		if( !settings.showChat )
			return;
		
		/*if ( PlayerInfos.ScreenState() && PlayerInfos.IsUsingStore() )
			return;*/
		
		GUI.skin = settings.skin;
		//window.y = Screen.height - window.height - 5;
		
		if (
			Event.current.type == EventType.keyDown && 
			Event.current.character == '\n' && inputField.Length <= 0)
		{
			if( lastUnfocus+0.1 < Time.time )
			{
				settings.usingChat = true;
				GUI.FocusWindow( 5 );
				GUI.FocusControl( "Chat input field" );
				Screen.lockCursor = false;
			}
		}

		//Screen.lockCursor = screenLock;
		window = GUI.Window( 5, window, GlobalChatWindow, "" );
	}
	
	public void GlobalChatWindow( int id )
	{
		GUILayout.BeginVertical();
		GUILayout.Space(10);
		GUILayout.EndVertical();
		
		// Begin a scroll view. All rects are calculated automatically - 
		// it will use up any available screen space and make sure contents flow correctly.
		// This is kept small with the last two parameters to force scrollbars to appear.
		scrollPosition = GUILayout.BeginScrollView( scrollPosition );
		
		foreach( FPSChatEntry entry in chatEntries )
		{	
			GUILayout.BeginHorizontal();
			
			if( entry.name == "" )
				GUILayout.Label( entry.text ); //Game message
			else
				GUILayout.Label( entry.name + ": " + entry.text );
			
			GUILayout.EndHorizontal();
			GUILayout.Space( 3 );
		}
		
		// End the scrollview we began above.
		GUILayout.EndScrollView ();
		
		if( Event.current.type == EventType.keyDown && 
		   Event.current.character == '\n' && 
		   inputField.Length > 0 )
		{
			HitEnter(inputField);
		}
		
		else if( Event.current.type == EventType.keyDown && 
		        Event.current.character == '\n' && 
		        inputField.Length == 0)
		{
			inputField = ""; //Clear line
			GUI.UnfocusWindow ();//Deselect chat
			lastUnfocus=Time.time;
			settings.usingChat=false;
		}
		
		GUI.SetNextControlName( "Chat input field" );
		inputField = GUILayout.TextField( inputField );
		
		if( Input.GetKeyDown("mouse 0") )
		{
			if( settings.usingChat )
			{
				settings.usingChat = false;
				GUI.UnfocusWindow();	//Deselect chat
				lastUnfocus=Time.time;
			}
		}
	}
	
	public void HitEnter( string msg )
	{
		msg = msg.Replace( "\n", "" );
		SendChat( playerName, msg );

		inputField = "";			// Clear line

		settings.usingChat = false;
		GUI.FocusControl( "" );
		GUI.UnfocusWindow();		// Deselect chat
		lastUnfocus = Time.time;
	}
	
	static public void StaticMsg( string msg )
	{
		thisScript.HitEnter( msg );
	}

	// Invoke the ChatSent event; called whenever a chat message is added
	protected virtual void OnChat( string message ) 
	{
		if ( ChatSent != null )
			ChatSent( message );
	}

	// Invoke the CommandSent event; called whenever a chat command is added
	protected virtual void OnCommand( string command ) 
	{
		if ( CommandSent != null )
			CommandSent( command );
	}

	public void SendChat( string msg)
	{
		SendChat( playerName, msg );
	}

	public void SendChat( string name, string msg )
	{
		if( msg.Length == 0 )
			return;

		FPSChatEntry entry = new FPSChatEntry();
		
		entry.name = name;
		entry.text = msg;
		
		chatEntries.Add(entry);
		//lastEntry = Time.time;
		
		//Remove old entries
		if( chatEntries.Count > settings.maxChatLines )
			chatEntries.RemoveAt(0);
		
		scrollPosition.y = int.MaxValue;

		if( msg[0] == '/' && msg.Length > 1 )
			OnCommand( msg.Substring(1) );
		else
			OnChat( msg );
	}
	
	// Add game messages etc
	public void addGameChatMessage( string str )
	{
		SendChat( "", str );
	}
}