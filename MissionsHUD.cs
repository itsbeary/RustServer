using System;
using Rust.UI;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200089E RID: 2206
public class MissionsHUD : SingletonComponent<MissionsHUD>
{
	// Token: 0x040031B4 RID: 12724
	public SoundDefinition listComplete;

	// Token: 0x040031B5 RID: 12725
	public SoundDefinition itemComplete;

	// Token: 0x040031B6 RID: 12726
	public SoundDefinition popup;

	// Token: 0x040031B7 RID: 12727
	public Canvas Canvas;

	// Token: 0x040031B8 RID: 12728
	public Text titleText;

	// Token: 0x040031B9 RID: 12729
	public GameObject timerObject;

	// Token: 0x040031BA RID: 12730
	public RustText timerText;
}
