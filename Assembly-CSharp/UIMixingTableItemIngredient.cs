using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200085E RID: 2142
public class UIMixingTableItemIngredient : MonoBehaviour
{
	// Token: 0x0600362E RID: 13870 RVA: 0x0014836C File Offset: 0x0014656C
	public void Init(Recipe.RecipeIngredient ingredient)
	{
		this.ItemIcon.sprite = ingredient.Ingredient.iconSprite;
		this.ItemCount.text = ingredient.Count.ToString();
		this.ItemIcon.enabled = true;
		this.ItemCount.enabled = true;
		this.ToolTip.Text = ingredient.Count.ToString() + " x " + ingredient.Ingredient.displayName.translated;
		this.ToolTip.enabled = true;
	}

	// Token: 0x0600362F RID: 13871 RVA: 0x001483FB File Offset: 0x001465FB
	public void InitBlank()
	{
		this.ItemIcon.enabled = false;
		this.ItemCount.enabled = false;
		this.ToolTip.enabled = false;
	}

	// Token: 0x04003040 RID: 12352
	public Image ItemIcon;

	// Token: 0x04003041 RID: 12353
	public Text ItemCount;

	// Token: 0x04003042 RID: 12354
	public Tooltip ToolTip;
}
