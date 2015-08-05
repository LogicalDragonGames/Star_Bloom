using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("DiaQ")]
[Tooltip("Gete a DiaQ variable as a string. Use convert actions to translate to other data types.")]
public class GetDiaQVariable : FsmStateAction
{
	[Tooltip("The name of the DiaQ variable to retrieve.")]
	public FsmString variableName;
	
	[Tooltip("Sore the result in a string.")]
	[UIHint(UIHint.Variable)]
	public FsmString storeResult;

	public override void OnEnter()
	{
		storeResult.Value = DiaQ.DiaQEngine.Instance.GetDiaQVarValue( variableName.Value );
	}

	public override void Reset()
	{
		variableName = new FsmString { UseVariable = true };
		storeResult = new FsmString { UseVariable = true };
	}
}
