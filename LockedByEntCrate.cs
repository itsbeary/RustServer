using System;
using UnityEngine;

// Token: 0x02000422 RID: 1058
public class LockedByEntCrate : LootContainer
{
	// Token: 0x060023C9 RID: 9161 RVA: 0x000E4724 File Offset: 0x000E2924
	public void SetLockingEnt(GameObject ent)
	{
		base.CancelInvoke(new Action(this.Think));
		this.SetLocked(false);
		this.lockingEnt = ent;
		if (this.lockingEnt != null)
		{
			base.InvokeRepeating(new Action(this.Think), UnityEngine.Random.Range(0f, 1f), 1f);
			this.SetLocked(true);
		}
	}

	// Token: 0x060023CA RID: 9162 RVA: 0x000E478C File Offset: 0x000E298C
	public void SetLocked(bool isLocked)
	{
		base.SetFlag(BaseEntity.Flags.OnFire, isLocked, false, true);
		base.SetFlag(BaseEntity.Flags.Locked, isLocked, false, true);
	}

	// Token: 0x060023CB RID: 9163 RVA: 0x000E47A3 File Offset: 0x000E29A3
	public void Think()
	{
		if (this.lockingEnt == null && base.IsLocked())
		{
			this.SetLockingEnt(null);
		}
	}

	// Token: 0x04001BD9 RID: 7129
	public GameObject lockingEnt;
}
