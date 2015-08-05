using UnityEngine;
using System.Collections;

[System.Serializable]
public class Recipe
{
	public string m_Product = "";
	public int m_Yield = 1;
	public RecipeEntry[] m_Ingredients = {};
	// visual representaion of recipe object
	public GameObject m_Prefab = null;

	[HideInInspector]
	public RecipeCategory m_Category = null;
}