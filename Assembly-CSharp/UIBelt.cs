using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000857 RID: 2135
public class UIBelt : SingletonComponent<UIBelt>
{
	// Token: 0x0600361F RID: 13855 RVA: 0x0014817D File Offset: 0x0014637D
	protected override void Awake()
	{
		this.ItemIcons = (from s in base.GetComponentsInChildren<ItemIcon>()
			orderby s.slot
			select s).ToList<ItemIcon>();
	}

	// Token: 0x06003620 RID: 13856 RVA: 0x001481B4 File Offset: 0x001463B4
	public ItemIcon GetItemIconAtSlot(int slot)
	{
		if (slot < 0 || slot >= this.ItemIcons.Count)
		{
			return null;
		}
		return this.ItemIcons[slot];
	}

	// Token: 0x0400302C RID: 12332
	public List<ItemIcon> ItemIcons;
}
