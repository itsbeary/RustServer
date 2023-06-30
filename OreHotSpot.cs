using System;

// Token: 0x02000179 RID: 377
public class OreHotSpot : BaseCombatEntity, ILOD
{
	// Token: 0x060017B1 RID: 6065 RVA: 0x000B3869 File Offset: 0x000B1A69
	public void OreOwner(OreResourceEntity newOwner)
	{
		this.owner = newOwner;
	}

	// Token: 0x060017B2 RID: 6066 RVA: 0x00048147 File Offset: 0x00046347
	public override void ServerInit()
	{
		base.ServerInit();
	}

	// Token: 0x060017B3 RID: 6067 RVA: 0x000B3872 File Offset: 0x000B1A72
	public override void OnAttacked(HitInfo info)
	{
		base.OnAttacked(info);
		if (base.isClient)
		{
			return;
		}
		if (this.owner)
		{
			this.owner.OnAttacked(info);
		}
	}

	// Token: 0x060017B4 RID: 6068 RVA: 0x000B389D File Offset: 0x000B1A9D
	public override void OnKilled(HitInfo info)
	{
		this.FireFinishEffect();
		base.OnKilled(info);
	}

	// Token: 0x060017B5 RID: 6069 RVA: 0x000B38AC File Offset: 0x000B1AAC
	public void FireFinishEffect()
	{
		if (this.finishEffect.isValid)
		{
			Effect.server.Run(this.finishEffect.resourcePath, base.transform.position, base.transform.forward, null, false);
		}
	}

	// Token: 0x0400107B RID: 4219
	public float visualDistance = 20f;

	// Token: 0x0400107C RID: 4220
	public GameObjectRef visualEffect;

	// Token: 0x0400107D RID: 4221
	public GameObjectRef finishEffect;

	// Token: 0x0400107E RID: 4222
	public GameObjectRef damageEffect;

	// Token: 0x0400107F RID: 4223
	public OreResourceEntity owner;
}
