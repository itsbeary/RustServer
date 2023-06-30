using System;
using UnityEngine;

// Token: 0x020004C3 RID: 1219
public class VehicleEngineController<TOwner> where TOwner : BaseVehicle, IEngineControllerUser
{
	// Token: 0x1700035E RID: 862
	// (get) Token: 0x060027E1 RID: 10209 RVA: 0x000F8D6B File Offset: 0x000F6F6B
	public VehicleEngineController<TOwner>.EngineState CurEngineState
	{
		get
		{
			if (this.owner.HasFlag(this.engineStartingFlag))
			{
				return VehicleEngineController<TOwner>.EngineState.Starting;
			}
			if (this.owner.HasFlag(BaseEntity.Flags.On))
			{
				return VehicleEngineController<TOwner>.EngineState.On;
			}
			return VehicleEngineController<TOwner>.EngineState.Off;
		}
	}

	// Token: 0x1700035F RID: 863
	// (get) Token: 0x060027E2 RID: 10210 RVA: 0x000F8D9D File Offset: 0x000F6F9D
	public bool IsOn
	{
		get
		{
			return this.CurEngineState == VehicleEngineController<TOwner>.EngineState.On;
		}
	}

	// Token: 0x17000360 RID: 864
	// (get) Token: 0x060027E3 RID: 10211 RVA: 0x000F8DA8 File Offset: 0x000F6FA8
	public bool IsOff
	{
		get
		{
			return this.CurEngineState == VehicleEngineController<TOwner>.EngineState.Off;
		}
	}

	// Token: 0x17000361 RID: 865
	// (get) Token: 0x060027E4 RID: 10212 RVA: 0x000F8DB3 File Offset: 0x000F6FB3
	public bool IsStarting
	{
		get
		{
			return this.CurEngineState == VehicleEngineController<TOwner>.EngineState.Starting;
		}
	}

	// Token: 0x17000362 RID: 866
	// (get) Token: 0x060027E5 RID: 10213 RVA: 0x000F8DBE File Offset: 0x000F6FBE
	public bool IsStartingOrOn
	{
		get
		{
			return this.CurEngineState > VehicleEngineController<TOwner>.EngineState.Off;
		}
	}

	// Token: 0x17000363 RID: 867
	// (get) Token: 0x060027E6 RID: 10214 RVA: 0x000F8DC9 File Offset: 0x000F6FC9
	// (set) Token: 0x060027E7 RID: 10215 RVA: 0x000F8DD1 File Offset: 0x000F6FD1
	public EntityFuelSystem FuelSystem { get; private set; }

	// Token: 0x060027E8 RID: 10216 RVA: 0x000F8DDC File Offset: 0x000F6FDC
	public VehicleEngineController(TOwner owner, bool isServer, float engineStartupTime, GameObjectRef fuelStoragePrefab, Transform waterloggedPoint = null, BaseEntity.Flags engineStartingFlag = BaseEntity.Flags.Reserved1)
	{
		this.FuelSystem = new EntityFuelSystem(isServer, fuelStoragePrefab, owner.children, true);
		this.owner = owner;
		this.isServer = isServer;
		this.engineStartupTime = engineStartupTime;
		this.waterloggedPoint = waterloggedPoint;
		this.engineStartingFlag = engineStartingFlag;
	}

	// Token: 0x060027E9 RID: 10217 RVA: 0x000F8E2E File Offset: 0x000F702E
	public VehicleEngineController<TOwner>.EngineState EngineStateFrom(BaseEntity.Flags flags)
	{
		if (flags.HasFlag(this.engineStartingFlag))
		{
			return VehicleEngineController<TOwner>.EngineState.Starting;
		}
		if (flags.HasFlag(BaseEntity.Flags.On))
		{
			return VehicleEngineController<TOwner>.EngineState.On;
		}
		return VehicleEngineController<TOwner>.EngineState.Off;
	}

	// Token: 0x060027EA RID: 10218 RVA: 0x000F8E60 File Offset: 0x000F7060
	public void TryStartEngine(BasePlayer player)
	{
		if (!this.isServer)
		{
			return;
		}
		if (this.owner.IsDead())
		{
			return;
		}
		if (this.IsStartingOrOn)
		{
			return;
		}
		if (player.net == null)
		{
			return;
		}
		if (!this.CanRunEngine())
		{
			this.owner.OnEngineStartFailed();
			return;
		}
		this.owner.SetFlag(this.engineStartingFlag, true, false, true);
		this.owner.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.owner.Invoke(new Action(this.FinishStartingEngine), this.engineStartupTime);
	}

	// Token: 0x060027EB RID: 10219 RVA: 0x000F8F04 File Offset: 0x000F7104
	public void FinishStartingEngine()
	{
		if (!this.isServer)
		{
			return;
		}
		if (this.owner.IsDead())
		{
			return;
		}
		if (this.IsOn)
		{
			return;
		}
		this.owner.SetFlag(BaseEntity.Flags.On, true, false, true);
		this.owner.SetFlag(this.engineStartingFlag, false, false, true);
	}

	// Token: 0x060027EC RID: 10220 RVA: 0x000F8F64 File Offset: 0x000F7164
	public void StopEngine()
	{
		if (!this.isServer)
		{
			return;
		}
		if (this.IsOff)
		{
			return;
		}
		this.CancelEngineStart();
		this.owner.SetFlag(BaseEntity.Flags.On, false, false, true);
		this.owner.SetFlag(this.engineStartingFlag, false, false, true);
	}

	// Token: 0x060027ED RID: 10221 RVA: 0x000F8FB6 File Offset: 0x000F71B6
	public void CheckEngineState()
	{
		if (this.IsStartingOrOn && !this.CanRunEngine())
		{
			this.StopEngine();
		}
	}

	// Token: 0x060027EE RID: 10222 RVA: 0x000F8FCE File Offset: 0x000F71CE
	public bool CanRunEngine()
	{
		return this.owner.MeetsEngineRequirements() && this.FuelSystem.HasFuel(false) && !this.IsWaterlogged() && !this.owner.IsDead();
	}

	// Token: 0x060027EF RID: 10223 RVA: 0x000F900D File Offset: 0x000F720D
	public bool IsWaterlogged()
	{
		return this.waterloggedPoint != null && WaterLevel.Test(this.waterloggedPoint.position, true, true, this.owner);
	}

	// Token: 0x060027F0 RID: 10224 RVA: 0x000F903C File Offset: 0x000F723C
	public int TickFuel(float fuelPerSecond)
	{
		if (this.IsOn)
		{
			return this.FuelSystem.TryUseFuel(Time.fixedDeltaTime, fuelPerSecond);
		}
		return 0;
	}

	// Token: 0x060027F1 RID: 10225 RVA: 0x000F9059 File Offset: 0x000F7259
	private void CancelEngineStart()
	{
		if (this.CurEngineState != VehicleEngineController<TOwner>.EngineState.Starting)
		{
			return;
		}
		this.owner.CancelInvoke(new Action(this.FinishStartingEngine));
	}

	// Token: 0x04002084 RID: 8324
	private readonly TOwner owner;

	// Token: 0x04002085 RID: 8325
	private readonly bool isServer;

	// Token: 0x04002086 RID: 8326
	private readonly float engineStartupTime;

	// Token: 0x04002087 RID: 8327
	private readonly Transform waterloggedPoint;

	// Token: 0x04002088 RID: 8328
	private readonly BaseEntity.Flags engineStartingFlag;

	// Token: 0x02000D33 RID: 3379
	public enum EngineState
	{
		// Token: 0x04004710 RID: 18192
		Off,
		// Token: 0x04004711 RID: 18193
		Starting,
		// Token: 0x04004712 RID: 18194
		On
	}
}
