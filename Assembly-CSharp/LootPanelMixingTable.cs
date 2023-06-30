using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200083E RID: 2110
public class LootPanelMixingTable : LootPanel, IInventoryChanged
{
	// Token: 0x04002F79 RID: 12153
	public GameObject controlsOn;

	// Token: 0x04002F7A RID: 12154
	public GameObject controlsOff;

	// Token: 0x04002F7B RID: 12155
	public Button StartMixingButton;

	// Token: 0x04002F7C RID: 12156
	public InfoBar ProgressBar;

	// Token: 0x04002F7D RID: 12157
	public GameObject recipeItemPrefab;

	// Token: 0x04002F7E RID: 12158
	public RectTransform recipeContentRect;
}
