using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007F3 RID: 2035
public class GameTip : SingletonComponent<GameTip>
{
	// Token: 0x04002DC3 RID: 11715
	public CanvasGroup canvasGroup;

	// Token: 0x04002DC4 RID: 11716
	public RustIcon icon;

	// Token: 0x04002DC5 RID: 11717
	public Image background;

	// Token: 0x04002DC6 RID: 11718
	public RustText text;

	// Token: 0x04002DC7 RID: 11719
	public GameTip.Theme[] themes;

	// Token: 0x02000E90 RID: 3728
	public enum Styles
	{
		// Token: 0x04004C85 RID: 19589
		Blue_Normal,
		// Token: 0x04004C86 RID: 19590
		Red_Normal,
		// Token: 0x04004C87 RID: 19591
		Blue_Long,
		// Token: 0x04004C88 RID: 19592
		Blue_Short,
		// Token: 0x04004C89 RID: 19593
		Server_Event
	}

	// Token: 0x02000E91 RID: 3729
	[Serializable]
	public struct Theme
	{
		// Token: 0x04004C8A RID: 19594
		public Icons Icon;

		// Token: 0x04004C8B RID: 19595
		public Color BackgroundColor;

		// Token: 0x04004C8C RID: 19596
		public Color ForegroundColor;

		// Token: 0x04004C8D RID: 19597
		public float duration;
	}
}
