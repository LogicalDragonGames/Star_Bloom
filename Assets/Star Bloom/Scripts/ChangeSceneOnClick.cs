using UnityEngine;
using System.Collections;

public class ChangeSceneOnClick : MonoBehaviour
{
	public string m_TargetScene = "";

	public void HandleClick()
	{
		Application.LoadLevel( m_TargetScene );
	}
}