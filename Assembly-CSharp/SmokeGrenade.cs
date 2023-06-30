using System;
using System.Collections.Generic;
using Rust.Ai;
using UnityEngine;

// Token: 0x02000427 RID: 1063
public class SmokeGrenade : TimedExplosive
{
	// Token: 0x0600241A RID: 9242 RVA: 0x000E6A47 File Offset: 0x000E4C47
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.CheckForWater), 1f, 1f);
	}

	// Token: 0x0600241B RID: 9243 RVA: 0x000E6A6C File Offset: 0x000E4C6C
	public override void Explode()
	{
		if (this.WaterFactor() >= 0.5f)
		{
			this.FinishUp();
			return;
		}
		if (base.IsOn())
		{
			return;
		}
		base.Invoke(new Action(this.FinishUp), this.smokeDuration);
		base.SetFlag(BaseEntity.Flags.On, true, false, true);
		base.SetFlag(BaseEntity.Flags.Open, true, false, true);
		base.InvalidateNetworkCache();
		base.SendNetworkUpdateImmediate(false);
		SmokeGrenade.activeGrenades.Add(this);
		if (this.creatorEntity)
		{
			Sense.Stimulate(new Sensation
			{
				Type = SensationType.Explosion,
				Position = this.creatorEntity.transform.position,
				Radius = this.explosionRadius * 17f,
				DamagePotential = 0f,
				InitiatorPlayer = (this.creatorEntity as BasePlayer),
				Initiator = this.creatorEntity
			});
		}
	}

	// Token: 0x0600241C RID: 9244 RVA: 0x000E6B52 File Offset: 0x000E4D52
	public void CheckForWater()
	{
		if (this.WaterFactor() >= 0.5f)
		{
			this.FinishUp();
		}
	}

	// Token: 0x0600241D RID: 9245 RVA: 0x000E6B67 File Offset: 0x000E4D67
	public void FinishUp()
	{
		if (this.killing)
		{
			return;
		}
		base.Kill(BaseNetworkable.DestroyMode.None);
		this.killing = true;
	}

	// Token: 0x0600241E RID: 9246 RVA: 0x000E6B80 File Offset: 0x000E4D80
	public override void DestroyShared()
	{
		SmokeGrenade.activeGrenades.Remove(this);
		base.DestroyShared();
	}

	// Token: 0x04001C1D RID: 7197
	public float smokeDuration = 45f;

	// Token: 0x04001C1E RID: 7198
	public GameObjectRef smokeEffectPrefab;

	// Token: 0x04001C1F RID: 7199
	public GameObjectRef igniteSound;

	// Token: 0x04001C20 RID: 7200
	public SoundPlayer soundLoop;

	// Token: 0x04001C21 RID: 7201
	private GameObject smokeEffectInstance;

	// Token: 0x04001C22 RID: 7202
	public static List<SmokeGrenade> activeGrenades = new List<SmokeGrenade>();

	// Token: 0x04001C23 RID: 7203
	public float fieldMin = 5f;

	// Token: 0x04001C24 RID: 7204
	public float fieldMax = 8f;

	// Token: 0x04001C25 RID: 7205
	protected bool killing;
}
