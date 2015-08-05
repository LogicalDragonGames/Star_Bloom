using UnityEngine;
using HutongGames.PlayMaker;

[ActionCategory("StarBloom")]
[Tooltip("Enables or disables player movement")]
public class SetPlayerMovementLock : FsmStateAction
{
	[RequiredField]
	[Tooltip("Whether or not the player's movement is locked.")]
	public FsmGameObject player;

	[RequiredField]
	[Tooltip("Whether or not the player's movement is locked.")]
	public FsmBool movementLocked = false;

	public override void OnEnter()
	{
		PlayerMovementController controller = player.Value.GetComponent<PlayerMovementController>();
		controller.SetDisabled( movementLocked.Value );
	}
}
