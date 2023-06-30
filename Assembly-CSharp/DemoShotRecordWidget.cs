using System;
using Rust.UI;
using UnityEngine;

// Token: 0x020007B9 RID: 1977
public class DemoShotRecordWidget : MonoBehaviour
{
	// Token: 0x04002BE6 RID: 11238
	public RustInput NameInput;

	// Token: 0x04002BE7 RID: 11239
	public GameObject RecordingRoot;

	// Token: 0x04002BE8 RID: 11240
	public GameObject PreRecordingRoot;

	// Token: 0x04002BE9 RID: 11241
	public RustButton CountdownToggle;

	// Token: 0x04002BEA RID: 11242
	public RustButton PauseOnSaveToggle;

	// Token: 0x04002BEB RID: 11243
	public RustButton ReturnToStartToggle;

	// Token: 0x04002BEC RID: 11244
	public RustButton RecordDofToggle;

	// Token: 0x04002BED RID: 11245
	public RustOption FolderDropdown;

	// Token: 0x04002BEE RID: 11246
	public GameObject RecordingUnderlay;

	// Token: 0x04002BEF RID: 11247
	public AudioSource CountdownAudio;

	// Token: 0x04002BF0 RID: 11248
	public GameObject ShotRecordTime;

	// Token: 0x04002BF1 RID: 11249
	public RustText ShotRecordTimeText;

	// Token: 0x04002BF2 RID: 11250
	public RustText ShotNameText;

	// Token: 0x04002BF3 RID: 11251
	public GameObject RecordingInProcessRoot;

	// Token: 0x04002BF4 RID: 11252
	public GameObject CountdownActiveRoot;

	// Token: 0x04002BF5 RID: 11253
	public GameObject CountdownActiveSliderRoot;

	// Token: 0x04002BF6 RID: 11254
	public RustSlider CountdownActiveSlider;

	// Token: 0x04002BF7 RID: 11255
	public RustText CountdownActiveText;
}
