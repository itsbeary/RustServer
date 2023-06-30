using System;
using UnityEngine;

// Token: 0x020008B7 RID: 2231
public class RepairCostIndicator : SingletonComponent<RepairCostIndicator>, IClientComponent
{
	// Token: 0x04003246 RID: 12870
	public RepairCostIndicatorRow[] Rows;

	// Token: 0x04003247 RID: 12871
	public CanvasGroup Fader;
}
