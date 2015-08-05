using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("StarBloom")]
[Tooltip("Gets information about the target relationship")]
public class GetRelationship : FsmStateAction
{
	[RequiredField]
	[Tooltip("Object with a Relationship component")]		
	public FsmOwnerDefault npcObject;
	
	[UIHint(UIHint.Tag)]
	[Tooltip("Current relationship category")]		
	public FsmInt category;
	
	[UIHint(UIHint.Tag)]
	[Tooltip("Max number of categories in the current relationship")]		
	public FsmInt maxCategories;

	[UIHint(UIHint.Tag)]
	[Tooltip("Progress into the current category")]		
	public FsmFloat progress;
	
	[UIHint(UIHint.Tag)]
	[Tooltip("Max progress of the current category")]		
	public FsmFloat maxProgress;

	public override void Reset ()
	{
		npcObject = null;
		category = null;
		maxCategories = null;
		progress = null;
		maxProgress = null;
	}

	public override void OnEnter()
	{
		NPC npc = Fsm.GetOwnerDefaultTarget(npcObject).GetComponent<NPC>();
		
		if( null != category )
			category.Value = npc.m_Relationship.m_CurrentCategory;

		if( null != maxCategories )
			maxCategories.Value = npc.m_Relationship.m_Categories.Length;

		if( null != progress )
			progress.Value = npc.m_Relationship.m_CurrentProgress;
		
		if( null != maxProgress )
			maxProgress.Value = npc.m_Relationship.m_Categories[npc.m_Relationship.m_CurrentCategory].m_MaxProgress;

		Finish();
	}
}