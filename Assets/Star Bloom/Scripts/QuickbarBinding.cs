using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuickbarBinding : MonoBehaviour
{
	public dfPanel m_QuickbarPanel;
	public dfScrollPanel m_Quickbar;
	public GameObject m_QuickbarEntryPrefab;
	public dfTweenFloat m_TweenIn;
	public dfTweenFloat m_TweenOut;
	public dfTweenFloat m_TweenMoveLeft;
	public dfTweenFloat m_TweenMoveRight;

	public float m_ScrollAmount = 80.0f;
	protected float m_UnitizedOffset = 0.0f;

	protected ItemMap m_ItemMap;
	protected Inventory m_Inventory;
	protected ToolFramework m_ToolFramework;

	public float ScrollAmount
	{
		get
		{
			return m_ScrollAmount;
		}
		set
		{
			m_Quickbar.ScrollPosition = new Vector2( m_ScrollAmount, 0.0f );
			m_ScrollAmount = value;
		}
	}

	public int m_SelectedIndex = 0;
	public int m_MaxIndex = 20;

	protected enum QuickbarState
	{
		Closed,
		TransitionIn,
		TransitionOut,
		Open
	}

	protected enum QuickbarSelectionState
	{
		Idle,
		MovingLeft,
		MovingRight
	}

	protected QuickbarState m_State = QuickbarState.Closed;
	protected QuickbarSelectionState m_SelectionState = QuickbarSelectionState.Idle;


	void Start()
	{
		m_UnitizedOffset = m_TweenMoveRight.EndValue;

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
		m_Inventory = player.GetComponent<Inventory>();
		m_ToolFramework = player.GetComponent<ToolFramework>();

	}

	void Update()
	{
		switch( m_State )
		{
		case QuickbarState.Closed:

			if( Input.GetButton( "QuickbarOpen" ) )
			{
				BindPlayerInventory();
				m_TweenIn.Reset();
				m_TweenIn.Play();
				m_State = QuickbarState.TransitionIn;
				m_TweenIn.TweenCompleted += HandleTweenInCompleted;
			}

			break;

		case QuickbarState.Open:

			if( !Input.GetButton( "QuickbarOpen" ) )
			{
				m_TweenOut.Reset();
				m_TweenOut.Play();
				m_State = QuickbarState.TransitionOut;
				m_TweenOut.TweenCompleted += HandleTweenOutCompleted;
				BindEquippedItem();
			}
			else
				UpdateQuickbar();

			break;

		case QuickbarState.TransitionIn:
			break;

		case QuickbarState.TransitionOut:
			break;
		}
	}

	public void BindEquippedItem()
	{
		InventorySlot slot = m_Inventory.m_Slots[m_SelectedIndex];

		bool isTool = true;
		ItemCategory toolsCat = m_ItemMap.m_Categories.FindCategory( "Tools" );

		Item item = toolsCat.FindItem( slot.Item );

		if( item == null )
		{
			isTool = false;
			m_ToolFramework.ChangeTool( "Hand" );

			item = m_ItemMap.m_Categories.FindItem( slot.Item );

			// TODO: Equippable items
			//if( null != item )
			//{
			//}
		}
		else
		{
			m_ToolFramework.ChangeTool( slot.Item );
		}
	}

	public void ClearQuickbar()
	{
		List<GameObject> children = new List<GameObject>();
		
		foreach (Transform child in transform) 
			children.Add(child.gameObject);
		
		for( int i = 0; i < children.Count; ++i )
			DestroyImmediate( children[i] );
	}

	public void AddEmptyEntry()
	{
		AddEntry( "Invalid", 0.0f );
	}

	public void AddEntry( string spriteName, float opacity = 1.0f )
	{
		dfControl slotControl = m_Quickbar.AddPrefab( m_QuickbarEntryPrefab );
		Transform slot = slotControl.transform;
		dfSprite sprite = slot.GetComponent<dfSprite>();

		sprite.SpriteName = spriteName;
		sprite.Opacity = opacity;
	}

	int GetIndexScrollAmt( int index )
	{
		return (int)(index*m_UnitizedOffset);
	}

	public void BindPlayerInventory()
	{
		ClearQuickbar();

		AddEmptyEntry();

		foreach( InventorySlot slot in m_Inventory.m_Slots )
		{
			Item item = m_ItemMap.m_Categories.FindItem( slot.Item );
			
			if( null == item )
			{
				Debug.LogWarning( "Failed to find item of type '" + slot.Item + "'" );
				AddEntry( "Invalid" );
				continue;
			}

			AddEntry( item.m_Icon );
		}

		m_MaxIndex = m_Inventory.m_Slots.Count;

		AddEmptyEntry();

		ScrollAmount = GetIndexScrollAmt( m_SelectedIndex );
	}

	void HandleTweenInCompleted( dfTweenPlayableBase sender )
	{
		m_State = QuickbarState.Open;
		m_TweenIn.TweenCompleted -= HandleTweenInCompleted;
	}

	void HandleTweenOutCompleted( dfTweenPlayableBase sender )
	{
		m_State = QuickbarState.Closed;
		m_TweenOut.TweenCompleted -= HandleTweenOutCompleted;
	}

	void UpdateQuickbar()
	{
		switch (m_SelectionState)
		{
		case QuickbarSelectionState.Idle:

			if( Input.GetButtonDown( "QuickbarMoveLeft" ) )
			{
				--m_SelectedIndex;
				if( m_SelectedIndex < 0 )
					m_SelectedIndex = m_MaxIndex-1;

				m_TweenMoveLeft.EndValue = GetIndexScrollAmt( m_SelectedIndex );
				m_TweenMoveLeft.Play();
				//m_TweenMoveLeft.TweenCompleted += HandleTweenMoveLeftCompleted;
				//m_SelectionState = QuickbarSelectionState.MovingLeft;
			}
			else if( Input.GetButtonDown( "QuickbarMoveRight" ) )
			{
				++m_SelectedIndex;
				if( m_SelectedIndex >= (m_MaxIndex-1) )
					m_SelectedIndex = 0;

				m_TweenMoveRight.EndValue = GetIndexScrollAmt( m_SelectedIndex );
				m_TweenMoveRight.Play();
				//m_TweenMoveRight.TweenCompleted += HandleTweenMoveRightCompleted;
				//m_SelectionState = QuickbarSelectionState.MovingRight;
			}

			break;

		case QuickbarSelectionState.MovingLeft:
			break;
		
		case QuickbarSelectionState.MovingRight:
			break;
		}
	}

	void HandleTweenMoveLeftCompleted( dfTweenPlayableBase sender )
	{
		--m_SelectedIndex;
		m_TweenMoveLeft.TweenCompleted -= HandleTweenMoveLeftCompleted;
		m_SelectionState = QuickbarSelectionState.Idle;
	}
	
	void HandleTweenMoveRightCompleted (dfTweenPlayableBase sender)
	{
		++m_SelectedIndex;
		m_TweenMoveRight.TweenCompleted -= HandleTweenMoveRightCompleted;
		m_SelectionState = QuickbarSelectionState.Idle;
	}
}