using UnityEngine;
using System.Collections;

public class SeedPouchTool : Tool
{
	SeedPouchTool()
	{
		InteractSelectionType = InteractSelectionTypes.Closest;
	}
	
	public override void Interact( GameObject obj )
	{
		DispatchToHandler( obj );
	}
}
