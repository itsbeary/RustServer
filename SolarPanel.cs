using System;
using UnityEngine;

// Token: 0x02000121 RID: 289
public class SolarPanel : IOEntity
{
	// Token: 0x060016A0 RID: 5792 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool IsRootEntity()
	{
		return true;
	}

	// Token: 0x060016A1 RID: 5793 RVA: 0x000AEBA7 File Offset: 0x000ACDA7
	public override int MaximalPowerOutput()
	{
		return this.maximalPowerOutput;
	}

	// Token: 0x060016A2 RID: 5794 RVA: 0x00007A44 File Offset: 0x00005C44
	public override int ConsumptionAmount()
	{
		return 0;
	}

	// Token: 0x060016A3 RID: 5795 RVA: 0x000AEBAF File Offset: 0x000ACDAF
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.SunUpdate), 1f, 5f, 2f);
	}

	// Token: 0x060016A4 RID: 5796 RVA: 0x000AEBD8 File Offset: 0x000ACDD8
	public void SunUpdate()
	{
		int num = this.currentEnergy;
		if (TOD_Sky.Instance.IsNight)
		{
			num = 0;
		}
		else
		{
			Vector3 sunDirection = TOD_Sky.Instance.SunDirection;
			float num2 = Vector3.Dot(this.sunSampler.transform.forward, sunDirection);
			float num3 = Mathf.InverseLerp(this.dot_minimum, this.dot_maximum, num2);
			if (num3 > 0f && !base.IsVisible(this.sunSampler.transform.position + sunDirection * 100f, 101f))
			{
				num3 = 0f;
			}
			num = Mathf.FloorToInt((float)this.maximalPowerOutput * num3 * base.healthFraction);
		}
		bool flag = this.currentEnergy != num;
		this.currentEnergy = num;
		if (flag)
		{
			this.MarkDirty();
		}
	}

	// Token: 0x060016A5 RID: 5797 RVA: 0x000668BB File Offset: 0x00064ABB
	public override int GetPassthroughAmount(int outputSlot = 0)
	{
		if (outputSlot != 0)
		{
			return 0;
		}
		return this.currentEnergy;
	}

	// Token: 0x04000EAD RID: 3757
	public Transform sunSampler;

	// Token: 0x04000EAE RID: 3758
	private const int tickrateSeconds = 60;

	// Token: 0x04000EAF RID: 3759
	public int maximalPowerOutput = 10;

	// Token: 0x04000EB0 RID: 3760
	public float dot_minimum = 0.1f;

	// Token: 0x04000EB1 RID: 3761
	public float dot_maximum = 0.6f;
}
