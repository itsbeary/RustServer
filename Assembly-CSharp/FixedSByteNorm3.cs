using System;
using UnityEngine;

// Token: 0x0200095F RID: 2399
public struct FixedSByteNorm3
{
	// Token: 0x06003991 RID: 14737 RVA: 0x0015533B File Offset: 0x0015353B
	public FixedSByteNorm3(Vector3 vec)
	{
		this.x = (sbyte)(vec.x * 128f);
		this.y = (sbyte)(vec.y * 128f);
		this.z = (sbyte)(vec.z * 128f);
	}

	// Token: 0x06003992 RID: 14738 RVA: 0x00155376 File Offset: 0x00153576
	public static explicit operator Vector3(FixedSByteNorm3 vec)
	{
		return new Vector3((float)vec.x * 0.0078125f, (float)vec.y * 0.0078125f, (float)vec.z * 0.0078125f);
	}

	// Token: 0x04003404 RID: 13316
	private const int FracBits = 7;

	// Token: 0x04003405 RID: 13317
	private const float MaxFrac = 128f;

	// Token: 0x04003406 RID: 13318
	private const float RcpMaxFrac = 0.0078125f;

	// Token: 0x04003407 RID: 13319
	public sbyte x;

	// Token: 0x04003408 RID: 13320
	public sbyte y;

	// Token: 0x04003409 RID: 13321
	public sbyte z;
}
