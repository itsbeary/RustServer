using System;

// Token: 0x020003B6 RID: 950
public class Hammer : BaseMelee
{
	// Token: 0x0600215B RID: 8539 RVA: 0x000DAD26 File Offset: 0x000D8F26
	public override bool CanHit(HitTest info)
	{
		return !(info.HitEntity == null) && !(info.HitEntity is BasePlayer) && info.HitEntity is BaseCombatEntity;
	}

	// Token: 0x0600215C RID: 8540 RVA: 0x000DAD58 File Offset: 0x000D8F58
	public override void DoAttackShared(HitInfo info)
	{
		BasePlayer ownerPlayer = base.GetOwnerPlayer();
		BaseCombatEntity baseCombatEntity = info.HitEntity as BaseCombatEntity;
		if (baseCombatEntity != null && ownerPlayer != null && base.isServer)
		{
			using (TimeWarning.New("DoRepair", 50))
			{
				baseCombatEntity.DoRepair(ownerPlayer);
			}
		}
		info.DoDecals = false;
		if (base.isServer)
		{
			Effect.server.ImpactEffect(info);
			return;
		}
		Effect.client.ImpactEffect(info);
	}
}
