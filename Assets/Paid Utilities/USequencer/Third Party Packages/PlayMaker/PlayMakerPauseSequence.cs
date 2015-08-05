using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("uSequencer")]
	[Tooltip("Pauses a uSequencer sequence")]
	public class PauseSequence : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(USSequencer))]
		[Tooltip("This is the sequence you would like to Pause.")]
		public FsmOwnerDefault sequenceToPlay;
		
		public override void Reset()
		{
			
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
			
			sequence.Pause();
			
			Finish();
		}
	}
}