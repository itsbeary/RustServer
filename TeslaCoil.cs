using System;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x02000126 RID: 294
public class TeslaCoil : IOEntity
{
	// Token: 0x060016BA RID: 5818 RVA: 0x000AF28B File Offset: 0x000AD48B
	public override int ConsumptionAmount()
	{
		return Mathf.CeilToInt(this.maxDamageOutput / this.powerToDamageRatio);
	}

	// Token: 0x060016BB RID: 5819 RVA: 0x000AF29F File Offset: 0x000AD49F
	public bool CanDischarge()
	{
		return base.healthFraction >= 0.25f;
	}

	// Token: 0x060016BC RID: 5820 RVA: 0x000AF2B4 File Offset: 0x000AD4B4
	public override void UpdateFromInput(int inputAmount, int inputSlot)
	{
		base.UpdateFromInput(inputAmount, inputSlot);
		if (inputAmount > 0 && this.CanDischarge())
		{
			float num = Time.time - this.lastDischargeTime;
			if (num < 0f)
			{
				num = 0f;
			}
			float num2 = Mathf.Min(this.dischargeTickRate - num, this.dischargeTickRate);
			base.InvokeRepeating(new Action(this.Discharge), num2, this.dischargeTickRate);
			base.SetFlag(BaseEntity.Flags.Reserved1, inputAmount < this.powerForHeavyShorting, false, false);
			base.SetFlag(BaseEntity.Flags.Reserved2, inputAmount >= this.powerForHeavyShorting, false, false);
			base.SetFlag(BaseEntity.Flags.On, true, false, true);
			return;
		}
		base.CancelInvoke(new Action(this.Discharge));
		base.SetFlag(BaseEntity.Flags.Reserved1, false, false, false);
		base.SetFlag(BaseEntity.Flags.Reserved2, false, false, false);
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
	}

	// Token: 0x060016BD RID: 5821 RVA: 0x000AF398 File Offset: 0x000AD598
	public void Discharge()
	{
		float num = Mathf.Clamp((float)this.currentEnergy * this.powerToDamageRatio, 0f, this.maxDamageOutput) * this.dischargeTickRate;
		this.lastDischargeTime = Time.time;
		if (this.targetTrigger.entityContents != null)
		{
			BaseEntity[] array = this.targetTrigger.entityContents.ToArray<BaseEntity>();
			if (array != null)
			{
				BaseEntity[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					BaseCombatEntity component = array2[i].GetComponent<BaseCombatEntity>();
					if (component && component.IsVisible(this.damageEyes.transform.position, component.CenterPoint(), float.PositiveInfinity))
					{
						component.OnAttacked(new HitInfo(this, component, DamageType.ElectricShock, num));
					}
				}
			}
		}
		float num2 = this.dischargeTickRate / this.maxDischargeSelfDamageSeconds * this.MaxHealth();
		base.Hurt(num2, DamageType.ElectricShock, this, false);
		if (!this.CanDischarge())
		{
			this.MarkDirty();
		}
	}

	// Token: 0x04000EC9 RID: 3785
	public TargetTrigger targetTrigger;

	// Token: 0x04000ECA RID: 3786
	public TriggerMovement movementTrigger;

	// Token: 0x04000ECB RID: 3787
	public float powerToDamageRatio = 2f;

	// Token: 0x04000ECC RID: 3788
	public float dischargeTickRate = 0.25f;

	// Token: 0x04000ECD RID: 3789
	public float maxDischargeSelfDamageSeconds = 120f;

	// Token: 0x04000ECE RID: 3790
	public float maxDamageOutput = 35f;

	// Token: 0x04000ECF RID: 3791
	public Transform damageEyes;

	// Token: 0x04000ED0 RID: 3792
	public const BaseEntity.Flags Flag_WeakShorting = BaseEntity.Flags.Reserved1;

	// Token: 0x04000ED1 RID: 3793
	public const BaseEntity.Flags Flag_StrongShorting = BaseEntity.Flags.Reserved2;

	// Token: 0x04000ED2 RID: 3794
	public int powerForHeavyShorting = 10;

	// Token: 0x04000ED3 RID: 3795
	private float lastDischargeTime;
}
