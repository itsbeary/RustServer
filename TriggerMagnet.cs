using System;
using UnityEngine;

// Token: 0x02000498 RID: 1176
public class TriggerMagnet : TriggerBase
{
	// Token: 0x060026A6 RID: 9894 RVA: 0x000F3410 File Offset: 0x000F1610
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
		if (!baseEntity.syncPosition)
		{
			return null;
		}
		if (!baseEntity.GetComponent<MagnetLiftable>())
		{
			return null;
		}
		return baseEntity.gameObject;
	}
}
