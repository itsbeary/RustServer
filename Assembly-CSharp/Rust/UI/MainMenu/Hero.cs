using System;
using UnityEngine;

namespace Rust.UI.MainMenu
{
	// Token: 0x02000B2A RID: 2858
	public class Hero : SingletonComponent<Hero>
	{
		// Token: 0x04003E09 RID: 15881
		public CanvasGroup CanvasGroup;

		// Token: 0x04003E0A RID: 15882
		public Video VideoPlayer;

		// Token: 0x04003E0B RID: 15883
		public RustText TitleText;

		// Token: 0x04003E0C RID: 15884
		public RustText ButtonText;

		// Token: 0x04003E0D RID: 15885
		public HttpImage TitleImage;

		// Token: 0x04003E0E RID: 15886
		[Header("Item Store Links")]
		public RustButton ItemStoreButton;

		// Token: 0x04003E0F RID: 15887
		public RustButton LimitedTabButton;

		// Token: 0x04003E10 RID: 15888
		public RustButton GeneralTabButton;
	}
}
