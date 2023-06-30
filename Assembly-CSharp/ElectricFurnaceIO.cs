using System;

// Token: 0x020004CE RID: 1230
public class ElectricFurnaceIO : IOEntity, IIndustrialStorage
{
	// Token: 0x06002832 RID: 10290 RVA: 0x000FA232 File Offset: 0x000F8432
	public override int ConsumptionAmount()
	{
		return this.PowerConsumption;
	}

	// Token: 0x06002833 RID: 10291 RVA: 0x000FA23A File Offset: 0x000F843A
	public override int DesiredPower()
	{
		if (base.GetParentEntity() == null)
		{
			return 0;
		}
		if (!base.GetParentEntity().IsOn())
		{
			return 0;
		}
		return this.PowerConsumption;
	}

	// Token: 0x06002834 RID: 10292 RVA: 0x000FA264 File Offset: 0x000F8464
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (base.isServer)
		{
			ElectricOven parentOven = this.GetParentOven();
			if (parentOven != null)
			{
				parentOven.OnIOEntityFlagsChanged(old, next);
			}
		}
	}

	// Token: 0x06002835 RID: 10293 RVA: 0x000FA29C File Offset: 0x000F849C
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		if (inputSlot == 0)
		{
			base.UpdateHasPower(inputAmount, inputSlot);
		}
		if (inputSlot == 1 && inputAmount > 0)
		{
			ElectricOven parentOven = this.GetParentOven();
			if (parentOven != null)
			{
				parentOven.StartCooking();
			}
		}
		if (inputSlot == 2 && inputAmount > 0)
		{
			ElectricOven parentOven2 = this.GetParentOven();
			if (parentOven2 != null)
			{
				parentOven2.StopCooking();
			}
		}
	}

	// Token: 0x06002836 RID: 10294 RVA: 0x000FA2F0 File Offset: 0x000F84F0
	private ElectricOven GetParentOven()
	{
		return base.GetParentEntity() as ElectricOven;
	}

	// Token: 0x17000367 RID: 871
	// (get) Token: 0x06002837 RID: 10295 RVA: 0x000FA2FD File Offset: 0x000F84FD
	public ItemContainer Container
	{
		get
		{
			return this.GetParentOven().inventory;
		}
	}

	// Token: 0x06002838 RID: 10296 RVA: 0x000FA30A File Offset: 0x000F850A
	public Vector2i InputSlotRange(int slotIndex)
	{
		return new Vector2i(1, 2);
	}

	// Token: 0x06002839 RID: 10297 RVA: 0x000FA313 File Offset: 0x000F8513
	public Vector2i OutputSlotRange(int slotIndex)
	{
		return new Vector2i(3, 5);
	}

	// Token: 0x0600283A RID: 10298 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnStorageItemTransferBegin()
	{
	}

	// Token: 0x0600283B RID: 10299 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnStorageItemTransferEnd()
	{
	}

	// Token: 0x17000368 RID: 872
	// (get) Token: 0x0600283C RID: 10300 RVA: 0x000037E7 File Offset: 0x000019E7
	public BaseEntity IndustrialEntity
	{
		get
		{
			return this;
		}
	}

	// Token: 0x040020A7 RID: 8359
	public int PowerConsumption = 3;
}
