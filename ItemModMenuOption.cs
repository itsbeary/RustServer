using System;
using UnityEngine;

// Token: 0x020005FA RID: 1530
public class ItemModMenuOption : ItemMod
{
	// Token: 0x06002DBC RID: 11708 RVA: 0x00113825 File Offset: 0x00111A25
	public override void ServerCommand(Item item, string command, BasePlayer player)
	{
		if (command != this.commandName)
		{
			return;
		}
		if (!this.actionTarget.CanDoAction(item, player))
		{
			return;
		}
		this.actionTarget.DoAction(item, player);
	}

	// Token: 0x06002DBD RID: 11709 RVA: 0x00113854 File Offset: 0x00111A54
	private void OnValidate()
	{
		if (this.actionTarget == null)
		{
			Debug.LogWarning("ItemModMenuOption: actionTarget is null!", base.gameObject);
		}
		if (string.IsNullOrEmpty(this.commandName))
		{
			Debug.LogWarning("ItemModMenuOption: commandName can't be empty!", base.gameObject);
		}
		if (this.option.icon == null)
		{
			Debug.LogWarning("No icon set for ItemModMenuOption " + base.gameObject.name, base.gameObject);
		}
	}

	// Token: 0x04002569 RID: 9577
	public string commandName;

	// Token: 0x0400256A RID: 9578
	public ItemMod actionTarget;

	// Token: 0x0400256B RID: 9579
	public BaseEntity.Menu.Option option;

	// Token: 0x0400256C RID: 9580
	[Tooltip("If true, this is the command that will run when an item is 'selected' on the toolbar")]
	public bool isPrimaryOption = true;
}
