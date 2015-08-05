using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToolFramework : MonoBehaviour
{
	protected GameObject m_EquippedTool;
	protected GameObject m_PreviousTool;
	protected Tool m_EquippedToolScript;
	protected Tool m_PreviousToolScript;

	[HideInInspector]
	public ItemMap m_ItemMap;
	
	[HideInInspector]
	public ItemCategory m_ToolsCategory;

	public Vector3 m_InteractZoneCenterOffset = new Vector3( 0.0f, 0.3f, 0.25f );
	public float m_InteractZoneRadius = 2.0f;
	public float m_InteractZoneHeight = 1.0f;
	private CapsuleCollider m_InternalIZ;
	private ArrayList m_ObjsInTrigger = new ArrayList();

	// Use this for initialization
	void Start()
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

		ChangeTool( m_ItemMap.CreateItem( m_ToolsCategory, "Hand" ) );

		m_InternalIZ = this.gameObject.AddComponent<CapsuleCollider>();
		UpdateInternalIZ();
	}

	// Update is called once per frame
	void Update()
	{
		UpdateInternalIZ();

		if( Input.GetKeyDown( "e" ) )
		{
			switch( m_EquippedToolScript.InteractSelectionType )
			{
				case Tool.InteractSelectionTypes.Closest:
				{
					Collider closest = GetClosestCollider();
					
					if( null == closest )
						return;

					m_EquippedToolScript.Interact( closest.gameObject );
				}
				break;

				case Tool.InteractSelectionTypes.All:
				{
					if( m_ObjsInTrigger.Count > 0 )
					{
						ArrayList intObjs = new ArrayList();

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
		{
			ChangeTool( m_ItemMap.CreateItem( m_ToolsCategory, cmd[1] ) );
		}
	}

	void ChangeTool( GameObject obj )
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

		Tool nextTool = GetToolScript( obj );
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

	Tool GetToolScript( GameObject obj )
	{
		return (Tool) obj.GetComponentInChildren<Tool>();
	}

	void UpdateInternalIZ()
	{
		m_InternalIZ.isTrigger = true;
		m_InternalIZ.center = m_InteractZoneCenterOffset;
		m_InternalIZ.radius = m_InteractZoneRadius;
		m_InternalIZ.height = m_InteractZoneHeight;
	}

	void OnTriggerEnter( Collider other )
	{
		if( other.transform.IsChildOf( transform.root ) )
			return;

		if( !m_ObjsInTrigger.Contains( other ) )
		{
			m_ObjsInTrigger.Add( other );
		}
	}

	void OnTriggerExit( Collider other )
	{
		if( m_ObjsInTrigger.Contains( other ) )
		{
			m_ObjsInTrigger.Remove( other );
		}
	}

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
