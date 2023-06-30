using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000420 RID: 1056
public class HelicopterDebris : ServerGib
{
	// Token: 0x060023B6 RID: 9142 RVA: 0x000E4137 File Offset: 0x000E2337
	public override void ServerInit()
	{
		base.ServerInit();
		this.tooHotUntil = Time.realtimeSinceStartup + 480f;
	}

	// Token: 0x060023B7 RID: 9143 RVA: 0x000E4150 File Offset: 0x000E2350
	public override void PhysicsInit(Mesh mesh)
	{
		base.PhysicsInit(mesh);
		if (base.isServer)
		{
			this.resourceDispenser = base.GetComponent<ResourceDispenser>();
			float num = Mathf.Clamp01(base.GetComponent<Rigidbody>().mass / this.massReductionScalar);
			this.resourceDispenser.containedItems = new List<ItemAmount>();
			if (num > 0.75f && this.hqMetal != null)
			{
				this.resourceDispenser.containedItems.Add(new ItemAmount(this.hqMetal, (float)Mathf.CeilToInt(7f * num)));
			}
			if (num > 0f)
			{
				if (this.metalFragments != null)
				{
					this.resourceDispenser.containedItems.Add(new ItemAmount(this.metalFragments, (float)Mathf.CeilToInt(150f * num)));
				}
				if (this.charcoal != null)
				{
					this.resourceDispenser.containedItems.Add(new ItemAmount(this.charcoal, (float)Mathf.CeilToInt(80f * num)));
				}
			}
			this.resourceDispenser.Initialize();
		}
	}

	// Token: 0x060023B8 RID: 9144 RVA: 0x000E4260 File Offset: 0x000E2460
	public bool IsTooHot()
	{
		return this.tooHotUntil > Time.realtimeSinceStartup;
	}

	// Token: 0x060023B9 RID: 9145 RVA: 0x000E4270 File Offset: 0x000E2470
	public override void OnAttacked(HitInfo info)
	{
		if (this.IsTooHot() && info.WeaponPrefab is BaseMelee)
		{
			if (info.Initiator is BasePlayer)
			{
				HitInfo hitInfo = new HitInfo();
				hitInfo.damageTypes.Add(DamageType.Heat, 5f);
				hitInfo.DoHitEffects = true;
				hitInfo.DidHit = true;
				hitInfo.HitBone = 0U;
				hitInfo.Initiator = this;
				hitInfo.PointStart = base.transform.position;
				Effect.server.Run("assets/bundled/prefabs/fx/impacts/additive/fire.prefab", info.Initiator, 0U, new Vector3(0f, 1f, 0f), Vector3.up, null, false);
				return;
			}
		}
		else
		{
			if (this.resourceDispenser)
			{
				this.resourceDispenser.OnAttacked(info);
			}
			base.OnAttacked(info);
		}
	}

	// Token: 0x04001BC4 RID: 7108
	public ItemDefinition metalFragments;

	// Token: 0x04001BC5 RID: 7109
	public ItemDefinition hqMetal;

	// Token: 0x04001BC6 RID: 7110
	public ItemDefinition charcoal;

	// Token: 0x04001BC7 RID: 7111
	[Tooltip("Divide mass by this amount to produce a scalar of resources, default = 5")]
	public float massReductionScalar = 5f;

	// Token: 0x04001BC8 RID: 7112
	private ResourceDispenser resourceDispenser;

	// Token: 0x04001BC9 RID: 7113
	private float tooHotUntil;
}
