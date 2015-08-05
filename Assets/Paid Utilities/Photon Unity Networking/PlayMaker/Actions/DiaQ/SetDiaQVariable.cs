using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("DiaQ")]
[Tooltip("Sets a DiaQ variable to a string value. Use string conversion actions for non-string data types.")]
public class SetDiaQVariable : FsmStateAction
{
	[RequiredField]
	[Tooltip("The name of the DiaQ variable to set.")]
	public FsmString variableName;

	[RequiredField]
	[Tooltip("The new value of the DiaQ variable.")]
	public FsmString value;
	
	public override void OnEnter()
	{
		DiaQ.DiaQEngine.Instance.SetDiaQVarValue( variableName.Value, value.Value );
	}

	public override void Reset()
	{
		variableName = new FsmString { UseVariable = true };
		value = new FsmString { UseVariable = true };
	}
}
