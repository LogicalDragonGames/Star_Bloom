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
}
