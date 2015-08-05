using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("uSequencer")]
	[Tooltip("Plays a uSequencer sequence from a specified time")]
	public class PlaySequenceFromTime : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(USSequencer))]
		[Tooltip("This is the sequence you would like to Play.")]
		public FsmOwnerDefault sequenceToPlay;
		
		[RequiredField]
		[Tooltip("This is the time you would like the sequence to start at.")]
		public FsmFloat timeToStart;
		
		public override void Reset()
		{
			timeToStart = 0.0f;
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
			
			sequence.RunningTime = timeToStart.Value;
			sequence.Play();
			
			Finish();
		}
	}
}