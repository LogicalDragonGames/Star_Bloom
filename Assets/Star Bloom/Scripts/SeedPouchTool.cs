using UnityEngine;
using System.Collections;

public class SeedPouchTool : PlayerTool
{
	public GameObject m_CropPrefab = null;

	SeedPouchTool()
	{
		InteractSelectionType = InteractSelectionTypes.Closest;
	}
	
	public override void Interact( GameObject obj )
	{
		DispatchToHandler( obj );
	}
}
