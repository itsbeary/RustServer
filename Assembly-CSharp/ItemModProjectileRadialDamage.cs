using System;
using System.Collections.Generic;
using Facepunch;
using Rust;
using UnityEngine;

// Token: 0x020005FF RID: 1535
public class ItemModProjectileRadialDamage : ItemModProjectileMod
{
	// Token: 0x06002DCF RID: 11727 RVA: 0x00113AA4 File Offset: 0x00111CA4
	public override void ServerProjectileHit(HitInfo info)
	{
		if (this.effect.isValid)
		{
			Effect.server.Run(this.effect.resourcePath, info.HitPositionWorld, info.HitNormalWorld, null, false);
		}
		List<BaseCombatEntity> list = Pool.GetList<BaseCombatEntity>();
		List<BaseCombatEntity> list2 = Pool.GetList<BaseCombatEntity>();
		Vis.Entities<BaseCombatEntity>(info.HitPositionWorld, this.radius, list2, 1237003025, QueryTriggerInteraction.Collide);
		foreach (BaseCombatEntity baseCombatEntity in list2)
		{
			if (baseCombatEntity.isServer && !list.Contains(baseCombatEntity) && (!(baseCombatEntity == info.HitEntity) || !this.ignoreHitObject))
			{
				baseCombatEntity.CenterPoint();
				Vector3 vector = baseCombatEntity.ClosestPoint(info.HitPositionWorld);
				float num = Vector3.Distance(vector, info.HitPositionWorld) / this.radius;
				if (num <= 1f)
				{
					float num2 = 1f - num;
					if (baseCombatEntity.IsVisibleAndCanSee(info.HitPositionWorld - info.ProjectileVelocity.normalized * 0.1f, float.PositiveInfinity) && baseCombatEntity.IsVisibleAndCanSee(info.HitPositionWorld - (vector - info.HitPositionWorld).normalized * 0.1f, float.PositiveInfinity))
					{
						list.Add(baseCombatEntity);
						baseCombatEntity.OnAttacked(new HitInfo(info.Initiator, baseCombatEntity, this.damage.type, this.damage.amount * num2));
					}
				}
			}
		}
		Pool.FreeList<BaseCombatEntity>(ref list);
		Pool.FreeList<BaseCombatEntity>(ref list2);
	}

	// Token: 0x0400257F RID: 9599
	public float radius = 0.5f;

	// Token: 0x04002580 RID: 9600
	public DamageTypeEntry damage;

	// Token: 0x04002581 RID: 9601
	public GameObjectRef effect;

	// Token: 0x04002582 RID: 9602
	public bool ignoreHitObject = true;
}
