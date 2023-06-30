using System;
using UnityEngine;

// Token: 0x0200040C RID: 1036
public class TargetTrigger : TriggerBase
{
	// Token: 0x0600235F RID: 9055 RVA: 0x000E2384 File Offset: 0x000E0584
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
		if (this.losEyes != null && !baseEntity.IsVisible(this.losEyes.transform.position, baseEntity.CenterPoint(), float.PositiveInfinity))
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x04001B35 RID: 6965
	[Tooltip("If set, the entering object must have line of sight to this transform to be added, note this is only checked on entry")]
	public Transform losEyes;
}
