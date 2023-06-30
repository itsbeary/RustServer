using System;
using UnityEngine;

// Token: 0x0200014A RID: 330
public class ArcadeNetworkTrigger : TriggerBase
{
	// Token: 0x06001734 RID: 5940 RVA: 0x000B0E94 File Offset: 0x000AF094
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
