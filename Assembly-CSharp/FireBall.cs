using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x020003F3 RID: 1011
public class FireBall : BaseEntity, ISplashable
{
	// Token: 0x060022C8 RID: 8904 RVA: 0x000DFB2D File Offset: 0x000DDD2D
	public void SetDelayedVelocity(Vector3 delayed)
	{
		if (this.delayedVelocity != Vector3.zero)
		{
			return;
		}
		this.delayedVelocity = delayed;
		base.Invoke(new Action(this.ApplyDelayedVelocity), 0.1f);
	}

	// Token: 0x060022C9 RID: 8905 RVA: 0x000DFB60 File Offset: 0x000DDD60
	private void ApplyDelayedVelocity()
	{
		this.SetVelocity(this.delayedVelocity);
		this.delayedVelocity = Vector3.zero;
	}

	// Token: 0x060022CA RID: 8906 RVA: 0x000DFB7C File Offset: 0x000DDD7C
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRepeating(new Action(this.Think), UnityEngine.Random.Range(0f, 1f), this.tickRate);
		float num = UnityEngine.Random.Range(this.lifeTimeMin, this.lifeTimeMax);
		float num2 = num * UnityEngine.Random.Range(0.9f, 1.1f);
		base.Invoke(new Action(this.Extinguish), num2);
		base.Invoke(new Action(this.TryToSpread), num * UnityEngine.Random.Range(0.3f, 0.5f));
		this.deathTime = Time.realtimeSinceStartup + num2;
		this.spawnTime = Time.realtimeSinceStartup;
	}

	// Token: 0x060022CB RID: 8907 RVA: 0x000DFC28 File Offset: 0x000DDE28
	public float GetDeathTime()
	{
		return this.deathTime;
	}

	// Token: 0x060022CC RID: 8908 RVA: 0x000DFC30 File Offset: 0x000DDE30
	public void AddLife(float amountToAdd)
	{
		float num = Mathf.Clamp(this.GetDeathTime() + amountToAdd, 0f, this.MaxLifeTime());
		base.Invoke(new Action(this.Extinguish), num);
		this.deathTime = num;
	}

	// Token: 0x060022CD RID: 8909 RVA: 0x000DFC70 File Offset: 0x000DDE70
	public float MaxLifeTime()
	{
		return this.lifeTimeMax * 2.5f;
	}

	// Token: 0x060022CE RID: 8910 RVA: 0x000DFC80 File Offset: 0x000DDE80
	public float TimeLeft()
	{
		float num = this.deathTime - Time.realtimeSinceStartup;
		if (num < 0f)
		{
			num = 0f;
		}
		return num;
	}

	// Token: 0x060022CF RID: 8911 RVA: 0x000DFCAC File Offset: 0x000DDEAC
	public void TryToSpread()
	{
		float num = 0.9f - this.generation * 0.1f;
		if (UnityEngine.Random.Range(0f, 1f) >= num)
		{
			return;
		}
		if (this.spreadSubEntity.isValid)
		{
			BaseEntity baseEntity = GameManager.server.CreateEntity(this.spreadSubEntity.resourcePath, default(Vector3), default(Quaternion), true);
			if (baseEntity)
			{
				baseEntity.transform.position = base.transform.position + Vector3.up * 0.25f;
				baseEntity.Spawn();
				Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection(45f, Vector3.up, true);
				baseEntity.creatorEntity = ((this.creatorEntity == null) ? baseEntity : this.creatorEntity);
				baseEntity.SetVelocity(modifiedAimConeDirection * UnityEngine.Random.Range(5f, 8f));
				baseEntity.SendMessage("SetGeneration", this.generation + 1f);
			}
		}
	}

	// Token: 0x060022D0 RID: 8912 RVA: 0x000DFDBC File Offset: 0x000DDFBC
	public void SetGeneration(int gen)
	{
		this.generation = (float)gen;
	}

	// Token: 0x060022D1 RID: 8913 RVA: 0x000DFDC8 File Offset: 0x000DDFC8
	public void Think()
	{
		if (!base.isServer)
		{
			return;
		}
		this.SetResting(Vector3.Distance(this.lastPos, base.transform.localPosition) < 0.25f);
		this.lastPos = base.transform.localPosition;
		if (this.IsResting())
		{
			this.DoRadialDamage();
		}
		if (this.WaterFactor() > 0.5f)
		{
			this.Extinguish();
		}
		if (this.wetness > this.waterToExtinguish)
		{
			this.Extinguish();
		}
	}

	// Token: 0x060022D2 RID: 8914 RVA: 0x000DFE48 File Offset: 0x000DE048
	public void DoRadialDamage()
	{
		List<Collider> list = Pool.GetList<Collider>();
		Vector3 vector = base.transform.position + new Vector3(0f, this.radius * 0.75f, 0f);
		Vis.Colliders<Collider>(vector, this.radius, list, this.AttackLayers, QueryTriggerInteraction.Collide);
		HitInfo hitInfo = new HitInfo();
		hitInfo.DoHitEffects = true;
		hitInfo.DidHit = true;
		hitInfo.HitBone = 0U;
		hitInfo.Initiator = ((this.creatorEntity == null) ? base.gameObject.ToBaseEntity() : this.creatorEntity);
		hitInfo.PointStart = base.transform.position;
		foreach (Collider collider in list)
		{
			if (!collider.isTrigger || (collider.gameObject.layer != 29 && collider.gameObject.layer != 18))
			{
				BaseCombatEntity baseCombatEntity = collider.gameObject.ToBaseEntity() as BaseCombatEntity;
				if (!(baseCombatEntity == null) && baseCombatEntity.isServer && baseCombatEntity.IsAlive() && (!this.ignoreNPC || !baseCombatEntity.IsNpc) && baseCombatEntity.IsVisible(vector, float.PositiveInfinity))
				{
					if (baseCombatEntity is BasePlayer)
					{
						Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", baseCombatEntity, 0U, new Vector3(0f, 1f, 0f), Vector3.up, null, false);
					}
					hitInfo.PointEnd = baseCombatEntity.transform.position;
					hitInfo.HitPositionWorld = baseCombatEntity.transform.position;
					hitInfo.damageTypes.Set(DamageType.Heat, this.damagePerSecond * this.tickRate);
					baseCombatEntity.OnAttacked(hitInfo);
				}
			}
		}
		Pool.FreeList<Collider>(ref list);
	}

	// Token: 0x060022D3 RID: 8915 RVA: 0x000E0048 File Offset: 0x000DE248
	public bool CanMerge()
	{
		return this.canMerge && this.TimeLeft() < this.MaxLifeTime() * 0.8f;
	}

	// Token: 0x060022D4 RID: 8916 RVA: 0x000E0068 File Offset: 0x000DE268
	public float TimeAlive()
	{
		return Time.realtimeSinceStartup - this.spawnTime;
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x000E0078 File Offset: 0x000DE278
	public void SetResting(bool isResting)
	{
		if (isResting != this.IsResting() && isResting && this.TimeAlive() > 1f && this.CanMerge())
		{
			List<Collider> list = Pool.GetList<Collider>();
			Vis.Colliders<Collider>(base.transform.position, 0.5f, list, 512, QueryTriggerInteraction.Collide);
			foreach (Collider collider in list)
			{
				BaseEntity baseEntity = collider.gameObject.ToBaseEntity();
				if (baseEntity)
				{
					FireBall fireBall = baseEntity.ToServer<FireBall>();
					if (fireBall && fireBall.CanMerge() && fireBall != this)
					{
						fireBall.Invoke(new Action(this.Extinguish), 1f);
						fireBall.canMerge = false;
						this.AddLife(fireBall.TimeLeft() * 0.25f);
					}
				}
			}
			Pool.FreeList<Collider>(ref list);
		}
		base.SetFlag(BaseEntity.Flags.OnFire, isResting, false, true);
	}

	// Token: 0x060022D6 RID: 8918 RVA: 0x000E0184 File Offset: 0x000DE384
	public void Extinguish()
	{
		base.CancelInvoke(new Action(this.Extinguish));
		if (!base.IsDestroyed)
		{
			base.Kill(BaseNetworkable.DestroyMode.None);
		}
	}

	// Token: 0x060022D7 RID: 8919 RVA: 0x000E01A7 File Offset: 0x000DE3A7
	public bool WantsSplash(ItemDefinition splashType, int amount)
	{
		return !base.IsDestroyed;
	}

	// Token: 0x060022D8 RID: 8920 RVA: 0x000E01B2 File Offset: 0x000DE3B2
	public int DoSplash(ItemDefinition splashType, int amount)
	{
		this.wetness += amount;
		return amount;
	}

	// Token: 0x060022D9 RID: 8921 RVA: 0x0000326F File Offset: 0x0000146F
	public bool IsResting()
	{
		return base.HasFlag(BaseEntity.Flags.OnFire);
	}

	// Token: 0x060022DA RID: 8922 RVA: 0x000E01C3 File Offset: 0x000DE3C3
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
	}

	// Token: 0x04001AA6 RID: 6822
	public float lifeTimeMin = 20f;

	// Token: 0x04001AA7 RID: 6823
	public float lifeTimeMax = 40f;

	// Token: 0x04001AA8 RID: 6824
	public ParticleSystem[] movementSystems;

	// Token: 0x04001AA9 RID: 6825
	public ParticleSystem[] restingSystems;

	// Token: 0x04001AAA RID: 6826
	[NonSerialized]
	public float generation;

	// Token: 0x04001AAB RID: 6827
	public GameObjectRef spreadSubEntity;

	// Token: 0x04001AAC RID: 6828
	public float tickRate = 0.5f;

	// Token: 0x04001AAD RID: 6829
	public float damagePerSecond = 2f;

	// Token: 0x04001AAE RID: 6830
	public float radius = 0.5f;

	// Token: 0x04001AAF RID: 6831
	public int waterToExtinguish = 200;

	// Token: 0x04001AB0 RID: 6832
	public bool canMerge;

	// Token: 0x04001AB1 RID: 6833
	public LayerMask AttackLayers = 1220225809;

	// Token: 0x04001AB2 RID: 6834
	public bool ignoreNPC;

	// Token: 0x04001AB3 RID: 6835
	private Vector3 lastPos = Vector3.zero;

	// Token: 0x04001AB4 RID: 6836
	private float deathTime;

	// Token: 0x04001AB5 RID: 6837
	private int wetness;

	// Token: 0x04001AB6 RID: 6838
	private float spawnTime;

	// Token: 0x04001AB7 RID: 6839
	private Vector3 delayedVelocity;
}
