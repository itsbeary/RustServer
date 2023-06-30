using System;
using UnityEngine;

// Token: 0x0200066B RID: 1643
public class DecorSocketMale : PrefabAttribute
{
	// Token: 0x06002FCC RID: 12236 RVA: 0x001200FB File Offset: 0x0011E2FB
	protected override Type GetIndexedType()
	{
		return typeof(DecorSocketMale);
	}

	// Token: 0x06002FCD RID: 12237 RVA: 0x00120107 File Offset: 0x0011E307
	protected void OnDrawGizmos()
	{
		Gizmos.color = new Color(0.5f, 0.5f, 1f, 1f);
		Gizmos.DrawSphere(base.transform.position, 1f);
	}
}
