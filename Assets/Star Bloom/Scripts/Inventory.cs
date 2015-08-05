using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Inventory : MonoBehaviour
{
	public List<InventorySlot> m_Slots;

	public void Resize( int size, InventorySlot c )
	{
		int cur = m_Slots.Count;
		if( size < cur )
			m_Slots.RemoveRange( size, cur - size );
		else if( size > cur )
		{
			if( size > m_Slots.Capacity )//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
				m_Slots.Capacity = size;

			m_Slots.AddRange( Enumerable.Repeat( c, size - cur ) );
		}
	}
	public void Resize( int size )
	{
		Resize( size, new InventorySlot() );
	}

	// Returns the index of an item
	public int Find( string itemName, int start = 0 )
	{
		for( int i = start; i < m_Slots.Count(); ++i )
		{
			if( m_Slots[i].Item == itemName )
				return i;
		}

		return (-1);
	}

	// Returns the index of an empty slot
	public int FindEmpty( int start = 0 )
	{
		for( int i = start; i < m_Slots.Count(); ++i )
		{
			if( m_Slots[i].Count == 0 )
				return i;
		}

		return (-1);
	}
	
	public InventorySlot FindSlot( string itemName, int start = 0 )
	{
		int slotIdx = Find( itemName, start );
		
		if( (-1) == slotIdx )
			return null;
		
		return m_Slots[slotIdx];
	}

	public InventorySlot FindEmptySlot( int start = 0 )
	{
		int slotIdx = FindEmpty( start );
		
		if( (-1) == slotIdx )
			return null;
		
		return m_Slots[slotIdx];
	}

	public bool Add( string itemName, int count = 1 )
	{
		// If the item already exists in the player's inventory
		// Increment that count (where possible)
		for( int i = 0; i != (-1); ++i )
		{
			i = Find( itemName, i );

			if( (-1) == i )
				break;

			if( AddToSlot( m_Slots[i], count ) )
				return true;
		}

		// Add the item to the player's inventory
		InventorySlot slot = FindEmptySlot();

		// No empty slots remain
		if( null == slot )
			return false;

		// Otherwise set the slot type and add to it
		slot.Item = itemName;

		if( AddToSlot( slot, count ) )
			return true;

		// This can occur if the number of items you're attempting to
		// add more than a max stack size at once
		// TODO: Support adding items to partial stacks and breaking
		// up slot consumption (probably recurse)
		return false;
	}

	public static bool AddToSlot( InventorySlot slot, int count = 1 )
	{
		if( slot.Count+count > 20 ) // TODO: StackSize in Item...
			return false;
		
		slot.Count += count;
		return true;
	}
}
