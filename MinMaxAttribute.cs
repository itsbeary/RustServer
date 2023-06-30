using System;
using UnityEngine;

// Token: 0x020008F6 RID: 2294
public class MinMaxAttribute : PropertyAttribute
{
	// Token: 0x060037BC RID: 14268 RVA: 0x0014D219 File Offset: 0x0014B419
	public MinMaxAttribute(float min, float max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x04003314 RID: 13076
	public float min;

	// Token: 0x04003315 RID: 13077
	public float max;
}
