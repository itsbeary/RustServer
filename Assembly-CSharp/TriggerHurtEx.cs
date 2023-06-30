using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x0200074C RID: 1868
public class TriggerHurtEx : TriggerBase, IServerComponent, IHurtTrigger
{
	// Token: 0x0600341E RID: 13342 RVA: 0x00142020 File Offset: 0x00140220
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
		if (!(baseEntity is BaseCombatEntity))
		{
			return null;
		}
		if (baseEntity.isClient)
		{
			return null;
		}
		return baseEntity.gameObject;
	}

	// Token: 0x0600341F RID: 13343 RVA: 0x00142070 File Offset: 0x00140270
	internal void DoDamage(BaseEntity ent, TriggerHurtEx.HurtType type, List<DamageTypeEntry> damage, GameObjectRef effect, float multiply = 1f)
	{
		if (!this.damageEnabled)
		{
			return;
		}
		using (TimeWarning.New("TriggerHurtEx.DoDamage", 0))
		{
			if (damage != null && damage.Count > 0)
			{
				BaseCombatEntity baseCombatEntity = ent as BaseCombatEntity;
				if (baseCombatEntity)
				{
					HitInfo hitInfo = new HitInfo();
					hitInfo.damageTypes.Add(damage);
					hitInfo.damageTypes.ScaleAll(multiply);
					hitInfo.DoHitEffects = true;
					hitInfo.DidHit = true;
					hitInfo.Initiator = base.gameObject.ToBaseEntity();
					hitInfo.PointStart = base.transform.position;
					hitInfo.PointEnd = baseCombatEntity.transform.position;
					if (type == TriggerHurtEx.HurtType.Simple)
					{
						baseCombatEntity.Hurt(hitInfo);
					}
					else
					{
						baseCombatEntity.OnAttacked(hitInfo);
					}
				}
			}
			if (effect.isValid)
			{
				Effect.server.Run(effect.resourcePath, ent, StringPool.closest, base.transform.position, Vector3.up, null, false);
			}
		}
	}

	// Token: 0x06003420 RID: 13344 RVA: 0x00142170 File Offset: 0x00140370
	internal override void OnEntityEnter(BaseEntity ent)
	{
		base.OnEntityEnter(ent);
		if (ent == null)
		{
			return;
		}
		if (this.entityAddList == null)
		{
			this.entityAddList = new List<BaseEntity>();
		}
		this.entityAddList.Add(ent);
		base.Invoke(new Action(this.ProcessQueues), 0.1f);
	}

	// Token: 0x06003421 RID: 13345 RVA: 0x001421C4 File Offset: 0x001403C4
	internal override void OnEntityLeave(BaseEntity ent)
	{
		base.OnEntityLeave(ent);
		if (ent == null)
		{
			return;
		}
		if (this.entityLeaveList == null)
		{
			this.entityLeaveList = new List<BaseEntity>();
		}
		this.entityLeaveList.Add(ent);
		base.Invoke(new Action(this.ProcessQueues), 0.1f);
	}

	// Token: 0x06003422 RID: 13346 RVA: 0x00142218 File Offset: 0x00140418
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), this.repeatRate, this.repeatRate);
	}

	// Token: 0x06003423 RID: 13347 RVA: 0x00142238 File Offset: 0x00140438
	internal override void OnEmpty()
	{
		base.CancelInvoke(new Action(this.OnTick));
	}

	// Token: 0x06003424 RID: 13348 RVA: 0x0014224C File Offset: 0x0014044C
	private void OnTick()
	{
		this.ProcessQueues();
		if (this.entityInfo != null)
		{
			foreach (KeyValuePair<BaseEntity, TriggerHurtEx.EntityTriggerInfo> keyValuePair in this.entityInfo.ToArray<KeyValuePair<BaseEntity, TriggerHurtEx.EntityTriggerInfo>>())
			{
				if (keyValuePair.Key.IsValid())
				{
					Vector3 position = keyValuePair.Key.transform.position;
					float magnitude = (position - keyValuePair.Value.lastPosition).magnitude;
					if (magnitude > 0.01f)
					{
						keyValuePair.Value.lastPosition = position;
						this.DoDamage(keyValuePair.Key, this.hurtTypeOnMove, this.damageOnMove, this.effectOnMove, magnitude);
					}
					this.DoDamage(keyValuePair.Key, this.hurtTypeOnTimer, this.damageOnTimer, this.effectOnTimer, this.repeatRate);
				}
			}
		}
	}

	// Token: 0x06003425 RID: 13349 RVA: 0x00142330 File Offset: 0x00140530
	private void ProcessQueues()
	{
		if (this.entityAddList != null)
		{
			foreach (BaseEntity baseEntity in this.entityAddList)
			{
				if (baseEntity.IsValid())
				{
					this.DoDamage(baseEntity, this.hurtTypeOnEnter, this.damageOnEnter, this.effectOnEnter, 1f);
					if (this.entityInfo == null)
					{
						this.entityInfo = new Dictionary<BaseEntity, TriggerHurtEx.EntityTriggerInfo>();
					}
					if (!this.entityInfo.ContainsKey(baseEntity))
					{
						this.entityInfo.Add(baseEntity, new TriggerHurtEx.EntityTriggerInfo
						{
							lastPosition = baseEntity.transform.position
						});
					}
				}
			}
			this.entityAddList = null;
		}
		if (this.entityLeaveList != null)
		{
			foreach (BaseEntity baseEntity2 in this.entityLeaveList)
			{
				if (baseEntity2.IsValid())
				{
					this.DoDamage(baseEntity2, this.hurtTypeOnLeave, this.damageOnLeave, this.effectOnLeave, 1f);
					if (this.entityInfo != null)
					{
						this.entityInfo.Remove(baseEntity2);
						if (this.entityInfo.Count == 0)
						{
							this.entityInfo = null;
						}
					}
				}
			}
			this.entityLeaveList.Clear();
		}
	}

	// Token: 0x04002A8E RID: 10894
	public float repeatRate = 0.1f;

	// Token: 0x04002A8F RID: 10895
	[Header("On Enter")]
	public List<DamageTypeEntry> damageOnEnter;

	// Token: 0x04002A90 RID: 10896
	public GameObjectRef effectOnEnter;

	// Token: 0x04002A91 RID: 10897
	public TriggerHurtEx.HurtType hurtTypeOnEnter;

	// Token: 0x04002A92 RID: 10898
	[Header("On Timer (damage per second)")]
	public List<DamageTypeEntry> damageOnTimer;

	// Token: 0x04002A93 RID: 10899
	public GameObjectRef effectOnTimer;

	// Token: 0x04002A94 RID: 10900
	public TriggerHurtEx.HurtType hurtTypeOnTimer;

	// Token: 0x04002A95 RID: 10901
	[Header("On Move (damage per meter)")]
	public List<DamageTypeEntry> damageOnMove;

	// Token: 0x04002A96 RID: 10902
	public GameObjectRef effectOnMove;

	// Token: 0x04002A97 RID: 10903
	public TriggerHurtEx.HurtType hurtTypeOnMove;

	// Token: 0x04002A98 RID: 10904
	[Header("On Leave")]
	public List<DamageTypeEntry> damageOnLeave;

	// Token: 0x04002A99 RID: 10905
	public GameObjectRef effectOnLeave;

	// Token: 0x04002A9A RID: 10906
	public TriggerHurtEx.HurtType hurtTypeOnLeave;

	// Token: 0x04002A9B RID: 10907
	public bool damageEnabled = true;

	// Token: 0x04002A9C RID: 10908
	internal Dictionary<BaseEntity, TriggerHurtEx.EntityTriggerInfo> entityInfo;

	// Token: 0x04002A9D RID: 10909
	internal List<BaseEntity> entityAddList;

	// Token: 0x04002A9E RID: 10910
	internal List<BaseEntity> entityLeaveList;

	// Token: 0x02000E66 RID: 3686
	public enum HurtType
	{
		// Token: 0x04004BBA RID: 19386
		Simple,
		// Token: 0x04004BBB RID: 19387
		IncludeBleedingAndScreenShake
	}

	// Token: 0x02000E67 RID: 3687
	public class EntityTriggerInfo
	{
		// Token: 0x04004BBC RID: 19388
		public Vector3 lastPosition;
	}
}
