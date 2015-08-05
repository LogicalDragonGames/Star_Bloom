using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIDecisionBinding : MonoBehaviour
{
	public dfPanel m_Panel = null;
	public dfScrollPanel m_Container = null;
	public GameObject m_OptionPrefab = null;
	public dfTweenFloat m_TweenIn = null;
	public dfTweenFloat m_TweenOut = null;

	public delegate void ButtonClickedHandler();
	public event ButtonClickedHandler OptionSelected;

	protected dfControl m_SelectedOption = null;
	protected Color m_OptionNormalTextColor = Color.white;
	protected Color m_OptionHoverTextColor = Color.white;
	protected Color m_OptionFocusTextColor = Color.white;
	protected Color m_OptionPressedTextColor = Color.white;
	protected Color m_OptionDisabledTextColor = Color.white;

	public void ClearOptions()
	{
		List<GameObject> children = new List<GameObject>();
		
		foreach( Transform child in transform ) 
			children.Add( child.gameObject );
		
		for( int i = 0; i < children.Count; ++i )
			DestroyImmediate( children[i] );
	}

	public void SetBackgroundSprite( string spriteName )
	{
		m_Container.BackgroundSprite = spriteName;
	}

	public void SetBackgroundColor( Color color )
	{
		m_Container.BackgroundColor = color;
	}

	public void SetOptionTextColor( Color normalColor, Color hoverColor, Color focusColor, Color pressedColor, Color disabledColor )
	{
		m_OptionNormalTextColor = normalColor;
	    m_OptionHoverTextColor = hoverColor;
	    m_OptionFocusTextColor = focusColor;
	    m_OptionPressedTextColor = pressedColor;
	    m_OptionDisabledTextColor = disabledColor;

		foreach( Transform child in transform )
		{
			dfButton btn = child.GetComponent<dfButton>();

			if( null != btn )
			{
				btn.TextColor = normalColor;
				btn.HoverTextColor = hoverColor;
				btn.FocusTextColor = focusColor;
				btn.PressedTextColor = pressedColor;
				btn.DisabledTextColor = disabledColor;
			}
		}
	}

	public void SetOptions( string[] options )
	{
		ClearOptions();

		foreach( string option in options )
		{
			dfButton optionControl = (dfButton) m_Container.AddPrefab( m_OptionPrefab );
			optionControl.Text = option;
			optionControl.TextColor = m_OptionNormalTextColor;
			optionControl.HoverTextColor = m_OptionHoverTextColor;
			optionControl.FocusTextColor = m_OptionFocusTextColor;
			optionControl.PressedTextColor = m_OptionPressedTextColor;
			optionControl.DisabledTextColor = m_OptionDisabledTextColor;
			optionControl.Click += HandleOptionClicked;
		}
	}

	void HandleOptionClicked( dfControl control, dfMouseEventArgs mouseEvent )
	{
		m_SelectedOption = control;

		if( null != OptionSelected )
			OptionSelected();
	}

	public int GetCurrentOptionIndex()
	{
		for( int i = 0; i < m_Container.transform.childCount; ++i )
		{
			dfControl control = m_Container.transform.GetChild(i).GetComponent<dfControl>();
			if( control == m_SelectedOption )
				return i;
		}

		return -1;
	}

	public void Show()
	{
		m_Panel.enabled = true;
		m_Panel.IsVisible = true;
		m_TweenIn.Play();
	}

	public void Hide()
	{
		m_TweenOut.Play();
		m_TweenOut.TweenCompleted += HandleHideComplete;
	}

	void HandleHideComplete (dfTweenPlayableBase sender)
	{
		m_Panel.IsVisible = false;
		m_Panel.enabled = false;
		m_TweenOut.TweenCompleted -= HandleHideComplete;
	}
}
