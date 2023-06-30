using System;
using UnityEngine;

// Token: 0x02000468 RID: 1128
public class BaseTrap : DecayEntity
{
	// Token: 0x06002579 RID: 9593 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void ObjectEntered(GameObject obj)
	{
	}

	// Token: 0x0600257A RID: 9594 RVA: 0x00072115 File Offset: 0x00070315
	public virtual void Arm()
	{
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x0600257B RID: 9595 RVA: 0x000063A5 File Offset: 0x000045A5
	public virtual void OnEmpty()
	{
	}
}
