using System;
using UnityEngine;

// Token: 0x0200066A RID: 1642
public class DecorSocketFemale : PrefabAttribute
{
	// Token: 0x06002FC9 RID: 12233 RVA: 0x001200BA File Offset: 0x0011E2BA
	protected override Type GetIndexedType()
	{
		return typeof(DecorSocketFemale);
	}

	// Token: 0x06002FCA RID: 12234 RVA: 0x001200C6 File Offset: 0x0011E2C6
	protected void OnDrawGizmos()
	{
		Gizmos.color = new Color(1f, 0.5f, 0.5f, 1f);
		Gizmos.DrawSphere(base.transform.position, 1f);
	}
}
