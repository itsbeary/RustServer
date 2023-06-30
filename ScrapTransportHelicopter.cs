using System;
using Rust;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class ScrapTransportHelicopter : MiniCopter, TriggerHurtNotChild.IHurtTriggerUser
{
	// Token: 0x06000124 RID: 292 RVA: 0x00007CE6 File Offset: 0x00005EE6
	public override void ServerInit()
	{
		base.ServerInit();
		this.rigidBody.inertiaTensor = new Vector3(19082f, 14356f, 7940f);
	}

	// Token: 0x06000125 RID: 293 RVA: 0x00007D0D File Offset: 0x00005F0D
	public override void OnHealthChanged(float oldvalue, float newvalue)
	{
		if (!base.isServer)
		{
			return;
		}
		base.Invoke(new Action(this.DelayedNetworking), 0.15f);
	}

	// Token: 0x06000126 RID: 294 RVA: 0x00007D2F File Offset: 0x00005F2F
	public void DelayedNetworking()
	{
		base.SendNetworkUpdate(BasePlayer.NetworkQueue.Update);
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00007D38 File Offset: 0x00005F38
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00007D44 File Offset: 0x00005F44
	public override void OnFlagsChanged(BaseEntity.Flags old, BaseEntity.Flags next)
	{
		base.OnFlagsChanged(old, next);
		if (GameInfo.HasAchievements && base.isServer && !old.HasFlag(BaseEntity.Flags.On) && next.HasFlag(BaseEntity.Flags.On) && base.GetDriver() != null)
		{
			int num = 0;
			foreach (BaseEntity baseEntity in this.children)
			{
				if (baseEntity.ToPlayer() != null)
				{
					num++;
				}
				BaseVehicleSeat baseVehicleSeat;
				if ((baseVehicleSeat = baseEntity as BaseVehicleSeat) != null && baseVehicleSeat.GetMounted() != null && baseVehicleSeat.GetMounted() != base.GetDriver())
				{
					num++;
				}
			}
			if (num >= 5)
			{
				base.GetDriver().GiveAchievement("RUST_AIR");
			}
		}
	}

	// Token: 0x06000129 RID: 297 RVA: 0x000070B9 File Offset: 0x000052B9
	public override int StartingFuelUnits()
	{
		return 100;
	}

	// Token: 0x0600012A RID: 298 RVA: 0x00006CA5 File Offset: 0x00004EA5
	public float GetDamageMultiplier(BaseEntity ent)
	{
		return 1f;
	}

	// Token: 0x0600012B RID: 299 RVA: 0x000063A5 File Offset: 0x000045A5
	public void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal)
	{
	}

	// Token: 0x0600012C RID: 300 RVA: 0x00007A44 File Offset: 0x00005C44
	protected override bool CanPushNow(BasePlayer pusher)
	{
		return false;
	}

	// Token: 0x0400014E RID: 334
	public Transform searchlightEye;

	// Token: 0x0400014F RID: 335
	public BoxCollider parentTriggerCollider;

	// Token: 0x04000150 RID: 336
	[Header("Damage Effects")]
	public ParticleSystemContainer tailDamageLight;

	// Token: 0x04000151 RID: 337
	public ParticleSystemContainer tailDamageHeavy;

	// Token: 0x04000152 RID: 338
	public ParticleSystemContainer mainEngineDamageLight;

	// Token: 0x04000153 RID: 339
	public ParticleSystemContainer mainEngineDamageHeavy;

	// Token: 0x04000154 RID: 340
	public ParticleSystemContainer cockpitSparks;

	// Token: 0x04000155 RID: 341
	public Transform tailDamageLightEffects;

	// Token: 0x04000156 RID: 342
	public Transform mainEngineDamageLightEffects;

	// Token: 0x04000157 RID: 343
	public SoundDefinition damagedFireSoundDef;

	// Token: 0x04000158 RID: 344
	public SoundDefinition damagedFireTailSoundDef;

	// Token: 0x04000159 RID: 345
	public SoundDefinition damagedSparksSoundDef;

	// Token: 0x0400015A RID: 346
	private Sound damagedFireSound;

	// Token: 0x0400015B RID: 347
	private Sound damagedFireTailSound;

	// Token: 0x0400015C RID: 348
	private Sound damagedSparksSound;

	// Token: 0x0400015D RID: 349
	public float pilotRotorScale = 1.5f;

	// Token: 0x0400015E RID: 350
	public float compassOffset;

	// Token: 0x0400015F RID: 351
	[ServerVar(Help = "Population active on the server", ShowInAdminUI = true)]
	public new static float population;

	// Token: 0x04000160 RID: 352
	public const string PASSENGER_ACHIEVEMENT = "RUST_AIR";

	// Token: 0x04000161 RID: 353
	public const int PASSENGER_ACHIEVEMENT_REQ_COUNT = 5;
}
