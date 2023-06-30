using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000128 RID: 296
public class LootPanelToolCupboard : LootPanel
{
	// Token: 0x04000ED6 RID: 3798
	public List<VirtualItemIcon> costIcons;

	// Token: 0x04000ED7 RID: 3799
	public Text costPerTimeText;

	// Token: 0x04000ED8 RID: 3800
	public Text protectedText;

	// Token: 0x04000ED9 RID: 3801
	public GameObject baseNotProtectedObj;

	// Token: 0x04000EDA RID: 3802
	public GameObject baseProtectedObj;

	// Token: 0x04000EDB RID: 3803
	public Translate.Phrase protectedPrefix;

	// Token: 0x04000EDC RID: 3804
	public Tooltip costToolTip;

	// Token: 0x04000EDD RID: 3805
	public Translate.Phrase blocksPhrase;
}
