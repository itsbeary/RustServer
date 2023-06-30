using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000108 RID: 264
public class ComputerMenu : UIDialog
{
	// Token: 0x04000DFF RID: 3583
	public RectTransform bookmarkContainer;

	// Token: 0x04000E00 RID: 3584
	public GameObject bookmarkPrefab;

	// Token: 0x04000E01 RID: 3585
	public List<RCBookmarkEntry> activeEntries = new List<RCBookmarkEntry>();
}
