using System;
using UnityEngine;

// Token: 0x0200012B RID: 299
[CreateAssetMenu(menuName = "Rust/NPCVendingOrderManifest")]
public class NPCVendingOrderManifest : ScriptableObject
{
	// Token: 0x060016CF RID: 5839 RVA: 0x000AF834 File Offset: 0x000ADA34
	public int GetIndex(NPCVendingOrder sample)
	{
		if (sample == null)
		{
			return -1;
		}
		for (int i = 0; i < this.orderList.Length; i++)
		{
			NPCVendingOrder npcvendingOrder = this.orderList[i];
			if (sample == npcvendingOrder)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x060016D0 RID: 5840 RVA: 0x000AF874 File Offset: 0x000ADA74
	public NPCVendingOrder GetFromIndex(int index)
	{
		if (this.orderList == null)
		{
			return null;
		}
		if (index < 0)
		{
			return null;
		}
		if (index >= this.orderList.Length)
		{
			return null;
		}
		return this.orderList[index];
	}

	// Token: 0x04000EE1 RID: 3809
	public NPCVendingOrder[] orderList;
}
