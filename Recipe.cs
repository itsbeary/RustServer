using System;
using UnityEngine;

// Token: 0x0200075E RID: 1886
[CreateAssetMenu(menuName = "Rust/Recipe")]
public class Recipe : ScriptableObject
{
	// Token: 0x17000447 RID: 1095
	// (get) Token: 0x06003489 RID: 13449 RVA: 0x0014483F File Offset: 0x00142A3F
	public string DisplayName
	{
		get
		{
			if (this.ProducedItem != null)
			{
				return this.ProducedItem.displayName.translated;
			}
			if (this.SpawnedItem != null)
			{
				return this.SpawnedItemName;
			}
			return null;
		}
	}

	// Token: 0x17000448 RID: 1096
	// (get) Token: 0x0600348A RID: 13450 RVA: 0x00144870 File Offset: 0x00142A70
	public string DisplayDescription
	{
		get
		{
			if (this.ProducedItem != null)
			{
				return this.ProducedItem.displayDescription.translated;
			}
			if (this.SpawnedItem != null)
			{
				return this.SpawnedItemDescription;
			}
			return null;
		}
	}

	// Token: 0x17000449 RID: 1097
	// (get) Token: 0x0600348B RID: 13451 RVA: 0x001448A1 File Offset: 0x00142AA1
	public Sprite DisplayIcon
	{
		get
		{
			if (this.ProducedItem != null)
			{
				return this.ProducedItem.iconSprite;
			}
			if (this.SpawnedItem != null)
			{
				return this.SpawnedItemIcon;
			}
			return null;
		}
	}

	// Token: 0x0600348C RID: 13452 RVA: 0x001448D0 File Offset: 0x00142AD0
	public bool ContainsItem(Item item)
	{
		if (item == null)
		{
			return false;
		}
		if (this.Ingredients == null)
		{
			return false;
		}
		foreach (Recipe.RecipeIngredient recipeIngredient in this.Ingredients)
		{
			if (item.info == recipeIngredient.Ingredient)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002AFB RID: 11003
	[Header("Produced Item")]
	public ItemDefinition ProducedItem;

	// Token: 0x04002AFC RID: 11004
	public int ProducedItemCount = 1;

	// Token: 0x04002AFD RID: 11005
	public bool CanQueueMultiple = true;

	// Token: 0x04002AFE RID: 11006
	[Header("Spawned Item")]
	public GameObjectRef SpawnedItem;

	// Token: 0x04002AFF RID: 11007
	public string SpawnedItemName;

	// Token: 0x04002B00 RID: 11008
	public string SpawnedItemDescription;

	// Token: 0x04002B01 RID: 11009
	public Sprite SpawnedItemIcon;

	// Token: 0x04002B02 RID: 11010
	[Header("Misc")]
	public bool RequiresBlueprint;

	// Token: 0x04002B03 RID: 11011
	public Recipe.RecipeIngredient[] Ingredients;

	// Token: 0x04002B04 RID: 11012
	public float MixingDuration;

	// Token: 0x02000E7E RID: 3710
	[Serializable]
	public struct RecipeIngredient
	{
		// Token: 0x04004C0B RID: 19467
		public ItemDefinition Ingredient;

		// Token: 0x04004C0C RID: 19468
		public int Count;
	}
}
