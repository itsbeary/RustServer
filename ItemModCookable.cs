using System;
using Facepunch.Rust;
using UnityEngine;

// Token: 0x020005F1 RID: 1521
public class ItemModCookable : ItemMod
{
	// Token: 0x06002D9B RID: 11675 RVA: 0x00112FF0 File Offset: 0x001111F0
	public void OnValidate()
	{
		if (this.amountOfBecome < 1)
		{
			this.amountOfBecome = 1;
		}
		if (this.becomeOnCooked == null)
		{
			Debug.LogWarning("[ItemModCookable] becomeOnCooked is unset! [" + base.name + "]", base.gameObject);
		}
	}

	// Token: 0x06002D9C RID: 11676 RVA: 0x00113030 File Offset: 0x00111230
	public bool CanBeCookedByAtTemperature(float temperature)
	{
		return temperature > (float)this.lowTemp && temperature < (float)this.highTemp;
	}

	// Token: 0x06002D9D RID: 11677 RVA: 0x00113048 File Offset: 0x00111248
	private void CycleCooking(Item item, float delta)
	{
		if (!this.CanBeCookedByAtTemperature(item.temperature) || item.cookTimeLeft < 0f)
		{
			if (this.setCookingFlag && item.HasFlag(Item.Flag.Cooking))
			{
				item.SetFlag(Item.Flag.Cooking, false);
				item.MarkDirty();
			}
			return;
		}
		if (this.setCookingFlag && !item.HasFlag(Item.Flag.Cooking))
		{
			item.SetFlag(Item.Flag.Cooking, true);
			item.MarkDirty();
		}
		item.cookTimeLeft -= delta;
		if (item.cookTimeLeft > 0f)
		{
			item.MarkDirty();
			return;
		}
		float num = item.cookTimeLeft * -1f;
		int num2 = 1 + Mathf.FloorToInt(num / this.cookTime);
		item.cookTimeLeft = this.cookTime - num % this.cookTime;
		BaseOven baseOven = item.GetEntityOwner() as BaseOven;
		num2 = Mathf.Min(num2, item.amount);
		if (item.amount > num2)
		{
			item.amount -= num2;
			item.MarkDirty();
		}
		else
		{
			item.Remove(0f);
		}
		Analytics.Azure.AddPendingItems(baseOven, item.info.shortname, num2, "smelt", true, false);
		if (this.becomeOnCooked != null)
		{
			Item item2 = ItemManager.Create(this.becomeOnCooked, this.amountOfBecome * num2, 0UL);
			Analytics.Azure.AddPendingItems(baseOven, item2.info.shortname, item2.amount, "smelt", false, false);
			if (item2 != null && !item2.MoveToContainer(item.parent, -1, true, false, null, true) && !item2.MoveToContainer(item.parent, -1, true, false, null, true))
			{
				item2.Drop(item.parent.dropPosition, item.parent.dropVelocity, default(Quaternion));
				if (item.parent.entityOwner && baseOven != null)
				{
					baseOven.OvenFull();
				}
			}
		}
	}

	// Token: 0x06002D9E RID: 11678 RVA: 0x00113219 File Offset: 0x00111419
	public override void OnItemCreated(Item itemcreated)
	{
		itemcreated.cookTimeLeft = this.cookTime;
		itemcreated.onCycle += this.CycleCooking;
	}

	// Token: 0x04002544 RID: 9540
	[ItemSelector(ItemCategory.All)]
	public ItemDefinition becomeOnCooked;

	// Token: 0x04002545 RID: 9541
	public float cookTime = 30f;

	// Token: 0x04002546 RID: 9542
	public int amountOfBecome = 1;

	// Token: 0x04002547 RID: 9543
	public int lowTemp;

	// Token: 0x04002548 RID: 9544
	public int highTemp;

	// Token: 0x04002549 RID: 9545
	public bool setCookingFlag;
}
