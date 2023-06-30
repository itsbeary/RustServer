using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005EB RID: 1515
public class ItemModConsumable : MonoBehaviour
{
	// Token: 0x06002D84 RID: 11652 RVA: 0x0011282C File Offset: 0x00110A2C
	public float GetIfType(MetabolismAttribute.Type typeToPick)
	{
		for (int i = 0; i < this.effects.Count; i++)
		{
			if (this.effects[i].type == typeToPick)
			{
				return this.effects[i].amount;
			}
		}
		return 0f;
	}

	// Token: 0x0400252C RID: 9516
	public int amountToConsume = 1;

	// Token: 0x0400252D RID: 9517
	public float conditionFractionToLose;

	// Token: 0x0400252E RID: 9518
	public string achievementWhenEaten;

	// Token: 0x0400252F RID: 9519
	public List<ItemModConsumable.ConsumableEffect> effects = new List<ItemModConsumable.ConsumableEffect>();

	// Token: 0x04002530 RID: 9520
	public List<ModifierDefintion> modifiers = new List<ModifierDefintion>();

	// Token: 0x02000D9B RID: 3483
	[Serializable]
	public class ConsumableEffect
	{
		// Token: 0x040048A2 RID: 18594
		public MetabolismAttribute.Type type;

		// Token: 0x040048A3 RID: 18595
		public float amount;

		// Token: 0x040048A4 RID: 18596
		public float time;

		// Token: 0x040048A5 RID: 18597
		public float onlyIfHealthLessThan = 1f;
	}
}
