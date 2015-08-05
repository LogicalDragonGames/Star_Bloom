using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Crop : MonoBehaviour
{
	public List<CropStage> m_Stages;

	public int TotalStagesDuration
	{
		get
		{
			int duration = 0;
			foreach( CropStage stage in m_Stages )
				duration += stage.m_DayDuration;
			
			return duration;
		}
	}

	public CropStage CurrentCropStage
	{
		get
		{
			int iter = 0;
			foreach( CropStage stage in m_Stages )
			{
				iter += stage.m_DayDuration;
				if( GameDayAge < iter )
					return stage;
			}

			// If there are no stages added, return null
			if( m_Stages.Count == 0 )
				return null;

			// If the crop is old, return the final stage
			return m_Stages[ m_Stages.Count-1 ];
		}
	}

	protected int m_GameDayAge = 0;
	public int GameDayAge
	{
		get
		{
			return m_GameDayAge;
		}
		set
		{
			if( m_GameDayAge == value )
				return;

			m_GameDayAge = value;

			UpdateCropStage();
		}
	}

	public bool m_Harvestable;

	[HideInInspector]
	public int m_HarvestYield;

	public string m_HarvestItemName;

	protected GameObject m_CropStageEntity = null;

	// Use this for initialization
	void Start()
	{
		UpdateCropStage();
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void UpdateCropStage()
	{
		CropStage curStage = CurrentCropStage;
		
		if( null == curStage )
			return;
		
		if( null != m_CropStageEntity )
		{
			DestroyImmediate( m_CropStageEntity );
			m_CropStageEntity = null;
		}
		
		if( null != curStage.m_Prefab )
		{
			m_CropStageEntity = (GameObject)GameObject.Instantiate( curStage.m_Prefab );
			m_CropStageEntity.transform.parent = this.transform;
			m_CropStageEntity.transform.localPosition = Vector3.zero;
		}
	}
}
