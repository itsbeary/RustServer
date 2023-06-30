using System;
using ConVar;
using Facepunch;
using Network;
using ProtoBuf;
using Rust;
using UnityEngine;

// Token: 0x020000B1 RID: 177
public class PlayerMetabolism : BaseMetabolism<global::BasePlayer>
{
	// Token: 0x06001026 RID: 4134 RVA: 0x00086864 File Offset: 0x00084A64
	public override bool OnRpcMessage(global::BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("PlayerMetabolism.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x000868A4 File Offset: 0x00084AA4
	public override void Reset()
	{
		base.Reset();
		this.poison.Reset();
		this.radiation_level.Reset();
		this.radiation_poison.Reset();
		this.temperature.Reset();
		this.oxygen.Reset();
		this.bleeding.Reset();
		this.wetness.Reset();
		this.dirtyness.Reset();
		this.comfort.Reset();
		this.pending_health.Reset();
		this.lastConsumeTime = float.NegativeInfinity;
		this.isDirty = true;
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x00086937 File Offset: 0x00084B37
	public override void ServerUpdate(BaseCombatEntity ownerEntity, float delta)
	{
		base.ServerUpdate(ownerEntity, delta);
		this.SendChangesToClient();
	}

	// Token: 0x06001029 RID: 4137 RVA: 0x00086948 File Offset: 0x00084B48
	internal bool HasChanged()
	{
		bool flag = this.isDirty;
		flag = this.calories.HasChanged() || flag;
		flag = this.hydration.HasChanged() || flag;
		flag = this.heartrate.HasChanged() || flag;
		flag = this.poison.HasChanged() || flag;
		flag = this.radiation_level.HasChanged() || flag;
		flag = this.radiation_poison.HasChanged() || flag;
		flag = this.temperature.HasChanged() || flag;
		flag = this.wetness.HasChanged() || flag;
		flag = this.dirtyness.HasChanged() || flag;
		flag = this.comfort.HasChanged() || flag;
		return this.pending_health.HasChanged() || flag;
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x000869F8 File Offset: 0x00084BF8
	protected override void DoMetabolismDamage(BaseCombatEntity ownerEntity, float delta)
	{
		base.DoMetabolismDamage(ownerEntity, delta);
		if (this.temperature.value < -20f)
		{
			this.owner.Hurt(Mathf.InverseLerp(1f, -50f, this.temperature.value) * delta * 1f, DamageType.Cold, null, true);
		}
		else if (this.temperature.value < -10f)
		{
			this.owner.Hurt(Mathf.InverseLerp(1f, -50f, this.temperature.value) * delta * 0.3f, DamageType.Cold, null, true);
		}
		else if (this.temperature.value < 1f)
		{
			this.owner.Hurt(Mathf.InverseLerp(1f, -50f, this.temperature.value) * delta * 0.1f, DamageType.Cold, null, true);
		}
		if (this.temperature.value > 60f)
		{
			this.owner.Hurt(Mathf.InverseLerp(60f, 200f, this.temperature.value) * delta * 5f, DamageType.Heat, null, true);
		}
		if (this.oxygen.value < 0.5f)
		{
			this.owner.Hurt(Mathf.InverseLerp(0.5f, 0f, this.oxygen.value) * delta * 20f, DamageType.Drowned, null, false);
		}
		if (this.bleeding.value > 0f)
		{
			float num = delta * 0.33333334f;
			this.owner.Hurt(num, DamageType.Bleeding, null, true);
			this.bleeding.Subtract(num);
		}
		if (this.poison.value > 0f)
		{
			this.owner.Hurt(this.poison.value * delta * 0.1f, DamageType.Poison, null, true);
		}
		if (ConVar.Server.radiation && this.radiation_poison.value > 0f)
		{
			float num2 = (1f + Mathf.Clamp01(this.radiation_poison.value / 25f) * 5f) * (delta / 5f);
			this.owner.Hurt(num2, DamageType.Radiation, null, true);
			this.radiation_poison.Subtract(num2);
		}
	}

	// Token: 0x0600102B RID: 4139 RVA: 0x00086C26 File Offset: 0x00084E26
	public bool SignificantBleeding()
	{
		return this.bleeding.value > 0f;
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x00086C3C File Offset: 0x00084E3C
	protected override void RunMetabolism(BaseCombatEntity ownerEntity, float delta)
	{
		BaseGameMode activeGameMode = BaseGameMode.GetActiveGameMode(true);
		float currentTemperature = this.owner.currentTemperature;
		float num = this.owner.currentComfort;
		float currentCraftLevel = this.owner.currentCraftLevel;
		this.owner.SetPlayerFlag(global::BasePlayer.PlayerFlags.Workbench1, currentCraftLevel == 1f);
		this.owner.SetPlayerFlag(global::BasePlayer.PlayerFlags.Workbench2, currentCraftLevel == 2f);
		this.owner.SetPlayerFlag(global::BasePlayer.PlayerFlags.Workbench3, currentCraftLevel == 3f);
		this.owner.SetPlayerFlag(global::BasePlayer.PlayerFlags.SafeZone, this.owner.InSafeZone());
		if (activeGameMode == null || activeGameMode.allowTemperature)
		{
			float num2 = currentTemperature;
			num2 -= this.DeltaWet() * 34f;
			float num3 = Mathf.Clamp(this.owner.baseProtection.amounts[18] * 1.5f, -1f, 1f);
			float num4 = Mathf.InverseLerp(20f, -50f, currentTemperature);
			float num5 = Mathf.InverseLerp(20f, 30f, currentTemperature);
			num2 += num4 * 70f * num3;
			num2 += num5 * 10f * Mathf.Abs(num3);
			num2 += this.heartrate.value * 5f;
			this.temperature.MoveTowards(num2, delta * 5f);
		}
		else
		{
			this.temperature.value = 25f;
		}
		if (this.temperature.value >= 40f)
		{
			num = 0f;
		}
		this.comfort.MoveTowards(num, delta / 5f);
		float num6 = 0.6f + 0.4f * this.comfort.value;
		if (this.calories.value > 100f && this.owner.healthFraction < num6 && this.radiation_poison.Fraction() < 0.25f && this.owner.SecondsSinceAttacked > 10f && !this.SignificantBleeding() && this.temperature.value >= 10f && this.hydration.value > 40f)
		{
			float num7 = Mathf.InverseLerp(this.calories.min, this.calories.max, this.calories.value);
			float num8 = 5f;
			float num9 = num8 * this.owner.MaxHealth() * 0.8f / 600f;
			num9 += num9 * num7 * 0.5f;
			float num10 = num9 / num8;
			num10 += num10 * this.comfort.value * 6f;
			ownerEntity.Heal(num10 * delta);
			this.calories.Subtract(num9 * delta);
			this.hydration.Subtract(num9 * delta * 0.2f);
		}
		float num11 = this.owner.estimatedSpeed2D / this.owner.GetMaxSpeed() * 0.75f;
		float num12 = Mathf.Clamp(0.05f + num11, 0f, 1f);
		this.heartrate.MoveTowards(num12, delta * 0.1f);
		if (!this.owner.IsGod())
		{
			float num13 = this.heartrate.Fraction() * 0.375f;
			this.calories.MoveTowards(0f, delta * num13);
			float num14 = 0.008333334f;
			num14 += Mathf.InverseLerp(40f, 60f, this.temperature.value) * 0.083333336f;
			num14 += this.heartrate.value * 0.06666667f;
			this.hydration.MoveTowards(0f, delta * num14);
		}
		bool flag = this.hydration.Fraction() <= 0f || this.radiation_poison.value >= 100f;
		this.owner.SetPlayerFlag(global::BasePlayer.PlayerFlags.NoSprint, flag);
		if (this.temperature.value > 40f)
		{
			this.hydration.Add(Mathf.InverseLerp(40f, 200f, this.temperature.value) * delta * -1f);
		}
		if (this.temperature.value < 10f)
		{
			float num15 = Mathf.InverseLerp(20f, -100f, this.temperature.value);
			this.heartrate.MoveTowards(Mathf.Lerp(0.2f, 1f, num15), delta * 2f * num15);
		}
		float num16 = this.owner.AirFactor();
		float num17 = ((num16 > this.oxygen.value) ? 1f : 0.1f);
		this.oxygen.MoveTowards(num16, delta * num17);
		float num18 = 0f;
		float num19 = 0f;
		if (this.owner.IsOutside(this.owner.eyes.position))
		{
			num18 = Climate.GetRain(this.owner.eyes.position) * Weather.wetness_rain;
			num19 = Climate.GetSnow(this.owner.eyes.position) * Weather.wetness_snow;
		}
		bool flag2 = this.owner.baseProtection.amounts[4] > 0f;
		float num20 = this.owner.currentEnvironmentalWetness;
		num20 = Mathf.Clamp(num20, 0f, 0.8f);
		float num21 = this.owner.WaterFactor();
		if (!flag2 && num21 > 0f)
		{
			this.wetness.value = Mathf.Max(this.wetness.value, Mathf.Clamp(num21, this.wetness.min, this.wetness.max));
		}
		float num22 = Mathx.Max(this.wetness.value, num18, num19, num20);
		num22 = Mathf.Min(num22, flag2 ? 0f : num22);
		this.wetness.MoveTowards(num22, delta * 0.05f);
		if (num21 < this.wetness.value && num20 <= 0f)
		{
			this.wetness.MoveTowards(0f, delta * 0.2f * Mathf.InverseLerp(0f, 100f, currentTemperature));
		}
		this.poison.MoveTowards(0f, delta * 0.5555556f);
		if (this.wetness.Fraction() > 0.4f && this.owner.estimatedSpeed > 0.25f && this.radiation_level.Fraction() == 0f)
		{
			this.radiation_poison.Subtract(this.radiation_poison.value * 0.2f * this.wetness.Fraction() * delta * 0.2f);
		}
		if (ConVar.Server.radiation && !this.owner.IsGod())
		{
			this.radiation_level.value = this.owner.radiationLevel;
			if (this.radiation_level.value > 0f)
			{
				this.radiation_poison.Add(this.radiation_level.value * delta);
			}
		}
		if (this.pending_health.value > 0f)
		{
			float num23 = Mathf.Min(1f * delta, this.pending_health.value);
			ownerEntity.Heal(num23);
			if (ownerEntity.healthFraction == 1f)
			{
				this.pending_health.value = 0f;
				return;
			}
			this.pending_health.Subtract(num23);
		}
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x0008739C File Offset: 0x0008559C
	private float DeltaHot()
	{
		return Mathf.InverseLerp(20f, 100f, this.temperature.value);
	}

	// Token: 0x0600102E RID: 4142 RVA: 0x000873B8 File Offset: 0x000855B8
	private float DeltaCold()
	{
		return Mathf.InverseLerp(20f, -50f, this.temperature.value);
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x000873D4 File Offset: 0x000855D4
	private float DeltaWet()
	{
		return this.wetness.value;
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x000873E1 File Offset: 0x000855E1
	public void UseHeart(float frate)
	{
		if (this.heartrate.value > frate)
		{
			this.heartrate.Add(frate);
			return;
		}
		this.heartrate.value = frate;
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x0008740C File Offset: 0x0008560C
	public void SendChangesToClient()
	{
		if (!this.HasChanged())
		{
			return;
		}
		this.isDirty = false;
		using (ProtoBuf.PlayerMetabolism playerMetabolism = this.Save())
		{
			base.baseEntity.ClientRPCPlayerAndSpectators<ProtoBuf.PlayerMetabolism>(null, base.baseEntity, "UpdateMetabolism", playerMetabolism);
		}
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x00087464 File Offset: 0x00085664
	public bool CanConsume()
	{
		return (!this.owner || !this.owner.IsHeadUnderwater()) && UnityEngine.Time.time - this.lastConsumeTime > 1f;
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x00087495 File Offset: 0x00085695
	public void MarkConsumption()
	{
		this.lastConsumeTime = UnityEngine.Time.time;
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x000874A4 File Offset: 0x000856A4
	public ProtoBuf.PlayerMetabolism Save()
	{
		ProtoBuf.PlayerMetabolism playerMetabolism = Facepunch.Pool.Get<ProtoBuf.PlayerMetabolism>();
		playerMetabolism.calories = this.calories.value;
		playerMetabolism.hydration = this.hydration.value;
		playerMetabolism.heartrate = this.heartrate.value;
		playerMetabolism.temperature = this.temperature.value;
		playerMetabolism.radiation_level = this.radiation_level.value;
		playerMetabolism.radiation_poisoning = this.radiation_poison.value;
		playerMetabolism.wetness = this.wetness.value;
		playerMetabolism.dirtyness = this.dirtyness.value;
		playerMetabolism.oxygen = this.oxygen.value;
		playerMetabolism.bleeding = this.bleeding.value;
		playerMetabolism.comfort = this.comfort.value;
		playerMetabolism.pending_health = this.pending_health.value;
		if (this.owner)
		{
			playerMetabolism.health = this.owner.Health();
		}
		return playerMetabolism;
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x000875A4 File Offset: 0x000857A4
	public void Load(ProtoBuf.PlayerMetabolism s)
	{
		this.calories.SetValue(s.calories);
		this.hydration.SetValue(s.hydration);
		this.comfort.SetValue(s.comfort);
		this.heartrate.value = s.heartrate;
		this.temperature.value = s.temperature;
		this.radiation_level.value = s.radiation_level;
		this.radiation_poison.value = s.radiation_poisoning;
		this.wetness.value = s.wetness;
		this.dirtyness.value = s.dirtyness;
		this.oxygen.value = s.oxygen;
		this.bleeding.value = s.bleeding;
		this.pending_health.value = s.pending_health;
		if (this.owner)
		{
			this.owner.health = s.health;
		}
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x0008769C File Offset: 0x0008589C
	public override MetabolismAttribute FindAttribute(MetabolismAttribute.Type type)
	{
		switch (type)
		{
		case MetabolismAttribute.Type.Poison:
			return this.poison;
		case MetabolismAttribute.Type.Radiation:
			return this.radiation_poison;
		case MetabolismAttribute.Type.Bleeding:
			return this.bleeding;
		case MetabolismAttribute.Type.HealthOverTime:
			return this.pending_health;
		}
		return base.FindAttribute(type);
	}

	// Token: 0x04000A59 RID: 2649
	public const float HotThreshold = 40f;

	// Token: 0x04000A5A RID: 2650
	public const float ColdThreshold = 5f;

	// Token: 0x04000A5B RID: 2651
	public const float OxygenHurtThreshold = 0.5f;

	// Token: 0x04000A5C RID: 2652
	public const float OxygenDepleteTime = 10f;

	// Token: 0x04000A5D RID: 2653
	public const float OxygenRefillTime = 1f;

	// Token: 0x04000A5E RID: 2654
	public MetabolismAttribute temperature = new MetabolismAttribute();

	// Token: 0x04000A5F RID: 2655
	public MetabolismAttribute poison = new MetabolismAttribute();

	// Token: 0x04000A60 RID: 2656
	public MetabolismAttribute radiation_level = new MetabolismAttribute();

	// Token: 0x04000A61 RID: 2657
	public MetabolismAttribute radiation_poison = new MetabolismAttribute();

	// Token: 0x04000A62 RID: 2658
	public MetabolismAttribute wetness = new MetabolismAttribute();

	// Token: 0x04000A63 RID: 2659
	public MetabolismAttribute dirtyness = new MetabolismAttribute();

	// Token: 0x04000A64 RID: 2660
	public MetabolismAttribute oxygen = new MetabolismAttribute();

	// Token: 0x04000A65 RID: 2661
	public MetabolismAttribute bleeding = new MetabolismAttribute();

	// Token: 0x04000A66 RID: 2662
	public MetabolismAttribute comfort = new MetabolismAttribute();

	// Token: 0x04000A67 RID: 2663
	public MetabolismAttribute pending_health = new MetabolismAttribute();

	// Token: 0x04000A68 RID: 2664
	public bool isDirty;

	// Token: 0x04000A69 RID: 2665
	private float lastConsumeTime;
}
