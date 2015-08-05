using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishingZone : InteractionHandler
{
	[SerializeField]
	public FishingZoneEntry[] m_AvailableFish = {};

	public AudioClip m_FishingCast;
	public AudioClip m_CorkSplash;
	public AudioClip m_HookAlert;
	public AudioClip m_FishCaught;
	public AudioClip m_FishEscaped;

	public float m_MinWaitTime = 2.0f;
	public float m_MaxWaitTime = 4.0f;
	public float m_CastTime = 0.2f;
	public float m_MinFightTime = 2.0f;
	public float m_MaxFightTime = 3.0f;
	public float m_ReelTime = 0.5f;

	public int m_MinRequiredHits = 3;
	public int m_MaxRequiredHits = 5;
	protected int m_RequiredHits = 0;

	protected Timer m_Timer;
	protected PlayerMovementController m_PlayerController = null;
	protected Inventory m_PlayerInventory = null;
	protected ItemMap m_ItemMap = null;
	protected ItemCategory m_FishCategory = null;
	protected dfLabel m_SystemMessage = null;

	protected enum FishingState
	{
		None,
		Casting,
		Waiting,
		HookedFish,
		Reeling,
		FishEscaped
	};

	protected FishingState m_State = FishingState.None;

	// Use this for initialization
	void Start()
	{
		GameObject player = GameObject.FindGameObjectWithTag( "Player" );
		m_PlayerController = player.GetComponent<PlayerMovementController>();

		// TODO: Player will have multiple inventories
		m_PlayerInventory = player.GetComponent<Inventory>();


		GameObject sceneMaster = GameObject.Find( "SceneMaster" );
		m_ItemMap = sceneMaster.GetComponent<ItemMap>();
		m_FishCategory = m_ItemMap.m_Categories.FindCategory( "Fish" );

		GameObject gui = GameObject.Find( "GUI" );
		Transform systemMessage = gui.transform.FindChild( "SystemMessage" );
		m_SystemMessage = systemMessage.GetComponent<dfLabel>();
	}

	public void DoAlertEffect( bool show )
	{
		GameObject gui = GameObject.Find( "GUI" );
		Transform hud = gui.transform.FindChild( "HUD" );
		Transform effects = hud.FindChild( "PlayerEffects" );
		Transform alert = effects.transform.FindChild( "Alert" );

		alert.gameObject.SetActive( show );
		dfSprite alertSprite = alert.GetComponent<dfSprite>();
		alertSprite.IsVisible = show;
	}

	public void ShowSystemMessage( string message )
	{
		m_SystemMessage.Show();
		m_SystemMessage.Text = message;
	}

	public void HideSystemMessage()
	{
		m_SystemMessage.Hide();
	}

	public void Update()
	{

		switch (m_State) 
		{
		case FishingState.None:
			return;

		case FishingState.Casting:

			if( m_Timer.IsFinished )
			{
				AudioSource.PlayClipAtPoint( m_CorkSplash, Camera.main.transform.position );
				m_Timer.Begin( Random.Range( m_MinWaitTime, m_MaxWaitTime ) );
				m_State = FishingState.Waiting;
				ShowSystemMessage( "Waiting for a fish to bite..." );
				Debug.Log( "Waiting for a fish to bite" );
			}
			return;

		case FishingState.Waiting:
			if( m_Timer.IsFinished )
			{
				AudioSource.PlayClipAtPoint( m_HookAlert, Camera.main.transform.position );
				m_Timer.Begin( Random.Range( m_MinFightTime, m_MaxFightTime ) );
				m_RequiredHits = Random.Range( m_MinRequiredHits, m_MaxRequiredHits+1 );
				DoAlertEffect( true );
				m_State = FishingState.HookedFish;
				ShowSystemMessage( "You've hooked one. Reel it in!" );
				Debug.Log( "You've hooked one. Reel it in!" );
			}
			else if( Input.GetButtonDown( "Interact" ) )
			{
				// Cancel fishing
				m_State = FishingState.Reeling;
				ShowSystemMessage( "You cancelled fishing. Quitter." );
				Debug.Log( "You cancelled fishing. Quitter." );
			}
			return;

		case FishingState.HookedFish:
			if( Input.GetButtonDown( "Interact" ) )
			{
				--m_RequiredHits;
			}

			if( m_Timer.IsFinished )
			{
				AudioSource.PlayClipAtPoint( m_FishEscaped, Camera.main.transform.position );
				m_Timer.Begin( m_ReelTime );
				DoAlertEffect( false );
				m_State = FishingState.Reeling;
				ShowSystemMessage( "That fish was too good for you." );
				Debug.Log( "That fish was too good for you." );
			}
			else if( m_RequiredHits <= 0 )
			{
				// Award fish
				CatchFish();
				AudioSource.PlayClipAtPoint( m_FishCaught, Camera.main.transform.position );
				m_Timer.Begin( m_ReelTime );
				DoAlertEffect( false );
				m_State = FishingState.Reeling;
			}

			return;

		case FishingState.Reeling:
			if( m_Timer.IsFinished )
			{
				HideSystemMessage();
				m_PlayerController.SetDisabled( false );
				m_State = FishingState.None;
			}
			return;

		default:
				break;
		}
	}

	public override void HandleInteraction( PlayerTool tool )
	{
		if( tool.GetType() != typeof(FishingRodTool) )
			return;

		if( m_State != FishingState.None )
		return;

		if( m_AvailableFish.Length == 0 )
			return; // No fish

		CastLine();
	}

	public void CastLine()
	{
		Debug.Log ( "Gone fishin'" );
		ShowSystemMessage( "Gone fishin'" );
		AudioSource.PlayClipAtPoint( m_FishingCast, Camera.main.transform.position );
		
		if( null == m_Timer )
			m_Timer = (Timer)gameObject.AddComponent<Timer>();

		m_State = FishingState.Casting;
		m_Timer.Begin( m_CastTime );
		m_PlayerController.SetDisabled();
	}

	public void CatchFish()
	{
		string fishType = GetRandomFish();
		
		// No fish found
		if( fishType == "" )
			return;
		
		Item fish = m_FishCategory.FindItem( fishType );
		
		if( fish == null )
		{
			Debug.LogWarning( "Could not find fish type '" + fishType + "' when fishing." );
			return;
		}

		float fishSize = Random.value * 50f;

		string msg = "You caught a " + fish.m_Name + "!\n";

		msg += string.Format( "It measures {0,2:.##} cm.\n", fishSize );

		if( fishSize < 5f )
			msg += "That's... microscopic. Are you even trying?";
		else if( fishSize < 10f )
			msg += "So, you managed to catch your own bait.";
		else if( fishSize < 20f )
			msg += "Pretty small. Try again.";
		else if( fishSize < 30f )
			msg += "Hey, that's a pretty decent fish!";
		else if( fishSize < 40f )
			msg += "Big fish is lunch!";
		else
			msg += "GARGANTUMUNGO! WAHOOO!!";
			
		ShowSystemMessage( msg );
		Console.Instance.addGameChatMessage( msg );
		m_PlayerInventory.Add( fish.m_Name );
	}

	public string GetRandomFish()
	{
		// Return empty if there are no possible fish
		if( m_AvailableFish.Length == 0 )
			return "";

		float maxProbabilityField = 0f;

		foreach( FishingZoneEntry entry in m_AvailableFish )
			maxProbabilityField += entry.Chance;

		float roll = Random.Range( 0f, maxProbabilityField );

		foreach( FishingZoneEntry entry in m_AvailableFish )
		{
			roll -= entry.Chance;

			if( roll <= 0f )
				return entry.Type;
		}

		// Edge case, return the end element (probably caused due to lossy precision)
		return m_AvailableFish[ m_AvailableFish.Length-1 ].Type;
	}
}
