using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("uSequencer")]
	[Tooltip("Sets the current time of a uSequencer sequence")]
	public class SetSequenceTime : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(USSequencer))]
		[Tooltip("This is the sequence you would like to Set the time on.")]
		public FsmOwnerDefault sequenceToPlay;
		
		[RequiredField]
		[Tooltip("This is the time you would like to set.")]
		public FsmFloat sequenceTime;
		
		public override void Reset()
		{
			sequenceTime = 0.0f;
		}
		
		public override void OnEnter()
		{
			if(Fsm == null)
				return;
			
			GameObject go = Fsm.GetOwnerDefaultTarget(sequenceToPlay);
			if(!go)
				return;
				
			USSequencer sequence = go.GetComponent<USSequencer>();
			if(!go)
				return;
			
			sequence.RunningTime = sequenceTime.Value;
			
			Finish();
		}
	}
}