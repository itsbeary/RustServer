using System;
using UnityEngine;

// Token: 0x0200012A RID: 298
[CreateAssetMenu(menuName = "Rust/NPC Vending Order")]
public class NPCVendingOrder : ScriptableObject
{
	// Token: 0x04000EE0 RID: 3808
	public NPCVendingOrder.Entry[] orders;

	// Token: 0x02000C35 RID: 3125
	[Serializable]
	public class Entry
	{
		// Token: 0x040042E9 RID: 17129
		public ItemDefinition sellItem;

		// Token: 0x040042EA RID: 17130
		public int sellItemAmount;

		// Token: 0x040042EB RID: 17131
		public bool sellItemAsBP;

		// Token: 0x040042EC RID: 17132
		public ItemDefinition currencyItem;

		// Token: 0x040042ED RID: 17133
		public int currencyAmount;

		// Token: 0x040042EE RID: 17134
		public bool currencyAsBP;

		// Token: 0x040042EF RID: 17135
		[Tooltip("The higher this number, the more likely this will be chosen")]
		public int weight;

		// Token: 0x040042F0 RID: 17136
		public int refillAmount = 1;

		// Token: 0x040042F1 RID: 17137
		public float refillDelay = 10f;
	}
}
