using System;
using UnityEngine;

// Token: 0x02000960 RID: 2400
public struct FixedSByteNorm4
{
	// Token: 0x06003993 RID: 14739 RVA: 0x001553A4 File Offset: 0x001535A4
	public FixedSByteNorm4(Vector4 vec)
	{
		this.x = (sbyte)(vec.x * 128f);
		this.y = (sbyte)(vec.y * 128f);
		this.z = (sbyte)(vec.z * 128f);
		this.w = (sbyte)(vec.w * 128f);
	}

	// Token: 0x06003994 RID: 14740 RVA: 0x001553FD File Offset: 0x001535FD
	public static explicit operator Vector4(FixedSByteNorm4 vec)
	{
		return new Vector4((float)vec.x * 0.0078125f, (float)vec.y * 0.0078125f, (float)vec.z * 0.0078125f, (float)vec.w * 0.0078125f);
	}

	// Token: 0x0400340A RID: 13322
	private const int FracBits = 7;

	// Token: 0x0400340B RID: 13323
	private const float MaxFrac = 128f;

	// Token: 0x0400340C RID: 13324
	private const float RcpMaxFrac = 0.0078125f;

	// Token: 0x0400340D RID: 13325
	public sbyte x;

	// Token: 0x0400340E RID: 13326
	public sbyte y;

	// Token: 0x0400340F RID: 13327
	public sbyte z;

	// Token: 0x04003410 RID: 13328
	public sbyte w;
}
