using UnityEngine;
using System.Collections;

public class FishingRodTool : PlayerTool
{
	public override void Interact( GameObject obj )
	{
		DispatchToHandler( obj );
	}
}
