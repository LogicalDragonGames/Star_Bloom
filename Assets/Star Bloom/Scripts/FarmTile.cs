using UnityEngine;
using System.Collections;

public class FarmTile
{
	public int m_WorldX = 0;
	public int m_WorldY = 0;
	public int m_LocalX = 0;
	public int m_LocalY = 0;
	public Vector3 m_Position = Vector3.zero;
	public bool m_IsWatered = false;
	public float m_AmountFertilized = 0.0f;
	protected GameObject m_Crop = null;
	public GameObject Crop
	{
		get { return m_Crop; }
		set
		{
			m_Crop = value;
			if( null != m_DirtMound )
				m_DirtMound.GetComponent<FarmTileBinding>().Crop = value;
		}
	}

	public GameObject m_DirtMound = null;
}