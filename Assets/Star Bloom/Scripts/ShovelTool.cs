﻿using UnityEngine;
using System.Collections;

public class ShovelTool : PlayerTool
{
	ShovelTool()
	{
		InteractSelectionType = InteractSelectionTypes.Closest;
	}
	
	public override void Interact( GameObject obj )
	{
		DispatchToHandler( obj );
	}
}