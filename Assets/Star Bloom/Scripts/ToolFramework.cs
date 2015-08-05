using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof(ItemMap), typeof(PlayerMovementController) )]
public class ToolFramework : MonoBehaviour
{
	public Vector3 m_ToolFieldCenter = Vector3.zero;
	public Vector3 m_ToolFieldSize = Vector3.zero;
	public GameObject m_ToolHighlightPrefab;

	[HideInInspector]
	public GameObject m_ToolField;
	protected BoxCollider m_ToolFieldZone;

	[HideInInspector]
	public GameObject m_ToolHighlight;
	public bool m_ToolHighlightActive = false;

	[HideInInspector]
	public string m_EquippedToolName = "";
	protected GameObject m_EquippedTool;
	protected GameObject m_PreviousTool;
	protected PlayerTool m_EquippedToolScript;
	protected PlayerTool m_PreviousToolScript;
	
	protected PlayerMovementController m_MovementController = null;
	
	[HideInInspector]
	public ItemMap m_ItemMap;
	
	[HideInInspector]
	public ItemCategory m_ToolsCategory;

	private List<Collider> m_ObjsInTrigger = new List<Collider>();
	
	// Use this for initialization
	void Awake()
	{
		Console.Instance.CommandSent += OnCommand;

		GameObject sceneMaster = GameObject.Find( "SceneMaster" );
		if( null == sceneMaster )
		{
			Debug.LogError( "Failed to find SceneMaster object" );
			return;
		}
		
		m_ItemMap = (ItemMap) sceneMaster.GetComponent<ItemMap>();
		if( null == m_ItemMap )
		{
			Debug.LogError( "Failed to find 'ItemMap' component on SceneMaster object" );
			return;
		}
		
		m_ToolsCategory = m_ItemMap.m_Categories.FindCategory( "Tools" );
		if( null == m_ToolsCategory )
		{
			Debug.LogError( "Failed to find 'Tools' category in ItemMap" );
			return;
		}

		m_MovementController = GetComponent<PlayerMovementController>();
		if( null == m_MovementController )
		{
			Debug.LogError( "Failed to find 'PlayerMovementController' component in Player" );
			return;
		}

		m_EquippedToolName = "Hand";
		ChangeTool( m_ItemMap.CreateItem( m_ToolsCategory, "Hand" ) );

		m_ToolField = new GameObject( "Tool Field" );
		m_ToolField.layer = 2;
		
		m_ToolFieldZone = m_ToolField.AddComponent<BoxCollider>();
		m_ToolHighlight = (GameObject)GameObject.Instantiate( m_ToolHighlightPrefab );

		Rigidbody rb = m_ToolField.AddComponent<Rigidbody>();
		rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

		CollisionDispatcher cd = m_ToolField.AddComponent<CollisionDispatcher>();
		cd.TriggerEnter += OnToolFieldEnter;
		cd.TriggerExit += OnToolFieldExit;

		UpdateToolField();
	}

	void UpdateToolField()
	{
		m_ToolField.transform.position = transform.position;
		m_ToolField.transform.rotation = transform.rotation;
		m_ToolFieldZone.isTrigger = true;
		m_ToolFieldZone.rigidbody.useGravity = false;
		m_ToolFieldZone.center = m_ToolFieldCenter;
		m_ToolFieldZone.size = m_ToolFieldSize;

		m_ToolHighlight.transform.position = m_ToolFieldZone.transform.position + transform.forward * m_ToolFieldZone.center.z;
		Vector3 pos = m_ToolHighlight.transform.position;
		WorldGridMap.Instance.ClampPosToCell( ref pos );
		m_ToolHighlight.transform.position = pos;
	}
	
	// Update is called once per frame
	void Update()
	{
		UpdateToolField();

		if( Input.GetButtonDown( "Interact" ) && m_MovementController.IsControllable )
		{
			// clear any potential null objects
			m_ObjsInTrigger.RemoveAll( item => item == null );

			switch( m_EquippedToolScript.InteractSelectionType )
			{
			case PlayerTool.InteractSelectionTypes.Closest:
			{
				Collider closest = GetClosestCollider();
				
				if( null == closest && m_EquippedToolScript.m_HandleNullInteractions )
					m_EquippedToolScript.Interact( (GameObject)null );

				else if( null == closest )
					return;

				else
					m_EquippedToolScript.Interact( closest.gameObject );
				
				
			}
			break;
				
			case PlayerTool.InteractSelectionTypes.All:
			{
				if( m_ObjsInTrigger.Count > 0 )
				{
					List<GameObject> intObjs = new List<GameObject>();

					foreach( Collider cObj in m_ObjsInTrigger )
					{
						intObjs.Add( cObj.gameObject );
					}
					
					m_EquippedToolScript.Interact( intObjs );
				}
			}
			break;
				
			default:
			{
				Debug.LogError( "Unknown interaction selection type: " + m_EquippedToolScript.InteractSelectionType );
				break;
			}
			}
		}
	}
	
	void OnCommand( string command )
	{
		string[] cmd = command.Split( ' ' );
		
		if( cmd.Length < 2 )
			return;
		
		if( cmd[0] == "equip" )
			ChangeTool( cmd[1] );
	}

	public void ChangeTool( string toolName )
	{
		GameObject tool = m_ItemMap.CreateItem( m_ToolsCategory, toolName );
		
		if( null != tool )
		{
			m_EquippedToolName = toolName;
			ChangeTool( tool );
		}
	}

	public void ChangeTool( GameObject obj )
	{
		if( null == obj )
		{
			Debug.Log( "Cannot change tool to null object" );
			return;
		}
		
		if( obj == m_EquippedTool )
		{
			Debug.Log( "Cannot change to tool - this tool is currently equipped" );
			return;
		}

		PlayerTool nextTool = GetToolScript( obj );
		if( null == nextTool )
		{
			Debug.Log( "Cannot locate tool script on object" );
			return;
		}
		
		m_PreviousTool = m_EquippedTool;
		m_PreviousToolScript = m_EquippedToolScript;
		m_EquippedTool = obj;
		m_EquippedToolScript = nextTool;
		
		m_EquippedToolScript.Attach();
		
		if( m_PreviousToolScript )
			m_PreviousToolScript.Detach();
	}
	
	public PlayerTool GetToolScript( GameObject obj )
	{
		return (PlayerTool)obj.GetComponentInChildren<PlayerTool>();
	}

	void OnToolFieldEnter( Collider other )
	{
		if( other.transform.IsChildOf( transform.root ) )
			return;

		if( other.GetComponent<InteractionHandler>() == null )
			return;

		if( !m_ObjsInTrigger.Contains( other ) )
		{
			Console.Instance.addGameChatMessage( "Added: " + other.name );
			m_ObjsInTrigger.Add( other );

			TileZone tz = other.GetComponent<TileZone>();
			if( null != tz )
				tz.SetToolField( m_ToolField );
		}
	}

	void OnToolFieldExit( Collider other )
	{
		if( m_ObjsInTrigger.Contains( other ) )
		{	
			Console.Instance.addGameChatMessage( "Remove: " + other.name );
			m_ObjsInTrigger.Remove( other );

			TileZone tz = other.GetComponent<TileZone>();
			if( null != tz )
				tz.ClearToolField();
		}
	}

	/*void OnTriggerEnter( Collider other )
	{
		if( other.transform.IsChildOf( transform.root ) )
			return;
		
		if( !m_ObjsInTrigger.Contains( other ) )
		{
			Console.Instance.addGameChatMessage( "Added: " + other.name );
			m_ObjsInTrigger.Add( other );
		}
	}
	
	void OnTriggerExit( Collider other )
	{
		if( m_ObjsInTrigger.Contains( other ) )
		{	
			Console.Instance.addGameChatMessage( "Remove: " + other.name );
			m_ObjsInTrigger.Remove( other );
		}
	}*/
	
	Collider GetClosestCollider()
	{
		Collider closestObj = null;
		float minDistSq = float.MaxValue;
		
		foreach( Collider obj in m_ObjsInTrigger )
		{
			float oDistSq = (transform.position - obj.transform.position).sqrMagnitude;
			if( oDistSq < minDistSq )
			{
				closestObj = obj;
				minDistSq = oDistSq;
			}
		}
		
		return closestObj;
	}
}