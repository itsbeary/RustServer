using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200086E RID: 2158
public class LookatTooltip : MonoBehaviour
{
	// Token: 0x040030D9 RID: 12505
	public static bool Enabled = true;

	// Token: 0x040030DA RID: 12506
	[NonSerialized]
	public BaseEntity currentlyLookingAt;

	// Token: 0x040030DB RID: 12507
	public RustText textLabel;

	// Token: 0x040030DC RID: 12508
	public Image icon;

	// Token: 0x040030DD RID: 12509
	public CanvasGroup canvasGroup;

	// Token: 0x040030DE RID: 12510
	public CanvasGroup infoGroup;

	// Token: 0x040030DF RID: 12511
	public CanvasGroup minimiseGroup;
}
