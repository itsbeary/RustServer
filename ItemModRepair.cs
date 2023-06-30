using System;
using UnityEngine;

// Token: 0x02000602 RID: 1538
public class ItemModRepair : ItemMod
{
	// Token: 0x06002DD5 RID: 11733 RVA: 0x00113ED4 File Offset: 0x001120D4
	public bool HasCraftLevel(BasePlayer player = null)
	{
		return player != null && player.isServer && player.currentCraftLevel >= (float)this.workbenchLvlRequired;
	}

	// Token: 0x06002DD6 RID: 11734 RVA: 0x00113EFC File Offset: 0x001120FC
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "refill")
		{
			if (player.IsSwimming())
			{
				return;
			}
			if (!this.HasCraftLevel(player))
			{
				return;
			}
			if (item.conditionNormalized >= 1f)
			{
				return;
			}
			item.DoRepair(this.conditionLost);
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}

	// Token: 0x0400258D RID: 9613
	public float conditionLost = 0.05f;

	// Token: 0x0400258E RID: 9614
	public GameObjectRef successEffect;

	// Token: 0x0400258F RID: 9615
	public int workbenchLvlRequired;
}
