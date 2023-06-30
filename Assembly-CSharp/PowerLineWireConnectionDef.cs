using System;
using UnityEngine;

// Token: 0x0200068F RID: 1679
[Serializable]
public class PowerLineWireConnectionDef
{
	// Token: 0x0600301D RID: 12317 RVA: 0x00121606 File Offset: 0x0011F806
	public PowerLineWireConnectionDef()
	{
	}

	// Token: 0x0600301E RID: 12318 RVA: 0x00121630 File Offset: 0x0011F830
	public PowerLineWireConnectionDef(PowerLineWireConnectionDef src)
	{
		this.inOffset = src.inOffset;
		this.outOffset = src.outOffset;
		this.radius = src.radius;
	}

	// Token: 0x040027B3 RID: 10163
	public Vector3 inOffset = Vector3.zero;

	// Token: 0x040027B4 RID: 10164
	public Vector3 outOffset = Vector3.zero;

	// Token: 0x040027B5 RID: 10165
	public float radius = 0.01f;

	// Token: 0x040027B6 RID: 10166
	public bool hidden;
}
