using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneMetadata : Singleton<SceneMetadata>
{
	public int LevelsLoaded = 0;
	public bool HasSceneLoadPlayerTransform = false;
	public Vector3 SceneLoadPlayerPosition = Vector3.zero;
	public Vector3 SceneLoadPlayerRotation = Vector3.zero;
	public List<InventorySlot> SceneLoadInventorySlots;
	public float SceneLoadStamina = 0.0f;
	public float SceneLoadStaminaMax = 0.0f;
	public string SceneLoadEquippedTool = "";
	protected Dictionary<string, FarmPlot> m_FarmPlots = new Dictionary<string, FarmPlot>();

	// Use this for initialization
	void Start()
	{
		DontDestroyOnLoad( transform.gameObject );
	}

	GameObject GetPlayer()
	{
		return GameObject.Find( "Player" );
	}

	public FarmPlot GetFarmPlot( string name )
	{
		if( !m_FarmPlots.ContainsKey( name ) )
			m_FarmPlots.Add( name, new FarmPlot() );

		return m_FarmPlots[ name ];
	}

	public void OnLevelWasLoaded( int level )
	{
		++LevelsLoaded;

		if( HasSceneLoadPlayerTransform )
			SetPlayerPosition();

		if( LevelsLoaded > 0 )
		{
			SetPlayerValues();
		}
	}

	public void PrepareTransition()
	{
		GameObject player = GetPlayer();

		// Player Inventory
		Inventory inventoryScript = player.GetComponent<Inventory>();
		SceneLoadInventorySlots = inventoryScript.m_Slots;

		// Equipped Tool
		ToolFramework toolFramework = player.GetComponent<ToolFramework>();
		SceneLoadEquippedTool = toolFramework.m_EquippedToolName;

		// Player Fatigue
		PlayerFatigue fatigueScript = player.GetComponent<PlayerFatigue>();
		SceneLoadStamina = fatigueScript.m_Stamina;
		SceneLoadStaminaMax = fatigueScript.m_MaxStamina;
	}

	public void SetPlayerPosition()
	{
		GameObject player = GetPlayer();

		if( null == player )
		{
			Debug.Log( "Failed to find player object." );
			return;
		}

		player.transform.position = SceneLoadPlayerPosition;
		player.transform.rotation = Quaternion.Euler( SceneLoadPlayerRotation );
	}

	public void SetPlayerValues()
	{
		GameObject player = GetPlayer();

		// Player Inventory
		Inventory inventoryScript = player.GetComponent<Inventory>();
		inventoryScript.m_Slots = SceneLoadInventorySlots;

		// Equipped Tool
		ToolFramework toolFramework = player.GetComponent<ToolFramework>();
		toolFramework.ChangeTool( SceneLoadEquippedTool );

		// Player Fatigue
		PlayerFatigue fatigueScript = player.GetComponent<PlayerFatigue>();
		fatigueScript.m_MaxStamina = SceneLoadStaminaMax;
		fatigueScript.SetStamina( SceneLoadStamina );
	}
}
