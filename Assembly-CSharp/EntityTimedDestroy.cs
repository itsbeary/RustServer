using System;
using UnityEngine;

// Token: 0x020003CF RID: 975
public class EntityTimedDestroy : EntityComponent<BaseEntity>
{
	// Token: 0x060021E5 RID: 8677 RVA: 0x000DC863 File Offset: 0x000DAA63
	private void OnEnable()
	{
		base.Invoke(new Action(this.TimedDestroy), this.secondsTillDestroy);
	}

	// Token: 0x060021E6 RID: 8678 RVA: 0x000DC87D File Offset: 0x000DAA7D
	private void TimedDestroy()
	{
		if (base.baseEntity != null)
		{
			base.baseEntity.Kill(BaseNetworkable.DestroyMode.None);
			return;
		}
		Debug.LogWarning("EntityTimedDestroy failed, baseEntity was already null!");
	}

	// Token: 0x04001A4A RID: 6730
	public float secondsTillDestroy = 1f;
}
