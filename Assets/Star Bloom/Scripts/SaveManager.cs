using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SaveIt;

public class SaveManager : Singleton<SaveManager>
{
	public void Start()
	{
		Console.Instance.CommandSent += OnCommand;
	}

	public bool Save( string fileName )
	{
		SaveIt.TableSerializer.Smart smart = new SaveIt.TableSerializer.Smart( fileName );
		if( null == smart )
			return false;

		SaveIt.SaveContext context = new SaveIt.SaveContext( smart );
		if( null == context )
			return false;

		//
		// Save World Information
		//
		GameObject sceneMaster = GameObject.Find( "SceneMaster" );
		if( null == sceneMaster )
			return false;

		WorldTimeSettings time = sceneMaster.GetComponent<WorldTimeSettings>(); 
		if( null == time )
			return false;

		context.Save<float>( time.Year, "Time_Year" );
		context.Save<float>( time.Month, "Time_Month" );
		context.Save<float>( time.Week, "Time_Week" );
		context.Save<float>( time.Day, "Time_Day" );
		context.Save<float>( time.Hour, "Time_Hour" );
		context.Save<float>( time.Minute, "Time_Minute" );
		context.Save<float>( time.Second, "Time_Second" );

		context.Save<bool>( time.TimePaused, "Time_TimePaused" );
		
		//
		// Save Player Information
		//
		GameObject player = GameObject.FindGameObjectWithTag( "Player" );

		// Player Inventory
		Inventory playerInv = player.GetComponent<Inventory>();
		if( null == playerInv )
			return false;

		context.Save<List<InventorySlot>>( playerInv.m_Slots, "Inventory" );

		// Tools
		ToolFramework toolFramework = player.GetComponent<ToolFramework>();
		if( null == toolFramework )
			return false;

		context.Save<string>( toolFramework.m_EquippedToolName, "EquippedTool" );

		// Player Fatigue
		PlayerFatigue fatigue = player.GetComponent<PlayerFatigue>();
		if( null == fatigue )
			return false;	

		context.Save<float>( fatigue.m_Stamina, "Stamina" );
		context.Save<float>( fatigue.m_MaxStamina, "MaxStamina" );
		

		// Relationships
		context.Save<List<KeyValuePair<string, Relationship>>>( RelationshipManager.Instance.ToList(), "Relationships" );

		context.Flush();

		return true;
	}

	public bool LoadHeader( string fileName, ref SaveHeader header )
	{
		SaveIt.TableSerializer.Smart smart = new SaveIt.TableSerializer.Smart( fileName );
		if( null == smart )
			return false;
		
		SaveIt.LoadContext context = new SaveIt.LoadContext( smart );
		if( null == context )
			return false;

		context.Load<float>( "Time_Year", out header.Year );
		context.Load<float>( "Time_Month", out header.Month );
		context.Load<float>( "Time_Week", out header.Week );
		context.Load<float>( "Time_Day", out header.Day );
		context.Load<float>( "Time_Hour", out header.Hour );
		context.Load<float>( "Time_Minute", out header.Minute );
		context.Load<float>( "Time_Second", out header.Second );

		return true;
	}

	public bool Load( string fileName )
	{
		SaveIt.TableSerializer.Smart smart = new SaveIt.TableSerializer.Smart( fileName );
		if( null == smart )
			return false;

		SaveIt.LoadContext context = new SaveIt.LoadContext( smart );
		if( null == context )
			return false;
	
		//
		// Save World Information
		//
		GameObject sceneMaster = GameObject.Find( "SceneMaster" );
		if( null == sceneMaster )
			return false;
		
		WorldTimeSettings time = sceneMaster.GetComponent<WorldTimeSettings>(); 
		if( null == time )
			return false;
		
		context.Load<float>( "Time_Year", out time.Year );
		context.Load<float>( "Time_Month", out time.Month );
		context.Load<float>( "Time_Week", out time.Week );
		context.Load<float>( "Time_Day", out time.Day );
		context.Load<float>( "Time_Hour", out time.Hour );
		context.Load<float>( "Time_Minute", out time.Minute );
		context.Load<float>( "Time_Second", out time.Second );

		context.Load<bool>( "Time_TimePaused", out time.TimePaused );

		//
		// Load Player Information
		//
		GameObject player = GameObject.FindGameObjectWithTag( "Player" );
		
		// Player Inventory
		Inventory playerInv = player.GetComponent<Inventory>();
		if( null == playerInv )
			return false;
		
		context.Load<List<InventorySlot>>( "Inventory", out playerInv.m_Slots );

		
		// Tools
		ToolFramework toolFramework = player.GetComponent<ToolFramework>();
		if( null == toolFramework )
			return false;

		string equippedTool = "";
		context.Load<string>( "EquippedTool", out equippedTool );
		toolFramework.ChangeTool( equippedTool );

		
		// Player Fatigue
		PlayerFatigue fatigue = player.GetComponent<PlayerFatigue>();
		if( null == fatigue )
			return false;
		
		context.Load<float>( "Stamina", out fatigue.m_Stamina );
		context.Load<float>( "MaxStamina", out fatigue.m_MaxStamina );


		// Relationships
		List<KeyValuePair<string, Relationship>> relationships = new List<KeyValuePair<string, Relationship>>();

		context.Load<List<KeyValuePair<string, Relationship>>>( "Relationships", out relationships );

		foreach( KeyValuePair<string, Relationship> entry in relationships )
		{
			RelationshipManager.Instance.UpdateRelationship( entry.Key, entry.Value );
		}

		NPC[] npcList = Object.FindObjectsOfType<NPC>();

		foreach( NPC npc in npcList )
		{
			npc.m_Relationship = RelationshipManager.Instance.GetRelationship( npc.m_NPCName );
		}

		return true;
	}

	void OnCommand( string command )
	{
		string[] cmd = command.Split( ' ' );
		
		if( cmd.Length < 2 )
			return;
		
		if( cmd[0] == "save" )
		{
			if( Save( cmd[1] ) )
				Console.Instance.addGameChatMessage( "Save succeeded: " + cmd[1] );
			else
				Console.Instance.addGameChatMessage( "Save failed: " + cmd[1] );
		}
		else if( cmd[0] == "load" )
		{
			if( Load( cmd[1] ) )
				Console.Instance.addGameChatMessage( "Load succeeded: " + cmd[1] );
			else
				Console.Instance.addGameChatMessage( "Load failed: " + cmd[1] );
			   
		}
	}
}