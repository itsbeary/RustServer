using System;
using Rust;
using Rust.Modular;
using UnityEngine;

// Token: 0x020004A8 RID: 1192
public class VehicleModuleEngine : VehicleModuleStorage
{
	// Token: 0x17000338 RID: 824
	// (get) Token: 0x06002717 RID: 10007 RVA: 0x0000441C File Offset: 0x0000261C
	public override bool HasAnEngine
	{
		get
		{
			return true;
		}
	}

	// Token: 0x17000339 RID: 825
	// (get) Token: 0x06002718 RID: 10008 RVA: 0x000F4926 File Offset: 0x000F2B26
	// (set) Token: 0x06002719 RID: 10009 RVA: 0x000F492E File Offset: 0x000F2B2E
	public bool IsUsable { get; private set; }

	// Token: 0x1700033A RID: 826
	// (get) Token: 0x0600271A RID: 10010 RVA: 0x000F4937 File Offset: 0x000F2B37
	// (set) Token: 0x0600271B RID: 10011 RVA: 0x000F493F File Offset: 0x000F2B3F
	public float PerformanceFractionAcceleration { get; private set; }

	// Token: 0x1700033B RID: 827
	// (get) Token: 0x0600271C RID: 10012 RVA: 0x000F4948 File Offset: 0x000F2B48
	// (set) Token: 0x0600271D RID: 10013 RVA: 0x000F4950 File Offset: 0x000F2B50
	public float PerformanceFractionTopSpeed { get; private set; }

	// Token: 0x1700033C RID: 828
	// (get) Token: 0x0600271E RID: 10014 RVA: 0x000F4959 File Offset: 0x000F2B59
	// (set) Token: 0x0600271F RID: 10015 RVA: 0x000F4961 File Offset: 0x000F2B61
	public float PerformanceFractionFuelEconomy { get; private set; }

	// Token: 0x1700033D RID: 829
	// (get) Token: 0x06002720 RID: 10016 RVA: 0x000F496A File Offset: 0x000F2B6A
	// (set) Token: 0x06002721 RID: 10017 RVA: 0x000F4972 File Offset: 0x000F2B72
	public float OverallPerformanceFraction { get; private set; }

	// Token: 0x1700033E RID: 830
	// (get) Token: 0x06002722 RID: 10018 RVA: 0x000F497B File Offset: 0x000F2B7B
	public bool AtLowPerformance
	{
		get
		{
			return this.OverallPerformanceFraction <= 0.5f;
		}
	}

	// Token: 0x1700033F RID: 831
	// (get) Token: 0x06002723 RID: 10019 RVA: 0x000F498D File Offset: 0x000F2B8D
	public bool AtPeakPerformance
	{
		get
		{
			return Mathf.Approximately(this.OverallPerformanceFraction, 1f);
		}
	}

	// Token: 0x17000340 RID: 832
	// (get) Token: 0x06002724 RID: 10020 RVA: 0x000F499F File Offset: 0x000F2B9F
	public int KW
	{
		get
		{
			return this.engine.engineKW;
		}
	}

	// Token: 0x17000341 RID: 833
	// (get) Token: 0x06002725 RID: 10021 RVA: 0x000F49AC File Offset: 0x000F2BAC
	public EngineAudioSet AudioSet
	{
		get
		{
			return this.engine.audioSet;
		}
	}

	// Token: 0x17000342 RID: 834
	// (get) Token: 0x06002726 RID: 10022 RVA: 0x000F49B9 File Offset: 0x000F2BB9
	private bool EngineIsOn
	{
		get
		{
			return base.Car != null && base.Car.CurEngineState == VehicleEngineController<GroundVehicle>.EngineState.On;
		}
	}

	// Token: 0x06002727 RID: 10023 RVA: 0x000F49D9 File Offset: 0x000F2BD9
	public override void InitShared()
	{
		base.InitShared();
		this.RefreshPerformanceStats(base.GetContainer() as EngineStorage);
	}

	// Token: 0x06002728 RID: 10024 RVA: 0x000F49F2 File Offset: 0x000F2BF2
	public override void OnEngineStateChanged(VehicleEngineController<GroundVehicle>.EngineState oldState, VehicleEngineController<GroundVehicle>.EngineState newState)
	{
		base.OnEngineStateChanged(oldState, newState);
		this.RefreshPerformanceStats(base.GetContainer() as EngineStorage);
	}

	// Token: 0x06002729 RID: 10025 RVA: 0x000F4A0D File Offset: 0x000F2C0D
	public override float GetMaxDriveForce()
	{
		if (!this.IsUsable)
		{
			return 0f;
		}
		return (float)this.engine.engineKW * 12.75f * this.PerformanceFractionTopSpeed;
	}

	// Token: 0x0600272A RID: 10026 RVA: 0x000F4A38 File Offset: 0x000F2C38
	public void RefreshPerformanceStats(EngineStorage engineStorage)
	{
		if (engineStorage == null)
		{
			this.IsUsable = false;
			this.PerformanceFractionAcceleration = 0f;
			this.PerformanceFractionTopSpeed = 0f;
			this.PerformanceFractionFuelEconomy = 0f;
		}
		else
		{
			this.IsUsable = engineStorage.isUsable;
			this.PerformanceFractionAcceleration = this.GetPerformanceFraction(engineStorage.accelerationBoostPercent);
			this.PerformanceFractionTopSpeed = this.GetPerformanceFraction(engineStorage.topSpeedBoostPercent);
			this.PerformanceFractionFuelEconomy = this.GetPerformanceFraction(engineStorage.fuelEconomyBoostPercent);
		}
		this.OverallPerformanceFraction = (this.PerformanceFractionAcceleration + this.PerformanceFractionTopSpeed + this.PerformanceFractionFuelEconomy) / 3f;
	}

	// Token: 0x0600272B RID: 10027 RVA: 0x000F4ADC File Offset: 0x000F2CDC
	private float GetPerformanceFraction(float statBoostPercent)
	{
		if (!this.IsUsable)
		{
			return 0f;
		}
		float num = Mathf.Lerp(0f, 0.25f, base.healthFraction);
		float num2;
		if (base.healthFraction == 0f)
		{
			num2 = 0f;
		}
		else
		{
			num2 = statBoostPercent * 0.75f;
		}
		return num + num2;
	}

	// Token: 0x0600272C RID: 10028 RVA: 0x000F4B2B File Offset: 0x000F2D2B
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		this.RefreshPerformanceStats(base.GetContainer() as EngineStorage);
	}

	// Token: 0x0600272D RID: 10029 RVA: 0x000F4B45 File Offset: 0x000F2D45
	public override bool CanBeLooted(BasePlayer player)
	{
		return base.CanBeLooted(player);
	}

	// Token: 0x0600272E RID: 10030 RVA: 0x000F4B54 File Offset: 0x000F2D54
	public override void VehicleFixedUpdate()
	{
		if (!this.isSpawned || !base.IsOnAVehicle)
		{
			return;
		}
		base.VehicleFixedUpdate();
		if (!base.Vehicle.IsMovingOrOn || base.Car == null)
		{
			return;
		}
		if (base.Car.CurEngineState == VehicleEngineController<GroundVehicle>.EngineState.On && this.IsUsable)
		{
			float num = Mathf.Lerp(this.engine.idleFuelPerSec, this.engine.maxFuelPerSec, Mathf.Abs(base.Car.GetThrottleInput()));
			num /= this.PerformanceFractionFuelEconomy;
			base.Car.TickFuel(num);
		}
	}

	// Token: 0x0600272F RID: 10031 RVA: 0x000F4BEC File Offset: 0x000F2DEC
	public override float GetAdjustedDriveForce(float absSpeed, float topSpeed)
	{
		float maxDriveForce = this.GetMaxDriveForce();
		float num = Mathf.Lerp(0.0002f, 0.7f, this.PerformanceFractionAcceleration);
		float num2 = MathEx.BiasedLerp(1f - absSpeed / topSpeed, num);
		return maxDriveForce * num2;
	}

	// Token: 0x06002730 RID: 10032 RVA: 0x000F4C28 File Offset: 0x000F2E28
	public override void Hurt(HitInfo info)
	{
		base.Hurt(info);
		if (info.damageTypes.GetMajorityDamageType() == DamageType.Decay)
		{
			return;
		}
		float num = info.damageTypes.Total();
		EngineStorage engineStorage = base.GetContainer() as EngineStorage;
		if (engineStorage != null && num > 0f)
		{
			engineStorage.OnModuleDamaged(num);
		}
	}

	// Token: 0x06002731 RID: 10033 RVA: 0x000F4C7C File Offset: 0x000F2E7C
	public override void OnHealthChanged(float oldValue, float newValue)
	{
		base.OnHealthChanged(oldValue, newValue);
		if (!base.isServer)
		{
			return;
		}
		this.RefreshPerformanceStats(base.GetContainer() as EngineStorage);
	}

	// Token: 0x06002732 RID: 10034 RVA: 0x000F4CA0 File Offset: 0x000F2EA0
	public override bool AdminFixUp(int tier)
	{
		if (!base.AdminFixUp(tier))
		{
			return false;
		}
		EngineStorage engineStorage = base.GetContainer() as EngineStorage;
		engineStorage.AdminAddParts(tier);
		this.RefreshPerformanceStats(engineStorage);
		return true;
	}

	// Token: 0x04001F71 RID: 8049
	[SerializeField]
	private VehicleModuleEngine.Engine engine;

	// Token: 0x04001F77 RID: 8055
	private const float FORCE_MULTIPLIER = 12.75f;

	// Token: 0x04001F78 RID: 8056
	private const float HEALTH_PERFORMANCE_FRACTION = 0.25f;

	// Token: 0x04001F79 RID: 8057
	private const float LOW_PERFORMANCE_THRESHOLD = 0.5f;

	// Token: 0x04001F7A RID: 8058
	private Sound badPerformanceLoop;

	// Token: 0x04001F7B RID: 8059
	private SoundModulation.Modulator badPerformancePitchModulator;

	// Token: 0x04001F7C RID: 8060
	private float prevSmokePercent;

	// Token: 0x04001F7D RID: 8061
	private const float MIN_FORCE_BIAS = 0.0002f;

	// Token: 0x04001F7E RID: 8062
	private const float MAX_FORCE_BIAS = 0.7f;

	// Token: 0x02000D21 RID: 3361
	[Serializable]
	public class Engine
	{
		// Token: 0x040046D4 RID: 18132
		[Header("Engine Stats")]
		public int engineKW;

		// Token: 0x040046D5 RID: 18133
		public float idleFuelPerSec = 0.25f;

		// Token: 0x040046D6 RID: 18134
		public float maxFuelPerSec = 0.25f;

		// Token: 0x040046D7 RID: 18135
		[Header("Engine Audio")]
		public EngineAudioSet audioSet;

		// Token: 0x040046D8 RID: 18136
		[Header("Engine FX")]
		public ParticleSystemContainer[] engineParticles;

		// Token: 0x040046D9 RID: 18137
		public ParticleSystem[] exhaustSmoke;

		// Token: 0x040046DA RID: 18138
		public ParticleSystem[] exhaustBackfire;

		// Token: 0x040046DB RID: 18139
		public float exhaustSmokeMinOpacity = 0.1f;

		// Token: 0x040046DC RID: 18140
		public float exhaustSmokeMaxOpacity = 0.7f;

		// Token: 0x040046DD RID: 18141
		public float exhaustSmokeChangeRate = 0.5f;
	}
}
