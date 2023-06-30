using System;
using UnityEngine;

// Token: 0x0200058A RID: 1418
public class TriggerEnsnare : TriggerBase
{
	// Token: 0x06002B90 RID: 11152 RVA: 0x00108AF0 File Offset: 0x00106CF0
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
		return baseEntity.gameObject;
	}

	// Token: 0x04002378 RID: 9080
	public bool blockHands = true;
}
