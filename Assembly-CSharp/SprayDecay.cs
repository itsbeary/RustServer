using System;
using ConVar;
using UnityEngine;

// Token: 0x020003EB RID: 1003
public class SprayDecay : global::Decay
{
	// Token: 0x0600228F RID: 8847 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool ShouldDecay(BaseEntity entity)
	{
		return true;
	}

	// Token: 0x06002290 RID: 8848 RVA: 0x00029EBC File Offset: 0x000280BC
	public override float GetDecayDelay(BaseEntity entity)
	{
		return 0f;
	}

	// Token: 0x06002291 RID: 8849 RVA: 0x000DF23C File Offset: 0x000DD43C
	public override float GetDecayDuration(BaseEntity entity)
	{
		return Mathf.Max(Global.SprayDuration, 1f);
	}
}
