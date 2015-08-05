using UnityEngine;
using System.Collections;

public class SceneName : MonoBehaviour
{
	public string m_SceneName = "";

	// Use this for initialization
	void Start()
	{
		if( m_SceneName == "" )
			m_SceneName = Application.loadedLevelName;
	}
}
