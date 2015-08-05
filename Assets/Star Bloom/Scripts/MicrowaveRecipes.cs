using UnityEngine;
using System.Collections;

public class MicrowaveRecipes : RecipeList {

	// Use this for initialization
	void Start () 
	{
		m_ListType = "Microwave Recipes";
		Recipe r1 = new Recipe();
		r1.m_Product = "Chips and Queso";
		r1.m_Yield = 1;
		uint NUM_INGREDIENTS = 2;
		r1.m_Ingredients = new RecipeEntry[NUM_INGREDIENTS];
		r1.m_Ingredients[0].m_Item = "Tortilla Chips";
		r1.m_Ingredients[0].m_Count = 1;
		r1.m_Ingredients[1].m_Item = "Queso Dip";
		r1.m_Ingredients[1].m_Count = 1;
		m_Recipes.Add(r1);
	}
}
