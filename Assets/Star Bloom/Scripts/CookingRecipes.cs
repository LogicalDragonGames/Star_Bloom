using UnityEngine;
using System.Collections;

public class CookingRecipes : RecipeList 
{



	// Use this for initialization
	void Start () 
	{
		InteractionHandler ih = GetComponent<InteractionHandler>();

		if (ih)
			ih.InteractionOccurred += OnInteraction;
		else
			Debug.LogError("Could not find 'InteractionHandler' component");
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	public void OnInteraction(PlayerTool tool)
	{
		Debug.Log("I'm interacting");

		if (tool.GetType() == typeof(HandTool))
		{
			PrintAllRecipes();
		}
		else if (tool.GetType() == typeof(HoeTool))
		{
			Console.Instance.addGameChatMessage("I ain't yo ho ah nu uh!");
		}
		else if (tool.GetType() == typeof(FryingPanTool))
		{
			//PrintRecipes();
			RecipeMap rm = RecipeMap.Instance;
			RecipeCategory rc = rm.m_Categories.FindCategory("FryingPan");
			PrintRecipesInCategory(rc);
		}
	}

	public void FindRecipe()
	{
		if (Random.Range(0, 2) == 0)
			Console.Instance.addGameChatMessage("If you wanna bake a pretty cake");
		else
			Console.Instance.addGameChatMessage("Gatta learn cooking by the book");

	}

	public void PrintAllRecipes()
	{
		RecipeMap rm = RecipeMap.Instance;

		PrintRecipiesRecursive(rm.m_Categories);
	}

	//public void PrintRecipes()
	//{
	//    Console.Instance.addGameChatMessage(m_ListType + " Recipes");
	//    foreach (Recipe rec in m_Recipes)
	//    {
	//        Console.Instance.addGameChatMessage(rec.ToString());
	//    }
	//}

	public void PrintRecipesInCategory(RecipeCategory rc)
	{
		Console.Instance.addGameChatMessage(rc.m_Name + ":\n");
		foreach(Recipe recipe in rc.m_Recipes.m_Recipes)
		{
			Console.Instance.addGameChatMessage(recipe.m_Product + "(" + recipe.m_Yield.ToString() + ")\nIngredients:\n");
			foreach (RecipeEntry ing in recipe.m_Ingredients)
			{
				Console.Instance.addGameChatMessage(ing.m_Item + "(" + ing.m_Count.ToString() + ")\n");
			}
		}
	}

	public void PrintRecipiesRecursive(RecipeCategory rc)
	{
		Console.Instance.addGameChatMessage(rc.m_Name + ":\n");
		foreach (Recipe recipe in rc.m_Recipes.m_Recipes)
		{
			Console.Instance.addGameChatMessage(recipe.m_Product + "(" + recipe.m_Yield.ToString() + ")\nIngredients:\n");
			foreach (RecipeEntry ing in recipe.m_Ingredients)
			{
				Console.Instance.addGameChatMessage(ing.m_Item + "(" + ing.m_Count.ToString() + ")\n");
			}
		}

		foreach (RecipeCategory rec in rc.m_Categories)
		{
			PrintRecipiesRecursive(rec);
		}
	}

}


