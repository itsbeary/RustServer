using System;
using System.Linq;
using Rust;
using UnityEngine;

// Token: 0x0200058D RID: 1421
public class TriggerHurt : TriggerBase, IServerComponent, IHurtTrigger
{
	// Token: 0x06002B97 RID: 11159 RVA: 0x00108C4C File Offset: 0x00106E4C
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
		return baseEntity.gameObject;
	}

	// Token: 0x06002B98 RID: 11160 RVA: 0x00108C8F File Offset: 0x00106E8F
	internal override void OnObjects()
	{
		base.InvokeRepeating(new Action(this.OnTick), 0f, 1f / this.DamageTickRate);
	}

	// Token: 0x06002B99 RID: 11161 RVA: 0x00108CB4 File Offset: 0x00106EB4
	internal override void OnEmpty()
	{
		base.CancelInvoke(new Action(this.OnTick));
	}

	// Token: 0x06002B9A RID: 11162 RVA: 0x00108CC8 File Offset: 0x00106EC8
	private void OnTick()
	{
		BaseEntity baseEntity = base.gameObject.ToBaseEntity();
		if (this.entityContents == null)
		{
			return;
		}
		foreach (BaseEntity baseEntity2 in this.entityContents.ToArray<BaseEntity>())
		{
			if (baseEntity2.IsValid())
			{
				BaseCombatEntity baseCombatEntity = baseEntity2 as BaseCombatEntity;
				if (!(baseCombatEntity == null) && this.CanHurt(baseCombatEntity))
				{
					baseCombatEntity.Hurt(this.DamagePerSecond * (1f / this.DamageTickRate), this.damageType, baseEntity, true);
				}
			}
		}
	}

	// Token: 0x06002B9B RID: 11163 RVA: 0x0000441C File Offset: 0x0000261C
	protected virtual bool CanHurt(BaseCombatEntity ent)
	{
		return true;
	}

	// Token: 0x0400237D RID: 9085
	public float DamagePerSecond = 1f;

	// Token: 0x0400237E RID: 9086
	public float DamageTickRate = 4f;

	// Token: 0x0400237F RID: 9087
	public DamageType damageType;
}
