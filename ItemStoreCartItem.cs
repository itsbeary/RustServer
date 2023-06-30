using System;
using TMPro;
using UnityEngine;

// Token: 0x02000879 RID: 2169
public class ItemStoreCartItem : MonoBehaviour
{
	// Token: 0x0600365A RID: 13914 RVA: 0x0014878D File Offset: 0x0014698D
	public void Init(int index, IPlayerItemDefinition def)
	{
		this.Index = index;
		this.Name.text = def.Name;
		this.Price.text = def.LocalPriceFormatted;
	}

	// Token: 0x04003104 RID: 12548
	public int Index;

	// Token: 0x04003105 RID: 12549
	public TextMeshProUGUI Name;

	// Token: 0x04003106 RID: 12550
	public TextMeshProUGUI Price;
}
