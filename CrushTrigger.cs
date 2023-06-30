using System;
using UnityEngine;

// Token: 0x0200047E RID: 1150
public class CrushTrigger : TriggerHurt
{
	// Token: 0x060025FF RID: 9727 RVA: 0x000F08A0 File Offset: 0x000EEAA0
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
		if (!this.includeNPCs && baseEntity.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002600 RID: 9728 RVA: 0x000F08F5 File Offset: 0x000EEAF5
	protected override bool CanHurt(BaseCombatEntity ent)
	{
		return (!this.requireCentreBelowPosition || ent.CenterPoint().y <= base.transform.position.y) && base.CanHurt(ent);
	}

	// Token: 0x04001E6B RID: 7787
	public bool includeNPCs = true;

	// Token: 0x04001E6C RID: 7788
	public bool requireCentreBelowPosition;
}
