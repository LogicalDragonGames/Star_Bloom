using UnityEngine;
using System.Collections;
using Gamelogic.Grids;

public class FarmPlotZone : TileZone
{
	public string m_PlotName;
	public GameObject m_DirtMoundPrefab = null;
	
	protected FarmPlot m_FarmPlot = null;

	// Use this for initializationss
	protected override void Start()
	{
		base.Start();

		m_FarmPlot = SceneMetadata.Instance.GetFarmPlot( m_PlotName );
		InteractionHandler ih = GetComponent<InteractionHandler>();
		ih.InteractionOccurred += HandleInteractionOccurred;
	
		m_FarmPlot.m_Cols = (int)m_GridSize.x;
		m_FarmPlot.m_Rows = (int)m_GridSize.y;

		m_FarmPlot.ResizeTileArray();

		/*foreach( FarmTile tile in m_FarmPlot.m_Tiles )
		{
			tile.m_DirtMound = (GameObject)GameObject.Instantiate( m_DirtMoundPrefab );
			tile.m_DirtMound.transform.position = tile.m_Position;
		}*/

		WorldTime.Instance.GameDayElapsed += HandleGameDayElapsed;
	}

	void OnDestroy()
	{
		if( null != WorldTime.Instance )
			WorldTime.Instance.GameDayElapsed -= HandleGameDayElapsed;
	}

	void HandleGameDayElapsed (uint _val)
	{
		foreach( FarmTile tile in m_FarmPlot.m_Tiles )
		{
			if( null == tile.m_DirtMound )
				continue;

			Crop cropScript = tile.m_DirtMound.GetComponentInChildren<Crop>();
			if( null == cropScript )
				continue;

			cropScript.GameDayAge += 1;
		}
	}

	void HandleInteractionOccurred( PlayerTool tool )
	{
		if( !m_SelectionOnGrid )
			return;

		if( tool.GetType() == typeof( HoeTool ) )
		{
			FarmTile tile = m_FarmPlot.m_Tiles[ m_SelectedPoint.X, m_SelectedPoint.Y ];
			if( tile.m_DirtMound == null )
			{
				tile.m_DirtMound = (GameObject)GameObject.Instantiate( m_DirtMoundPrefab );
				tile.m_DirtMound.transform.position = tile.m_Position;
			}
		}
		else if( tool.GetType() == typeof( SeedPouchTool ) )
		{
			SeedPouchTool pouch = (SeedPouchTool)tool;
			FarmTile tile = m_FarmPlot.m_Tiles[ m_SelectedPoint.X, m_SelectedPoint.Y ];
			tile.Crop = pouch.m_CropPrefab;
		}
	}

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();

		if( null == m_ToolField )
			return;

		// If the selection point is not on the grid, return
		if( m_SelectionOnGrid )
			m_ToolFramework.m_ToolHighlightActive = true;
		else
		{
			m_ToolFramework.m_ToolHighlightActive = false;
			return;
		}
	}
}