using UnityEngine;
using System.Collections;

public class HandTool : PlayerTool
{
	protected Item m_HeldItem;
	protected GameObject m_ItemObj;
	protected ItemMap m_ItemMap;

	public bool HasHeldItem
	{
		get { return !(m_HeldItem == null); }
	}

	HandTool()
	{
		InteractSelectionType = InteractSelectionTypes.All;
		m_HandleNullInteractions = true;
	}

	void Start()
	{
		m_ItemMap = GameObject.Find( "SceneMaster" ).GetComponent<ItemMap>();
	}

	public override void Interact( GameObject obj )
	{
		if( obj == null )
			DropHeldItem();

		DispatchToHandler( obj );
	}

	public string GetHeldItem()
	{
		if( m_HeldItem == null )
			return "";
		else
			return m_HeldItem.m_Name;
	}

	public void SetHeldItem( string itemName )
	{
		SetHeldItem( m_ItemMap.m_Categories.FindItem( itemName ) );
	}

	public void SetHeldItem( Item item )
	{
		if( item == null )
			return;

		StoreHeldItem();
		m_HeldItem = item;

		m_ItemObj = m_ItemMap.CreateItem( item ); 
		m_ItemObj.transform.parent = HandTransform;
	}

	public void StoreHeldItem()
	{
		if( m_HeldItem == null )
			return;
		
		m_HeldItem = null;
		
		if( null != m_ItemObj )
			DestroyImmediate( m_ItemObj );

	}

	public void ClearHeldItem()
	{
		if( m_HeldItem == null )
			return;

		m_HeldItem = null;

		if( null != m_ItemObj )
			DestroyImmediate( m_ItemObj );
	}

	public void DropHeldItem()
	{
		if( m_HeldItem == null )
			return;

		m_ItemObj.transform.parent = null;
		Ray ray = new Ray( m_ItemObj.transform.position, Vector3.down );
		RaycastHit hit;

		Physics.Raycast( ray, out hit );
		m_ItemObj.transform.position = hit.point;

		m_HeldItem = null;
		m_ItemObj = null;
	}
}