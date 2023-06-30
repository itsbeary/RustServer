using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000852 RID: 2130
public class RepairBenchPanel : LootPanel
{
	// Token: 0x04002FF6 RID: 12278
	public Text infoText;

	// Token: 0x04002FF7 RID: 12279
	public Button repairButton;

	// Token: 0x04002FF8 RID: 12280
	public Color gotColor;

	// Token: 0x04002FF9 RID: 12281
	public Color notGotColor;

	// Token: 0x04002FFA RID: 12282
	public Translate.Phrase phraseEmpty;

	// Token: 0x04002FFB RID: 12283
	public Translate.Phrase phraseNotRepairable;

	// Token: 0x04002FFC RID: 12284
	public Translate.Phrase phraseRepairNotNeeded;

	// Token: 0x04002FFD RID: 12285
	public Translate.Phrase phraseNoBlueprint;

	// Token: 0x04002FFE RID: 12286
	public GameObject skinsPanel;

	// Token: 0x04002FFF RID: 12287
	public GameObject changeSkinDialog;

	// Token: 0x04003000 RID: 12288
	public IconSkinPicker picker;

	// Token: 0x04003001 RID: 12289
	public GameObject attachmentSkinBlocker;
}
