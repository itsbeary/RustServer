using System;
using UnityEngine;

// Token: 0x02000962 RID: 2402
public struct Half4
{
	// Token: 0x06003997 RID: 14743 RVA: 0x00155498 File Offset: 0x00153698
	public Half4(Vector4 vec)
	{
		this.x = Mathf.FloatToHalf(vec.x);
		this.y = Mathf.FloatToHalf(vec.y);
		this.z = Mathf.FloatToHalf(vec.z);
		this.w = Mathf.FloatToHalf(vec.w);
	}

	// Token: 0x06003998 RID: 14744 RVA: 0x001554E9 File Offset: 0x001536E9
	public static explicit operator Vector4(Half4 vec)
	{
		return new Vector4(Mathf.HalfToFloat(vec.x), Mathf.HalfToFloat(vec.y), Mathf.HalfToFloat(vec.z), Mathf.HalfToFloat(vec.w));
	}

	// Token: 0x04003414 RID: 13332
	public ushort x;

	// Token: 0x04003415 RID: 13333
	public ushort y;

	// Token: 0x04003416 RID: 13334
	public ushort z;

	// Token: 0x04003417 RID: 13335
	public ushort w;
}
