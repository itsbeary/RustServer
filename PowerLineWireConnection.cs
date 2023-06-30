using System;
using UnityEngine;

// Token: 0x02000690 RID: 1680
[Serializable]
public class PowerLineWireConnection
{
	// Token: 0x040027B7 RID: 10167
	public Vector3 inOffset = Vector3.zero;

	// Token: 0x040027B8 RID: 10168
	public Vector3 outOffset = Vector3.zero;

	// Token: 0x040027B9 RID: 10169
	public float radius = 0.01f;

	// Token: 0x040027BA RID: 10170
	public Transform start;

	// Token: 0x040027BB RID: 10171
	public Transform end;
}
