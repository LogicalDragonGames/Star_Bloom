using UnityEngine;
using System.Collections;
using Gamelogic.Grids;

public class FarmPlot
{
	public RectPoint m_MinCell;
	public RectPoint m_MaxCell;
	public FarmPlot m_FarmPlot;
	
	public int m_Cols = 0;
	public int m_Rows = 0;

	public FarmTile[,] m_Tiles = null;

	public void ResizeTileArray()
	{
		m_Tiles = new FarmTile[m_Cols,m_Rows];

		for( int x = 0; x < m_Cols; ++x )
		{
			for( int y = 0; y < m_Rows; ++y )
			{
				m_Tiles[x,y] = new FarmTile();
				FarmTile tile = m_Tiles[x,y];
				tile.m_LocalX = x;
				tile.m_LocalY = y;
				tile.m_WorldX = m_MinCell.X + x;
				tile.m_WorldY = m_MinCell.Y + y;

				RectPoint pt = new RectPoint( tile.m_WorldX, tile.m_WorldY );
				tile.m_Position = WorldGridMap.Instance.GetCellPosition( pt );
			}
		}
	}
}