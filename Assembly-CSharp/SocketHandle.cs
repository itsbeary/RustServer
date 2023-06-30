using System;
using UnityEngine;

// Token: 0x02000274 RID: 628
public class SocketHandle : PrefabAttribute
{
	// Token: 0x06001CEB RID: 7403 RVA: 0x000C85AA File Offset: 0x000C67AA
	protected override Type GetIndexedType()
	{
		return typeof(SocketHandle);
	}

	// Token: 0x06001CEC RID: 7404 RVA: 0x000C85B8 File Offset: 0x000C67B8
	internal void AdjustTarget(ref Construction.Target target, float maxplaceDistance)
	{
		Vector3 worldPosition = this.worldPosition;
		Vector3 vector = target.ray.origin + target.ray.direction * maxplaceDistance - worldPosition;
		target.ray.direction = (vector - target.ray.origin).normalized;
	}
}
