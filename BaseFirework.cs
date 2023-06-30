using System;
using System.Collections.Generic;
using Rust;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class BaseFirework : BaseCombatEntity, IIgniteable
{
	// Token: 0x0600004E RID: 78 RVA: 0x0000326F File Offset: 0x0000146F
	public bool IsLit()
	{
		return base.HasFlag(BaseEntity.Flags.OnFire);
	}

	// Token: 0x0600004F RID: 79 RVA: 0x00003278 File Offset: 0x00001478
	public bool IsExhausted()
	{
		return base.HasFlag(BaseEntity.Flags.Reserved8);
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00003285 File Offset: 0x00001485
	public static int NumActiveFireworks()
	{
		return BaseFirework._activeFireworks.Count;
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00003294 File Offset: 0x00001494
	public virtual void TryLightFuse()
	{
		if (this.IsExhausted() || this.IsLit())
		{
			return;
		}
		base.SetFlag(BaseEntity.Flags.OnFire, true, false, true);
		base.EnableGlobalBroadcast(true);
		base.Invoke(new Action(this.Begin), this.fuseLength);
		this.pickup.enabled = false;
		base.EnableSaving(false);
	}

	// Token: 0x06000052 RID: 82 RVA: 0x000032EF File Offset: 0x000014EF
	public virtual void Begin()
	{
		base.SetFlag(BaseEntity.Flags.OnFire, false, false, true);
		base.SetFlag(BaseEntity.Flags.On, true, false, false);
		base.SendNetworkUpdate_Flags();
		base.Invoke(new Action(this.OnExhausted), this.activityLength);
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00003324 File Offset: 0x00001524
	public virtual void OnExhausted()
	{
		base.SetFlag(BaseEntity.Flags.Reserved8, true, false, false);
		base.SetFlag(BaseEntity.Flags.OnFire, false, false, false);
		base.SetFlag(BaseEntity.Flags.On, false, false, false);
		base.EnableGlobalBroadcast(false);
		base.SendNetworkUpdate_Flags();
		base.Invoke(new Action(this.Cleanup), this.corpseDuration);
		BaseFirework._activeFireworks.Remove(this);
	}

	// Token: 0x06000054 RID: 84 RVA: 0x00003384 File Offset: 0x00001584
	public void Cleanup()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	// Token: 0x06000055 RID: 85 RVA: 0x0000338D File Offset: 0x0000158D
	internal override void DoServerDestroy()
	{
		BaseFirework._activeFireworks.Remove(this);
		base.DoServerDestroy();
	}

	// Token: 0x06000056 RID: 86 RVA: 0x000033A1 File Offset: 0x000015A1
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (!base.isServer)
		{
			return;
		}
		if (info.damageTypes.Has(DamageType.Heat))
		{
			this.StaggeredTryLightFuse();
		}
	}

	// Token: 0x06000057 RID: 87 RVA: 0x000033C7 File Offset: 0x000015C7
	public void Ignite(Vector3 fromPos)
	{
		this.StaggeredTryLightFuse();
	}

	// Token: 0x06000058 RID: 88 RVA: 0x000033D0 File Offset: 0x000015D0
	public void StaggeredTryLightFuse()
	{
		if (this.IsExhausted() || this.IsLit())
		{
			return;
		}
		if (this.limitActiveCount)
		{
			if (BaseFirework.NumActiveFireworks() >= BaseFirework.maxActiveFireworks)
			{
				base.SetFlag(BaseEntity.Flags.OnFire, true, false, true);
				base.Invoke(new Action(this.StaggeredTryLightFuse), 0.35f);
				return;
			}
			BaseFirework._activeFireworks.Add(this);
			base.SetFlag(BaseEntity.Flags.OnFire, false, false, false);
		}
		base.Invoke(new Action(this.TryLightFuse), UnityEngine.Random.Range(0.1f, 0.3f));
	}

	// Token: 0x06000059 RID: 89 RVA: 0x0000345C File Offset: 0x0000165C
	public bool CanIgnite()
	{
		return !this.IsExhausted() && !this.IsLit();
	}

	// Token: 0x0600005A RID: 90 RVA: 0x00003471 File Offset: 0x00001671
	public override bool CanPickup(BasePlayer player)
	{
		return !this.IsExhausted() && base.CanPickup(player) && !this.IsLit();
	}

	// Token: 0x04000045 RID: 69
	public float fuseLength = 3f;

	// Token: 0x04000046 RID: 70
	public float activityLength = 10f;

	// Token: 0x04000047 RID: 71
	public const BaseEntity.Flags Flag_Spent = BaseEntity.Flags.Reserved8;

	// Token: 0x04000048 RID: 72
	public float corpseDuration = 15f;

	// Token: 0x04000049 RID: 73
	public bool limitActiveCount;

	// Token: 0x0400004A RID: 74
	[ServerVar]
	public static int maxActiveFireworks = 25;

	// Token: 0x0400004B RID: 75
	public static HashSet<BaseFirework> _activeFireworks = new HashSet<BaseFirework>();
}
