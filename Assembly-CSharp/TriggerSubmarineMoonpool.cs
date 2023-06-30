using System;
using UnityEngine;

// Token: 0x020005A0 RID: 1440
public class TriggerSubmarineMoonpool : TriggerBase, IServerComponent
{
	// Token: 0x06002BF7 RID: 11255 RVA: 0x0010A458 File Offset: 0x00108658
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
		BaseSubmarine baseSubmarine;
		if (baseEntity.isServer && (baseSubmarine = baseEntity as BaseSubmarine) != null)
		{
			return baseSubmarine.gameObject;
		}
		return null;
	}

	// Token: 0x06002BF8 RID: 11256 RVA: 0x0010A4A8 File Offset: 0x001086A8
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		BaseSubmarine baseSubmarine;
		if ((baseSubmarine = ent as BaseSubmarine) != null)
		{
			baseSubmarine.OnSurfacedInMoonpool();
		}
	}
}
