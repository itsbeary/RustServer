using System;

// Token: 0x020001BD RID: 445
public class ItemModPager : ItemModRFListener
{
	// Token: 0x06001926 RID: 6438 RVA: 0x000B9760 File Offset: 0x000B7960
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		base.ServerCommand(item, command, player);
		PagerEntity component = ItemModAssociatedEntity<BaseEntity>.GetAssociatedEntity(item, true).GetComponent<PagerEntity>();
		if (component)
		{
			if (command == "stop")
			{
				component.SetOff();
				return;
			}
			if (command == "silenton")
			{
				component.SetSilentMode(true);
				return;
			}
			if (command == "silentoff")
			{
				component.SetSilentMode(false);
			}
		}
	}
}
