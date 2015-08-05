using UnityEngine;
using System.Collections;

public class SaveSlot : MonoBehaviour
{
	public dfPanel m_SaveDialog;
	public dfButton m_SaveButton;

	SaveDialogEventHandler m_SaveScript;

	void Start()
	{
		m_SaveScript = m_SaveDialog.GetComponent<SaveDialogEventHandler>();
		m_SaveDialog.IsVisibleChanged += HandleSaveDialogVisibleChanged;
		m_SaveButton.Click += HandleClick;
	}

	void OnDestroy()
	{
		if( null != m_SaveDialog )
			m_SaveDialog.IsVisibleChanged -= HandleSaveDialogVisibleChanged;

		if( null != m_SaveButton )
			m_SaveButton.Click -= HandleClick;
	}

	public void HandleClick (dfControl control, dfMouseEventArgs mouseEvent)
	{
		m_SaveScript.m_CurrentSaveSlot = this;
		m_SaveDialog.IsVisible = true;
	}

	public void HandleSaveDialogVisibleChanged( dfControl control, bool visible )
	{
		m_SaveButton.IsInteractive = !visible;
	}

	void OnSaveMenuOpened()
	{
		LoadHeader();
	}

	void LoadHeader()
	{
		Transform slotName = transform.Find( "SlotName" );
		dfLabel slotNameLabel = slotName.GetComponent<dfLabel>();

		SaveHeader saveHeader = new SaveHeader();
		bool loadedHeader = SaveManager.Instance.LoadHeader( slotNameLabel.Text, ref saveHeader );

		if( !loadedHeader )
		{
			return;
		}
	}
}
