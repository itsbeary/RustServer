using System;
using System.Collections.Generic;
using Facepunch;
using Network;
using Rust;
using UnityEngine;

// Token: 0x0200007E RID: 126
public class GunTrap : StorageContainer
{
	// Token: 0x06000BF0 RID: 3056 RVA: 0x00068F94 File Offset: 0x00067194
	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		using (TimeWarning.New("GunTrap.OnRpcMessage", 0))
		{
		}
		return base.OnRpcMessage(player, rpc, msg);
	}

	// Token: 0x06000BF1 RID: 3057 RVA: 0x00068FD4 File Offset: 0x000671D4
	public override string Categorize()
	{
		return "GunTrap";
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x00068FDC File Offset: 0x000671DC
	public bool UseAmmo()
	{
		foreach (Item item in base.inventory.itemList)
		{
			if (item.info == this.ammoType && item.amount > 0)
			{
				item.UseItem(1);
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000BF3 RID: 3059 RVA: 0x00069058 File Offset: 0x00067258
	public void FireWeapon()
	{
		if (!this.UseAmmo())
		{
			return;
		}
		Effect.server.Run(this.gun_fire_effect.resourcePath, this, StringPool.Get(this.muzzlePos.gameObject.name), Vector3.zero, Vector3.zero, null, false);
		for (int i = 0; i < this.numPellets; i++)
		{
			this.FireBullet();
		}
	}

	// Token: 0x06000BF4 RID: 3060 RVA: 0x000690B8 File Offset: 0x000672B8
	public void FireBullet()
	{
		float num = 10f;
		Vector3 vector = this.muzzlePos.transform.position - this.muzzlePos.forward * 0.25f;
		Vector3 forward = this.muzzlePos.transform.forward;
		Vector3 modifiedAimConeDirection = AimConeUtil.GetModifiedAimConeDirection((float)this.aimCone, forward, true);
		Vector3 vector2 = vector + modifiedAimConeDirection * 300f;
		base.ClientRPC<Vector3>(null, "CLIENT_FireGun", vector2);
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		int num2 = 1220225793;
		GamePhysics.TraceAll(new Ray(vector, modifiedAimConeDirection), 0.1f, list, 300f, num2, QueryTriggerInteraction.UseGlobal, null);
		for (int i = 0; i < list.Count; i++)
		{
			RaycastHit raycastHit = list[i];
			BaseEntity entity = raycastHit.GetEntity();
			if (!(entity != null) || (!(entity == this) && !entity.EqualNetID(this)))
			{
				if (entity as BaseCombatEntity != null)
				{
					HitInfo hitInfo = new HitInfo(this, entity, DamageType.Bullet, num, raycastHit.point);
					entity.OnAttacked(hitInfo);
					if (entity is BasePlayer || entity is BaseNpc)
					{
						Effect.server.ImpactEffect(new HitInfo
						{
							HitPositionWorld = raycastHit.point,
							HitNormalWorld = -raycastHit.normal,
							HitMaterial = StringPool.Get("Flesh")
						});
					}
				}
				if (!(entity != null) || entity.ShouldBlockProjectiles())
				{
					vector2 = raycastHit.point;
					return;
				}
			}
		}
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x00069244 File Offset: 0x00067444
	public override void ServerInit()
	{
		base.ServerInit();
		base.InvokeRandomized(new Action(this.TriggerCheck), UnityEngine.Random.Range(0f, 1f), 0.5f, 0.1f);
	}

	// Token: 0x06000BF6 RID: 3062 RVA: 0x00069277 File Offset: 0x00067477
	public void TriggerCheck()
	{
		if (this.CheckTrigger())
		{
			this.FireWeapon();
		}
	}

	// Token: 0x06000BF7 RID: 3063 RVA: 0x00069288 File Offset: 0x00067488
	public bool CheckTrigger()
	{
		List<RaycastHit> list = Pool.GetList<RaycastHit>();
		HashSet<BaseEntity> entityContents = this.trigger.entityContents;
		bool flag = false;
		if (entityContents != null)
		{
			foreach (BaseEntity baseEntity in entityContents)
			{
				BasePlayer component = baseEntity.GetComponent<BasePlayer>();
				if (!component.IsSleeping() && component.IsAlive() && !component.IsBuildingAuthed())
				{
					list.Clear();
					GamePhysics.TraceAll(new Ray(component.eyes.position, (this.GetEyePosition() - component.eyes.position).normalized), 0f, list, 9f, 1218519297, QueryTriggerInteraction.UseGlobal, null);
					for (int i = 0; i < list.Count; i++)
					{
						BaseEntity entity = list[i].GetEntity();
						if (entity != null && (entity == this || entity.EqualNetID(this)))
						{
							flag = true;
							break;
						}
						if (!(entity != null) || entity.ShouldBlockProjectiles())
						{
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
			}
		}
		Pool.FreeList<RaycastHit>(ref list);
		return flag;
	}

	// Token: 0x06000BF8 RID: 3064 RVA: 0x000233C8 File Offset: 0x000215C8
	public bool IsTriggered()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved1);
	}

	// Token: 0x06000BF9 RID: 3065 RVA: 0x000693D0 File Offset: 0x000675D0
	public Vector3 GetEyePosition()
	{
		return this.eyeTransform.position;
	}

	// Token: 0x040007A9 RID: 1961
	public GameObjectRef gun_fire_effect;

	// Token: 0x040007AA RID: 1962
	public GameObjectRef bulletEffect;

	// Token: 0x040007AB RID: 1963
	public GameObjectRef triggeredEffect;

	// Token: 0x040007AC RID: 1964
	public Transform muzzlePos;

	// Token: 0x040007AD RID: 1965
	public Transform eyeTransform;

	// Token: 0x040007AE RID: 1966
	public int numPellets = 15;

	// Token: 0x040007AF RID: 1967
	public int aimCone = 30;

	// Token: 0x040007B0 RID: 1968
	public float sensorRadius = 1.25f;

	// Token: 0x040007B1 RID: 1969
	public ItemDefinition ammoType;

	// Token: 0x040007B2 RID: 1970
	public TargetTrigger trigger;

	// Token: 0x02000BDC RID: 3036
	public static class GunTrapFlags
	{
		// Token: 0x04004197 RID: 16791
		public const BaseEntity.Flags Triggered = BaseEntity.Flags.Reserved1;
	}
}
