using UnityEngine;
using System.Collections;

[System.Serializable]
public class Item
{
	public string m_Name = "";
	public string m_Icon = "Invalid";
	public GameObject m_Prefab = null;

	[HideInInspector]
	public ItemCategory m_Category = null;
}