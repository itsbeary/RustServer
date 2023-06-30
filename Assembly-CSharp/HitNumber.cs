using System;
using UnityEngine;

// Token: 0x0200013D RID: 317
public class HitNumber : MonoBehaviour
{
	// Token: 0x06001714 RID: 5908 RVA: 0x000B0719 File Offset: 0x000AE919
	public int ColorToMultiplier(HitNumber.HitType type)
	{
		switch (type)
		{
		case HitNumber.HitType.Yellow:
			return 1;
		case HitNumber.HitType.Green:
			return 3;
		case HitNumber.HitType.Blue:
			return 5;
		case HitNumber.HitType.Purple:
			return 10;
		case HitNumber.HitType.Red:
			return 20;
		default:
			return 0;
		}
	}

	// Token: 0x06001715 RID: 5909 RVA: 0x000B0744 File Offset: 0x000AE944
	public void OnDrawGizmos()
	{
		Gizmos.color = Color.white;
		Gizmos.DrawSphere(base.transform.position, 0.025f);
	}

	// Token: 0x04000F45 RID: 3909
	public HitNumber.HitType hitType;

	// Token: 0x02000C37 RID: 3127
	public enum HitType
	{
		// Token: 0x040042F9 RID: 17145
		Yellow,
		// Token: 0x040042FA RID: 17146
		Green,
		// Token: 0x040042FB RID: 17147
		Blue,
		// Token: 0x040042FC RID: 17148
		Purple,
		// Token: 0x040042FD RID: 17149
		Red
	}
}
