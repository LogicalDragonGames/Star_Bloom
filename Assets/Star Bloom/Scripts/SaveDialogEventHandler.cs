using UnityEngine;
using System.Collections;

public class SaveDialogEventHandler : MonoBehaviour
{
	public SaveSlot m_CurrentSaveSlot;
	public bool WasVisible = false;
	protected GameObject m_SceneMaster;
	protected dfPanel m_SDPanel;

	public void Start()
	{
		m_SceneMaster = GameObject.Find( "SceneMaster" );
		if( null == m_SceneMaster )
		{
			Debug.LogError( "Failed to find 'SceneMaster' object" );
			return;
		}

		m_SDPanel = GetComponent<dfPanel>();
	}

	public void Update()
	{
		WasVisible = m_SDPanel.IsVisible;

		if( m_SDPanel.IsVisible )
		{
			if( Input.GetButtonDown( "Back" ) )
			{
				m_SDPanel.IsVisible = false;
			}
		}
	}

	public void OnClick( dfControl ignore, dfMouseEventArgs args )
	{
		if( args.Buttons != dfMouseButtons.Left )
			return;

		if( args.Used )
			return;

		if( args.Clicks > 1 )
			return;

		if( args.Buttons != dfMouseButtons.Left )
			return;

		string slotName = m_CurrentSaveSlot.transform.Find( "SlotName" ).GetComponent<dfLabel>().Text;

		switch( args.Source.name )
		{
		case "SaveBtn":
			Console.Instance.addGameChatMessage( "Saving '" + slotName + "'..." );
			SaveManager.Instance.Save( slotName );
			m_SDPanel.IsVisible = false;
			break;

		case "LoadBtn":
			Console.Instance.addGameChatMessage( "Loading '" + slotName + "'..." );
			SaveManager.Instance.Load( slotName );
			m_SDPanel.IsVisible = false;
			break;
		}
	}
}
