using System;
using UnityEngine;

// Token: 0x0200058F RID: 1423
public class TriggerLadder : TriggerBase
{
	// Token: 0x06002BA7 RID: 11175 RVA: 0x00109238 File Offset: 0x00107438
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity as BasePlayer == null)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x04002390 RID: 9104
	public TriggerLadder.LadderType Type;

	// Token: 0x02000D78 RID: 3448
	public enum LadderType
	{
		// Token: 0x04004809 RID: 18441
		Rungs,
		// Token: 0x0400480A RID: 18442
		Rope
	}
}
