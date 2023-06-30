using System;
using UnityEngine;

// Token: 0x0200025E RID: 606
public class DecayPoint : PrefabAttribute
{
	// Token: 0x06001C9F RID: 7327 RVA: 0x000C7235 File Offset: 0x000C5435
	public bool IsOccupied(BaseEntity entity)
	{
		return entity.IsOccupied(this.socket);
	}

	// Token: 0x06001CA0 RID: 7328 RVA: 0x000C7243 File Offset: 0x000C5443
	protected override Type GetIndexedType()
	{
		return typeof(DecayPoint);
	}

	// Token: 0x04001535 RID: 5429
	[Tooltip("If this point is occupied this will take this % off the power of the decay")]
	public float protection = 0.25f;

	// Token: 0x04001536 RID: 5430
	public Socket_Base socket;
}
