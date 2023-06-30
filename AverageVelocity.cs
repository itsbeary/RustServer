using System;
using UnityEngine;

// Token: 0x02000291 RID: 657
public class AverageVelocity
{
	// Token: 0x06001D4E RID: 7502 RVA: 0x000CA374 File Offset: 0x000C8574
	public void Record(Vector3 newPos)
	{
		float num = Time.time - this.time;
		if (num < 0.1f)
		{
			return;
		}
		if (this.pos.sqrMagnitude > 0f)
		{
			Vector3 vector = newPos - this.pos;
			this.averageVelocity = vector * (1f / num);
			this.averageSpeed = this.averageVelocity.magnitude;
		}
		this.time = Time.time;
		this.pos = newPos;
	}

	// Token: 0x17000267 RID: 615
	// (get) Token: 0x06001D4F RID: 7503 RVA: 0x000CA3EC File Offset: 0x000C85EC
	public float Speed
	{
		get
		{
			return this.averageSpeed;
		}
	}

	// Token: 0x17000268 RID: 616
	// (get) Token: 0x06001D50 RID: 7504 RVA: 0x000CA3F4 File Offset: 0x000C85F4
	public Vector3 Average
	{
		get
		{
			return this.averageVelocity;
		}
	}

	// Token: 0x040015E3 RID: 5603
	private Vector3 pos;

	// Token: 0x040015E4 RID: 5604
	private float time;

	// Token: 0x040015E5 RID: 5605
	private float lastEntry;

	// Token: 0x040015E6 RID: 5606
	private float averageSpeed;

	// Token: 0x040015E7 RID: 5607
	private Vector3 averageVelocity;
}
