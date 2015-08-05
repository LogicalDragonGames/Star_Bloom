using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("StarBloom")]
[Tooltip("Sets information about the target relationship")]
public class SetRelationship : FsmStateAction
{
	[RequiredField]
	[Tooltip("Object with a Relationship component")]		
	public FsmOwnerDefault npcObject;
	
	[Tooltip("Current relationship category")]		
	public FsmInt category;
	
	[Tooltip("Max number of categories in the current relationship")]		
	public FsmInt maxCategories;
	
	[Tooltip("Progress into the current category")]		
	public FsmFloat progress;
	
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
			npc.m_Relationship.m_CurrentCategory = category.Value;
		
		if( null != maxCategories )
			System.Array.Resize( ref npc.m_Relationship.m_Categories, maxCategories.Value );
		
		if( null != progress )
			npc.m_Relationship.m_CurrentProgress = progress.Value;
		
		if( null != maxProgress )
			npc.m_Relationship.m_Categories[npc.m_Relationship.m_CurrentCategory].m_MaxProgress = maxProgress.Value;
		
		Finish();
	}
}