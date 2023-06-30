using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020007ED RID: 2029
public class HostileNote : MonoBehaviour, IClientComponent
{
	// Token: 0x04002DA0 RID: 11680
	public CanvasGroup warnGroup;

	// Token: 0x04002DA1 RID: 11681
	public CanvasGroup group;

	// Token: 0x04002DA2 RID: 11682
	public CanvasGroup timerGroup;

	// Token: 0x04002DA3 RID: 11683
	public CanvasGroup smallWarning;

	// Token: 0x04002DA4 RID: 11684
	public Text timerText;

	// Token: 0x04002DA5 RID: 11685
	public Text smallWarningText;

	// Token: 0x04002DA6 RID: 11686
	public static float unhostileTime;

	// Token: 0x04002DA7 RID: 11687
	public static float weaponDrawnDuration;

	// Token: 0x04002DA8 RID: 11688
	public Color warnColor;

	// Token: 0x04002DA9 RID: 11689
	public Color hostileColor;

	// Token: 0x04002DAA RID: 11690
	public float requireDistanceToSafeZone = 200f;
}
