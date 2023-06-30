using System;
using System.Collections.Generic;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x02000308 RID: 776
public class DirectionalDamageTrigger : TriggerBase
{
	// Token: 0x06001ECD RID: 7885 RVA: 0x000D1D48 File Offset: 0x000CFF48
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

	// Token: 0x06001ECE RID: 7886 RVA: 0x000D1D95 File Offset: 0x000CFF95
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), this.repeatRate, this.repeatRate);
	}

	// Token: 0x06001ECF RID: 7887 RVA: 0x000D1DB5 File Offset: 0x000CFFB5
	internal override void OnEmpty()
	{
		base.CancelInvoke(new Action(this.OnTick));
	}

	// Token: 0x06001ED0 RID: 7888 RVA: 0x000D1DCC File Offset: 0x000CFFCC
	private void OnTick()
	{
		if (this.attackEffect.isValid)
		{
			Effect.server.Run(this.attackEffect.resourcePath, base.transform.position, Vector3.up, null, false);
		}
		if (this.entityContents == null)
		{
			return;
		}
		foreach (BaseEntity baseEntity in this.entityContents.ToArray<BaseEntity>())
		{
			if (baseEntity.IsValid())
			{
				BaseCombatEntity baseCombatEntity = baseEntity as BaseCombatEntity;
				if (!(baseCombatEntity == null))
				{
					HitInfo hitInfo = new HitInfo();
					hitInfo.damageTypes.Add(this.damageType);
					hitInfo.DoHitEffects = true;
					hitInfo.DidHit = true;
					hitInfo.PointStart = base.transform.position;
					hitInfo.PointEnd = baseCombatEntity.transform.position;
					baseCombatEntity.Hurt(hitInfo);
				}
			}
		}
	}

	// Token: 0x040017B8 RID: 6072
	public float repeatRate = 1f;

	// Token: 0x040017B9 RID: 6073
	public List<DamageTypeEntry> damageType;

	// Token: 0x040017BA RID: 6074
	public GameObjectRef attackEffect;
}
