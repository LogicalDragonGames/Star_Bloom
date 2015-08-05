using UnityEngine;
using System.Collections;

public class HoeTool : Tool
{
	HoeTool()
	{
		InteractSelectionType = InteractSelectionTypes.Closest;
	}
	
	public override void Interact( GameObject obj )
	{
		DispatchToHandler( obj );
	}
}
