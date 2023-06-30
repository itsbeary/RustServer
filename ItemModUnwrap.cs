using System;
using UnityEngine;

// Token: 0x02000185 RID: 389
public class ItemModUnwrap : ItemMod
{
	// Token: 0x060017EB RID: 6123 RVA: 0x000B4320 File Offset: 0x000B2520
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command == "unwrap")
		{
			if (item.amount <= 0)
			{
				return;
			}
			item.UseItem(1);
			int num = UnityEngine.Random.Range(this.minTries, this.maxTries + 1);
			for (int i = 0; i < num; i++)
			{
				this.revealList.SpawnIntoContainer(player.inventory.containerMain);
			}
			if (this.successEffect.isValid)
			{
				Effect.server.Run(this.successEffect.resourcePath, player.eyes.position, default(Vector3), null, false);
			}
		}
	}

	// Token: 0x040010AC RID: 4268
	public LootSpawn revealList;

	// Token: 0x040010AD RID: 4269
	public GameObjectRef successEffect;

	// Token: 0x040010AE RID: 4270
	public int minTries = 1;

	// Token: 0x040010AF RID: 4271
	public int maxTries = 1;
}
