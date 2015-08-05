using UnityEngine;
using System.Collections;

public class Obtainable : InteractionHandler
{
	public string m_ItemName;

	public override void HandleInteraction (PlayerTool tool)
	{
		if( tool.GetType() == typeof(HandTool) )
		{
			HandTool hand = (HandTool)tool;

			Inventory inventory = GameObject.Find( "Player" ).GetComponent<Inventory>();
			inventory.Add( m_ItemName );
			DestroyImmediate( this.gameObject );

			/*
			string heldItem = hand.GetHeldItem();

			if( heldItem == "" )
			{
				hand.SetHeldItem( m_ItemName );

				// TODO: Use this item instead of creating a duplicate
			}*/
		}
	}
}
