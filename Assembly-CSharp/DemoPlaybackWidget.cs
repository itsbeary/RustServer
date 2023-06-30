using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020007B2 RID: 1970
public class DemoPlaybackWidget : MonoBehaviour
{
	// Token: 0x04002BC3 RID: 11203
	public RustSlider DemoProgress;

	// Token: 0x04002BC4 RID: 11204
	public RustText DemoName;

	// Token: 0x04002BC5 RID: 11205
	public RustText DemoDuration;

	// Token: 0x04002BC6 RID: 11206
	public RustText DemoCurrentTime;

	// Token: 0x04002BC7 RID: 11207
	public GameObject PausedRoot;

	// Token: 0x04002BC8 RID: 11208
	public GameObject PlayingRoot;

	// Token: 0x04002BC9 RID: 11209
	public RectTransform DemoPlaybackHandle;

	// Token: 0x04002BCA RID: 11210
	public RectTransform ShotPlaybackWindow;

	// Token: 0x04002BCB RID: 11211
	public RustButton LoopButton;

	// Token: 0x04002BCC RID: 11212
	public GameObject ShotButtonRoot;

	// Token: 0x04002BCD RID: 11213
	public RustText ShotNameText;

	// Token: 0x04002BCE RID: 11214
	public GameObject ShotNameRoot;

	// Token: 0x04002BCF RID: 11215
	public RectTransform ShotRecordWindow;
}
