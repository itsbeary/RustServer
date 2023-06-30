using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI
{
	// Token: 0x02000B17 RID: 2839
	public class ReportBug : UIDialog
	{
		// Token: 0x04003D98 RID: 15768
		public GameObject GetInformation;

		// Token: 0x04003D99 RID: 15769
		public GameObject Finished;

		// Token: 0x04003D9A RID: 15770
		public RustInput Subject;

		// Token: 0x04003D9B RID: 15771
		public RustInput Message;

		// Token: 0x04003D9C RID: 15772
		public RustButton ReportButton;

		// Token: 0x04003D9D RID: 15773
		public RustButtonGroup Category;

		// Token: 0x04003D9E RID: 15774
		public RustIcon ProgressIcon;

		// Token: 0x04003D9F RID: 15775
		public RustText ProgressText;

		// Token: 0x04003DA0 RID: 15776
		public RawImage ScreenshotImage;

		// Token: 0x04003DA1 RID: 15777
		public GameObject ScreenshotRoot;

		// Token: 0x04003DA2 RID: 15778
		public UIBackgroundBlur BlurController;

		// Token: 0x04003DA3 RID: 15779
		public RustButton SubmitButton;

		// Token: 0x04003DA4 RID: 15780
		public GameObject SubmitErrorRoot;

		// Token: 0x04003DA5 RID: 15781
		public RustText CooldownText;

		// Token: 0x04003DA6 RID: 15782
		public RustText ContentMissingText;
	}
}
