using System;
using UnityEngine;

// Token: 0x020003E3 RID: 995
public class WaterPump : LiquidContainer
{
	// Token: 0x06002249 RID: 8777 RVA: 0x000DE19D File Offset: 0x000DC39D
	public override int ConsumptionAmount()
	{
		return this.PowerConsumption;
	}

	// Token: 0x0600224A RID: 8778 RVA: 0x000DE1A8 File Offset: 0x000DC3A8
	private void CreateWater()
	{
		if (this.IsFull())
		{
			return;
		}
		ItemDefinition atPoint = WaterResource.GetAtPoint(this.WaterResourceLocation.position);
		if (atPoint != null)
		{
			base.inventory.AddItem(atPoint, this.AmountPerPump, 0UL, ItemContainer.LimitStack.Existing);
			base.UpdateOnFlag();
		}
	}

	// Token: 0x0600224B RID: 8779 RVA: 0x000DE1F4 File Offset: 0x000DC3F4
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		bool flag = next.HasFlag(BaseEntity.Flags.Reserved8);
		if (base.isServer && old.HasFlag(BaseEntity.Flags.Reserved8) != flag)
		{
			if (flag)
			{
				if (!base.IsInvoking(new Action(this.CreateWater)))
				{
					base.InvokeRandomized(new Action(this.CreateWater), this.PumpInterval, this.PumpInterval, this.PumpInterval * 0.1f);
					return;
				}
			}
			else if (base.IsInvoking(new Action(this.CreateWater)))
			{
				base.CancelInvoke(new Action(this.CreateWater));
			}
		}
	}

	// Token: 0x0600224C RID: 8780 RVA: 0x000728A0 File Offset: 0x00070AA0
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		return Mathf.Clamp(base.GetLiquidCount(), 0, this.maxOutputFlow);
	}

	// Token: 0x0600224D RID: 8781 RVA: 0x000DDF38 File Offset: 0x000DC138
	private bool IsFull()
	{
		return base.inventory.itemList.Count != 0 && base.inventory.itemList[0].amount >= base.inventory.maxStackSize;
	}

	// Token: 0x170002E0 RID: 736
	// (get) Token: 0x0600224E RID: 8782 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsGravitySource
	{
		get
		{
			return true;
		}
	}

	// Token: 0x04001A77 RID: 6775
	public Transform WaterResourceLocation;

	// Token: 0x04001A78 RID: 6776
	public float PumpInterval = 20f;

	// Token: 0x04001A79 RID: 6777
	public int AmountPerPump = 30;

	// Token: 0x04001A7A RID: 6778
	public int PowerConsumption = 5;
}
