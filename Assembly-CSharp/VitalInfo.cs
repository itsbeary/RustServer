using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x020008EC RID: 2284
public class VitalInfo : MonoBehaviour, IClientComponent, IVitalNotice
{
	// Token: 0x040032FC RID: 13052
	public HudElement Element;

	// Token: 0x040032FD RID: 13053
	public Image InfoImage;

	// Token: 0x040032FE RID: 13054
	public VitalInfo.Vital VitalType;

	// Token: 0x040032FF RID: 13055
	public TextMeshProUGUI text;

	// Token: 0x02000EBE RID: 3774
	public enum Vital
	{
		// Token: 0x04004D2B RID: 19755
		BuildingBlocked,
		// Token: 0x04004D2C RID: 19756
		CanBuild,
		// Token: 0x04004D2D RID: 19757
		Crafting,
		// Token: 0x04004D2E RID: 19758
		CraftLevel1,
		// Token: 0x04004D2F RID: 19759
		CraftLevel2,
		// Token: 0x04004D30 RID: 19760
		CraftLevel3,
		// Token: 0x04004D31 RID: 19761
		DecayProtected,
		// Token: 0x04004D32 RID: 19762
		Decaying,
		// Token: 0x04004D33 RID: 19763
		SafeZone,
		// Token: 0x04004D34 RID: 19764
		Buffed,
		// Token: 0x04004D35 RID: 19765
		Pet
	}
}
