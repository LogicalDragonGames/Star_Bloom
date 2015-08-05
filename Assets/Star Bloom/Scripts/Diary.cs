using UnityEngine;
using System.Collections;

public class Diary : MonoBehaviour
{
	protected GameObject m_GUI = null;
	protected dfPanel m_SaveMenu =  null;
	protected SaveMenuHandler m_SaveMenuScript = null;

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

		m_GUI = GameObject.Find( "GUI" );
		if( null == m_GUI )
		{
			Debug.LogError( "Could not locate 'GUI' object" );
			return;
		}

		Transform smgo = m_GUI.transform.FindChild( "SaveMenu" );
		if( null == smgo )
		{
			Debug.LogError( "Could not locate 'SaveDialog' game object" );
			return;
		}

		m_SaveMenu = smgo.gameObject.GetComponent<dfPanel>();
		if( null == m_SaveMenu )
		{
			Debug.LogError( "Could not locate 'SaveDialog' component" );
			return;
		}

		m_SaveMenuScript = smgo.GetComponent<SaveMenuHandler>();
	}
	
	// Update is called once per frame
	void Update()
	{
	}

	void OnInteraction( PlayerTool tool )
	{
		if( tool.GetType() == typeof( HandTool ) )
		{
			m_SaveMenuScript.ShowMenu();
		}
	}
}