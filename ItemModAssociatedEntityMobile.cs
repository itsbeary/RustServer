using System;

// Token: 0x020003A2 RID: 930
public class ItemModAssociatedEntityMobile : ItemModAssociatedEntity<MobileInventoryEntity>
{
	// Token: 0x170002B3 RID: 691
	// (get) Token: 0x060020C5 RID: 8389 RVA: 0x0000441C File Offset: 0x0000261C
	protected override bool AllowNullParenting
	{
		get
		{
			return true;
		}
	}

	// Token: 0x060020C6 RID: 8390 RVA: 0x000D8944 File Offset: 0x000D6B44
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		base.ServerCommand(item, command, player);
		MobileInventoryEntity associatedEntity = ItemModAssociatedEntity<MobileInventoryEntity>.GetAssociatedEntity(item, true);
		if (command == "silenton")
		{
			associatedEntity.SetSilentMode(true);
			return;
		}
		if (command == "silentoff")
		{
			associatedEntity.SetSilentMode(false);
		}
	}
}
