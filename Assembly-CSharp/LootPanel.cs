using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000836 RID: 2102
public class LootPanel : MonoBehaviour
{
	// Token: 0x04002F5B RID: 12123
	public Text Title;

	// Token: 0x04002F5C RID: 12124
	public RustText TitleText;

	// Token: 0x04002F5D RID: 12125
	public bool hideInvalidIcons;

	// Token: 0x04002F5E RID: 12126
	[Tooltip("Only needed if hideInvalidIcons is true")]
	public CanvasGroup canvasGroup;

	// Token: 0x02000E98 RID: 3736
	public interface IHasLootPanel
	{
		// Token: 0x170006F9 RID: 1785
		// (get) Token: 0x060052FF RID: 21247
		Translate.Phrase LootPanelTitle { get; }
	}
}
