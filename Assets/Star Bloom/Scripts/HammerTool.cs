using UnityEngine;
using System.Collections;

public class HammerTool : PlayerTool
{
	HammerTool()
	{
	}
	
	public override void Interact( GameObject obj )
	{
		DispatchToHandler( obj );
	}
}
