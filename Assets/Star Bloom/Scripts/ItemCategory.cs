using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ItemCategory
{
	public string m_Name = "New Category";
	public List<Item> m_Items;
	public List<ItemCategory> m_Categories;
	
	private Dictionary<string, Item> m_ItemMap = new Dictionary<string, Item>();
	private Dictionary<string, ItemCategory> m_CategoryMap = new Dictionary<string, ItemCategory>();
	
	void Start()
	{
		// Transcribe the list of items to
		// the item map for faster lookup times
		foreach( Item item in m_Items )
		{
			item.m_Category = this;
			m_ItemMap.Add( item.m_Name, item );
		}

		// Transcribe the list of categories to 
		// the category map for faster lookup times
		foreach( ItemCategory category in m_Categories )
		{
			m_CategoryMap.Add( category.m_Name, category );
		}
	}

	public ItemCategory GetCategory( string categoryName )
	{
		return m_CategoryMap[categoryName];
	}

	public Item GetItem( string itemName )
	{
		return m_ItemMap[itemName];
	}

	public Item GetItem( string[] categoryPath, string itemName, bool searchIfNotFound = true )
	{
		ItemCategory category = this;
		ItemCategory prevCat = this;

		for( uint i = 0; i < categoryPath.Length; ++i )
		{
			prevCat = category;
			category = prevCat.GetCategory( categoryPath[i] );

			// Failed to find the category listed in the path
			if( category == null )
				break;
		}

		// If we didn't finish traversing the category path do a
		// breadth search for the item from the previous category
		if( null == category )
		{
			if( searchIfNotFound )
				return prevCat.SearchCatItemRecurse( itemName );
			else
				return null;
		}

		// If the item wasn't found in this category do a breadth search
		Item item = GetItem( itemName );
		if( null == item )
		{
			if( searchIfNotFound )
				return SearchCatItemRecurse( itemName );
			else
				return null;
		}

		return item;
	}

	public Item GetItem( ItemCategory category, string itemName )
	{
		return category.SearchCatItemRecurse( itemName );
	}
	
	public Item FindItem( string itemName )
	{
		return SearchCatItemRecurse( itemName );
	}

	// Breadth-first recursive search of item/category tree
	protected Item SearchCatItemRecurse( string itemName )
	{
		Item userItem = null;
		foreach( Item item in m_Items )
			if( item.m_Name == itemName )
				userItem = item;
		
		if( null != userItem )
			return userItem;
		
		foreach( ItemCategory category in m_Categories )
		{
			userItem = category.SearchCatItemRecurse( itemName );
			
			if( null != userItem )
				return userItem;
		}
		
		return null;
	}
	
	public ItemCategory FindCategory( string categoryName )
	{
		return SearchCatRecurse( categoryName );
	}

	protected ItemCategory SearchCatRecurse( string categoryName )
	{
		ItemCategory userCat = null;
		foreach( ItemCategory category in m_Categories )
			if( category.m_Name == categoryName )
				userCat = category;
		
		if( null != userCat )
			return userCat;
		
		foreach( ItemCategory category in m_Categories )
		{
			userCat = category.SearchCatRecurse( categoryName );
			
			if( null != userCat )
				return userCat;
		}
		
		return null;
	}
}
