using System;
using UnityEngine;

// Token: 0x020001E5 RID: 485
public class AITraversalWaitPoint : MonoBehaviour
{
	// Token: 0x060019C6 RID: 6598 RVA: 0x000BC7A2 File Offset: 0x000BA9A2
	public bool Occupied()
	{
		return Time.time > this.nextFreeTime;
	}

	// Token: 0x060019C7 RID: 6599 RVA: 0x000BC7B1 File Offset: 0x000BA9B1
	public void Occupy(float dur = 1f)
	{
		this.nextFreeTime = Time.time + dur;
	}

	// Token: 0x0400126E RID: 4718
	public float nextFreeTime;
}
