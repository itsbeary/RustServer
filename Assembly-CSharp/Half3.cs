using System;
using UnityEngine;

// Token: 0x02000961 RID: 2401
public struct Half3
{
	// Token: 0x06003995 RID: 14741 RVA: 0x00155438 File Offset: 0x00153638
	public Half3(Vector3 vec)
	{
		this.x = Mathf.FloatToHalf(vec.x);
		this.y = Mathf.FloatToHalf(vec.y);
		this.z = Mathf.FloatToHalf(vec.z);
	}

	// Token: 0x06003996 RID: 14742 RVA: 0x0015546D File Offset: 0x0015366D
	public static explicit operator Vector3(Half3 vec)
	{
		return new Vector3(Mathf.HalfToFloat(vec.x), Mathf.HalfToFloat(vec.y), Mathf.HalfToFloat(vec.z));
	}

	// Token: 0x04003411 RID: 13329
	public ushort x;

	// Token: 0x04003412 RID: 13330
	public ushort y;

	// Token: 0x04003413 RID: 13331
	public ushort z;
}
