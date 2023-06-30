using System;
using TMPro;
using UnityEngine;

// Token: 0x020008ED RID: 2285
public class VitalNote : MonoBehaviour, IClientComponent, IVitalNotice
{
	// Token: 0x04003300 RID: 13056
	public VitalNote.Vital VitalType;

	// Token: 0x04003301 RID: 13057
	public FloatConditions showIf;

	// Token: 0x04003302 RID: 13058
	public TextMeshProUGUI valueText;

	// Token: 0x02000EBF RID: 3775
	public enum Vital
	{
		// Token: 0x04004D37 RID: 19767
		Comfort,
		// Token: 0x04004D38 RID: 19768
		Radiation,
		// Token: 0x04004D39 RID: 19769
		Poison,
		// Token: 0x04004D3A RID: 19770
		Cold,
		// Token: 0x04004D3B RID: 19771
		Bleeding,
		// Token: 0x04004D3C RID: 19772
		Hot,
		// Token: 0x04004D3D RID: 19773
		Oxygen,
		// Token: 0x04004D3E RID: 19774
		Wet,
		// Token: 0x04004D3F RID: 19775
		Hygiene,
		// Token: 0x04004D40 RID: 19776
		Starving,
		// Token: 0x04004D41 RID: 19777
		Dehydration
	}
}
