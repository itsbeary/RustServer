using System;
using Facepunch.Extend;
using UnityEngine;

// Token: 0x02000177 RID: 375
[ConsoleSystem.Factory("note")]
public class note : ConsoleSystem
{
	// Token: 0x060017AE RID: 6062 RVA: 0x000B37D4 File Offset: 0x000B19D4
	[ServerUserVar]
	public static void update(ConsoleSystem.Arg arg)
	{
		ItemId itemID = arg.GetItemID(0, default(ItemId));
		string @string = arg.GetString(1, "");
		Item item = arg.Player().inventory.FindItemUID(itemID);
		if (item == null)
		{
			return;
		}
		item.text = @string.Truncate(1024, null);
		item.MarkDirty();
	}
}
