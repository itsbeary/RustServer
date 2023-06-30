using System;
using System.Collections.Generic;

// Token: 0x0200075F RID: 1887
public static class RecipeDictionary
{
	// Token: 0x0600348E RID: 13454 RVA: 0x00144938 File Offset: 0x00142B38
	public static void CacheRecipes(RecipeList recipeList)
	{
		if (recipeList == null)
		{
			return;
		}
		Dictionary<int, List<Recipe>> dictionary;
		if (RecipeDictionary.recipeListsDict.TryGetValue(recipeList.FilenameStringId, out dictionary))
		{
			return;
		}
		Dictionary<int, List<Recipe>> dictionary2 = new Dictionary<int, List<Recipe>>();
		RecipeDictionary.recipeListsDict.Add(recipeList.FilenameStringId, dictionary2);
		foreach (Recipe recipe in recipeList.Recipes)
		{
			List<Recipe> list = null;
			if (!dictionary2.TryGetValue(recipe.Ingredients[0].Ingredient.itemid, out list))
			{
				list = new List<Recipe>();
				dictionary2.Add(recipe.Ingredients[0].Ingredient.itemid, list);
			}
			list.Add(recipe);
		}
	}

	// Token: 0x0600348F RID: 13455 RVA: 0x001449E8 File Offset: 0x00142BE8
	public static Recipe GetMatchingRecipeAndQuantity(RecipeList recipeList, List<Item> orderedIngredients, out int quantity)
	{
		quantity = 0;
		if (recipeList == null)
		{
			return null;
		}
		if (orderedIngredients == null || orderedIngredients.Count == 0)
		{
			return null;
		}
		List<Recipe> recipesByFirstIngredient = RecipeDictionary.GetRecipesByFirstIngredient(recipeList, orderedIngredients[0]);
		if (recipesByFirstIngredient == null)
		{
			return null;
		}
		foreach (Recipe recipe in recipesByFirstIngredient)
		{
			if (!(recipe == null) && recipe.Ingredients.Length == orderedIngredients.Count)
			{
				bool flag = true;
				int num = int.MaxValue;
				int num2 = 0;
				foreach (Recipe.RecipeIngredient recipeIngredient in recipe.Ingredients)
				{
					Item item = orderedIngredients[num2];
					if (recipeIngredient.Ingredient != item.info || item.amount < recipeIngredient.Count)
					{
						flag = false;
						break;
					}
					int num3 = item.amount / recipeIngredient.Count;
					if (num2 == 0)
					{
						num = num3;
					}
					else if (num3 < num)
					{
						num = num3;
					}
					num2++;
				}
				if (flag)
				{
					quantity = num;
					if (quantity > 1 && !recipe.CanQueueMultiple)
					{
						quantity = 1;
					}
					return recipe;
				}
			}
		}
		return null;
	}

	// Token: 0x06003490 RID: 13456 RVA: 0x00144B34 File Offset: 0x00142D34
	private static List<Recipe> GetRecipesByFirstIngredient(RecipeList recipeList, Item firstIngredient)
	{
		if (recipeList == null)
		{
			return null;
		}
		if (firstIngredient == null)
		{
			return null;
		}
		Dictionary<int, List<Recipe>> dictionary;
		if (!RecipeDictionary.recipeListsDict.TryGetValue(recipeList.FilenameStringId, out dictionary))
		{
			RecipeDictionary.CacheRecipes(recipeList);
		}
		if (dictionary == null)
		{
			return null;
		}
		List<Recipe> list;
		if (!dictionary.TryGetValue(firstIngredient.info.itemid, out list))
		{
			return null;
		}
		return list;
	}

	// Token: 0x06003491 RID: 13457 RVA: 0x00144B88 File Offset: 0x00142D88
	public static bool ValidIngredientForARecipe(Item ingredient, RecipeList recipeList)
	{
		if (ingredient == null)
		{
			return false;
		}
		if (recipeList == null)
		{
			return false;
		}
		foreach (Recipe recipe in recipeList.Recipes)
		{
			if (!(recipe == null) && recipe.ContainsItem(ingredient))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002B05 RID: 11013
	private static Dictionary<uint, Dictionary<int, List<Recipe>>> recipeListsDict = new Dictionary<uint, Dictionary<int, List<Recipe>>>();
}
