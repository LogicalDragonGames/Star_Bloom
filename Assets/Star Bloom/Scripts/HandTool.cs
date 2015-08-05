using UnityEngine;
using System.Collections;

public class HandTool : Tool
{
	HandTool()
	{
		InteractSelectionType = InteractSelectionTypes.All;
	}

	public override void Interact( GameObject obj )
	{
		DispatchToHandler( obj );
	}
}
