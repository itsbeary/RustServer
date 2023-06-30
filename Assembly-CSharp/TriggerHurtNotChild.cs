using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x0200058E RID: 1422
public class TriggerHurtNotChild : TriggerBase, IServerComponent, IHurtTrigger
{
	// Token: 0x06002B9D RID: 11165 RVA: 0x00108D6C File Offset: 0x00106F6C
	internal override GameObject InterestedInObject(GameObject obj)
	{
		obj = base.InterestedInObject(obj);
		if (obj == null)
		{
			return null;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity == null)
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		if (this.ignoreNPC && baseEntity.IsNpc)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x06002B9E RID: 11166 RVA: 0x00108DC1 File Offset: 0x00106FC1
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), 0f, 1f / this.DamageTickRate);
	}

	// Token: 0x06002B9F RID: 11167 RVA: 0x00108DE8 File Offset: 0x00106FE8
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (ent != null && this.DamageDelay > 0f)
		{
			if (this.entryTimes == null)
			{
				this.entryTimes = new Dictionary<BaseEntity, float>();
			}
			this.entryTimes.Add(ent, Time.time);
		}
	}

	// Token: 0x06002BA0 RID: 11168 RVA: 0x00108E36 File Offset: 0x00107036
	internal override void OnEntityLeave(BaseEntity ent)
	{
		if (ent != null && this.entryTimes != null)
		{
			this.entryTimes.Remove(ent);
		}
		base.OnEntityLeave(ent);
	}

	// Token: 0x06002BA1 RID: 11169 RVA: 0x00108E5D File Offset: 0x0010705D
	internal override void OnEmpty()
	{
		base.CancelInvoke(new Action(this.OnTick));
	}

	// Token: 0x06002BA2 RID: 11170 RVA: 0x00108E71 File Offset: 0x00107071
	protected void OnEnable()
	{
		this.timeSinceAcivation = 0f;
		this.hurtTiggerUser = this.SourceEntity as TriggerHurtNotChild.IHurtTriggerUser;
	}

	// Token: 0x06002BA3 RID: 11171 RVA: 0x00108E94 File Offset: 0x00107094
	public new void OnDisable()
	{
		base.CancelInvoke(new Action(this.OnTick));
		base.OnDisable();
	}

	// Token: 0x06002BA4 RID: 11172 RVA: 0x00108EB0 File Offset: 0x001070B0
	private bool IsInterested(BaseEntity ent)
	{
		if (this.timeSinceAcivation < this.activationDelay)
		{
			return false;
		}
		BasePlayer basePlayer = ent.ToPlayer();
		if (basePlayer != null)
		{
			if (basePlayer.isMounted)
			{
				BaseVehicle mountedVehicle = basePlayer.GetMountedVehicle();
				if (this.SourceEntity != null && mountedVehicle == this.SourceEntity)
				{
					return false;
				}
				if (this.ignoreAllVehicleMounted && mountedVehicle != null)
				{
					return false;
				}
			}
			if (this.SourceEntity != null && basePlayer.HasEntityInParents(this.SourceEntity))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002BA5 RID: 11173 RVA: 0x00108F44 File Offset: 0x00107144
	private void OnTick()
	{
		if (this.entityContents.IsNullOrEmpty<BaseEntity>())
		{
			return;
		}
		List<BaseEntity> list = Pool.GetList<BaseEntity>();
		list.AddRange(this.entityContents);
		foreach (BaseEntity baseEntity in list)
		{
			float num;
			if (baseEntity.IsValid() && this.IsInterested(baseEntity) && (this.DamageDelay <= 0f || this.entryTimes == null || !this.entryTimes.TryGetValue(baseEntity, out num) || num + this.DamageDelay <= Time.time) && (!this.RequireUpAxis || Vector3.Dot(baseEntity.transform.up, base.transform.up) >= 0f))
			{
				float num2 = this.DamagePerSecond * 1f / this.DamageTickRate;
				if (this.UseSourceEntityDamageMultiplier && this.hurtTiggerUser != null)
				{
					num2 *= this.hurtTiggerUser.GetDamageMultiplier(baseEntity);
				}
				if (baseEntity.IsNpc)
				{
					num2 *= this.npcMultiplier;
				}
				if (baseEntity is ResourceEntity)
				{
					num2 *= this.resourceMultiplier;
				}
				Vector3 vector = baseEntity.transform.position + Vector3.up * 1f;
				bool flag = baseEntity is BasePlayer || baseEntity is BaseNpc;
				BaseEntity baseEntity2 = null;
				BaseEntity baseEntity3 = null;
				if (this.hurtTiggerUser != null)
				{
					baseEntity2 = this.hurtTiggerUser.GetPlayerDamageInitiator();
					baseEntity3 = this.SourceEntity.LookupPrefab();
				}
				if (baseEntity2 == null)
				{
					if (this.SourceEntity != null)
					{
						baseEntity2 = this.SourceEntity;
					}
					else
					{
						baseEntity2 = base.gameObject.ToBaseEntity();
					}
				}
				HitInfo hitInfo = new HitInfo
				{
					DoHitEffects = true,
					HitEntity = baseEntity,
					HitPositionWorld = vector,
					HitPositionLocal = baseEntity.transform.InverseTransformPoint(vector),
					HitNormalWorld = Vector3.up,
					HitMaterial = (flag ? StringPool.Get("Flesh") : 0U),
					WeaponPrefab = baseEntity3,
					Initiator = baseEntity2
				};
				hitInfo.damageTypes = new DamageTypeList();
				hitInfo.damageTypes.Set(this.damageType, num2);
				baseEntity.OnAttacked(hitInfo);
				if (this.hurtTiggerUser != null)
				{
					this.hurtTiggerUser.OnHurtTriggerOccupant(baseEntity, this.damageType, num2);
				}
				if (this.triggerHitImpacts)
				{
					Effect.server.ImpactEffect(hitInfo);
				}
			}
		}
		Pool.FreeList<BaseEntity>(ref list);
		base.RemoveInvalidEntities();
	}

	// Token: 0x04002380 RID: 9088
	public float DamagePerSecond = 1f;

	// Token: 0x04002381 RID: 9089
	public float DamageTickRate = 4f;

	// Token: 0x04002382 RID: 9090
	public float DamageDelay;

	// Token: 0x04002383 RID: 9091
	public DamageType damageType;

	// Token: 0x04002384 RID: 9092
	public bool ignoreNPC = true;

	// Token: 0x04002385 RID: 9093
	public float npcMultiplier = 1f;

	// Token: 0x04002386 RID: 9094
	public float resourceMultiplier = 1f;

	// Token: 0x04002387 RID: 9095
	public bool triggerHitImpacts = true;

	// Token: 0x04002388 RID: 9096
	public bool RequireUpAxis;

	// Token: 0x04002389 RID: 9097
	public BaseEntity SourceEntity;

	// Token: 0x0400238A RID: 9098
	public bool UseSourceEntityDamageMultiplier = true;

	// Token: 0x0400238B RID: 9099
	public bool ignoreAllVehicleMounted;

	// Token: 0x0400238C RID: 9100
	public float activationDelay;

	// Token: 0x0400238D RID: 9101
	private Dictionary<BaseEntity, float> entryTimes;

	// Token: 0x0400238E RID: 9102
	private TimeSince timeSinceAcivation;

	// Token: 0x0400238F RID: 9103
	private TriggerHurtNotChild.IHurtTriggerUser hurtTiggerUser;

	// Token: 0x02000D77 RID: 3447
	public interface IHurtTriggerUser
	{
		// Token: 0x06005126 RID: 20774
		BasePlayer GetPlayerDamageInitiator();

		// Token: 0x06005127 RID: 20775
		float GetDamageMultiplier(BaseEntity ent);

		// Token: 0x06005128 RID: 20776
		void OnHurtTriggerOccupant(BaseEntity hurtEntity, DamageType damageType, float damageTotal);
	}
}
