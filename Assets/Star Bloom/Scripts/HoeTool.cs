using UnityEngine;
using System.Collections;

public class HoeTool : PlayerTool
{	
	public override void Interact( GameObject obj )
	{
		DispatchToHandler( obj );
	}
}
