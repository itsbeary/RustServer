using System;
using UnityEngine;

// Token: 0x020004A5 RID: 1189
public class ModularVehicleShopFront : ShopFront
{
	// Token: 0x06002711 RID: 10001 RVA: 0x000F48CE File Offset: 0x000F2ACE
	public override bool CanBeLooted(BasePlayer player)
	{
		return this.WithinUseDistance(player) && base.CanBeLooted(player);
	}

	// Token: 0x06002712 RID: 10002 RVA: 0x000F48E2 File Offset: 0x000F2AE2
	private bool WithinUseDistance(BasePlayer player)
	{
		return base.Distance(player.eyes.position) <= this.maxUseDistance;
	}

	// Token: 0x04001F6B RID: 8043
	[SerializeField]
	private float maxUseDistance = 1.5f;
}
