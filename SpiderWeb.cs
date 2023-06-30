using System;
using UnityEngine;

// Token: 0x02000168 RID: 360
public class SpiderWeb : BaseCombatEntity
{
	// Token: 0x0600177A RID: 6010 RVA: 0x000B2399 File Offset: 0x000B0599
	public bool Fresh()
	{
		return !base.HasFlag(BaseEntity.Flags.Reserved1) && !base.HasFlag(BaseEntity.Flags.Reserved2) && !base.HasFlag(BaseEntity.Flags.Reserved3) && !base.HasFlag(BaseEntity.Flags.Reserved4);
	}

	// Token: 0x0600177B RID: 6011 RVA: 0x000B23D4 File Offset: 0x000B05D4
	public override void ServerInit()
	{
		base.ServerInit();
		if (this.Fresh())
		{
			int num = UnityEngine.Random.Range(0, 4);
			BaseEntity.Flags flags = BaseEntity.Flags.Reserved1;
			if (num == 0)
			{
				flags = BaseEntity.Flags.Reserved1;
			}
			else if (num == 1)
			{
				flags = BaseEntity.Flags.Reserved2;
			}
			else if (num == 2)
			{
				flags = BaseEntity.Flags.Reserved3;
			}
			else if (num == 3)
			{
				flags = BaseEntity.Flags.Reserved4;
			}
			base.SetFlag(flags, true, false, true);
		}
	}
}
