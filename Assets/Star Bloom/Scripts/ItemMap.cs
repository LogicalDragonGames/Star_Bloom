using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemMap : MonoBehaviour
{
	public ItemCategory m_Categories = new ItemCategory();

	void Start()
	{
		m_Categories.m_Name = "Root";
	}

	public GameObject CreateItem( string[] categoryPath, string itemName )
	{
		Item item = m_Categories.GetItem( categoryPath, itemName );
		if( null == item )
			return null;
		
		if( null == item.m_Prefab )
			return null;
		
		return (GameObject)Instantiate( item.m_Prefab );
	}

	public GameObject CreateItem( ItemCategory category, string itemName )
	{
		Item item = m_Categories.GetItem( category, itemName );
		if( null == item )
			return null;
		
		if( null == item.m_Prefab )
			return null;
		
		return (GameObject)Instantiate( item.m_Prefab );
	}
}