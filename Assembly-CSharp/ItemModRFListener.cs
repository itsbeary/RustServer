using System;

// Token: 0x020001BE RID: 446
public class ItemModRFListener : ItemModAssociatedEntity<BaseEntity>
{
	// Token: 0x06001928 RID: 6440 RVA: 0x000B97D0 File Offset: 0x000B79D0
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		base.ServerCommand(item, command, player);
	}

	// Token: 0x040011B8 RID: 4536
	public GameObjectRef frequencyPanelPrefab;
}
