using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("StarBloom")]
[Tooltip("Adds progress (positive or negative) to the target relationship")]
public class AddRelationshipProgress : FsmStateAction
{
	[RequiredField]
	[Tooltip("Object with a Relationship component")]		
	public FsmOwnerDefault npcObject;
	
	[Tooltip("Adds the specified amount to the current relationship progress")]		
	public FsmFloat progress;

	public override void Reset ()
	{
		npcObject = null;
		progress = null;
	}
	
	public override void OnEnter()
	{
		NPC npc = Fsm.GetOwnerDefaultTarget(npcObject).GetComponent<NPC>();

		npc.m_Relationship.AddProgress( progress.Value );

		Finish();
	}
}