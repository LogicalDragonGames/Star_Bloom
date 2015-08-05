using UnityEngine;
using System.Collections;

public class FarmTileBinding : MonoBehaviour
{
	public Material m_UnwateredMaterial;
	public Material m_WateredMaterial;
	public Material[] m_FertilizedMaterials;

	public FarmTile m_Tile = null;

	protected GameObject m_Crop = null;
	public GameObject Crop
	{
		get
		{
			return m_Crop;
		}
		set
		{
			if( m_Crop == value )
				return;

			if( m_Crop )
				DestroyImmediate( m_Crop );

			if( null == value )
				return;

			m_Crop = (GameObject)GameObject.Instantiate( value );
			m_Crop.transform.parent = transform;
			m_Crop.transform.localPosition = Vector3.zero;
		}
	}

	protected bool m_IsWatered = false;
	public bool IsWatered
	{
		get
		{
			if( null != m_Tile )
				m_IsWatered = m_Tile.m_IsWatered;

			return m_IsWatered;
		}

		set
		{
			m_IsWatered = value;
			UpdateMaterials();
		}
	}

	protected float m_AmountFertilized = 0.0f;
	public float AmountFertilized
	{
		get
		{
			if( null != m_Tile )
				m_AmountFertilized = m_Tile.m_AmountFertilized;

			return m_AmountFertilized;
		}
		set
		{
			if( m_AmountFertilized != value )
			{
				m_AmountFertilized = value;

				if( m_FertilizedMaterials.Length == 0 )
				{
					Debug.LogWarning( "Set fertilized dirt material!" );
					return;
				}
			}
		}
	}

	void Start()
	{
		UpdateMaterials();
	}

	protected void UpdateMaterials()
	{
		Material wateredMat = null;
		Material dirtMat = null;

		if( m_IsWatered )
			wateredMat = m_WateredMaterial;
		else
			wateredMat = m_UnwateredMaterial;

		int matIndex = (int)(m_AmountFertilized * (m_FertilizedMaterials.Length-1) ); // Truncate
		dirtMat = m_FertilizedMaterials[matIndex];

		renderer.sharedMaterials = new Material[] { dirtMat, wateredMat };
	}
}