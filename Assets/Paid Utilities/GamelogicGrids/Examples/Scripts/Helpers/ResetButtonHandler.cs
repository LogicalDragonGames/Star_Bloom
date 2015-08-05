//----------------------------------------------//
// Gamelogic Grids                              //
// http://www.gamelogic.co.za                   //
// Copyright (c) 2013 Gamelogic (Pty) Ltd       //
//----------------------------------------------//

using UnityEngine;

public class ResetButtonHandler : GLMonoBehaviour
{
	public GameObject resetable;
	
	public void OnClick()
	{
		(resetable.GetComponent(typeof(IResetable)) as IResetable).Reset();
	}
}
