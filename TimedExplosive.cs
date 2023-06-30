using System;
using System.Collections.Generic;
using Facepunch;
using Facepunch.Rust;
using Rust;
using Rust.Ai;
using UnityEngine;

// Token: 0x02000467 RID: 1127
public class TimedExplosive : BaseEntity, ServerProjectile.IProjectileImpact
{
	// Token: 0x0600255F RID: 9567 RVA: 0x000EC140 File Offset: 0x000EA340
	public void SetDamageScale(float scale)
	{
		foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
		{
			damageTypeEntry.amount *= scale;
		}
	}

	// Token: 0x06002560 RID: 9568 RVA: 0x0000627E File Offset: 0x0000447E
	public override float GetNetworkTime()
	{
		return Time.fixedTime;
	}

	// Token: 0x06002561 RID: 9569 RVA: 0x000EC198 File Offset: 0x000EA398
	public override void ServerInit()
	{
		this.lastBounceTime = Time.time;
		base.ServerInit();
		this.SetFuse(this.GetRandomTimerTime());
		base.ReceiveCollisionMessages(true);
		if (this.waterCausesExplosion || this.AlwaysRunWaterCheck)
		{
			base.InvokeRepeating(new Action(this.WaterCheck), 0f, 0.5f);
		}
	}

	// Token: 0x17000315 RID: 789
	// (get) Token: 0x06002562 RID: 9570 RVA: 0x00007A44 File Offset: 0x00005C44
	protected virtual bool AlwaysRunWaterCheck
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06002563 RID: 9571 RVA: 0x000EC1F6 File Offset: 0x000EA3F6
	public virtual void WaterCheck()
	{
		if (this.waterCausesExplosion && this.WaterFactor() >= 0.5f)
		{
			this.Explode();
		}
	}

	// Token: 0x06002564 RID: 9572 RVA: 0x000EC213 File Offset: 0x000EA413
	public virtual void SetFuse(float fuseLength)
	{
		if (base.isServer)
		{
			base.Invoke(new Action(this.Explode), fuseLength);
			base.SetFlag(BaseEntity.Flags.Reserved2, true, false, true);
		}
	}

	// Token: 0x06002565 RID: 9573 RVA: 0x000EC23F File Offset: 0x000EA43F
	public virtual float GetRandomTimerTime()
	{
		return UnityEngine.Random.Range(this.timerAmountMin, this.timerAmountMax);
	}

	// Token: 0x06002566 RID: 9574 RVA: 0x000EC252 File Offset: 0x000EA452
	public virtual void ProjectileImpact(RaycastHit info, Vector3 rayOrigin)
	{
		this.Explode();
	}

	// Token: 0x06002567 RID: 9575 RVA: 0x000EC25A File Offset: 0x000EA45A
	public virtual void Explode()
	{
		this.Explode(base.PivotPoint());
	}

	// Token: 0x06002568 RID: 9576 RVA: 0x000EC268 File Offset: 0x000EA468
	public virtual void Explode(Vector3 explosionFxPos)
	{
		Analytics.Azure.OnExplosion(this);
		Collider component = base.GetComponent<Collider>();
		if (component)
		{
			component.enabled = false;
		}
		GameObjectRef gameObjectRef = this.explosionEffect;
		if (this.underwaterExplosionEffect.isValid || this.watersurfaceExplosionEffect.isValid)
		{
			WaterLevel.WaterInfo waterInfo = WaterLevel.GetWaterInfo(explosionFxPos - new Vector3(0f, 0.25f, 0f), true, false, null, false);
			if (waterInfo.isValid && waterInfo.overallDepth > 0.5f)
			{
				if (waterInfo.currentDepth > 1f)
				{
					gameObjectRef = (this.underwaterExplosionEffect.isValid ? this.underwaterExplosionEffect : this.watersurfaceExplosionEffect);
				}
				else
				{
					gameObjectRef = (this.watersurfaceExplosionEffect.isValid ? this.watersurfaceExplosionEffect : this.underwaterExplosionEffect);
				}
			}
		}
		if (gameObjectRef.isValid)
		{
			Effect.server.Run(gameObjectRef.resourcePath, explosionFxPos, this.explosionUsesForward ? base.transform.forward : Vector3.up, null, true);
		}
		if (this.damageTypes.Count > 0)
		{
			if (this.onlyDamageParent)
			{
				Vector3 vector = base.CenterPoint();
				DamageUtil.RadiusDamage(this.creatorEntity, base.LookupPrefab(), vector, this.minExplosionRadius, this.explosionRadius, this.damageTypes, 134383872, true);
				BaseEntity baseEntity = base.GetParentEntity();
				BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
				while (baseCombatEntity == null && baseEntity != null && baseEntity.HasParent())
				{
					baseEntity = baseEntity.GetParentEntity();
					baseCombatEntity = baseEntity as BaseCombatEntity;
				}
				if (baseEntity == null || !baseEntity.gameObject.IsOnLayer(Layer.Construction))
				{
					List<BuildingBlock> list = Pool.GetList<BuildingBlock>();
					Vis.Entities<BuildingBlock>(vector, this.explosionRadius, list, 2097152, QueryTriggerInteraction.Ignore);
					BuildingBlock buildingBlock = null;
					float num = float.PositiveInfinity;
					foreach (BuildingBlock buildingBlock2 in list)
					{
						if (!buildingBlock2.isClient && !buildingBlock2.IsDestroyed && buildingBlock2.healthFraction > 0f)
						{
							float num2 = Vector3.Distance(buildingBlock2.ClosestPoint(vector), vector);
							if (num2 < num && buildingBlock2.IsVisible(vector, this.explosionRadius))
							{
								buildingBlock = buildingBlock2;
								num = num2;
							}
						}
					}
					if (buildingBlock)
					{
						HitInfo hitInfo = new HitInfo();
						hitInfo.Initiator = this.creatorEntity;
						hitInfo.WeaponPrefab = base.LookupPrefab();
						hitInfo.damageTypes.Add(this.damageTypes);
						hitInfo.PointStart = vector;
						hitInfo.PointEnd = buildingBlock.transform.position;
						float num3 = 1f - Mathf.Clamp01((num - this.minExplosionRadius) / (this.explosionRadius - this.minExplosionRadius));
						hitInfo.damageTypes.ScaleAll(num3);
						buildingBlock.Hurt(hitInfo);
					}
					Pool.FreeList<BuildingBlock>(ref list);
				}
				if (baseCombatEntity)
				{
					HitInfo hitInfo2 = new HitInfo();
					hitInfo2.Initiator = this.creatorEntity;
					hitInfo2.WeaponPrefab = base.LookupPrefab();
					hitInfo2.damageTypes.Add(this.damageTypes);
					baseCombatEntity.Hurt(hitInfo2);
				}
				else if (baseEntity != null)
				{
					HitInfo hitInfo3 = new HitInfo();
					hitInfo3.Initiator = this.creatorEntity;
					hitInfo3.WeaponPrefab = base.LookupPrefab();
					hitInfo3.damageTypes.Add(this.damageTypes);
					hitInfo3.PointStart = vector;
					hitInfo3.PointEnd = baseEntity.transform.position;
					baseEntity.OnAttacked(hitInfo3);
				}
				if (this.creatorEntity != null && this.damageTypes != null)
				{
					float num4 = 0f;
					foreach (DamageTypeEntry damageTypeEntry in this.damageTypes)
					{
						num4 += damageTypeEntry.amount;
					}
					Sense.Stimulate(new Sensation
					{
						Type = SensationType.Explosion,
						Position = this.creatorEntity.transform.position,
						Radius = this.explosionRadius * 17f,
						DamagePotential = num4,
						InitiatorPlayer = (this.creatorEntity as BasePlayer),
						Initiator = this.creatorEntity
					});
				}
			}
			else
			{
				DamageUtil.RadiusDamage(this.creatorEntity, base.LookupPrefab(), base.CenterPoint(), this.minExplosionRadius, this.explosionRadius, this.damageTypes, 1210222849, true);
				if (this.creatorEntity != null && this.damageTypes != null)
				{
					float num5 = 0f;
					foreach (DamageTypeEntry damageTypeEntry2 in this.damageTypes)
					{
						num5 += damageTypeEntry2.amount;
					}
					Sense.Stimulate(new Sensation
					{
						Type = SensationType.Explosion,
						Position = this.creatorEntity.transform.position,
						Radius = this.explosionRadius * 17f,
						DamagePotential = num5,
						InitiatorPlayer = (this.creatorEntity as BasePlayer),
						Initiator = this.creatorEntity
					});
				}
			}
			this.BlindAnyAI();
		}
		if (base.IsDestroyed || base.HasFlag(BaseEntity.Flags.Broken))
		{
			return;
		}
		base.Kill(BaseNetworkable.DestroyMode.Gib);
	}

	// Token: 0x06002569 RID: 9577 RVA: 0x000EC808 File Offset: 0x000EAA08
	private void BlindAnyAI()
	{
		if (!this.BlindAI)
		{
			return;
		}
		int brainsInSphere = BaseEntity.Query.Server.GetBrainsInSphere(base.transform.position, 10f, TimedExplosive.queryResults, null);
		for (int i = 0; i < brainsInSphere; i++)
		{
			BaseEntity baseEntity = TimedExplosive.queryResults[i];
			if (Vector3.Distance(base.transform.position, baseEntity.transform.position) <= this.aiBlindRange)
			{
				BaseAIBrain component = baseEntity.GetComponent<BaseAIBrain>();
				if (!(component == null))
				{
					BaseEntity brainBaseEntity = component.GetBrainBaseEntity();
					if (!(brainBaseEntity == null) && brainBaseEntity.IsVisible(base.CenterPoint(), float.PositiveInfinity))
					{
						float num = this.aiBlindDuration * component.BlindDurationMultiplier * UnityEngine.Random.Range(0.6f, 1.4f);
						component.SetBlinded(num);
						TimedExplosive.queryResults[i] = null;
					}
				}
			}
		}
	}

	// Token: 0x0600256A RID: 9578 RVA: 0x000EC8E4 File Offset: 0x000EAAE4
	public override void OnCollision(Collision collision, BaseEntity hitEntity)
	{
		if (this.canStick && !this.IsStuck())
		{
			bool flag = true;
			if (hitEntity)
			{
				flag = this.CanStickTo(hitEntity);
				if (!flag)
				{
					Collider component = base.GetComponent<Collider>();
					if (collision.collider != null && component != null)
					{
						Physics.IgnoreCollision(collision.collider, component);
					}
				}
			}
			if (flag)
			{
				this.DoCollisionStick(collision, hitEntity);
			}
		}
		if (this.explodeOnContact && !base.IsBusy())
		{
			this.SetMotionEnabled(false);
			base.SetFlag(BaseEntity.Flags.Busy, true, false, false);
			base.Invoke(new Action(this.Explode), 0.015f);
			return;
		}
		this.DoBounceEffect();
	}

	// Token: 0x0600256B RID: 9579 RVA: 0x000EC990 File Offset: 0x000EAB90
	public virtual bool CanStickTo(BaseEntity entity)
	{
		DecorDeployable decorDeployable;
		return !entity.TryGetComponent<DecorDeployable>(out decorDeployable) && !(entity is Drone) && (!(entity is BaseVehicle) || entity is Tugboat);
	}

	// Token: 0x0600256C RID: 9580 RVA: 0x000EC9C8 File Offset: 0x000EABC8
	private void DoBounceEffect()
	{
		if (!this.bounceEffect.isValid)
		{
			return;
		}
		if (Time.time - this.lastBounceTime < 0.2f)
		{
			return;
		}
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component && component.velocity.magnitude < 1f)
		{
			return;
		}
		if (this.bounceEffect.isValid)
		{
			Effect.server.Run(this.bounceEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		}
		this.lastBounceTime = Time.time;
	}

	// Token: 0x0600256D RID: 9581 RVA: 0x000ECA58 File Offset: 0x000EAC58
	private void DoCollisionStick(Collision collision, BaseEntity ent)
	{
		ContactPoint contact = collision.GetContact(0);
		this.DoStick(contact.point, contact.normal, ent, collision.collider);
	}

	// Token: 0x0600256E RID: 9582 RVA: 0x000ECA88 File Offset: 0x000EAC88
	public virtual void SetMotionEnabled(bool wantsMotion)
	{
		Rigidbody rigidbody = base.GetComponent<Rigidbody>();
		if (wantsMotion)
		{
			if (rigidbody == null && this.hadRB)
			{
				rigidbody = base.gameObject.AddComponent<Rigidbody>();
				rigidbody.mass = this.rbMass;
				rigidbody.drag = this.rbDrag;
				rigidbody.angularDrag = this.rbAngularDrag;
				rigidbody.collisionDetectionMode = this.rbCollisionMode;
				rigidbody.useGravity = true;
				rigidbody.isKinematic = false;
				return;
			}
		}
		else if (rigidbody != null)
		{
			this.hadRB = true;
			this.rbMass = rigidbody.mass;
			this.rbDrag = rigidbody.drag;
			this.rbAngularDrag = rigidbody.angularDrag;
			this.rbCollisionMode = rigidbody.collisionDetectionMode;
			UnityEngine.Object.Destroy(rigidbody);
		}
	}

	// Token: 0x0600256F RID: 9583 RVA: 0x000ECB48 File Offset: 0x000EAD48
	public bool IsStuck()
	{
		Rigidbody component = base.GetComponent<Rigidbody>();
		if (component && !component.isKinematic)
		{
			return false;
		}
		Collider component2 = base.GetComponent<Collider>();
		return (!component2 || !component2.enabled) && this.parentEntity.IsValid(true);
	}

	// Token: 0x06002570 RID: 9584 RVA: 0x000ECB94 File Offset: 0x000EAD94
	public void DoStick(Vector3 position, Vector3 normal, BaseEntity ent, Collider collider)
	{
		if (ent == null)
		{
			return;
		}
		if (ent is TimedExplosive)
		{
			if (!ent.HasParent())
			{
				return;
			}
			position = ent.transform.position;
			ent = ent.parentEntity.Get(true);
		}
		this.SetMotionEnabled(false);
		if (base.HasChild(ent))
		{
			return;
		}
		base.transform.position = position;
		base.transform.rotation = Quaternion.LookRotation(normal, base.transform.up);
		if (collider != null)
		{
			base.SetParent(ent, ent.FindBoneID(collider.transform), true, false);
		}
		else
		{
			base.SetParent(ent, StringPool.closest, true, false);
		}
		if (this.stickEffect.isValid)
		{
			Effect.server.Run(this.stickEffect.resourcePath, base.transform.position, Vector3.up, null, true);
		}
		base.ReceiveCollisionMessages(false);
	}

	// Token: 0x06002571 RID: 9585 RVA: 0x000ECC78 File Offset: 0x000EAE78
	private void UnStick()
	{
		if (!base.GetParentEntity())
		{
			return;
		}
		base.SetParent(null, true, true);
		this.SetMotionEnabled(true);
		base.ReceiveCollisionMessages(true);
	}

	// Token: 0x06002572 RID: 9586 RVA: 0x000ECC9F File Offset: 0x000EAE9F
	internal override void OnParentRemoved()
	{
		this.UnStick();
	}

	// Token: 0x06002573 RID: 9587 RVA: 0x000ECCA7 File Offset: 0x000EAEA7
	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
	}

	// Token: 0x06002574 RID: 9588 RVA: 0x000ECCB0 File Offset: 0x000EAEB0
	public override void PostServerLoad()
	{
		base.PostServerLoad();
		if (this.parentEntity.IsValid(true))
		{
			this.DoStick(base.transform.position, base.transform.forward, this.parentEntity.Get(true), null);
		}
	}

	// Token: 0x06002575 RID: 9589 RVA: 0x000ECCEF File Offset: 0x000EAEEF
	public override void Load(BaseNetworkable.LoadInfo info)
	{
		base.Load(info);
		if (info.msg.explosive != null)
		{
			this.parentEntity.uid = info.msg.explosive.parentid;
		}
	}

	// Token: 0x06002576 RID: 9590 RVA: 0x000ECD20 File Offset: 0x000EAF20
	public virtual void SetCollisionEnabled(bool wantsCollision)
	{
		Collider component = base.GetComponent<Collider>();
		if (component && component.enabled != wantsCollision)
		{
			component.enabled = wantsCollision;
		}
	}

	// Token: 0x04001D8E RID: 7566
	public float timerAmountMin = 10f;

	// Token: 0x04001D8F RID: 7567
	public float timerAmountMax = 20f;

	// Token: 0x04001D90 RID: 7568
	public float minExplosionRadius;

	// Token: 0x04001D91 RID: 7569
	public float explosionRadius = 10f;

	// Token: 0x04001D92 RID: 7570
	public bool explodeOnContact;

	// Token: 0x04001D93 RID: 7571
	public bool canStick;

	// Token: 0x04001D94 RID: 7572
	public bool onlyDamageParent;

	// Token: 0x04001D95 RID: 7573
	public bool BlindAI;

	// Token: 0x04001D96 RID: 7574
	public float aiBlindDuration = 2.5f;

	// Token: 0x04001D97 RID: 7575
	public float aiBlindRange = 4f;

	// Token: 0x04001D98 RID: 7576
	public GameObjectRef explosionEffect;

	// Token: 0x04001D99 RID: 7577
	[Tooltip("Optional: Will fall back to watersurfaceExplosionEffect or explosionEffect if not assigned.")]
	public GameObjectRef underwaterExplosionEffect;

	// Token: 0x04001D9A RID: 7578
	[Tooltip("Optional: Will fall back to underwaterExplosionEffect or explosionEffect if not assigned.")]
	public GameObjectRef watersurfaceExplosionEffect;

	// Token: 0x04001D9B RID: 7579
	public GameObjectRef stickEffect;

	// Token: 0x04001D9C RID: 7580
	public GameObjectRef bounceEffect;

	// Token: 0x04001D9D RID: 7581
	public bool explosionUsesForward;

	// Token: 0x04001D9E RID: 7582
	public bool waterCausesExplosion;

	// Token: 0x04001D9F RID: 7583
	public List<DamageTypeEntry> damageTypes = new List<DamageTypeEntry>();

	// Token: 0x04001DA0 RID: 7584
	[NonSerialized]
	private float lastBounceTime;

	// Token: 0x04001DA1 RID: 7585
	private bool hadRB;

	// Token: 0x04001DA2 RID: 7586
	private float rbMass;

	// Token: 0x04001DA3 RID: 7587
	private float rbDrag;

	// Token: 0x04001DA4 RID: 7588
	private float rbAngularDrag;

	// Token: 0x04001DA5 RID: 7589
	private CollisionDetectionMode rbCollisionMode;

	// Token: 0x04001DA6 RID: 7590
	private static BaseEntity[] queryResults = new BaseEntity[64];
}
