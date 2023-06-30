using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200030D RID: 781
public class ItemButtonTools : MonoBehaviour
{
	// Token: 0x06001EDB RID: 7899 RVA: 0x000D20B8 File Offset: 0x000D02B8
	public void GiveSelf(int amount)
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "inventory.giveid", new object[]
		{
			this.itemDef.itemid,
			amount
		});
	}

	// Token: 0x06001EDC RID: 7900 RVA: 0x000D20EC File Offset: 0x000D02EC
	public void GiveArmed()
	{
		ConsoleSystem.Run(ConsoleSystem.Option.Client, "inventory.givearm", new object[] { this.itemDef.itemid });
	}

	// Token: 0x06001EDD RID: 7901 RVA: 0x000063A5 File Offset: 0x000045A5
	public void GiveBlueprint()
	{
	}

	// Token: 0x040017C6 RID: 6086
	public Image image;

	// Token: 0x040017C7 RID: 6087
	public ItemDefinition itemDef;
}
