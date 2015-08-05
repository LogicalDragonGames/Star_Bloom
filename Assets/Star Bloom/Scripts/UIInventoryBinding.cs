using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIInventoryBinding : MonoBehaviour
{
	ItemMap m_ItemMap;
	Inventory m_PlayerInventory = null;
	public dfScrollPanel m_Container;
	public GameObject m_InventorySlotPrefab;

	public void Start()
	{
		GameObject sceneMaster = GameObject.Find( "SceneMaster" );
		if( null == sceneMaster )
		{
			Debug.LogError( "Failed to find 'SceneMaster' object");
			return;
		}

		m_ItemMap = sceneMaster.GetComponent<ItemMap>();
		if( null == m_ItemMap )
		{
			Debug.LogError( "Failed to find 'ItemMap' component attached to SceneMaster" );
			return;
		}

		GameObject player = GameObject.Find( "Player" );
		if( null == player )
		{
			Debug.LogError( "Failed to find game object tagged 'Player'");
			return;
		}

		m_PlayerInventory = player.GetComponent<Inventory>();
		if( null == m_PlayerInventory )
		{
			Debug.LogError ( "Failed to find 'Inventory' component attached to Player" );
			return;
		}
	}

	public void OnMenuOpened()
	{
		ClearInventory();
		PopulateInventory();
	}

	public void ClearInventory()
	{
		List<GameObject> children = new List<GameObject>();

		foreach (Transform child in transform) 
			children.Add(child.gameObject);

		for( int i = 0; i < children.Count; ++i )
			DestroyImmediate( children[i] );
	}

	public void PopulateInventory()
	{
		foreach( InventorySlot slot in m_PlayerInventory.m_Slots )
		{
			Console.Instance.addGameChatMessage( slot.Item );
			
			dfControl slotControl = m_Container.AddPrefab( m_InventorySlotPrefab );
			
			Transform countObj = slotControl.transform.Find( "Count" );
			Transform iconObj = slotControl.transform.Find( "Icon" );
			dfLabel countLabel = countObj.GetComponent<dfLabel>();
			dfSprite iconSprite = iconObj.GetComponent<dfSprite>();

			// If there are more than 0 of an item and the item type is 'valid'
			if( slot.Count > 0 && slot.Item != "" )
			{	
				Item item = m_ItemMap.m_Categories.FindItem( slot.Item );

				if( null == item )
				{
					Debug.LogWarning( "Failed to find item of type '" + slot.Item + "'" );
					countLabel.Text = "";
					continue;
				}
				else
				{
					iconSprite.SpriteName = item.m_Icon;
					iconSprite.Width = iconSprite.SpriteInfo.sizeInPixels.x;
					iconSprite.Height = iconSprite.SpriteInfo.sizeInPixels.y;
					countLabel.Text = slot.Count.ToString();
				}
			}
		}
	}
}
