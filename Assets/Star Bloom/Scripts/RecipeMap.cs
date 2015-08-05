using UnityEngine;
using System.Collections;

public class RecipeMap : Singleton<RecipeMap> 
{
	public RecipeCategory m_Categories = new RecipeCategory();


	// Use this for initialization
	void Start () {
		m_Categories.m_Name = "All Recipes";
		
		RecipeCategory rc1 = new RecipeCategory();
		rc1.m_Name = "Frying Pan Recipes";
		rc1.m_Recipes = new FryingPanRecipes();
		m_Categories.m_Categories.Add(rc1);

		RecipeCategory rc2 = new RecipeCategory();
		rc2.m_Name = "Oven Recipes";
		rc2.m_Recipes = new OvenRecipes();
		m_Categories.m_Categories.Add(rc2);

		RecipeCategory rc3 = new RecipeCategory();
		rc3.m_Name = "Microwave Recipes";
		rc3.m_Recipes = new MicrowaveRecipes();
		m_Categories.m_Categories.Add(rc3);
	}
	
	// used for creating a recipe
	public GameObject CreateRecipe(string[] categoryPath, string recipeName)
	{
		Recipe recipe = m_Categories.GetRecipe(categoryPath, recipeName);
		if (null == recipe)
			return null;

		if (null == recipe.m_Prefab)
			return null;

		return (GameObject)Instantiate(recipe.m_Prefab);
	}


	public GameObject CreateRecipe(RecipeCategory category, string recipeName)
	{
		Recipe recipe = m_Categories.GetRecipe(category, recipeName);
		if (null == recipe)
			return null;

		if (null == recipe.m_Prefab)
			return null;

		return (GameObject)Instantiate(recipe.m_Prefab);
	}

}
