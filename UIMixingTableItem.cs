using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200085D RID: 2141
public class UIMixingTableItem : MonoBehaviour
{
	// Token: 0x0600362C RID: 13868 RVA: 0x001482C4 File Offset: 0x001464C4
	public void Init(Recipe recipe)
	{
		if (recipe == null)
		{
			return;
		}
		this.ItemIcon.sprite = recipe.DisplayIcon;
		this.TextItemNameAndQuantity.text = recipe.ProducedItemCount.ToString() + " x " + recipe.DisplayName;
		this.ItemTooltip.Text = recipe.DisplayDescription;
		for (int i = 0; i < this.Ingredients.Length; i++)
		{
			if (i >= recipe.Ingredients.Length)
			{
				this.Ingredients[i].InitBlank();
			}
			else
			{
				this.Ingredients[i].Init(recipe.Ingredients[i]);
			}
		}
	}

	// Token: 0x0400303C RID: 12348
	public Image ItemIcon;

	// Token: 0x0400303D RID: 12349
	public Tooltip ItemTooltip;

	// Token: 0x0400303E RID: 12350
	public RustText TextItemNameAndQuantity;

	// Token: 0x0400303F RID: 12351
	public UIMixingTableItemIngredient[] Ingredients;
}
