using System;
using UnityEngine;

// Token: 0x020003D8 RID: 984
public class BoxStorage : StorageContainer
{
	// Token: 0x06002207 RID: 8711 RVA: 0x000DD5D0 File Offset: 0x000DB7D0
	public override Vector3 GetDropPosition()
	{
		return base.ClosestPoint(base.GetDropPosition() + base.LastAttackedDir * 10f);
	}

	// Token: 0x06002208 RID: 8712 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool SupportsChildDeployables()
	{
		return true;
	}

	// Token: 0x06002209 RID: 8713 RVA: 0x000A875B File Offset: 0x000A695B
	public override bool CanPickup(BasePlayer player)
	{
		return this.children.Count == 0 && base.CanPickup(player);
	}
}
