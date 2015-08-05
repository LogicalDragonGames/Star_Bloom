using UnityEngine;
using System.Collections;

public class Diary : MonoBehaviour
{


	// Use this for initialization
	void Start()
	{
		InteractionHandler ih = GetComponent<InteractionHandler>();

		if( null == ih )
		{
			Debug.LogError( "Could not locate 'InteractionHandler' component" );
			return;
		}

		ih.InteractionOccurred += OnInteraction;
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void OnInteraction( Tool tool )
	{
			Console.Instance.addGameChatMessage( "Saving..." );
		
		if( tool.GetType() == typeof( HandTool ) )
		{
		}
	}
}