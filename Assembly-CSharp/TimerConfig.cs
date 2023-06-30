using System;
using UnityEngine.UI;

// Token: 0x02000135 RID: 309
public class TimerConfig : UIDialog
{
	// Token: 0x04000F08 RID: 3848
	[NonSerialized]
	private CustomTimerSwitch timerSwitch;

	// Token: 0x04000F09 RID: 3849
	public InputField input;

	// Token: 0x04000F0A RID: 3850
	public static float minTime = 0.25f;

	// Token: 0x04000F0B RID: 3851
	public float seconds;
}
