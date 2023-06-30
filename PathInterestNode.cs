using System;
using UnityEngine;

// Token: 0x020001AD RID: 429
public class PathInterestNode : MonoBehaviour, IAIPathInterestNode
{
	// Token: 0x17000214 RID: 532
	// (get) Token: 0x060018DF RID: 6367 RVA: 0x0002C887 File Offset: 0x0002AA87
	public Vector3 Position
	{
		get
		{
			return base.transform.position;
		}
	}

	// Token: 0x17000215 RID: 533
	// (get) Token: 0x060018E0 RID: 6368 RVA: 0x000B88C8 File Offset: 0x000B6AC8
	// (set) Token: 0x060018E1 RID: 6369 RVA: 0x000B88D0 File Offset: 0x000B6AD0
	public float NextVisitTime { get; set; }

	// Token: 0x060018E2 RID: 6370 RVA: 0x000B88D9 File Offset: 0x000B6AD9
	public void OnDrawGizmos()
	{
		Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
		Gizmos.DrawSphere(base.transform.position, 0.5f);
	}
}
