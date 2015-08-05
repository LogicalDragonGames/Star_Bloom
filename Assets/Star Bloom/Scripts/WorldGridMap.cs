using UnityEngine;
using System.Collections;
using Gamelogic.Grids;

public class WorldGridMap : Singleton<WorldGridMap>
{
	public IMap3D<RectPoint> m_Map;

	protected WorldGridMapSettings m_Settings;

	public Vector3 Origin { get{ return m_Settings.m_Origin; } }
	public Vector2 CellSize { get{ return m_Settings.m_CellDimensions; } }
	public float CellWidth { get{ return m_Settings.m_CellDimensions.x; } }
	public float CellHeight { get{ return m_Settings.m_CellDimensions.y; } }
	
	public static readonly Vector3 Up = Vector3.forward;
	public static readonly Vector3 Down = -Vector3.forward;
	public static readonly Vector3 Left = -Vector3.right;
	public static readonly Vector3 Right = Vector3.right;
	public static readonly Vector3 UpLeft = Up+Left;
	public static readonly Vector3 UpRight = Up+Right;
	public static readonly Vector3 DownLeft = Down+Left;
	public static readonly Vector3 DownRight = Down+Right;

	void Awake()
	{
		m_Settings = GameObject.Find( "SceneMaster" ).GetComponent<WorldGridMapSettings>();
		m_Map = new RectMap( CellSize ).To3DXZ();
	}

	public void ClampPosToCell( ref Vector3 _position )
	{
		RectPoint cell = GetClosestCell( _position );
		_position = GetCellPosition( cell );
	}

	public RectPoint GetClosestCell( Vector3 _position )
	{
		if( null == m_Map )
			return RectPoint.Zero;

		Vector3 pos = _position - m_Settings.m_Origin; // Translate into local space
		return m_Map[ pos ];
	}

	public RectPoint GetCell( RectPoint originCell, Vector3 dir )
	{
		if( null == m_Map )
			return RectPoint.Zero;

		Vector3 origin = new Vector3( originCell.X, 0f, originCell.Y );
		Vector3 targetCellPos = origin + CellSize.magnitude * dir;

		return m_Map[targetCellPos];
	}

	public Vector3 GetCellPosition( RectPoint cell )
	{
		return new Vector3( cell.X, 0f, cell.Y ) + m_Settings.m_Origin;
	}
}