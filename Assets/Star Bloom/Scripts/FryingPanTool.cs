using UnityEngine;
using System.Collections;

public class FryingPanTool : PlayerTool 
{
	FryingPanTool()
	{
		InteractSelectionType = InteractSelectionTypes.Closest;
	}
	
	public override void Interact( GameObject obj )
	{
		DispatchToHandler( obj );
	}
}
