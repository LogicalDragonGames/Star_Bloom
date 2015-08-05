using UnityEngine;
using System.Collections;
using Gamelogic.Grids;

public class WorldGridMapSettings : MonoBehaviour
{
	public Vector2 m_CellDimensions = Vector2.one;
	public Vector3 m_Origin = Vector2.zero;
	public bool m_TransformIsOrigin = false;
	public Color m_GridLineColor = Color.white;
	public Color m_CellLineColor = Color.black;
	public bool m_RenderGrid = true;
	public bool m_RenderCellNormal = true;
	public float m_CellNormalHeight = 1.0f;

	void Awake()
	{
		if( m_TransformIsOrigin )
			m_Origin = transform.position;
	}

	void Update()
	{
		if( m_TransformIsOrigin )
			m_Origin = transform.position;
	}

	void OnDrawGizmos()
	{
		if( m_TransformIsOrigin )
			m_Origin = transform.position;

		DrawGridLines();
	}
	
	void DrawGridLines()
	{
		Vector2 cellSize = m_CellDimensions;
		Vector2 offset = new Vector2( m_CellDimensions.x*-0.5f, m_CellDimensions.y*-0.5f );
		
		const int colCount = 200;
		const int rowCount = 200;
		Vector2 gridSize = new Vector2( cellSize.x * colCount, cellSize.y * rowCount );
		Vector2 gridMin = new Vector2( -0.5f * gridSize.x, -0.5f * gridSize.y );
		Vector2 gridMax = new Vector2( 0.5f * gridSize.x, 0.5f * gridSize.y );

		if( m_RenderGrid )
		{
			Gizmos.color = m_GridLineColor;
			
			for( int i = 0; i < rowCount+1; ++i )
			{
				Gizmos.DrawLine(
					new Vector3( gridMin.x + offset.x, 0f, gridMin.y + i*cellSize.y + offset.y ) + m_Origin + Vector3.up*0.015f,
					new Vector3( gridMax.x + offset.x, 0f, gridMin.y + i*cellSize.y + offset.y ) + m_Origin + Vector3.up*0.015f );
				
			}
			
			for( int i = 0; i < colCount+1; ++i )
			{
				Gizmos.DrawLine(
					new Vector3( gridMin.x + i*cellSize.x + offset.x, 0f, gridMin.y + offset.y ) + m_Origin + Vector3.up*0.015f,
					new Vector3( gridMin.x + i*cellSize.x + offset.x, 0f, gridMax.y + offset.y ) + m_Origin + Vector3.up*0.015f );
			}
		}

		if( m_RenderCellNormal )
		{
			Gizmos.color = m_CellLineColor;
	
			for( int x = 0; x < colCount; ++x )
			{
				for( int y = 0; y < rowCount; ++y )
				{
					Vector3 localGridPos = new Vector3( gridMin.x + x*cellSize.x, 0f, gridMin.y + y*cellSize.y );
					Vector3 globalGridPos = localGridPos + m_Origin;
					Gizmos.DrawLine( globalGridPos, globalGridPos + Vector3.up * m_CellNormalHeight );
				}
			}
		}
	}
}
