using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x0200046D RID: 1133
public class WildlifeTrap : StorageContainer
{
	// Token: 0x06002584 RID: 9604 RVA: 0x000233C8 File Offset: 0x000215C8
	public bool HasCatch()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06002585 RID: 9605 RVA: 0x0002A700 File Offset: 0x00028900
	public bool IsTrapActive()
	{
		return base.HasFlag(BaseEntity.Flags.On);
	}

	// Token: 0x06002586 RID: 9606 RVA: 0x00089E18 File Offset: 0x00088018
	public override void ResetState()
	{
		base.ResetState();
	}

	// Token: 0x06002587 RID: 9607 RVA: 0x000ECE54 File Offset: 0x000EB054
	public void SetTrapActive(bool trapOn)
	{
		if (trapOn == this.IsTrapActive())
		{
			return;
		}
		base.CancelInvoke(new Action(this.TrapThink));
		base.SetFlag(BaseEntity.Flags.On, trapOn, false, true);
		if (trapOn)
		{
			base.InvokeRepeating(new Action(this.TrapThink), this.tickRate * 0.8f + this.tickRate * UnityEngine.Random.Range(0f, 0.4f), this.tickRate);
		}
	}

	// Token: 0x06002588 RID: 9608 RVA: 0x000ECEC8 File Offset: 0x000EB0C8
	public int GetBaitCalories()
	{
		int num = 0;
		foreach (Item item in base.inventory.itemList)
		{
			ItemModConsumable component = item.info.GetComponent<ItemModConsumable>();
			if (!(component == null) && !this.ignoreBait.Contains(item.info))
			{
				foreach (ItemModConsumable.ConsumableEffect consumableEffect in component.effects)
				{
					if (consumableEffect.type == MetabolismAttribute.Type.Calories && consumableEffect.amount > 0f)
					{
						num += Mathf.CeilToInt(consumableEffect.amount * (float)item.amount);
					}
				}
			}
		}
		return num;
	}

	// Token: 0x06002589 RID: 9609 RVA: 0x000ECFB8 File Offset: 0x000EB1B8
	public void DestroyRandomFoodItem()
	{
		int count = base.inventory.itemList.Count;
		int num = UnityEngine.Random.Range(0, count);
		for (int i = 0; i < count; i++)
		{
			int num2 = num + i;
			if (num2 >= count)
			{
				num2 -= count;
			}
			Item item = base.inventory.itemList[num2];
			if (item != null && !(item.info.GetComponent<ItemModConsumable>() == null))
			{
				item.UseItem(1);
				return;
			}
		}
	}

	// Token: 0x0600258A RID: 9610 RVA: 0x000ED02C File Offset: 0x000EB22C
	public void UseBaitCalories(int numToUse)
	{
		foreach (Item item in base.inventory.itemList)
		{
			int itemCalories = this.GetItemCalories(item);
			if (itemCalories > 0)
			{
				numToUse -= itemCalories;
				item.UseItem(1);
				if (numToUse <= 0)
				{
					break;
				}
			}
		}
	}

	// Token: 0x0600258B RID: 9611 RVA: 0x000ED09C File Offset: 0x000EB29C
	public int GetItemCalories(Item item)
	{
		ItemModConsumable component = item.info.GetComponent<ItemModConsumable>();
		if (component == null)
		{
			return 0;
		}
		foreach (ItemModConsumable.ConsumableEffect consumableEffect in component.effects)
		{
			if (consumableEffect.type == MetabolismAttribute.Type.Calories && consumableEffect.amount > 0f)
			{
				return Mathf.CeilToInt(consumableEffect.amount);
			}
		}
		return 0;
	}

	// Token: 0x0600258C RID: 9612 RVA: 0x000ED128 File Offset: 0x000EB328
	public virtual void TrapThink()
	{
		int baitCalories = this.GetBaitCalories();
		if (baitCalories <= 0)
		{
			return;
		}
		TrappableWildlife randomWildlife = this.GetRandomWildlife();
		if (baitCalories >= randomWildlife.caloriesForInterest && UnityEngine.Random.Range(0f, 1f) <= randomWildlife.successRate)
		{
			this.UseBaitCalories(randomWildlife.caloriesForInterest);
			if (UnityEngine.Random.Range(0f, 1f) <= this.trapSuccessRate)
			{
				this.TrapWildlife(randomWildlife);
			}
		}
	}

	// Token: 0x0600258D RID: 9613 RVA: 0x000ED194 File Offset: 0x000EB394
	public void TrapWildlife(TrappableWildlife trapped)
	{
		Item item = ItemManager.Create(trapped.inventoryObject, UnityEngine.Random.Range(trapped.minToCatch, trapped.maxToCatch + 1), 0UL);
		if (!item.MoveToContainer(base.inventory, -1, true, false, null, true))
		{
			item.Remove(0f);
		}
		else
		{
			base.SetFlag(BaseEntity.Flags.Reserved1, true, false, true);
		}
		this.SetTrapActive(false);
		base.Hurt(this.StartMaxHealth() * 0.1f, DamageType.Decay, null, false);
	}

	// Token: 0x0600258E RID: 9614 RVA: 0x00073EB0 File Offset: 0x000720B0
	public void ClearTrap()
	{
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, true);
	}

	// Token: 0x0600258F RID: 9615 RVA: 0x000ED20D File Offset: 0x000EB40D
	public bool HasBait()
	{
		return this.GetBaitCalories() > 0;
	}

	// Token: 0x06002590 RID: 9616 RVA: 0x000ED218 File Offset: 0x000EB418
	public override void PlayerStoppedLooting(BasePlayer player)
	{
		this.SetTrapActive(this.HasBait());
		this.ClearTrap();
		base.PlayerStoppedLooting(player);
	}

	// Token: 0x06002591 RID: 9617 RVA: 0x000ED233 File Offset: 0x000EB433
	public override bool OnStartBeingLooted(BasePlayer baseEntity)
	{
		this.ClearTrap();
		return base.OnStartBeingLooted(baseEntity);
	}

	// Token: 0x06002592 RID: 9618 RVA: 0x000ED244 File Offset: 0x000EB444
	public TrappableWildlife GetRandomWildlife()
	{
		int num = this.targetWildlife.Sum((WildlifeTrap.WildlifeWeight x) => x.weight);
		int num2 = UnityEngine.Random.Range(0, num);
		for (int i = 0; i < this.targetWildlife.Count; i++)
		{
			num -= this.targetWildlife[i].weight;
			if (num2 >= num)
			{
				return this.targetWildlife[i].wildlife;
			}
		}
		return null;
	}

	// Token: 0x04001DB0 RID: 7600
	public float tickRate = 60f;

	// Token: 0x04001DB1 RID: 7601
	public GameObjectRef trappedEffect;

	// Token: 0x04001DB2 RID: 7602
	public float trappedEffectRepeatRate = 30f;

	// Token: 0x04001DB3 RID: 7603
	public float trapSuccessRate = 0.5f;

	// Token: 0x04001DB4 RID: 7604
	public List<ItemDefinition> ignoreBait;

	// Token: 0x04001DB5 RID: 7605
	public List<WildlifeTrap.WildlifeWeight> targetWildlife;

	// Token: 0x02000D0D RID: 3341
	public static class WildlifeTrapFlags
	{
		// Token: 0x04004694 RID: 18068
		public const BaseEntity.Flags Occupied = BaseEntity.Flags.Reserved1;
	}

	// Token: 0x02000D0E RID: 3342
	[Serializable]
	public class WildlifeWeight
	{
		// Token: 0x04004695 RID: 18069
		public TrappableWildlife wildlife;

		// Token: 0x04004696 RID: 18070
		public int weight;
	}
}
