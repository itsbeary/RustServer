using System;
using UnityEngine;

// Token: 0x02000850 RID: 2128
public class QuickCraft : SingletonComponent<QuickCraft>, IInventoryChanged
{
	// Token: 0x04002FEB RID: 12267
	public GameObjectRef craftButton;

	// Token: 0x04002FEC RID: 12268
	public GameObject empty;

	// Token: 0x04002FED RID: 12269
	public Sprite FavouriteOnSprite;

	// Token: 0x04002FEE RID: 12270
	public Sprite FavouriteOffSprite;

	// Token: 0x04002FEF RID: 12271
	public Color FavouriteOnColor;

	// Token: 0x04002FF0 RID: 12272
	public Color FavouriteOffColor;
}
