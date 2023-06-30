using System;
using UnityEngine;

// Token: 0x02000515 RID: 1301
public class CapturePointTrigger : TriggerBase
{
	// Token: 0x060029AD RID: 10669 RVA: 0x000FF920 File Offset: 0x000FDB20
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
		if (baseEntity as BasePlayer == null)
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
