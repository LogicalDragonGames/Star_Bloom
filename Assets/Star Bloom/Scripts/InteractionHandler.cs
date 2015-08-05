using UnityEngine;
using System.Collections;

public class InteractionHandler : MonoBehaviour
{
	public delegate void InteractionEventHandler( Tool tool );
	public InteractionEventHandler InteractionOccurred;

	public void HandleInteraction( Tool tool )
	{
		if ( InteractionOccurred != null )
			InteractionOccurred( tool );
	}
}
