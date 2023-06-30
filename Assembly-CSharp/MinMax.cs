using System;
using UnityEngine;

// Token: 0x020008F5 RID: 2293
[Serializable]
public class MinMax
{
	// Token: 0x060037B8 RID: 14264 RVA: 0x0014D1B6 File Offset: 0x0014B3B6
	public MinMax(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	// Token: 0x060037B9 RID: 14265 RVA: 0x0014D1D7 File Offset: 0x0014B3D7
	public float Random()
	{
		return UnityEngine.Random.Range(this.x, this.y);
	}

	// Token: 0x060037BA RID: 14266 RVA: 0x0014D1EA File Offset: 0x0014B3EA
	public float Lerp(float t)
	{
		return Mathf.Lerp(this.x, this.y, t);
	}

	// Token: 0x060037BB RID: 14267 RVA: 0x0014D1FE File Offset: 0x0014B3FE
	public float Lerp(float a, float b, float t)
	{
		return Mathf.Lerp(this.x, this.y, Mathf.InverseLerp(a, b, t));
	}

	// Token: 0x04003312 RID: 13074
	public float x;

	// Token: 0x04003313 RID: 13075
	public float y = 1f;
}
