using System;
using UnityEngine;

// Token: 0x02000460 RID: 1120
public class RandomItemDispenser : PrefabAttribute, IServerComponent
{
	// Token: 0x0600252F RID: 9519 RVA: 0x000EB3D7 File Offset: 0x000E95D7
	protected override Type GetIndexedType()
	{
		return typeof(RandomItemDispenser);
	}

	// Token: 0x06002530 RID: 9520 RVA: 0x000EB3E4 File Offset: 0x000E95E4
	public void DistributeItems(BasePlayer forPlayer, Vector3 distributorPosition)
	{
		foreach (RandomItemDispenser.RandomItemChance randomItemChance in this.Chances)
		{
			bool flag = this.TryAward(randomItemChance, forPlayer, distributorPosition);
			if (this.OnlyAwardOne && flag)
			{
				break;
			}
		}
	}

	// Token: 0x06002531 RID: 9521 RVA: 0x000EB424 File Offset: 0x000E9624
	private bool TryAward(RandomItemDispenser.RandomItemChance itemChance, BasePlayer forPlayer, Vector3 distributorPosition)
	{
		float num = UnityEngine.Random.Range(0f, 1f);
		if (itemChance.Chance >= num)
		{
			Item item = ItemManager.Create(itemChance.Item, itemChance.Amount, 0UL);
			if (item != null)
			{
				if (forPlayer)
				{
					forPlayer.GiveItem(item, BaseEntity.GiveItemReason.ResourceHarvested);
				}
				else
				{
					item.Drop(distributorPosition + Vector3.up * 0.5f, Vector3.up, default(Quaternion));
				}
			}
			return true;
		}
		return false;
	}

	// Token: 0x04001D6C RID: 7532
	public RandomItemDispenser.RandomItemChance[] Chances;

	// Token: 0x04001D6D RID: 7533
	public bool OnlyAwardOne = true;

	// Token: 0x02000D05 RID: 3333
	[Serializable]
	public struct RandomItemChance
	{
		// Token: 0x04004684 RID: 18052
		public ItemDefinition Item;

		// Token: 0x04004685 RID: 18053
		public int Amount;

		// Token: 0x04004686 RID: 18054
		[Range(0f, 1f)]
		public float Chance;
	}
}
