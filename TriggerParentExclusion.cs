using System;
using UnityEngine;

// Token: 0x02000599 RID: 1433
public class TriggerParentExclusion : TriggerBase, IServerComponent
{
	// Token: 0x06002BD1 RID: 11217 RVA: 0x00109BF8 File Offset: 0x00107DF8
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
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
