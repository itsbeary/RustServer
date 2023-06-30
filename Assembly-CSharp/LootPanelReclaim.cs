using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000841 RID: 2113
public class LootPanelReclaim : LootPanel
{
	// Token: 0x04002F81 RID: 12161
	public int oldOverflow = -1;

	// Token: 0x04002F82 RID: 12162
	public Text overflowText;

	// Token: 0x04002F83 RID: 12163
	public GameObject overflowObject;

	// Token: 0x04002F84 RID: 12164
	public static readonly Translate.Phrase MorePhrase = new Translate.Phrase("reclaim.more", "additional items...");
}
