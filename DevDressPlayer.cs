using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200031F RID: 799
public class DevDressPlayer : MonoBehaviour
{
	// Token: 0x06001F15 RID: 7957 RVA: 0x000D3548 File Offset: 0x000D1748
	private void ServerInitComponent()
	{
		BasePlayer component = base.GetComponent<BasePlayer>();
		if (this.DressRandomly)
		{
			this.DoRandomClothes(component);
		}
		foreach (ItemAmount itemAmount in this.clothesToWear)
		{
			if (!(itemAmount.itemDef == null))
			{
				ItemManager.Create(itemAmount.itemDef, 1, 0UL).MoveToContainer(component.inventory.containerWear, -1, true, false, null, true);
			}
		}
	}

	// Token: 0x06001F16 RID: 7958 RVA: 0x000D35DC File Offset: 0x000D17DC
	private void DoRandomClothes(BasePlayer player)
	{
		string text = "";
		foreach (ItemDefinition itemDefinition in (from x in ItemManager.GetItemDefinitions()
			where x.GetComponent<ItemModWearable>()
			orderby Guid.NewGuid()
			select x).Take(UnityEngine.Random.Range(0, 4)))
		{
			ItemManager.Create(itemDefinition, 1, 0UL).MoveToContainer(player.inventory.containerWear, -1, true, false, null, true);
			text = text + itemDefinition.shortname + " ";
		}
		text = text.Trim();
		if (text == "")
		{
			text = "naked";
		}
		player.displayName = text;
	}

	// Token: 0x04001804 RID: 6148
	public bool DressRandomly;

	// Token: 0x04001805 RID: 6149
	public List<ItemAmount> clothesToWear;
}
