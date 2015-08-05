using UnityEngine;
using System.Collections;

public class InteractionHandler : MonoBehaviour
{
	public delegate void InteractionEventHandler( PlayerTool tool );
	public event InteractionEventHandler InteractionOccurred;

	public virtual void HandleInteraction( PlayerTool tool )
	{
		if ( InteractionOccurred != null )
			InteractionOccurred( tool );
	}
}
