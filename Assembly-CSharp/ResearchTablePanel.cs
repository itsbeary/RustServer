using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000853 RID: 2131
public class ResearchTablePanel : LootPanel
{
	// Token: 0x04003002 RID: 12290
	public Button researchButton;

	// Token: 0x04003003 RID: 12291
	public Text timerText;

	// Token: 0x04003004 RID: 12292
	public GameObject itemDescNoItem;

	// Token: 0x04003005 RID: 12293
	public GameObject itemDescTooBroken;

	// Token: 0x04003006 RID: 12294
	public GameObject itemDescNotResearchable;

	// Token: 0x04003007 RID: 12295
	public GameObject itemDescTooMany;

	// Token: 0x04003008 RID: 12296
	public GameObject itemTakeBlueprint;

	// Token: 0x04003009 RID: 12297
	public GameObject itemDescAlreadyResearched;

	// Token: 0x0400300A RID: 12298
	public GameObject itemDescDefaultBlueprint;

	// Token: 0x0400300B RID: 12299
	public Text successChanceText;

	// Token: 0x0400300C RID: 12300
	public ItemIcon scrapIcon;

	// Token: 0x0400300D RID: 12301
	[NonSerialized]
	public bool wasResearching;

	// Token: 0x0400300E RID: 12302
	public GameObject[] workbenchReqs;
}
