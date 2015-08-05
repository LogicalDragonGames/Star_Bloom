using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("uSequencer")]
	[Tooltip("Plays a uSequencer sequence")]
	public class PlaySequence : FsmStateAction
	{
		[RequiredField]
		[CheckForComponent(typeof(USSequencer))]
		[Tooltip("This is the sequence you would like to Play.")]
		public FsmOwnerDefault sequenceToPlay;
		
		[RequiredField]
		[Tooltip("Use this flag if you'd like to start the sequence from the beginning.")]
		public FsmBool startFromBeginning = false;

		
		[Tooltip("This event is fired when playback is finished.")]
		public FsmEvent PlaybackFinished;

		public override void Reset()
		{
			startFromBeginning = false;
			PlaybackFinished = null;
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
			
			if(startFromBeginning.Value)
				sequence.Stop();
			
			sequence.PlaybackFinished += OnPlaybackFinished;
			sequence.Play();

			
			Finish();
		}

		protected void OnPlaybackFinished()
		{
			if( null == PlaybackFinished )
				return;

			Fsm.Event ( PlaybackFinished );
		}
	}
}