using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000855 RID: 2133
public class SelectedItem : SingletonComponent<SelectedItem>, IInventoryChanged
{
	// Token: 0x04003017 RID: 12311
	public Image icon;

	// Token: 0x04003018 RID: 12312
	public Image iconSplitter;

	// Token: 0x04003019 RID: 12313
	public RustText title;

	// Token: 0x0400301A RID: 12314
	public RustText description;

	// Token: 0x0400301B RID: 12315
	public GameObject splitPanel;

	// Token: 0x0400301C RID: 12316
	public GameObject itemProtection;

	// Token: 0x0400301D RID: 12317
	public GameObject menuOption;

	// Token: 0x0400301E RID: 12318
	public GameObject optionsParent;

	// Token: 0x0400301F RID: 12319
	public GameObject innerPanelContainer;
}
