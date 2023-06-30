using System;
using System.Collections.Generic;
using ConVar;
using Facepunch;
using UnityEngine;

// Token: 0x02000125 RID: 293
public class Sprinkler : IOEntity
{
	// Token: 0x170001F5 RID: 501
	// (get) Token: 0x060016AF RID: 5807 RVA: 0x000AEE98 File Offset: 0x000AD098
	public override bool BlockFluidDraining
	{
		get
		{
			return this.currentFuelSource != null;
		}
	}

	// Token: 0x060016B0 RID: 5808 RVA: 0x0004E9D7 File Offset: 0x0004CBD7
	public override int ConsumptionAmount()
	{
		return 2;
	}

	// Token: 0x060016B1 RID: 5809 RVA: 0x000AEEA6 File Offset: 0x000AD0A6
	public override void UpdateHasPower(int inputAmount, int inputSlot)
	{
		base.UpdateHasPower(inputAmount, inputSlot);
		this.SetSprinklerState(inputAmount > 0);
	}

	// Token: 0x060016B2 RID: 5810 RVA: 0x00036ECC File Offset: 0x000350CC
	public override int CalculateCurrentEnergy(int inputAmount, int inputSlot)
	{
		return inputAmount;
	}

	// Token: 0x060016B3 RID: 5811 RVA: 0x000AEEBC File Offset: 0x000AD0BC
	private void DoSplash()
	{
		using (TimeWarning.New("SprinklerSplash", 0))
		{
			int num = this.WaterPerSplash;
			if (this.updateSplashableCache > this.SplashFrequency * 4f || this.forceUpdateSplashables)
			{
				this.cachedSplashables.Clear();
				this.forceUpdateSplashables = false;
				this.updateSplashableCache = 0f;
				Vector3 position = this.Eyes.position;
				Vector3 up = base.transform.up;
				float num2 = Server.sprinklerEyeHeightOffset;
				float num3 = Vector3.Angle(up, Vector3.up) / 180f;
				num3 = Mathf.Clamp(num3, 0.2f, 1f);
				num2 *= num3;
				Vector3 vector = position + up * (Server.sprinklerRadius * 0.5f);
				Vector3 vector2 = position + up * num2;
				List<BaseEntity> list = Facepunch.Pool.GetList<BaseEntity>();
				global::Vis.Entities<BaseEntity>(vector, vector2, Server.sprinklerRadius, list, 1237003025, QueryTriggerInteraction.Collide);
				if (list.Count > 0)
				{
					foreach (BaseEntity baseEntity in list)
					{
						ISplashable splashable;
						IOEntity ioentity;
						if (!baseEntity.isClient && (splashable = baseEntity as ISplashable) != null && !this.cachedSplashables.Contains(splashable) && splashable.WantsSplash(this.currentFuelType, num) && baseEntity.IsVisible(position, float.PositiveInfinity) && ((ioentity = baseEntity as IOEntity) == null || !base.IsConnectedTo(ioentity, IOEntity.backtracking, false)))
						{
							this.cachedSplashables.Add(splashable);
						}
					}
				}
				Facepunch.Pool.FreeList<BaseEntity>(ref list);
			}
			if (this.cachedSplashables.Count > 0)
			{
				int num4 = num / this.cachedSplashables.Count;
				foreach (ISplashable splashable2 in this.cachedSplashables)
				{
					if (!splashable2.IsUnityNull<ISplashable>() && splashable2.WantsSplash(this.currentFuelType, num4))
					{
						int num5 = splashable2.DoSplash(this.currentFuelType, num4);
						num -= num5;
						if (num <= 0)
						{
							break;
						}
					}
				}
			}
			if (this.DecayPerSplash > 0f)
			{
				base.Hurt(this.DecayPerSplash);
			}
		}
	}

	// Token: 0x060016B4 RID: 5812 RVA: 0x000AF154 File Offset: 0x000AD354
	public void SetSprinklerState(bool wantsOn)
	{
		if (wantsOn)
		{
			this.TurnOn();
			return;
		}
		this.TurnOff();
	}

	// Token: 0x060016B5 RID: 5813 RVA: 0x000AF168 File Offset: 0x000AD368
	public void TurnOn()
	{
		if (base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.forceUpdateSplashables = true;
		if (!base.IsInvoking(new Action(this.DoSplash)))
		{
			base.InvokeRandomized(new Action(this.DoSplash), this.SplashFrequency * 0.5f, this.SplashFrequency, this.SplashFrequency * 0.2f);
		}
	}

	// Token: 0x060016B6 RID: 5814 RVA: 0x000AF1D4 File Offset: 0x000AD3D4
	public void TurnOff()
	{
		if (!base.IsOn())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.On, false, false, true);
		if (base.IsInvoking(new Action(this.DoSplash)))
		{
			base.CancelInvoke(new Action(this.DoSplash));
		}
		this.currentFuelSource = null;
		this.currentFuelType = null;
	}

	// Token: 0x060016B7 RID: 5815 RVA: 0x000AF228 File Offset: 0x000AD428
	public override void SetFuelType(ItemDefinition def, IOEntity source)
	{
		base.SetFuelType(def, source);
		this.currentFuelType = def;
		this.currentFuelSource = source;
	}

	// Token: 0x060016B8 RID: 5816 RVA: 0x000AF240 File Offset: 0x000AD440
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.fromDisk)
		{
			base.SetFlag(BaseEntity.Flags.On, false, false, false);
		}
	}

	// Token: 0x04000EC0 RID: 3776
	public float SplashFrequency = 1f;

	// Token: 0x04000EC1 RID: 3777
	public Transform Eyes;

	// Token: 0x04000EC2 RID: 3778
	public int WaterPerSplash = 1;

	// Token: 0x04000EC3 RID: 3779
	public float DecayPerSplash = 0.8f;

	// Token: 0x04000EC4 RID: 3780
	private ItemDefinition currentFuelType;

	// Token: 0x04000EC5 RID: 3781
	private IOEntity currentFuelSource;

	// Token: 0x04000EC6 RID: 3782
	private HashSet<ISplashable> cachedSplashables = new HashSet<ISplashable>();

	// Token: 0x04000EC7 RID: 3783
	private TimeSince updateSplashableCache;

	// Token: 0x04000EC8 RID: 3784
	private bool forceUpdateSplashables;
}
