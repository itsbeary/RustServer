using System;
using UnityEngine;

// Token: 0x0200043F RID: 1087
public class LargeShredderTrigger : TriggerBase
{
	// Token: 0x060024AE RID: 9390 RVA: 0x000E931C File Offset: 0x000E751C
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

	// Token: 0x060024AF RID: 9391 RVA: 0x000E9378 File Offset: 0x000E7578
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		this.shredder.OnEntityEnteredTrigger(ent);
	}

	// Token: 0x04001C9B RID: 7323
	public LargeShredder shredder;
}
