using System;
using UnityEngine;

// Token: 0x0200059F RID: 1439
public class TriggerSnowmobileAchievement : TriggerBase
{
	// Token: 0x06002BF5 RID: 11253 RVA: 0x0010A41C File Offset: 0x0010861C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity != null && baseEntity.isServer && baseEntity.ToPlayer() != null)
		{
			return baseEntity.gameObject;
		}
		return null;
	}
}
