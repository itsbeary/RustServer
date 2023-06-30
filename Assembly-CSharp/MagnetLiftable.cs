using System;
using UnityEngine;

// Token: 0x02000497 RID: 1175
public class MagnetLiftable : EntityComponent<BaseEntity>
{
	// Token: 0x17000324 RID: 804
	// (get) Token: 0x060026A2 RID: 9890 RVA: 0x000F33E3 File Offset: 0x000F15E3
	// (set) Token: 0x060026A3 RID: 9891 RVA: 0x000F33EB File Offset: 0x000F15EB
	public BasePlayer associatedPlayer { get; private set; }

	// Token: 0x060026A4 RID: 9892 RVA: 0x000F33F4 File Offset: 0x000F15F4
	public virtual void SetMagnetized(bool wantsOn, BaseMagnet magnetSource, BasePlayer player)
	{
		this.associatedPlayer = player;
	}

	// Token: 0x04001F22 RID: 7970
	public ItemAmount[] shredResources;

	// Token: 0x04001F23 RID: 7971
	public Vector3 shredDirection = Vector3.forward;
}
