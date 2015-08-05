using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RecipeCategory {

	public string m_Name = "Recipe Category";
	public RecipeList m_Recipes;
	public List<RecipeCategory> m_Categories;

	private Dictionary<string, Recipe> m_RecipeMap = new Dictionary<string, Recipe>();
	private Dictionary<string, RecipeCategory> m_CategoriesMap = new Dictionary<string, RecipeCategory>();


	// Use this for initialization
	void Start () {
		// Transcribe the list of items to
		// the item map for faster lookup times
		foreach (Recipe rec in m_Recipes.m_Recipes)
		{
			rec.m_Category = this;
			m_RecipeMap.Add(rec.m_Product, rec);
		}
	
		foreach (RecipeCategory cat in m_Categories)
		{
			m_CategoriesMap.Add(cat.m_Name, cat);
		}
	}

	public RecipeCategory GetCategory(string categoryName)
	{
		return m_CategoriesMap[categoryName];
	}

	public Recipe GetRecipe(string recipeName)
	{
		return m_RecipeMap[recipeName];
	}

	public Recipe GetRecipe(string[] categoryPath, string recipeName, bool searchIfNotFound = true)
	{
		RecipeCategory category = this;
		RecipeCategory prevCat = this;

		for (uint i = 0; i < categoryPath.Length; ++i)
		{
			prevCat = category;
			category = prevCat.GetCategory(categoryPath[i]);

			// Failed to find the category listed in the path
			if (null == category)
				break;
		}

		if (null == category)
		{
			if (searchIfNotFound)
				return prevCat.SearchCatRecipeRecurse(recipeName);
			else 
				return null;
		}

		Recipe recipe = GetRecipe(recipeName);
		if (null == recipe)
		{
			if (searchIfNotFound)
				return SearchCatRecipeRecurse(recipeName);
		}

		return recipe;
	}


	public Recipe GetRecipe(RecipeCategory category, string recipeName)
	{
		return category.SearchCatRecipeRecurse(recipeName);
	}

	public Recipe FindRecipe(string recipeName)
	{
		return SearchCatRecipeRecurse(recipeName);
	}

	// Breath-firth recursive recipe/category search
	protected Recipe SearchCatRecipeRecurse(string recipeName)
	{
		Recipe myRecipe = null;
		foreach (Recipe recipe in m_Recipes.m_Recipes)
		{
			if (recipe.m_Product == recipeName)
				myRecipe = recipe;
		}

		if (null != myRecipe)
			return myRecipe;

		foreach (RecipeCategory category in m_Categories)
		{
			myRecipe = category.SearchCatRecipeRecurse(recipeName);
			if (null != myRecipe)
				return myRecipe;
		}

		return null;
	}


	public RecipeCategory FindCategory(string categoryName)
	{
		return SearchCatRecurse(categoryName);
	}

	protected RecipeCategory SearchCatRecurse(string categoryName)
	{
		RecipeCategory myCat = null;
		if (m_Name == categoryName)
			return this;

		foreach (RecipeCategory category in m_Categories)
		{
			if (category.m_Name == categoryName)
				myCat = category;
		}

		if (null != myCat)
			return myCat;

		foreach (RecipeCategory category in m_Categories)
		{
			myCat = category.SearchCatRecurse(categoryName);
			if (null != myCat)
				return myCat;
		}

		return null;
	}


}
