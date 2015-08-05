using UnityEngine;
using System.Collections;

public class OvenRecipes : RecipeList {

	// Use this for initialization
	void Start () 
	{
		m_ListType = "Oven Recipes";
		Recipe r1 = new Recipe();
		r1.m_Product = "Baked Potato";
		r1.m_Yield = 1;
		uint NUM_INGREDIENTS = 3;
		r1.m_Ingredients = new RecipeEntry[NUM_INGREDIENTS];
		r1.m_Ingredients[0].m_Item = "russet potato";
		r1.m_Ingredients[0].m_Count= 1;
		r1.m_Ingredients[1].m_Item = "sour creme";
		r1.m_Ingredients[1].m_Count = 1;
		r1.m_Ingredients[2].m_Item = "green onion";
		r1.m_Ingredients[2].m_Count = 1;
		m_Recipes.Add(r1);

		Recipe r2 = new Recipe();
		r2.m_Product = "Queso Dip";
		r2.m_Yield = 1;
		NUM_INGREDIENTS = 3;
		r2.m_Ingredients = new RecipeEntry[NUM_INGREDIENTS];
		r2.m_Ingredients[0].m_Item = "american cheese";
		r2.m_Ingredients[0].m_Count = 1;
		r2.m_Ingredients[1].m_Item = "tomato";
		r2.m_Ingredients[1].m_Count = 1;
		r2.m_Ingredients[2].m_Item = "habanero pepper";
		r2.m_Ingredients[2].m_Count = 1;
		m_Recipes.Add(r2);

		Recipe r3 = new Recipe();
		r3.m_Product = "Tortilla Chips";
		r3.m_Yield = 1;
		NUM_INGREDIENTS = 5;
		r3.m_Ingredients = new RecipeEntry[NUM_INGREDIENTS];
		r3.m_Ingredients[0].m_Item = "flour";
		r3.m_Ingredients[0].m_Count = 1;
		r3.m_Ingredients[1].m_Item = "egg";
		r3.m_Ingredients[1].m_Count = 1;
		r3.m_Ingredients[2].m_Item = "olive oil";
		r3.m_Ingredients[2].m_Count = 1;
		r3.m_Ingredients[3].m_Item = "salt";
		r3.m_Ingredients[3].m_Count = 1;
		r3.m_Ingredients[4].m_Item = "pepper";
		r3.m_Ingredients[4].m_Count = 1;
		m_Recipes.Add(r3);


	}
	
}
