using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x02000122 RID: 290
public class Igniter : IOEntity
{
	// Token: 0x060016A7 RID: 5799 RVA: 0x000AECC6 File Offset: 0x000ACEC6
	public override int ConsumptionAmount()
	{
		return this.PowerConsumption;
	}

	// Token: 0x060016A8 RID: 5800 RVA: 0x000AECD0 File Offset: 0x000ACED0
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		if (inputAmount > 0)
		{
			base.InvokeRepeating(new Action(this.IgniteInRange), this.IgniteStartDelay, this.IgniteFrequency);
			return;
		}
		if (base.IsInvoking(new Action(this.IgniteInRange)))
		{
			base.CancelInvoke(new Action(this.IgniteInRange));
		}
	}

	// Token: 0x060016A9 RID: 5801 RVA: 0x000AED30 File Offset: 0x000ACF30
	private void IgniteInRange()
	{
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		Vis.Entities<BaseEntity>(this.LineOfSightEyes.position, this.IgniteRange, list, 1237019409, QueryTriggerInteraction.Collide);
		int num = 0;
		foreach (BaseEntity baseEntity in list)
		{
			if (!baseEntity.HasFlag(BaseEntity.Flags.On) && baseEntity.IsVisible(this.LineOfSightEyes.position, float.PositiveInfinity))
			{
				IIgniteable igniteable;
				if (baseEntity.isServer && baseEntity is BaseOven)
				{
					(baseEntity as BaseOven).StartCooking();
					if (baseEntity.HasFlag(BaseEntity.Flags.On))
					{
						num++;
					}
				}
				else if (baseEntity.isServer && (igniteable = baseEntity as IIgniteable) != null && igniteable.CanIgnite())
				{
					igniteable.Ignite(base.transform.position);
					num++;
				}
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		base.Hurt(this.SelfDamagePerIgnite, DamageType.ElectricShock, this, false);
	}

	// Token: 0x04000EB2 RID: 3762
	public float IgniteRange = 5f;

	// Token: 0x04000EB3 RID: 3763
	public float IgniteFrequency = 1f;

	// Token: 0x04000EB4 RID: 3764
	public float IgniteStartDelay;

	// Token: 0x04000EB5 RID: 3765
	public Transform LineOfSightEyes;

	// Token: 0x04000EB6 RID: 3766
	public float SelfDamagePerIgnite = 0.5f;

	// Token: 0x04000EB7 RID: 3767
	public int PowerConsumption = 2;
}
