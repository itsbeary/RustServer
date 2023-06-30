using System;
using Rust.UI;
using UnityEngine;

// Token: 0x0200087E RID: 2174
public class MainMenuSystem : SingletonComponent<MainMenuSystem>
{
	// Token: 0x04003131 RID: 12593
	public static bool isOpen = true;

	// Token: 0x04003132 RID: 12594
	public static Action OnOpenStateChanged;

	// Token: 0x04003133 RID: 12595
	public RustButton SessionButton;

	// Token: 0x04003134 RID: 12596
	public GameObject SessionPanel;

	// Token: 0x04003135 RID: 12597
	public GameObject NewsStoriesAlert;

	// Token: 0x04003136 RID: 12598
	public GameObject ItemStoreAlert;

	// Token: 0x04003137 RID: 12599
	public GameObject CompanionAlert;

	// Token: 0x04003138 RID: 12600
	public GameObject DemoBrowser;

	// Token: 0x04003139 RID: 12601
	public GameObject DemoBrowserButton;

	// Token: 0x0400313A RID: 12602
	public GameObject SuicideButton;

	// Token: 0x0400313B RID: 12603
	public GameObject EndDemoButton;

	// Token: 0x0400313C RID: 12604
	public GameObject ReflexModeOption;

	// Token: 0x0400313D RID: 12605
	public GameObject ReflexLatencyMarkerOption;
}
