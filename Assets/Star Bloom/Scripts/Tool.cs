using UnityEngine;
using System.Collections;

public class Tool : MonoBehaviour
{
	public Vector3 HandOffset = Vector3.zero;
	public Vector3 HandRotation = Vector3.zero;
	public Vector3 HandScale = Vector3.one;
	public string HandName = "Dwarf/Bip003/Bip003 Pelvis/Bip003 Spine/Bip003 Spine1/Bip003 Neck/Bip003 R Clavicle/Bip003 R UpperArm/Bip003 R Forearm/Bip003 R Hand";
	protected Transform HandTransform = null;

	public enum InteractSelectionTypes
	{
		Closest = 0,
		All
	};

	public InteractSelectionTypes InteractSelectionType = InteractSelectionTypes.Closest;

	public void Start()
	{
	}

	public void Attach()
	{
		GameObject player = GameObject.FindGameObjectWithTag( "Player" );
		
		if( null == player )
		{
			Debug.LogWarning( "Could not find player object" );
			return;
		}
		
		HandTransform = player.transform.FindChild( HandName );
		
		if( null == HandTransform )
		{
			Debug.LogWarning( "Could not find hand bone" );
			return;
		}

		this.transform.parent = HandTransform;

		transform.localPosition += HandOffset;

		Quaternion rot = transform.localRotation;
		rot.eulerAngles += HandRotation;
		transform.localRotation = rot;

		Vector3 scale = transform.localScale;
		scale.x *= HandScale.x;
		scale.y *= HandScale.y;
		scale.z *= HandScale.z;
		transform.localScale = scale;
	}

	public void Detach()
	{
		Destroy( this.gameObject );
	}

	// This is called whenever the player interacts with an object
	// If the interact selection type is set to Closest, the single
	// argument convenience function is piped
	public void Interact( ArrayList objects )
	{
		foreach( GameObject obj in objects )
			Interact( obj );
	}

	public virtual void Interact( GameObject obj )
	{
		InteractionHandler ih = obj.GetComponent<InteractionHandler>();

		if( null == ih )
			ih = obj.transform.root.GetComponent<InteractionHandler>();

		if( null != ih )
			ih.HandleInteraction( this );
	}

	public virtual void DispatchToHandler( GameObject obj )
	{
		if( null == obj )
			return;

		InteractionHandler ih = obj.GetComponent<InteractionHandler>();

		if( null == ih )
			ih = obj.transform.root.GetComponentInChildren<InteractionHandler>();

		if( null != ih )
			ih.HandleInteraction( this );
	}
}
