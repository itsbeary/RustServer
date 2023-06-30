using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200082F RID: 2095
public class ItemPreviewIcon : BaseMonoBehaviour, IInventoryChanged, IItemAmountChanged, IItemIconChanged
{
	// Token: 0x04002F40 RID: 12096
	public ItemContainerSource containerSource;

	// Token: 0x04002F41 RID: 12097
	[Range(0f, 64f)]
	public int slot;

	// Token: 0x04002F42 RID: 12098
	public bool setSlotFromSiblingIndex = true;

	// Token: 0x04002F43 RID: 12099
	public CanvasGroup iconContents;

	// Token: 0x04002F44 RID: 12100
	public Image iconImage;

	// Token: 0x04002F45 RID: 12101
	public Text amountText;

	// Token: 0x04002F46 RID: 12102
	[NonSerialized]
	public Item item;
}
