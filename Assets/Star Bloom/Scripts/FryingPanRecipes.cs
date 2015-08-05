using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FryingPanRecipes : RecipeList {

	void Start () 
	{
		m_ListType = "Frying Pan Recipes";
		Recipe r1 = new Recipe();
		r1.m_Product = "Fried Rice";
		r1.m_Yield = 1;
		uint NUM_INGREDIENTS = 8;
		r1.m_Ingredients = new RecipeEntry[NUM_INGREDIENTS];
		r1.m_Ingredients[0].m_Item = "rice";
		r1.m_Ingredients[0].m_Count = 1;
		r1.m_Ingredients[1].m_Item = "olive oil";
		r1.m_Ingredients[1].m_Count = 1;
		r1.m_Ingredients[2].m_Item = "carrot";
		r1.m_Ingredients[2].m_Count = 1;
		r1.m_Ingredients[3].m_Item = "pea pod";
		r1.m_Ingredients[3].m_Count = 3;
		r1.m_Ingredients[4].m_Item = "yellow onion";
		r1.m_Ingredients[4].m_Count = 1;
		r1.m_Ingredients[5].m_Item = "green onion";
		r1.m_Ingredients[5].m_Count = 1;
		r1.m_Ingredients[6].m_Item = "salt";
		r1.m_Ingredients[6].m_Count = 1;
		r1.m_Ingredients[7].m_Item = "peppercorn pepper";
		r1.m_Ingredients[7].m_Count = 1;
		m_Recipes.Add(r1);

		Recipe r2 = new Recipe();
		r2.m_Product = "French Fries";
		r2.m_Yield = 1;
		NUM_INGREDIENTS = 4;
		r2.m_Ingredients = new RecipeEntry[NUM_INGREDIENTS];
		r2.m_Ingredients[0].m_Item = "potato";
		r2.m_Ingredients[0].m_Count = 1;
		r2.m_Ingredients[1].m_Item = "olive oil";
		r2.m_Ingredients[1].m_Count = 1;
		r2.m_Ingredients[2].m_Item = "salt";
		r2.m_Ingredients[2].m_Count = 1;
		r2.m_Ingredients[3].m_Item = "peppercorn pepper";
		r2.m_Ingredients[3].m_Count = 3;
		m_Recipes.Add(r2);

		Recipe r3 = new Recipe();
		r3.m_Product = "Lo Mein";
		r3.m_Yield = 1;
		NUM_INGREDIENTS = 6;
		r3.m_Ingredients = new RecipeEntry[NUM_INGREDIENTS];
		r3.m_Ingredients[0].m_Item = "egg noodles";
		r3.m_Ingredients[0].m_Count = 1;
		r3.m_Ingredients[1].m_Item = "olive oil";
		r3.m_Ingredients[1].m_Count = 1;
		r3.m_Ingredients[2].m_Item = "green pepper";
		r3.m_Ingredients[2].m_Count = 1;
		r3.m_Ingredients[3].m_Item = "yellow onion";
		r3.m_Ingredients[3].m_Count = 1;
		r3.m_Ingredients[4].m_Item = "salt";
		r3.m_Ingredients[4].m_Count = 1;
		r3.m_Ingredients[5].m_Item = "peppercorn pepper";
		r3.m_Ingredients[5].m_Count = 1;
		m_Recipes.Add(r3);

	}


}
