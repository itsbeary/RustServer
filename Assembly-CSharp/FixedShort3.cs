using System;
using UnityEngine;

// Token: 0x0200095E RID: 2398
public struct FixedShort3
{
	// Token: 0x0600398F RID: 14735 RVA: 0x001552D2 File Offset: 0x001534D2
	public FixedShort3(Vector3 vec)
	{
		this.x = (short)(vec.x * 1024f);
		this.y = (short)(vec.y * 1024f);
		this.z = (short)(vec.z * 1024f);
	}

	// Token: 0x06003990 RID: 14736 RVA: 0x0015530D File Offset: 0x0015350D
	public static explicit operator Vector3(FixedShort3 vec)
	{
		return new Vector3((float)vec.x * 0.0009765625f, (float)vec.y * 0.0009765625f, (float)vec.z * 0.0009765625f);
	}

	// Token: 0x040033FE RID: 13310
	private const int FracBits = 10;

	// Token: 0x040033FF RID: 13311
	private const float MaxFrac = 1024f;

	// Token: 0x04003400 RID: 13312
	private const float RcpMaxFrac = 0.0009765625f;

	// Token: 0x04003401 RID: 13313
	public short x;

	// Token: 0x04003402 RID: 13314
	public short y;

	// Token: 0x04003403 RID: 13315
	public short z;
}
