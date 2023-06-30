using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000863 RID: 2147
public class WorkbenchPanel : LootPanel, IInventoryChanged
{
	// Token: 0x0400307B RID: 12411
	public Button experimentButton;

	// Token: 0x0400307C RID: 12412
	public Text timerText;

	// Token: 0x0400307D RID: 12413
	public Text costText;

	// Token: 0x0400307E RID: 12414
	public GameObject expermentCostParent;

	// Token: 0x0400307F RID: 12415
	public GameObject controlsParent;

	// Token: 0x04003080 RID: 12416
	public GameObject allUnlockedNotification;

	// Token: 0x04003081 RID: 12417
	public GameObject informationParent;

	// Token: 0x04003082 RID: 12418
	public GameObject cycleIcon;

	// Token: 0x04003083 RID: 12419
	public TechTreeDialog techTreeDialog;
}
